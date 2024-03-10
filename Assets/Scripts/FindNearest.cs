using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindNearest : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        foreach (Transform seekerTransform in Spawner.SeekerTransforms)
        {
            Vector3 seekerPos = seekerTransform.localPosition;
            Vector3 nearestTargetPos = default;
            float nearestDistSq = float.MaxValue;
            
            foreach(Transform targetTransform in Spawner.TargetTransforms)
            {
                Vector3 offset = targetTransform.localPosition - seekerPos;
                float distSq = offset.sqrMagnitude;
                if (distSq < nearestDistSq)
                {
                    nearestDistSq = distSq;
                    nearestTargetPos = targetTransform.localPosition;
                }
            }
            Debug.DrawLine(seekerPos, nearestTargetPos);
        }
        
    }
}
