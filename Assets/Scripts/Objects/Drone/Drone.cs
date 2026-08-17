using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Drone : MonoBehaviour, IHittable
{
    private References refs;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip laserSound;
    [SerializeField] private AudioClip onHitSound;
    [SerializeField] private AudioClip onDestroyedSound;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject droneDestroyedParticles;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Image healthBarColor;
    [SerializeField] private Material laserMat;
    
    public Action onDestroyed;
    public Action onDestroyedByPlayer;
    public Action onPlayerCollision;
    
    private float maxHealth;
    public float health;
    public bool oneShot;
    public bool damageable = true;

    private bool isFrozen;
    private GameObject laserCylinder;
    private GameObject laserSphere;

    [NonSerialized] public DronePathfindMethod pathfinding;
    private static GameObject dronePrefab;

    private void Start()
    {
        refs = References.Refs;

        if (!oneShot && damageable)
        {
            maxHealth = health;
            UpdateHealthBar();
        }
        else
        {
            HideHealthBar();
        }

        SetUpLaser();
    }

    private void Update()
    {
        pathfinding.TryPathfind();
    }

    private void LateUpdate()
    {
        SetHealthBarRotation();
    }
    
    public void Freeze(float freezeTime = -1)
    {
        isFrozen = true;
        pathfinding.Freeze();

        if (freezeTime > 0)
            StartCoroutine(UnfreezeRoutine());
        return;
        
        IEnumerator UnfreezeRoutine()
        {
            yield return new WaitForSeconds(freezeTime);
            Unfreeze();
        }
    }
    public void Unfreeze()
    {
        isFrozen = false;
        pathfinding.Unfreeze();
    }
    
    public void OnHit()
    {
        if (!damageable || isFrozen)
            return;
        
        if (!oneShot)
        {
            health -= refs.gunData.damage;
            if (health <= 0)
                PlayerDestroyDrone();
            else
                UpdateHealthBar();
        }
        
        if (oneShot)
            PlayerDestroyDrone();
        
        audioSource.PlayOneShot(onHitSound);
    }

    private void UpdateHealthBar()
    {
        if (oneShot)
            return;
        
        var newValue = health / maxHealth;
        healthBar.value = newValue;

        healthBarColor.color = newValue switch
        {
            <= 0.25f => Color.red,
            <= 0.5f => Color.yellow,
            _ => Color.green
        };
    }

    private void SetHealthBarRotation()
    {
        healthBar.transform.LookAt(refs.camera.transform.position);
    }

    private void HideHealthBar()
    {
        healthBar.gameObject.SetActive(false);
    }

    private void PlayerDestroyDrone()
    {
        DestroyDrone();
        onDestroyedByPlayer?.Invoke();
    }

    public void DestroyDrone(float waitTime = -1)
    {
        StartCoroutine(Routine());
        return;

        IEnumerator Routine()
        {
            if (waitTime > 0)
                yield return new WaitForSeconds(waitTime);
            
            Destroy(GetComponent<Collider>());
            pathfinding.Freeze();
            HideHealthBar();
            onDestroyed?.Invoke();

            yield return new WaitForSeconds(Random.Range(0, 0.2f));
            
            animator.SetTrigger("Destroy");
            
            yield return new WaitForSeconds(0.2f);
            
            audioSource.PlayOneShot(onDestroyedSound);
            
            yield return new WaitForSeconds(0.35f);
        
            var particles = Instantiate(droneDestroyedParticles, transform.position, Quaternion.identity);
        
            yield return new WaitForSeconds(1);
        
            Destroy(particles);
            Destroy(gameObject);
        }
    }

    private void SetUpLaser()
    {
        //Create a cylinder that will go from the drone to the target position and
        //a sphere that will go at the end of the cylinder
        
        laserCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Destroy(laserCylinder.GetComponent<Collider>());
        laserCylinder.GetComponent<Renderer>().material = laserMat;
        
        laserSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(laserSphere.GetComponent<Collider>());
        laserSphere.GetComponent<Renderer>().material = laserMat;
        
        HideLaser();
    }

    public void FireLaser(Vector3 target, float duration)
    {
        audioSource.PlayOneShot(laserSound);
        
        var radius = refs.droneData.laserWidth / 2;
        var direction = target - transform.position;
        var length = direction.magnitude;
        
        //Set the positions
        laserCylinder.transform.position = (transform.position + target) / 2;
        laserSphere.transform.position = target;
        
        //Set the scales
        laserCylinder.transform.localScale = new Vector3(radius, length / 2, radius);
        laserSphere.transform.localScale = new Vector3(radius, radius, radius);
        
        //Set the rotation of the cylinder
        laserCylinder.transform.up = direction;

        StartCoroutine(HideLaserRoutine());
        return;

        IEnumerator HideLaserRoutine()
        {
            yield return new WaitForSeconds(duration);
            HideLaser();
        }
    }

    private void HideLaser()
    {
        laserCylinder.transform.localScale = Vector3.zero;
        laserSphere.transform.localScale = Vector3.zero;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isFrozen && other.CompareTag("Player"))
            onPlayerCollision?.Invoke();
    }

    public static Drone SpawnNewAtPosition(Vector3 position)
    {
        var rotation = Quaternion.LookRotation(Challenge.GetPlayerPosition() - position);
        return SpawnNew(position, rotation);
    }
    
    public static Drone SpawnNewBehindWall(Vector3 roomCenter)
    {
        var refs = References.Refs;
        
        //Distance from the center of the room to behind a wall, where the drones should spawn
        var distFromCenter = refs.gameData.CenterToWall + 1;
        var randomPointOnSide = Random.Range(-distFromCenter, distFromCenter);
        var y = Random.Range(refs.droneData.droneSpawnMinY, refs.droneData.droneSpawnMaxY);

        //Choose a random side and spawn the drone at the random point on that side
        var spawnPosition = Random.Range(0, 4) switch
        {
            0 => new Vector3(roomCenter.x + randomPointOnSide, y, roomCenter.z + distFromCenter),
            1 => new Vector3(roomCenter.x + randomPointOnSide, y, roomCenter.z - distFromCenter),
            2 => new Vector3(roomCenter.x + distFromCenter, y, roomCenter.z + randomPointOnSide),
            3 => new Vector3(roomCenter.x - distFromCenter, y, roomCenter.z + randomPointOnSide),
            _ => throw new Exception("Unable to spawn Drone")
        };
    
        //Make the drone look at the center of the room
        var spawnRotation = Quaternion.LookRotation(roomCenter - spawnPosition);
        
        return SpawnNew(spawnPosition, spawnRotation);
    }

    private static Drone SpawnNew(Vector3 position, Quaternion rotation)
    {
        var drone = Instantiate(References.Refs.droneData.dronePrefab, position, rotation);
        return drone.GetComponentInChildren<Drone>();
    }
}
