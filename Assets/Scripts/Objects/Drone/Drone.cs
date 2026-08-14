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
                DestroyDrone();
            else
                UpdateHealthBar();
        }
        
        if (oneShot)
            DestroyDrone();
        
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

    public void DestroyDrone()
    {
        StartCoroutine(Routine());
        return;

        IEnumerator Routine()
        {
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
    
    /// <summary>
    /// Spawn a new drone
    /// </summary>
    /// <param name="center">The center of the room</param>
    /// <param name="spawnBehindWall"><br/>True: Spawn the drone behind one of the walls<br/>
    /// False: Spawn the drone in the center of the room</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static Drone SpawnNew(Vector3 center, bool spawnBehindWall)
    {
        var refs = References.Refs;

        Vector3 spawnPosition;
        Quaternion spawnRotation;

        if (spawnBehindWall)
        {
            //Distance from the center of the room to behind a wall, where the drones should spawn
            var distFromCenter = refs.gameData.CenterToWall + 1;
            var randomPointOnSide = Random.Range(-distFromCenter, distFromCenter);
            var y = Random.Range(refs.droneData.droneSpawnMinY, refs.droneData.droneSpawnMaxY);

            //Choose a random side and spawn the drone at the random point on that side
            spawnPosition = Random.Range(0, 4) switch
            {
                0 => new Vector3(center.x + randomPointOnSide, y, center.z + distFromCenter),
                1 => new Vector3(center.x + randomPointOnSide, y, center.z - distFromCenter),
                2 => new Vector3(center.x + distFromCenter, y, center.z + randomPointOnSide),
                3 => new Vector3(center.x - distFromCenter, y, center.z + randomPointOnSide),
                _ => throw new Exception("SpawnNew(): unable to spawn Drone")
            };
        
            //Make the drone look at the center of the room
            spawnRotation = Quaternion.LookRotation(center - spawnPosition);
        }
        else
        {
            spawnPosition = center;
            spawnRotation = Quaternion.LookRotation(Challenge.GetPlayerPosition() - center);
        }

        //Create the drone
        var drone = Instantiate(refs.droneData.dronePrefab, spawnPosition, spawnRotation);
        return drone.GetComponentInChildren<Drone>();
    }
}
