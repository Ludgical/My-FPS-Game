using System;
using UnityEngine;

public abstract class Challenge : MonoBehaviour
{
    protected References refs;
    
    /// The doors that should be opened when the challenge is completed
    [NonSerialized] public DoorScript[] openOnCompleted;

    public bool challengeStarted { get; private set; }
    protected bool challengeCompleted { get; private set; }

    private void Start()
    {
        refs = References.Refs;

        refs.challengeTracker.AddChallenge();
        SetUpTrigger();
        InitializeChallenge();
    }

    /// Create and set up the trigger collider to trigger when you enter to room with this challenge
    private void SetUpTrigger()
    {
        var onEnterRoomTrigger = gameObject.AddComponent<BoxCollider>();
        onEnterRoomTrigger.isTrigger = true;
        onEnterRoomTrigger.center = new Vector3(0, 6, -refs.gameData.CenterToDoor - 0.5f);
        onEnterRoomTrigger.size = new Vector3(10, 12, 0);
    }
    // When something enters the trigger at the door
    private void OnTriggerEnter(Collider other)
    {
        if (challengeStarted || refs.challengeTracker.ChallengeActive || !other.CompareTag("Player"))
            return;
        
        StartChallenge();
        refs.challengeTracker.StartChallenge();
        challengeStarted = true;
    }
    
    /// Called when the challenge is created
    protected abstract void InitializeChallenge();
    /// Called when the player enters the room with this challenge
    protected abstract void StartChallenge();
    
    /// Call this method when the challenge is completed
    protected void CompleteChallenge()
    {
        if (!challengeStarted || challengeCompleted)
            return;
        challengeCompleted = true;
        
        foreach (var door in openOnCompleted)
            door.OpenDoor();
        
        refs.challengeTracker.CompleteChallenge();
    }
    
    /// Returns the position where challenges can simulate the player being
    public static Vector3 GetPlayerPosition()
    {
        var refs = References.Refs;
        var colliderHeight = refs.player.IsCrouched
            ? refs.playerData.crouchedColliderHeight
            : refs.playerData.colliderHeight;
        var offset = new Vector3(0, colliderHeight * 0.7f, 0);
        return refs.player.transform.position + offset;
    }
}
