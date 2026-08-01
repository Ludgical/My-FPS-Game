using UnityEngine;

public class ChallengeSphereScript : MonoBehaviour, IHittable
{
     [SerializeField] private TestChallenge2 testChallenge2;
     
     public void OnHit()
     {
          if (!testChallenge2.challengeStarted)
               return;
          
          testChallenge2.SphereHit();
     }
}
