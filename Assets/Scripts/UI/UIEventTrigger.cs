using System.Collections;
using System.Collections.Generic;   
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventTrigger : MonoBehaviour, IPointerEnterHandler,IPointerDownHandler,ISelectHandler,ISubmitHandler
{
    [SerializeField] AudioDate selectSFX;
    [SerializeField] AudioDate submitSFX;

    public void OnPointerEnter(PointerEventData eventData)
    {
        AutoManager.Instance.PlaySFX(selectSFX);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AutoManager.Instance.PlaySFX(submitSFX);
    }

    public void OnSelect(BaseEventData eventData)
    {
        AutoManager.Instance.PlaySFX(selectSFX);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        AutoManager.Instance.PlaySFX(submitSFX);
    }
}