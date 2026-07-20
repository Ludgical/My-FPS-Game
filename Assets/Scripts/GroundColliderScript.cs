using UnityEngine;

public class GroundColliderScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        References.Refs.player.isOnGround = true;
    }

    private void OnTriggerExit(Collider other)
    {
        References.Refs.player.isOnGround = false;
    }
}
