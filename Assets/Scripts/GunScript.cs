using UnityEngine;

public class GunScript : MonoBehaviour
{
    //This is the gunPosition object

    private Vector3 position;
    private Quaternion rotation;

    [SerializeField] private Transform gunTransform;
    [SerializeField] private Camera camera;

    private Vector3 velocity;
    
    public void SetZPosition(float fov)
    {
        //Set the gun's z-value 
        var gunPos = gunTransform.localPosition;
        gunPos.z = -0.00667f * fov + 1.24f;
        gunTransform.localPosition = gunPos;
    }
}
