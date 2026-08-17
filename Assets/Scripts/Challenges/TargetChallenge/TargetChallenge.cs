using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scriptable_Objects;
using UnityEngine;
using Random = UnityEngine.Random;

public class TargetChallenge : Challenge
{
    private TargetChallengeData cd;

    [SerializeField] private GameObject[] targetGroups;
    [SerializeField] private Transform playerTransform;
    
    private TargetScript[][] targetsInGroups;
    private readonly List<TargetScript> notHitTargets = new();
    private TargetScript lastHitTarget;
    private int droneIndex;
    private float droneSpeedDecrease;
    private int level;
    
    protected override void InitializeChallenge()
    {
        cd = refs.targetChallengeData;

        if (cd.depthToSpawnDrone < 1)
            throw new Exception("depthToSpawnDrone must be greater than 0");
        if (cd.droneLaserTime > cd.destroyDroneTime || cd.destroyDroneTime > cd.restartTime)
            throw new Exception("droneLaserTime < destroyDroneTime < restartTime");
        if (cd.restartTime <= 0.5f)
            throw new Exception("restartTime must be greater than to 0.5");

        targetsInGroups = targetGroups.Select(group => 
            group.GetComponentsInChildren<TargetScript>()).ToArray();

        notHitTargets.Capacity = targetsInGroups.Sum(targets => targets.Length);
        
        foreach (var targets in targetsInGroups)
        {
            foreach (var target in targets)
            {
                target.onHit += () => OnTargetHit(target);
                target.TurnOffTargetInstant();
            }
        }

        targetGroups[0].SetActive(true);
        targetGroups[1].SetActive(false);
        targetGroups[2].SetActive(false);
    }

    protected override void StartChallenge()
    {
        StartCoroutine(StartChallengeRoutine());
    }

    private IEnumerator StartChallengeRoutine()
    {
        refs.player.FreezeMovement();
        
        //Distance between player and where the player is supposed to be
        var dist = Vector3.Distance(
            refs.player.transform.position,
            playerTransform.position);
        
        //Run this for timeToGoToPlayerPos seconds
        for (var time = 0f; time < cd.timeToGoToPlayerPos; time += Time.deltaTime)
        {
            //Move the player closer to the target position
            refs.player.transform.position = Vector3.MoveTowards(
                refs.player.transform.position,
                playerTransform.position,
                (dist / cd.timeToGoToPlayerPos) * Time.deltaTime);
            
            yield return null;
        }
        
        SetUpLevel();
    }

    private void OnTargetHit(TargetScript target)
    {
        if (!target.isOn)
            return;
        
        target.TurnOffTarget();
        notHitTargets.Remove(target);

        //Level completed
        if (notHitTargets.Count == 0)
        {
            NextLevel();
            return;
        }

        //Spawn drone at the right index
        if (notHitTargets.Count == droneIndex)
            SpawnDrone();
        //Otherwise, turn on a target
        else
            TurnOnRandomTarget();
    }
    
    private void NextLevel()
    {
        //All levels completed
        if (level == targetGroups.Length - 1)
        {
            StartCoroutine(CompleteChallengeTimer());
            return;
            
            IEnumerator CompleteChallengeTimer()
            {
                yield return new WaitForSeconds(cd.nextLevelTime);
                
                OnChallengeCompleted();
            }
        }
        
        level++;
        droneSpeedDecrease = 0;
        SetUpLevel(cd.nextLevelTime);
    }

    private void SetUpLevel(float waitTime = -1)
    {
        StartCoroutine(SetUpLevelTimer());
        return;
        
        IEnumerator SetUpLevelTimer()
        {
            if (waitTime > 0)
                yield return new WaitForSeconds(waitTime);
            
            //Add targets from this level and the previous levels to notHitTargets
            notHitTargets.Clear();
            for (var i = 0; i <= level; i++)
                notHitTargets.AddRange(targetsInGroups[i]);
        
            //How many targets should be remaining when the drone spawns?
            droneIndex = Random.Range(
                cd.depthToSpawnDrone, notHitTargets.Count - cd.depthToSpawnDrone + 1);
        
            targetGroups[level].SetActive(true);
            
            TurnOnRandomTarget();
        }
    }

    private void SpawnDrone()
    {
        var droneY = Random.Range(cd.droneMinY, cd.droneMaxY);
        //Where the drone should go and look
        var targetPos = playerTransform.position + new Vector3(0, droneY, 0);
        var direction = targetPos - transform.position;
        direction.y = 0;
        direction.Normalize();
        
        var dronePos = targetPos + direction * cd.droneDistanceBehind;
        var drone = Drone.SpawnNewAtPosition(dronePos);
        
        drone.pathfinding = new DronePathfindMethods.GoAndStay(drone)
        {
            maxSpeed = cd.droneSpeed - droneSpeedDecrease,
            velocitySmoothTime = cd.droneVelocitySmoothTime,
            targetPos = targetPos,
            facing = targetPos
        };

        drone.oneShot = true;

        drone.onDestroyedByPlayer += TurnOnRandomTarget;
        drone.onPlayerCollision += () =>
        {
            drone.FireLaser(targetPos, cd.droneLaserTime);
            drone.DestroyDrone(cd.destroyDroneTime);
            drone.Freeze();
            droneSpeedDecrease += cd.droneSpeedDecreasePerRestart;
            droneSpeedDecrease = Mathf.Clamp(droneSpeedDecrease, 0, cd.droneSpeed);
            SetUpLevel(cd.restartTime);
        };
    }

    private void TurnOnRandomTarget()
    {
        var target = notHitTargets[Random.Range(0, notHitTargets.Count)];
        target.TurnOnTargetInstant();
    }

    private void OnChallengeCompleted()
    {
        foreach (var group in targetGroups)
            group.SetActive(false);
        
        refs.player.UnfreezeMovement();
        
        CompleteChallenge();
    }
}
