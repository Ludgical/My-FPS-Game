using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetScript : MonoBehaviour, IHittable
{
    private References refs;
    
    [SerializeField] private DoorScript parentDoor;
    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip targetHitSound;
    [SerializeField] private Material redOffMat;
    [SerializeField] private Material whiteOffMat;
    [SerializeField] private int targetNumber;

    private bool isHit;
    private Material redOnMat;
    private Material whiteOnMat;
    private List<Material> targetMaterials;
    private const int RedIndex = 1;
    private const int WhiteIndex = 2;

    private void Start()
    {
        //The start room door doesn't have targets
        if (parentDoor.isStartRoomDoor)
        {
            Destroy(gameObject);
            return;
        }
        
        refs = References.Refs;
        
        //The materials on the target
        targetMaterials = renderer.materials.ToList();
        //Copy and save the red on and white on materials for when the target resets
        redOnMat = new Material(targetMaterials[RedIndex]);
        whiteOnMat = new Material(targetMaterials[WhiteIndex]);
        
        refs.gameLogic.onResetScene += ResetTarget;
    }

    public void OnHit()
    {
        //If the target is already hit or if the number on this target 
        //is not 1 more than the number on the previously hit target, return
        if (isHit || targetNumber != parentDoor.lastHitTargetNumber + 1)
            return;
        
        isHit = true;
        
        //Update the last hit target number and turn off this target
        parentDoor.lastHitTargetNumber = targetNumber;
        StartCoroutine(OnTargetHitRoutine());
        
        //If the fourth target is hit, open the door
        if (targetNumber == 4)
            parentDoor.OpenDoor(0);
    }
    
    private IEnumerator OnTargetHitRoutine()
    {
        yield return new WaitForSeconds(0.05f);
            
        audioSource.PlayOneShot(targetHitSound);
            
        yield return new WaitForSeconds(0.05f);
            
        while (true)
        {
            //How far the red color is from being turned off
            var diffRedColor = targetMaterials[RedIndex].color - redOffMat.color;
            var diffRed = diffRedColor.r + diffRedColor.g + diffRedColor.b;
            //How far the white color is from being turned off
            var diffWhiteColor = targetMaterials[WhiteIndex].color - whiteOffMat.color;
            var diffWhite = diffWhiteColor.r + diffWhiteColor.g + diffWhiteColor.b;
            //If both colors are very close to being off, stop updating the color
            if (diffRed + diffWhite < 0.0001)
                break;

            //Make the colors more similar to their goal colors using Lerp
            targetMaterials[RedIndex].Lerp(targetMaterials[RedIndex], redOffMat, 0.06f);
            targetMaterials[WhiteIndex].Lerp(targetMaterials[WhiteIndex], whiteOffMat, 0.06f);
                
            //Apply the materials
            renderer.SetMaterials(targetMaterials);
                
            yield return null;
        }
    }

    private void ResetTarget()
    {
        isHit = false;
        
        StopAllCoroutines();
        
        targetMaterials[RedIndex].CopyPropertiesFromMaterial(redOnMat);
        targetMaterials[WhiteIndex].CopyPropertiesFromMaterial(whiteOnMat);
        renderer.SetMaterials(targetMaterials);
    }
}
