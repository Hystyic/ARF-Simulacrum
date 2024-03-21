using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARListCaller : MonoBehaviour
{
    public void SetAllowTouch(bool b) => ARList.AllowTouch = b;
    public void SetIndex(int i) => ARList.Instance.SetIndex(i);
    public void SetIndexByBaseRef(GameObject b) => ARList.Instance.SetIndexByBaseRef_Void(b);
    public void SetIndexBySpawnedRef(GameObject s) => ARList.Instance.SetIndexBySpawnedRef_Void(s);
    public void LogCurrent() => ARList.Instance.LogCurrent();
}
