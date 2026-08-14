using UnityEngine;

public class ChallengeTracker : MonoBehaviour
{
    private int challengeCount;
    private int completedChallengeCount;
    public bool ChallengeActive { get; private set; }

    public void AddChallenge()
    {
        challengeCount++;
    }

    public void StartChallenge()
    {
        ChallengeActive = true;
    }
    
    public void CompleteChallenge()
    {
        ChallengeActive = false;
        completedChallengeCount++;
        if (completedChallengeCount == challengeCount)
            References.Refs.gameLogic.GameCompleted();
    }
}
