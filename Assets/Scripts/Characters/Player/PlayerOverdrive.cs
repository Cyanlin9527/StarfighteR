using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerOverdrive : MonoBehaviour
{
    public static UnityAction on = delegate {};
    public static UnityAction off = delegate {};
    [SerializeField]GameObject triggerVFX;
    [SerializeField]GameObject engineVFXNormal;
    [SerializeField]GameObject engineVFXOverdrive;

    [SerializeField]AudioDate onSFX;
    [SerializeField]AudioDate offSFX;

    void Awake() 
    {
        on += On;
        off += Off;
    }
    void OnDestory()
    {
        on -= On;
        off -= Off;
    }
    void On()
    {
        triggerVFX.SetActive(true);
        engineVFXNormal.SetActive(false);
        engineVFXOverdrive.SetActive(true);
        AutoManager.Instance.PlayRandomSFX(onSFX);
    }

    void Off()
    {
        engineVFXOverdrive.SetActive(false); 
        engineVFXNormal.SetActive(true);
        AutoManager.Instance.PlayRandomSFX(offSFX);
    }
}
