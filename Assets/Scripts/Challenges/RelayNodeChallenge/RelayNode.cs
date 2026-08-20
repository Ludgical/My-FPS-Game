using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelayNode : MonoBehaviour, IHittable
{
    public Action onHit;
    [NonSerialized] public bool isHit;

    [SerializeField] private Renderer renderer;
    [SerializeField] private Material[] onMats;
    private List<Material> currentMats;
    
    public void OnHit()
    {
        onHit?.Invoke();
    }

    public void TurnOn(float fadeOutSpeed)
    {
        if (renderer.materials.Length != onMats.Length)
            throw new Exception("There are not the same amount of materials on the node as in the on mats");
        
        //Set currentMats to the materials currently on the node
        currentMats = new List<Material>(onMats.Length);
        for (var i = 0; i < onMats.Length; i++)
            currentMats.Add(new Material(renderer.materials[i]));

        StartCoroutine(Routine());
        return;

        IEnumerator Routine()
        {
            //Fade the node for 0.5 seconds
            for (var time = 0f; time < 0.5f; time += Time.deltaTime)
            {
                //Make the colors more similar to their goal colors using Lerp
                for (var i = 0; i < onMats.Length; i++)
                    currentMats[i].Lerp(currentMats[i], onMats[i], fadeOutSpeed * Time.deltaTime);

                //Apply the materials
                renderer.SetMaterials(currentMats);

                yield return null;
            }
        }
    }
}
