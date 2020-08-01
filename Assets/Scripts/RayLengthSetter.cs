using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayLengthSetter : MonoBehaviour
{
    [SerializeField] private GameObject ray;

    public float Length {
        get {
            return ray.transform.localScale.z;
        }
        set {
            var scale = ray.transform.localScale;
            scale.z = value;
            ray.transform.localScale = scale;
        }
    }
}
