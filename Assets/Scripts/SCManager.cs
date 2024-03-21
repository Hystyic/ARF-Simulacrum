using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class SCManager : MonoBehaviour
{
    public static SCManager Instance { get; private set; } = null;

    public static ARList ARList { get; private set; } = null;
    public static ARSession ARSession { get; private set; } = null;

    void Start()
    {
        if (Instance is not null)
            Destroy(this.gameObject);

        SceneManager.sceneLoaded += (sc, _) => {
            switch (sc.name)
            {
                case Global.gameScene:
                    ARList.AllowTouch = true;
                    break;

                case Global.menuScene:
                case Global.menuSceneAfter:
                    ARList.AllowTouch = false;
                    // ARList.Instance.ResetRendering();
                    ARList.Instance.DestroySpawned();
                    ARList.Instance.EnablePlanes();
                    ARSession.Reset();
                    break;

                default: break;
            }
#if true
            // var arSession = FindAnyObjectByType<ARSession>();
            // arSession.Reset();
#else
            var xrManagerSettings = UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager;
            xrManagerSettings.DeinitializeLoader();
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex); // reload current scene
            xrManagerSettings.InitializeLoaderSync();
#endif
            
            
        };

        Instance = this;
        ARList = ARList.Instance;
        ARSession = FindObjectOfType<ARSession>();
        
        DontDestroyOnLoad(Instance);
        DontDestroyOnLoad(ARList);
        DontDestroyOnLoad(ARSession);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
