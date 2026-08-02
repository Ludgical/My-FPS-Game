using UnityEngine;

public class ChallengeCubeScript : MonoBehaviour, IHittable
{
     [SerializeField] private TestChallenge challenge;
     
     public void OnHit()
     {
          if (!challenge.challengeStarted)
               return;
          
          challenge.CubeHit();
     }
}
