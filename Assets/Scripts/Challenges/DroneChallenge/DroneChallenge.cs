using System.Collections.Generic;
using System.Linq;
using Scriptable_Objects;
using TMPro;
using UnityEngine;

public class DroneChallenge : Challenge
{
    private DroneChallengeData cd;

    [SerializeField] private GameObject canvas;
    [SerializeField] private TMP_Text toDestroyText;

    private int dronesDestroyed;
    private int dronesToDestroy;
    private List<Drone> drones;
    private Drone chasingDrone;
    private float timeSinceLastDroneSpawn;

    protected override void InitializeChallenge()
    {
        cd = refs.droneChallengeData;
        
        dronesToDestroy = cd.dronesToDestroy;
        
        SpawnChasingDrone();
        
        drones = new List<Drone>();
    }

    protected override void StartChallenge()
    {
        for (var i = 0; i < cd.immediateDroneSpawnCount; i++)
            SpawnDrone();
        
        chasingDrone.Unfreeze();
        
        canvas.SetActive(true);
    }
    
    private void Update()
    {
        if (!challengeStarted || challengeCompleted || drones.Count >= cd.maxDroneAmount)
            return;

        if (timeSinceLastDroneSpawn >= cd.droneSpawnDelay)
        {
            SpawnDrone();
            timeSinceLastDroneSpawn = 0;
        }

        timeSinceLastDroneSpawn += Time.deltaTime;
    }

    private void SpawnDrone()
    {
        var drone = Drone.SpawnNewBehindWall(roomCenter:transform.position);

        drone.pathfinding = new DronePathfindMethods.Circle(drone)
        {
            velocitySmoothTime = cd.droneVelocitySmoothTime,
            maxSpeed = cd.droneMaxSpeed,
            center = transform.position + new Vector3(
                0, Random.Range(cd.dronePathfindMinY, cd.dronePathfindMaxY), 0),
            radius = Random.Range(cd.dronePathfindMinRadius, cd.dronePathfindMaxRadius),
            faceCenter = false,
            smoothOnTransition = true,
            rotationDirection = Random.value < 0.5 ? -1 : 1
        };

        drone.health = cd.droneHealth;

        drone.onDestroyed += () => OnDroneDestroyed(drone);
        
        drones.Add(drone);
    }

    private void SpawnChasingDrone()
    {
        var drone = Drone.SpawnNewAtPosition(
            position:transform.position + new Vector3(0, cd.chasingDroneY, 0),
            rotation:Quaternion.Euler(0, transform.eulerAngles.y + 180, 0));
        
        drone.pathfinding = new DronePathfindMethods.TowardsPlayer(drone)
        {
            velocitySmoothTime = cd.chasingDroneVelocitySmoothTime,
            maxSpeed = cd.chasingDroneMaxSpeed,
            rotationSpeed = cd.chasingDroneRotationSpeed
        };
        
        drone.damageable = false;

        drone.onPlayerCollision += () => OnChasingDroneHitPlayer(drone);
        
        drone.Freeze();
        
        chasingDrone = drone;
    }

    private void OnDroneDestroyed(Drone drone)
    {
        dronesDestroyed++;
        SetToDestroyText();
        drones.Remove(drone);
        if (dronesDestroyed == dronesToDestroy)
            OnChallengeCompleted();
    }

    private void OnChasingDroneHitPlayer(Drone drone)
    {
        dronesToDestroy++;
        SetToDestroyText();
        drone.FireLaser(GetPlayerPosition(), cd.chasingDroneFreezeTime);
        drone.Freeze(cd.chasingDroneFreezeTime);
    }

    private void SetToDestroyText()
    {
        toDestroyText.text = (dronesToDestroy - dronesDestroyed).ToString();
    }

    private void OnChallengeCompleted()
    {
        while (drones.Count > 0)
            drones.First().DestroyDrone();
        
        chasingDrone.DestroyDrone();
        
        canvas.SetActive(false);
        
        CompleteChallenge();
    }
}
