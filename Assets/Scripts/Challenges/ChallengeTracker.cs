using UnityEngine;

public class ChallengeTracker : MonoBehaviour
{
    private int challengeCount;
    private int completedChallengeCount;

    public void AddChallenge()
    {
        challengeCount++;
    }
    
    public void CompleteChallenge()
    {
        completedChallengeCount++;
        if (completedChallengeCount == challengeCount)
            References.Refs.gameLogic.GameCompleted();
    }
}
