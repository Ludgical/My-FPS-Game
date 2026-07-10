using UnityEngine;

public class GroundColliderScript : MonoBehaviour
{
    [SerializeField] private PlayerScript player;
    
    private void OnTriggerEnter(Collider other)
    {
        player.SetIsOnGround(true);
    }

    private void OnTriggerExit(Collider other)
    {
        player.SetIsOnGround(false);
    }
}
