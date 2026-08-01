using System;
using UnityEngine;

public abstract class Challenge : MonoBehaviour
{
    private References refs;
    
    public bool challengeStarted { get; private set; }
    private bool challengeCompleted;
    
    /// The doors that should be opened when the challenge is completed
    [NonSerialized] public DoorScript[] openOnCompleted;

    private void Start()
    {
        refs = References.Refs;

        refs.challengeTracker.AddChallenge();
        
        SetUpTrigger();
    }

    /// Create and set up the trigger collider to trigger when you enter to room with this challenge
    private void SetUpTrigger()
    {
        var onEnterRoomTrigger = gameObject.AddComponent<BoxCollider>();
        onEnterRoomTrigger.isTrigger = true;
        onEnterRoomTrigger.center = new Vector3(0, 6, -MapGenerator.CenterToDoor);
        onEnterRoomTrigger.size = new Vector3(10, 12, 0);
    }
    // When something enters the trigger at the door
    private void OnTriggerEnter(Collider other)
    {
        if (challengeStarted || !other.CompareTag("Player"))
            return;
        
        StartChallenge();
        challengeStarted = true;
    }
    
    /// Called when the player enters the room with this challenge
    protected abstract void StartChallenge();
    
    /// Call this method when the challenge is completed
    protected void CompleteChallenge()
    {
        if (!challengeStarted || challengeCompleted)
            return;
        challengeCompleted = true;
        
        foreach (var door in openOnCompleted)
            door.OpenDoor(0);
        
        refs.challengeTracker.CompleteChallenge();
    }
}
