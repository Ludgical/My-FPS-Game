using Scriptable_Objects;
using UnityEngine;

public class References : MonoBehaviour
{
    public static References Refs;
    
    public GameLogic gameLogic;
    public GameUtil gameUtil;
    public ChallengeTracker challengeTracker;
    public PlayerScript player;
    public GunScript gun;
    public Camera camera;
    
    public PlayerData playerData;
    public GunData gunData;
    public GameData gameData;
    public DroneData droneData;
    public CrystalChallengeData crystalChallengeData;
    public DroneChallengeData droneChallengeData;
    public TargetChallengeData targetChallengeData;
    public RelayNodeChallengeData relayNodeChallengeData;

    private void Awake()
    {
        Refs = this;
    }
}
