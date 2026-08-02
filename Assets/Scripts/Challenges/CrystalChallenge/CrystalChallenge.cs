using UnityEngine;

public class CrystalChallenge : Challenge
{
    private const int crystalAmount = 20;
    private const float outerR = 17;
    private const float outerR2 = outerR * outerR;
    private const float innerR = 6;
    private const float innerR2 = innerR * innerR;

    private int crystalsHit;
    
    [SerializeField] private GameObject crystalPrefab;
    
    protected override void InitializeChallenge()
    {
        //Create crystals in a donut shape
         
        for (var i = 0; i < crystalAmount; i++)
        {
            //Generate polar coordinates
            var angle = Random.value * 2 * Mathf.PI;
            var radius2 = Random.value * (outerR2 - innerR2) + innerR2;
            var radius = Mathf.Sqrt(radius2);
            
            //Convert to x, y coordinates
            var x = radius * Mathf.Cos(angle);
            var z = radius * Mathf.Sin(angle);
            
            //Generate y coordinate
            var y = Random.Range(4, 7);
            
            var crystalPos = transform.position + new Vector3(x, y, z);
            var crystal = Instantiate(crystalPrefab, crystalPos, crystalPrefab.transform.rotation);
            crystal.GetComponent<Crystal>().challenge = this;
        }
    }
    
    protected override void StartChallenge() { }
    
    public void OnCrystalCollected()
    {
        crystalsHit += 1;
        if (crystalsHit == crystalAmount)
            CompleteChallenge();
    }
}
