using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildRendering : MonoBehaviour
{
    public bool Rendering { get; private set; } = true;
    public (float x, float z) InitRotation = new(0, 0);
    
    [SerializeField] bool defaultRendering = true;
    void Start()
    {
        InitRotation = new(transform.rotation.x, transform.rotation.z);
        SetChildMRenderers(defaultRendering);
    }

    void Update()
    {
        transform.rotation = new Quaternion(
                InitRotation.x,
                transform.rotation.y,
                InitRotation.z,
                transform.rotation.w
            );
    }
    public void ToggleRenderers() => SetChildMRenderers(!Rendering);

    public void SetChildMRenderers(bool b)
    {
        Rendering = b;
        foreach(Transform child in transform)
            child.GetComponent<MeshRenderer>().enabled = b;
    }
}
