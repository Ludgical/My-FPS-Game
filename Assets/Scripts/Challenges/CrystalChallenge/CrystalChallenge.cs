using UnityEngine;

public class CrystalChallenge : Challenge
{
    private const int crystalAmount = 20;
    private const float outerR = 17;
    private const float outerR2 = outerR * outerR;
    private const float innerR = 6;
    private const float innerR2 = innerR * innerR;

    private const float droneSpawnMinDelay = 3;
    private const float droneSpawnMaxDelay = 5;

    private int crystalsHit;
    private float droneSpawnDelay;
    private float timeSinceLastDroneSpawn;
    
    [SerializeField] private GameObject crystalPrefab;
    
    protected override void InitializeChallenge()
    {
        //Create crystals in a donut shape
         
        for (var i = 0; i < crystalAmount; i++)
        {
            //Generate polar coordinates
            var angle = Random.value * 2 * Mathf.PI;
            var radius2 = Random.value * (outerR2 - innerR2) + innerR2;
            var radius = Mathf.Sqrt(radius2);
            
            //Convert to x, y coordinates
            var x = radius * Mathf.Cos(angle);
            var z = radius * Mathf.Sin(angle);
            
            //Generate y coordinate
            var y = Random.Range(4, 7);
            
            var crystalPos = transform.position + new Vector3(x, y, z);
            var crystal = Instantiate(crystalPrefab, crystalPos, crystalPrefab.transform.rotation);
            crystal.GetComponent<Crystal>().challenge = this;
        }
    }

    protected override void StartChallenge()
    {
        SetDroneSpawnDelay();
    }
    
    public void OnCrystalCollected()
    {
        crystalsHit += 1;
        if (crystalsHit == crystalAmount)
            CompleteChallenge();
    }

    private void Update()
    {
        if (!challengeStarted || challengeCompleted)
            return;
        
        if (timeSinceLastDroneSpawn >= droneSpawnDelay)
            SpawnDrone();
        
        timeSinceLastDroneSpawn += Time.deltaTime;
    }

    private void SpawnDrone()
    {
        var drone = Drone.SpawnNew(center:transform.position);
        drone.pathfindMethod = new DronePathfindMethods.TowardsPlayer(drone.transform)
        {
            smoothTime = 1.5f,
            maxSpeed = -1
        };
        
        SetDroneSpawnDelay();
    }

    private void SetDroneSpawnDelay()
    {
        timeSinceLastDroneSpawn = 0;
        droneSpawnDelay = Random.Range(droneSpawnMinDelay, droneSpawnMaxDelay);
    }
}
