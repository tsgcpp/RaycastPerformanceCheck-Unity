using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class TestRaycaster : MonoBehaviour
{
    public float maxDistance = 1000f;

    public LayerMask layerMask = ~0;

    public int raycastCountPerSeconds = 10000;

    private int layerMaskInteger;

    private string samplingName;

    void Start()
    {
        layerMaskInteger = layerMask;
        samplingName = "Raycast Performance Check - " + gameObject.name;
    }

    void Update()
    {
        Vector3 position = transform.position;
        Vector3 direction = transform.forward;

        Profiler.BeginSample(samplingName);
        Raycast(position, direction);
        Profiler.EndSample();
    }

    private void Raycast(Vector3 position, Vector3 direction)
    {
        for (int i = 0; i < raycastCountPerSeconds; ++i) {
            RaycastHit hit;
            bool isHit = Physics.Raycast(
                origin: position,
                direction: direction,
                hitInfo: out hit,
                maxDistance: maxDistance,
                layerMask: layerMaskInteger,
                queryTriggerInteraction: QueryTriggerInteraction.Ignore);
        }
    }
}
