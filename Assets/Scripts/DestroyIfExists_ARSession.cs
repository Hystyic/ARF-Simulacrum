using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARCore;

[RequireComponent(typeof(UnityEngine.XR.ARFoundation.ARSession))]
public class DestroyIfExists_ARSession : MonoBehaviour
{
    static DestroyIfExists_ARSession Instance = null;
    void Awake()
    {
        if (Instance is not null)
            Destroy(this.gameObject);

        Instance = this;
    }
}
