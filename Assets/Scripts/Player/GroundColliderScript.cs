using UnityEngine;

public class GroundColliderScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.isTrigger)
            References.Refs.player.SetIsOnGround(true);
    }

    private void OnTriggerExit(Collider collider)
    {
        if (!collider.isTrigger)
            References.Refs.player.SetIsOnGround(false);
    }
}
