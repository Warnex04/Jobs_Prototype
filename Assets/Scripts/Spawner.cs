using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // The set of targets and seekers are fixed, so rather than 
    // retrieve them every frame, we'll cache 
    // their transforms in these field.
    public static Transform[] TargetTransforms;
    public static Transform[] SeekerTransforms;
    public GameObject seekerPrefab;
    public GameObject targetPrefab;
    public int numSeekers;
    public int numTargets;
    public Vector2 Bounds;

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(123);

        TargetTransforms = new Transform[numTargets];
        for (int i = 0; i < numTargets; i++)
        {
            GameObject target = GameObject.Instantiate(targetPrefab);
            Target targetScript = target.GetComponent<Target>();
            Vector2 dir = Random.insideUnitCircle;
            targetScript.Direction = new Vector3(dir.x,0,dir.y);
            TargetTransforms[i] = target.transform;
            target.transform.localPosition = new Vector3(Random.Range(0, Bounds.x), 0, Random.Range(0, Bounds.y));
        }

        SeekerTransforms = new Transform[numSeekers];
        for (int i = 0; i < numSeekers; i++)
        {
            GameObject seeker = GameObject.Instantiate(seekerPrefab);
            Seeker seekerScript = seeker.GetComponent<Seeker>();
            Vector2 dir = Random.insideUnitCircle;
            seekerScript.Direction = new Vector3(dir.x, 0, dir.y);
            SeekerTransforms[i] = seeker.transform;
            seeker.transform.localPosition = new Vector3(Random.Range(0, Bounds.x), 0, Random.Range(0, Bounds.y));
        }

    }

}