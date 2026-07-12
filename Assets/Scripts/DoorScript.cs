using System.Collections;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] private GameObject doorLeft;
    [SerializeField] private GameObject doorRight;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip doorOpenSound;

    //Offset from the center of the left door to the center of the right door
    private Vector3 offset;
    //Where the sides of the door should be when the door is open
    private Vector3 doorLeftGoal;
    private Vector3 doorRightGoal;

    private void Start()
    {
        offset = doorLeft.transform.position - doorRight.transform.position;
        doorLeftGoal = doorLeft.transform.position + offset;
        doorRightGoal = doorRight.transform.position - offset;
    }

    public void OpenDoor()
    {
        //Start coroutine to open the door
        StartCoroutine(OpenDoorRoutine());
        audioSource.PlayOneShot(doorOpenSound);
    }

    private IEnumerator OpenDoorRoutine()
    {
        // yield return new WaitForSeconds(0.2f);
        
        var leftVelocity = Vector3.zero;
        var rightVelocity = Vector3.zero;
        
        //Current offset between the centers of the sides
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

    public void CloseDoor()
    {
        
    }
}
