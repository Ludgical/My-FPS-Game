using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RelayNodeChallenge : Challenge
{
    private RelayNodeChallengeData cd;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip failSound;
    [SerializeField] private GameObject nodePrefab;

    private Image screenImage;
    private List<GameObject> nodes;
    private float? lastNodeHitTime;
    private bool hitNodeThisShot;
    private int nodesHit;
    private bool waiting;
    private float timeBetweenHits;
    
    protected override void InitializeChallenge()
    {
        cd = refs.relayNodeChallengeData;
        
        screenImage = GameObject.FindGameObjectWithTag("ScreenImage").GetComponent<Image>();
        
        timeBetweenHits = cd.maxTimeBetweenHits;

        refs.gun.onFire += OnGunFire;
        
        SpawnNodes();
    }

    private void SpawnNodes()
    {
        nodes = new List<GameObject>(cd.nodeAmount);
        for (var i = 0; i < cd.nodeAmount; i++)
            //Try to spawn nodes until one if far enough away from all other nodes
            while(!TrySpawnNode(cd.radius, Random.value, Random.Range(cd.minY, cd.maxY)));
    }

    private bool TrySpawnNode(float radius, float angle, float y)
    {
        //Convert polar coordinates
        var angleRad = angle * 2 * Mathf.PI;
        var x = radius * Mathf.Sin(angleRad);
        var z = radius * Mathf.Cos(angleRad);
        var position = new Vector3(
            transform.position.x + x, y, transform.position.z + z);
        
        //Make sure the node isn't too close to another node
        foreach (var otherNodeObject in nodes)
        {
            var dist = Vector3.Distance(position, otherNodeObject.transform.position);
            if (dist < cd.nodeMinDistance)
                return false;
        }
        
        var direction = position - transform.position;
        direction.y = 0;
        var nodeObject = Instantiate(
            nodePrefab, position, Quaternion.LookRotation(direction));
        nodes.Add(nodeObject);
        
        var node = nodeObject.GetComponent<RelayNode>();
        node.onHit += () => OnNodeHit(node);

        return true;
    }

    private void OnNodeHit(RelayNode node)
    {
        if (!challengeStarted || waiting || node.isHit)
            return;
        
        node.TurnOn(cd.fadeOutSpeed);
        node.isHit = true;
        hitNodeThisShot = true;
        nodesHit++;
        lastNodeHitTime = Time.time;
        
        if (nodesHit == cd.nodeAmount)
            OnChallengeCompleted();
    }

    private void OnChallengeCompleted()
    {
        foreach (var node in nodes)
            Destroy(node, Random.Range(0, 0.4f));
        
        CompleteChallenge();
    }

    private void Update()
    {
        hitNodeThisShot = false;
        
        if (lastNodeHitTime == null || challengeCompleted)
            return;
        if (Time.time - lastNodeHitTime > timeBetweenHits)
            OnFailToHitNode();
    }
    
    private void OnGunFire()
    {
        if (challengeStarted && !challengeCompleted && !waiting && !hitNodeThisShot)
            OnFailToHitNode();
    }

    private void OnFailToHitNode()
    {
        audioSource.PlayOneShot(failSound);
        timeBetweenHits += cd.timeBetweenHitsIncrease;
        StartWaiting();
        StartCoroutine(StopWaiting(cd.waitTime));
    }

    private void StartWaiting()
    {
        waiting = true;
        lastNodeHitTime = null;
        FadeScreenImageInAndOut(new Color(1, 0, 0, 0.05f));
    }
    private IEnumerator StopWaiting(float delay)
    {
        yield return new WaitForSeconds(delay);
        waiting = false;
        FadeScreenImageInAndOut(new Color(0, 1, 0, 0.05f));
    }

    private void FadeScreenImageInAndOut(Color color)
    {
        var transparent = new Color(0, 0, 0, 0);
        StartCoroutine(FadeScreenImageRoutine(transparent, color, 0));
        StartCoroutine(FadeScreenImageRoutine(color, transparent, cd.fadeScreenImageTime));
    }
    
    private IEnumerator FadeScreenImageRoutine(Color start, Color end, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        for (var time = 0f; time < cd.fadeScreenImageTime; time += Time.deltaTime)
        {
            screenImage.color = Color.Lerp(start, end, time / cd.fadeScreenImageTime);
            yield return null;
        }

        screenImage.color = end;
    }

    protected override void StartChallenge() { }
}
