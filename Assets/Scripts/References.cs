using Scriptable_Objects;
using UnityEngine;

public class References : MonoBehaviour
{
    public static References Refs;
    
    public GameLogic gameLogic;
    public PlayerScript player;
    public Animator playerAnimator;
    public Camera camera;
    public Transform gunPivot;
    public Transform delayedFollowPivot;
    public GameObject gunObject;
    public GunScript gun;
    public GameObject startUI;
    public PlayerData playerData;
    public GunData gunData;

    private void Awake()
    {
        Refs = this;
    }
}
