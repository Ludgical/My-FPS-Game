using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Crystal : MonoBehaviour, IHittable
{
    private float yRotation;
    private float startY;
    private float timeSeconds;
    private bool isHit;

    /// How many degrees the crystal should rotate every second
    private float rotationPerSecond = 60;

    /// How long it takes the crystal to bob up and down and go back to its start position
    private const float bobDurationSeconds = 2.5f;

    /// How much the crystal should move up and down
    private const float bobHeightMultiplier = 0.2f;
    
    public CrystalChallenge challenge;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip crystalSound1;
    [SerializeField] private AudioClip crystalSound2;
    
    private void Start()
    {
        if (Random.value < 0.5f)
            rotationPerSecond = -rotationPerSecond;
        startY = transform.position.y;

        //Set the rotation and time to random values so the crystals aren't synced up
        SetRotationY(Random.Range(0, 359));
        timeSeconds = Random.value * bobDurationSeconds;
        SetHeight();
    }

    private void Update()
    {
        //Update the time and set the height and rotation
        timeSeconds += Time.deltaTime;
        timeSeconds %= bobDurationSeconds;
        SetHeight();
        SetRotationY(yRotation + rotationPerSecond * Time.deltaTime);
    }

    /// Set the y position of the crystal to follow a sin curve
    private void SetHeight()
    {
        if (isHit)
            return;
        
        var degrees = timeSeconds / bobDurationSeconds; //0 - 1
        var radians = degrees * 2 * Mathf.PI;
        var newY = startY + Mathf.Sin(radians) * bobHeightMultiplier;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    /// Set the y rotation of the crystal to <c>y</c>
    private void SetRotationY(float y)
    {
        yRotation = y;
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x, yRotation, transform.rotation.eulerAngles.z);
    }

    public void OnHit()
    {
        if (!challenge.challengeStarted)
            return;
        isHit = true;
        
        Destroy(GetComponent<Collider>());
        StartCoroutine(MoveToCenter());
        
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(Random.value < 0.5f ? crystalSound1 : crystalSound2);
    }

    private IEnumerator MoveToCenter()
    {
        var center = challenge.transform.position + new Vector3(0, 6, 0);
        
        //Position above the crystal
        var aboveGoal = new Vector3(
            transform.position.x, transform.position.y + 5, transform.position.z);
        
        //Direction from the center to the far goal
        var farGoalOffset = center - aboveGoal - new Vector3(0, 5, 0);
        //Position across the center of the room from the perspective of the crystal
        var farGoal = center + farGoalOffset * 3;
        
        //Distance from the center to the far goal
        var centerToFar = (farGoal - center).magnitude;
        
        //Velocity of the crystal
        var velocity = new Vector3(0, 20, 0);
        
        //Move the crystal towards aboveGoal
        while ((transform.position - aboveGoal).magnitude > 3f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, aboveGoal, ref velocity, 1.5f);
            yield return null;
        }
        //Move the crystal toward farGoal until it's in the center of the room
        while ((transform.position - farGoal).magnitude > centerToFar)
        {
            transform.position = Vector3.SmoothDamp(transform.position, farGoal, ref velocity, 2f);
            yield return null;
        }
        
        challenge.OnCrystalCollected();
        Destroy(gameObject);
    }
}
