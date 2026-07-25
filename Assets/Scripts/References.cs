using Scriptable_Objects;
using UnityEngine;

public class References : MonoBehaviour
{
    public static References Refs;
    
    public GameLogic gameLogic;
    public PlayerScript player;
    public Animator playerAnimator;
    public Camera camera;
    public GunScript gun;
    public Transform delayedFollowPivot;
    public Transform gunPivot;
    public PlayerData playerData;
    public GunData gunData;
    public GameData gameData;

    private void Awake()
    {
        Refs = this;
    }
}
