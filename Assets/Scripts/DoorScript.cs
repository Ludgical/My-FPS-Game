using System.Collections;
using System.Linq;
using UnityEngine;

public class DoorScript : Objective
{
    private References refs;
    
    [SerializeField] private GameObject doorLeft;
    [SerializeField] private GameObject doorRight;
    [SerializeField] private TargetScript[] targets;
    [SerializeField] public bool isStartRoomDoor;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip doorOpenSound;
    
    private Vector3 doorLeftStart;
    private Vector3 doorRightStart;
    private Vector3 offset;
    private Vector3 doorLeftGoal;
    private Vector3 doorRightGoal;

    public int lastHitTargetNumber;
    
    private void Start()
    {
        refs = References.Refs;
        
        //Where the sides of the door started
        doorLeftStart = doorLeft.transform.position;
        doorRightStart = doorRight.transform.position;
        
        //Offset from the center of the left door to the center of the right door
        offset = doorRightStart - doorLeftStart;
        
        //Where the sides of the door should be when the door is open
        doorLeftGoal = doorLeft.transform.position - offset;
        doorRightGoal = doorRight.transform.position + offset;
        
        if (!isStartRoomDoor)
            SetUpTargetDoor();
        
        if (isStartRoomDoor)
            SetUpStartRoomDoor();
    }

    private void SetUpTargetDoor()
    {
        refs.gameLogic.onResetScene += ResetDoor;
        
        SetUpTargets();
    }

    private void SetUpTargets()
    {
        RandomizeTargetPositions();
    }

    private void RandomizeTargetPositions()
    {
        //Get the positions of all the targets of the door
        var availableTargetPositions = 
            targets.Select(target => target.gameObject.transform.localPosition).ToList();
        
        //Assign every target a random position out of the target positions
        for (var i = targets.Length - 1; i >= 0; i--)
        {
            var j = Random.Range(0, i + 1);
            targets[i].transform.localPosition = availableTargetPositions[j];
            availableTargetPositions.RemoveAt(j);
        }
    }
    
    private void SetUpStartRoomDoor()
    {
        refs.gameLogic.onPlay += () => OpenDoor(1.2f);
        refs.gameLogic.onResetScene += ResetDoor;
    }

    public void OpenDoor(float waitTime)
    {
        //Start coroutine to open the door
        StartCoroutine(OpenDoorRoutine(waitTime));
        
        //Until there are actual objectives
        CompleteObjective();
    }

    private IEnumerator OpenDoorRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        audioSource.PlayOneShot(doorOpenSound);
        
        //For the smooth damp
        var leftVelocity = Vector3.zero;
        var rightVelocity = Vector3.zero;
        
        //Current offset between the centers of the sides of the door
        var currentOffset = offset;
        
        //While the door isn't fully open
        while (offset.magnitude * 3 - 0.01 > currentOffset.magnitude)
        {
            //Move the sides of the door closer to their goals
            doorLeft.transform.position = 
                Vector3.SmoothDamp(doorLeft.transform.position, doorLeftGoal, ref leftVelocity, 0.7f);
            doorRight.transform.position = 
                Vector3.SmoothDamp(doorRight.transform.position, doorRightGoal, ref rightVelocity, 0.7f);
            
            //Set the current offset between the sides
            currentOffset = doorLeft.transform.position - doorRight.transform.position;
            //Wait for the next frame
            yield return null;
        }
        
        //Put the sides exactly where they're supposed to be
        doorLeft.transform.position = doorLeftGoal;
        doorRight.transform.position = doorRightGoal;
    }

    private void ResetDoor()
    {
        lastHitTargetNumber = 0;
        
        StopAllCoroutines();
        
        //Close the door instantly
        doorLeft.transform.position = doorLeftStart;
        doorRight.transform.position = doorRightStart;
        
        if (!isStartRoomDoor)
            RandomizeTargetPositions();
    }
}
