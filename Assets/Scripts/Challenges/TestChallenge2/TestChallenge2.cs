using UnityEngine;

public class TestChallenge2 : Challenge
{
    [SerializeField] private GameObject sphere;
    
    protected override void StartChallenge()
    {
        sphere.transform.localPosition = new Vector3(0, 5, 0);
    }

    public void SphereHit()
    {
        sphere.transform.localPosition = new Vector3(0, 7, 0);
        CompleteChallenge();
    }
}