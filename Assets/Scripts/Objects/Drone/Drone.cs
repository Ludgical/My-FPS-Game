using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Drone : MonoBehaviour, IHittable
{
    private References refs;
    
    public Action onDeath;
    public Action onPlayerCollision;
    
    private bool isVulnerable = true;
    private float maxHealth;
    private float currentHealth;
    private bool oneShot;
    
    public DronePathfindMethod pathfindMethod;
    private static GameObject dronePrefab;

    private void Start()
    {
        refs = References.Refs;
        
        onDeath += OnDeath;
    }

    private void Update()
    {
        pathfindMethod.Pathfind();
    }
    
    public void OnHit()
    {
        if (!isVulnerable)
            return;

        if (oneShot)
            onDeath?.Invoke();

        currentHealth -= refs.gunData.damage;
        if (currentHealth <= 0)
            onDeath?.Invoke();
    }

    private void OnDeath()
    {
        Destroy(gameObject);
    }

    public static Drone SpawnNew(Vector3 center)
    {
        var refs = References.Refs;
        
        var distFromCenter = refs.gameData.CenterToWall + 1;
        var randomPointOnSide = Random.Range(-distFromCenter, distFromCenter);
        var y = Random.Range(2f, 10f);

        var spawnPosition = Random.Range(0, 4) switch
        {
            0 => new Vector3(center.x + randomPointOnSide, y, center.z + distFromCenter),
            1 => new Vector3(center.x + randomPointOnSide, y, center.z - distFromCenter),
            2 => new Vector3(center.x + distFromCenter, y, center.z + randomPointOnSide),
            3 => new Vector3(center.x - distFromCenter, y, center.z + randomPointOnSide),
            _ => throw new Exception("SpawnNew(): unable to spawn Drone")
        };

        var drone = Instantiate(refs.dronePrefab, spawnPosition, Quaternion.identity);
        return drone.GetComponent<Drone>();
    }
}
