using UnityEngine;

public class GroundColliderScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider _)
    {
        References.Refs.player.SetIsOnGround(true);
    }

    private void OnTriggerExit(Collider _)
    {
        References.Refs.player.SetIsOnGround(false);
    }
}
