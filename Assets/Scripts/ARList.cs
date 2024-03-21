using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// For tutorial video, see my YouTube channel: <seealso href="https://www.youtube.com/@xiennastudio">YouTube channel</seealso>
/// How to use this script:
/// - Add ARPlaneManager to XROrigin GameObject.
/// - Add ARRaycastManager to XROrigin GameObject.
/// - Attach this script to XROrigin GameObject.
/// - Add the prefab that will be spawned to the <see cref="basePrefabs"/>
/// 
/// Touch to place the <see cref="basePrefabs"/> object on the touch position.
/// Will only placed the object if the touch position is on detected trackables.
/// Move the existing spawned object on the touch position.
/// Using Unity old input system.
/// </summary>
[HelpURL("https://youtu.be/HkNVp04GOEI")]
[RequireComponent(typeof(ARRaycastManager))]
public class ARList : MonoBehaviour
{
    /// <summary>
    /// The prefab that will be instantiated on touch.
    /// </summary>
    [SerializeField]
    [Tooltip("Instantiates this prefabs from this list on a plane at the touch location.")]
    public List<GameObject> basePrefabs;

    [SerializeField] LayerMask keyMask;

    /// <summary>
    /// The instantiated objects.
    /// </summary>
    public List<GameObject> SpawnedObjects { get; private set; }

    /// <summary>
    /// Current object index.
    /// </summary>
    int index = 0;

    public static bool AllowTouch { get; set; } = false;

    ARRaycastManager aRRaycastManager;
    List<ARRaycastHit> hits = new();
    [SerializeField] Camera _arCamera;

    public static ARList Instance { get; private set; } = null;
    public int ObjectsLeft { get => SpawnedObjects.Where(go => go.GetComponent<ChildRendering>().Rendering).Count(); }

    void Awake()
    {
        if (Instance is not null)
            Destroy(this.gameObject);

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (basePrefabs.Count == 0)
            Debug.LogError("No base prefabs found while instantiating ARList!");

        SpawnedObjects   = Enumerable
                            .Repeat<GameObject>(null, basePrefabs.Count)
                            .ToList();
        aRRaycastManager = GetComponent<ARRaycastManager>();
    }

    [HideInInspector] public bool placingScene = true;

    void Update()
    {
        // Check if there is existing touch.
        if (!AllowTouch || Input.touchCount == 0)
            return;

        // aRRaycastManager.raycastPrefab = basePrefabs[index];
        // Check if the raycast hit any trackables.
        if (SceneManager.GetActiveScene().name != Global.gameScene)
            PlaceByIndex();
        else
            FindSpawnedOnTouch();
    }

    void PlaceByIndex()
    {
        if (!aRRaycastManager.Raycast(Input.GetTouch(0).position, hits, TrackableType.PlaneWithinPolygon))
            return;

        // Raycast hits are sorted by distance, so the first hit means the closest.
        var hitPose = hits[0].pose;

        // Check if there is already spawned object. If there is none, instantiated the prefab.
        if (SpawnedObjects[index] == null)
        {
            SpawnedObjects[index] = Instantiate(basePrefabs[index], hitPose.position, hitPose.rotation);
            if (!SpawnedObjects[index].TryGetComponent<ARAnchor>(out _))
                SpawnedObjects[index].AddComponent<ARAnchor>();

            DontDestroyOnLoad(SpawnedObjects[index]);
        }
        else
        {
            // Change the spawned object position and rotation to the touch position.
            SpawnedObjects[index].transform.position = hitPose.position;
            SpawnedObjects[index].transform.rotation = hitPose.rotation;
        }

        // To make the spawned object always look at the camera. Delete if not needed.
        Vector3 lookPos = Camera.main.transform.position - SpawnedObjects[index].transform.position;
        lookPos.y = 0;
        SpawnedObjects[index].GetComponent<ChildRendering>().SetChildMRenderers(true);
        SpawnedObjects[index].transform.rotation = Quaternion.LookRotation(lookPos);
    }

    void FindSpawnedOnTouch()
    {
        // hits.All(h => SpawnedObjects.Contains(h.));
        // throw new NotImplementedException();
        Ray ray = _arCamera.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50f, layerMask: keyMask))
        {
            var go = hit.collider.gameObject;
            go.GetComponent<ChildRendering>().SetChildMRenderers(false);
        }
    }

    public void SetIndex(int index)
    {
        if (index < 0)
            throw new IndexOutOfRangeException($"Index for ARList object cannot be negative - Input: {index}");

        if (index >= basePrefabs.Count)
            throw new IndexOutOfRangeException(
                "Index for ARList object out of range - " 
                    + $"Input: {index}, "
                    + $"Last index: {basePrefabs.Count - 1}");

        this.index = index;
    }

    public bool SetIndexByBaseRef(GameObject go)
    {
        if (go == null)
            return false;
        
        int nextIndex = basePrefabs.IndexOf(go);
        if (nextIndex == -1)
            return false;

        index = nextIndex;
        return true;
    }

    public bool SetIndexBySpawnedRef(GameObject go)
    {
        if (go == null)
            return false;

        int nextIndex = SpawnedObjects.IndexOf(go);
        if (nextIndex == -1)
            return false;

        index = nextIndex;
        return true;
    }

    public void SetIndexByBaseRef_Void(GameObject go)
        => SetIndexByBaseRef(go);

    public void SetIndexBySpawnedRef_Void(GameObject go)
        => SetIndexBySpawnedRef(go);

    public void ResetRendering()
    {
        foreach (var go in SpawnedObjects)
            if (go is not null && go)
                go.GetComponent<ChildRendering>().SetChildMRenderers(true);
    }

    public void DestroySpawned()
    {
        for (int i = 0; i < SpawnedObjects.Count; i++)
        {
            var go = SpawnedObjects[i];
            if (go is null || !go) return;

            Destroy(go);
            SpawnedObjects[i] = null;
        }
    }

    public void EnablePlanes()
    {
        var planeManager = GetComponent<ARPlaneManager>();

        planeManager.enabled = true;
        foreach (var p in planeManager.trackables)
            p.gameObject.SetActive(true);
    }

    public bool AreAllObjectsPlaced()
        => SpawnedObjects.Count == basePrefabs.Count
        // Using .All instead of .Contains in case it uses '==' operator to look
        // for Unity GameObjects.
        && SpawnedObjects.All((go) => go is not null && go.GetComponent<ChildRendering>().Rendering);

    public bool AreAllObjectsFound()
        => SpawnedObjects.Count == basePrefabs.Count
        // Using .All instead of .Contains in case it uses '==' operator to look
        // for Unity GameObjects.
        && SpawnedObjects.All((go) => go is not null && !go.GetComponent<ChildRendering>().Rendering);

    public void LogCurrent()
        => Debug.Log($"Index: {index} - Base: {basePrefabs[index].name}");
}
