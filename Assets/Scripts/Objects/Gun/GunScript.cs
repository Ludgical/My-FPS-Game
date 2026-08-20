using System;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    private References refs;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gunFiringSound;
    [SerializeField] private MeshRenderer[] renderers;
    [SerializeField] private GameObject onHitParticles;
    [SerializeField] private AudioClip hitWallSound;
    [SerializeField] private Transform gunPivot;
    
    private float timeSinceLastShot;

    public Action onFire;

    private void Start()
    {
        refs = References.Refs;
        
        renderers = GetComponentsInChildren<MeshRenderer>();
        
        SetZValue();
        Settings.Player.FOV.onUpdated += SetZValue;
        
        SetVisible(false);
        refs.gameLogic.onPlay += () => SetVisible(true);
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }

    private void SetZValue()
    {
        //Set the gun's z-value based on the player's fov
        var gunPos = gunPivot.localPosition;
        gunPos.z = Settings.Player.FOV.Value / -150 + 1.24f;
        gunPivot.localPosition = gunPos;
    }

    public void TryFire()
    {
        //Can't fire if enough time hasn't passed since the last shot
        if (!refs.gameLogic.gameIsOn || timeSinceLastShot < refs.gunData.cooldown)
            return;
        timeSinceLastShot = 0;
        
        Fire();
    }

    private void Fire()
    {
        audioSource.PlayOneShot(gunFiringSound);
        refs.player.animator.SetTrigger("Fire");

        //Raycast forward from the camera, put everything it hit in an array and sort by distance
        var ray = new Ray(refs.camera.transform.position, refs.camera.transform.forward);
        var hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));
        
        //Go over every hit, starting with the closest
        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<IHittable>(out var hittable))
            {
                //If the raycast hit an IHittable, call the IHittable's OnHit method
                hittable.OnHit();
            }
            else if (hit.collider.isTrigger)
            {
                //If the raycast hit a trigger collider, check the next thing it hit
                continue;
            }
            
            OnHitParticles(hit.point, hit.normal, hit.collider.tag);
            break;
        }
        
        onFire?.Invoke();
    }

    private void OnHitParticles(Vector3 position, Vector3 rotation, string hitTag)
    {
        //Create the particle object and destroy it after 0.5 seconds
        var particles = Instantiate(onHitParticles, position, Quaternion.LookRotation(rotation));
        Destroy(particles, 0.5f);
        
        if (hitTag is "Wall" or "Ground")
            particles.GetComponent<AudioSource>().PlayOneShot(hitWallSound);
    }

    private void SetVisible(bool visible)
    {
        foreach (var renderer in renderers)
            renderer.enabled = visible;
    }
}
