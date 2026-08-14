using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scriptable_Objects;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrystalChallenge : Challenge
{
    private CrystalChallengeData cd;
    
    private int crystalsHit;
    private List<Drone> drones;
    private float droneSpawnDelay;
    private float timeSinceLastDroneSpawn;
    /// Waiting after the player got hit by a drone
    [NonSerialized] public bool waiting;

    [SerializeField] private GameObject crystalPrefab;
    
    protected override void InitializeChallenge()
    {
        cd = refs.crystalChallengeData;
        
        drones = new List<Drone>();
        
        SpawnCrystals();
    }

    private void SpawnCrystals()
    {
        var outerR2 = cd.outerRadius * cd.outerRadius;
        var innerR2 = cd.innerRadius * cd.innerRadius;
        
        //Create crystals in a donut shape
        for (var i = 0; i < cd.crystalAmount; i++)
        {
            //Generate polar coordinates
            var angle = Random.value * 2 * Mathf.PI;
            var radius2 = Random.value * (outerR2 - innerR2) + innerR2;
            var radius = Mathf.Sqrt(radius2);
            
            //Convert to x, y coordinates
            var x = radius * Mathf.Cos(angle);
            var z = radius * Mathf.Sin(angle);
            
            //Generate y coordinate
            var y = Random.Range(cd.crystalSpawnMinY, cd.crystalSpawnMaxY);
            
            //Create the crystal
            var crystalPos = transform.position + new Vector3(x, y, z);
            var crystal = Instantiate(crystalPrefab, crystalPos, crystalPrefab.transform.rotation);
            crystal.GetComponent<Crystal>().challenge = this;
        }
    }

    protected override void StartChallenge()
    {
        SetDroneSpawnDelay();
    }
    
    private void SetDroneSpawnDelay()
    {
        timeSinceLastDroneSpawn = 0;
        droneSpawnDelay = Random.Range(cd.droneSpawnMinDelay, cd.droneSpawnMaxDelay);
    }

    private void Update()
    {
        if (!challengeStarted || waiting || challengeCompleted)
            return;

        if (timeSinceLastDroneSpawn >= droneSpawnDelay)
        {
            SpawnDrone();
            SetDroneSpawnDelay();
        }

        timeSinceLastDroneSpawn += Time.deltaTime;
    }

    private void SpawnDrone()
    {
        var drone = Drone.SpawnNew(center:transform.position, spawnBehindWall:true);
        
        drone.pathfinding = new DronePathfindMethods.TowardsPlayer(drone)
        {
            velocitySmoothTime = cd.droneVelocitySmoothTime,
            maxSpeed = cd.droneMaxSpeed,
            speedIncreasePerSecond = cd.droneSpeedIncreasePerSecond,
            rotationSpeed = cd.droneRotationSpeed
        };
        
        drone.health = cd.droneHealth;
        
        drone.onPlayerCollision += () => StartCoroutine(WaitingRoutine(drone));
        drone.onDestroyed += () => drones.Remove(drone);
        
        drones.Add(drone);
    }
    
    public void OnCrystalCollected()
    {
        crystalsHit += 1;
        if (crystalsHit == cd.crystalAmount)
            OnChallengeCompleted();
    }

    private void OnChallengeCompleted()
    {
        while (drones.Count > 0)
            drones.First().DestroyDrone();
        
        CompleteChallenge();
    }

    private IEnumerator WaitingRoutine(Drone drone)
    {
        waiting = true;
        
        foreach (var d in drones)
            d.Freeze();
        
        //Make sure the waiting time is longer than the time the laser is active
        var laserActiveTime = cd.droneLaserActiveTime;
        if (cd.waitingTime < laserActiveTime)
            throw new Exception($"waitingTime can't be less than {laserActiveTime} seconds");
        
        drone.FireLaser(GetPlayerPosition(), laserActiveTime);

        yield return new WaitForSeconds(cd.waitingTime);
        
        foreach (var d in drones)
            d.Unfreeze();
        
        waiting = false;
    }
}
