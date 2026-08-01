using UnityEngine;

public class ChallengeCubeScript : MonoBehaviour, IHittable
{
     [SerializeField] private TestChallenge testChallenge;
     
     public void OnHit()
     {
          if (!testChallenge.challengeStarted)
               return;
          
          testChallenge.CubeHit();
     }
}
