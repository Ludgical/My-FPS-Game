using UnityEngine;

public class GunScript : MonoBehaviour
{
    //This is the gunPosition object

    [SerializeField] private Transform handTransform;
    [SerializeField] private Camera camera;

    private Vector3 velocity;
    
    public void SetZPosition(float fov)
    {
        //Set the gun's z-value 
        var gunPos = handTransform.localPosition;
        gunPos.z = -0.00667f * fov + 1.24f;
        handTransform.localPosition = gunPos;
    }

    private void LateUpdate()
    {
        //Make the gun follow the players movement and rotation
        transform.position = Vector3.SmoothDamp(transform.position, handTransform.position, ref velocity, 0.003f);
        transform.rotation = Quaternion.Slerp(transform.rotation, handTransform.rotation, 80 * Time.deltaTime);
    }
}
