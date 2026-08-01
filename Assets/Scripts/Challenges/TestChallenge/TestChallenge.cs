using UnityEngine;

public class TestChallenge : Challenge
{
    [SerializeField] private GameObject cube;
    
    protected override void StartChallenge()
    {
        cube.transform.localPosition = new Vector3(0, 5, 0);
    }

    public void CubeHit()
    {
        cube.GetComponent<Renderer>().material.color = Color.gray3;
        CompleteChallenge();
    }
}
