using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

using TMPro;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    ARPlaneManager planeManager;
    [SerializeField] TMP_Text keysText;
    [SerializeField] GameObject victoryUI;

    // Start is called before the first frame update
    void Start()
    {
        planeManager = ARList.Instance.GetComponent<ARPlaneManager>();
    }

    // Update is called once per frame
    void Update()
    {
        int left = ARList.Instance.ObjectsLeft;
        bool b = left == 0;
        keysText.text = "Keys left: " + left;
        keysText.transform.parent.gameObject.SetActive(!b);
        victoryUI.SetActive(b);
    }

    public void TogglePlanes()
    {
        planeManager.enabled = !planeManager.enabled;

        foreach (var p in planeManager.trackables)
            p.gameObject.SetActive(planeManager.enabled);
    }

    public void BackToMenu()
        => SceneManager.LoadScene(Global.menuSceneAfter);
}
