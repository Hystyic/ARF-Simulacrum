using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StartGame : MonoBehaviour
{
    [SerializeField] Button _button;
    [SerializeField] ARList _arlist = null;

    // Start is called before the first frame update
    void Start()
    {
        _button = GetComponent<Button>();
        if (_arlist is null)
            _arlist = ARList.Instance;

        _button.onClick.AddListener(() => {
            // Global.spawnedObjects = _arlist.SpawnedObjects;
            SceneManager.LoadScene(Global.gameScene);
        });
    }

    void Update()
    {
        _button.interactable = _arlist.AreAllObjectsPlaced();
    }
}
