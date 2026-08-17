using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetScript : MonoBehaviour, IHittable
{
    [SerializeField] private MeshRenderer renderer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip targetHitSound;
    [SerializeField] private Material redOffMat;
    [SerializeField] private Material whiteOffMat;
    private Material redOnMat;
    private Material whiteOnMat;

    public Action onHit;
    public bool isOn;
    
    private List<Material> targetMaterials;
    private const int RedIndex = 1;
    private const int WhiteIndex = 2;

    private void Awake()
    {
        targetMaterials = renderer.materials.ToList();
        
        redOnMat = new Material(targetMaterials[RedIndex]);
        whiteOnMat = new Material(targetMaterials[WhiteIndex]);
    }

    public void OnHit()
    {
        onHit?.Invoke();
    }
    
    public void TurnOnTargetInstant()
    {
        SetMats(redOnMat, whiteOnMat);
        isOn = true;
    }

    public void TurnOffTargetInstant()
    {
        SetMats(redOffMat, whiteOffMat);
        isOn = false;
    }

    public void TurnOffTarget()
    {
        StartCoroutine(TurnOffTargetRoutine());
        
        isOn = false;
        return;
        
        IEnumerator TurnOffTargetRoutine()
        {
            audioSource.PlayOneShot(targetHitSound);
            
            for (var time = 0f; time < 0.5f; time += Time.deltaTime)
            {
                //Make the colors more similar to their goal colors using Lerp
                targetMaterials[RedIndex].Lerp(targetMaterials[RedIndex], redOffMat, 0.06f);
                targetMaterials[WhiteIndex].Lerp(targetMaterials[WhiteIndex], whiteOffMat, 0.06f);

                //Apply the materials
                renderer.SetMaterials(targetMaterials);

                yield return null;
            }
        }
    }
    
    private void SetMats(Material red, Material white)
    {
        targetMaterials[RedIndex].CopyPropertiesFromMaterial(red);
        targetMaterials[WhiteIndex].CopyPropertiesFromMaterial(white);
        
        renderer.SetMaterials(targetMaterials);
    }
}
