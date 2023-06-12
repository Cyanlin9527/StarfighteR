using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBar : MonoBehaviour
{
    [SerializeField]Image fillImageBack;
    [SerializeField]Image fillImageFront;
    [SerializeField]bool delayFill = true;  //等待一段时间后再填充
    [SerializeField]float fillDelay = 0.5f;
    [SerializeField]float fillSpeed = 0.1f;
    float currentFillAmount;
    protected float targetFillAmount;
    float t;
    float previousFillAmount;
    WaitForSeconds waitForDelayFill; 
    Coroutine bufferedFillingCoroutine;
    private void Awake() 
    {
        
        if(TryGetComponent<Canvas>(out Canvas canvas))
        {
            canvas.worldCamera = Camera.main;
        }

        waitForDelayFill = new WaitForSeconds(fillDelay);
    }

    void OnDisable() 
    {
        StopAllCoroutines();
    }
    public virtual void Initialize(float currentValue, float maxValue)
    {
        currentFillAmount = currentValue / maxValue;
        targetFillAmount = currentFillAmount;
        fillImageBack.fillAmount = currentFillAmount;
        fillImageFront.fillAmount = currentFillAmount;
    }
    public void UpdateStats(float currentValue, float maxValue)
    {
        targetFillAmount = currentValue / maxValue;

        if(bufferedFillingCoroutine != null)
        {
            StopCoroutine(bufferedFillingCoroutine);
        }
        //状态减少时
        if(currentFillAmount > targetFillAmount)
        {
            //前面的图片填充值等于目标填充值
            fillImageFront.fillAmount = targetFillAmount;
            //后面的填充值慢慢减少。
            bufferedFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageBack));

            return;
        }
        //状态增加时
        if(currentFillAmount < targetFillAmount)
        {
            //后面的图片填充值等于目标填充值
            fillImageBack.fillAmount = targetFillAmount;
            //前面的填充值慢慢增加。
            bufferedFillingCoroutine = StartCoroutine(BufferedFillingCoroutine(fillImageFront));
        }
    }
    

    protected virtual IEnumerator BufferedFillingCoroutine(Image image)
    {
        if(delayFill)
        {
            yield return waitForDelayFill;
        }
        t = 0f;

        previousFillAmount = currentFillAmount;
        while(t < 1f)
        {
            t += Time.deltaTime * fillSpeed;
            currentFillAmount = Mathf.Lerp(previousFillAmount, targetFillAmount, t);
            image.fillAmount = currentFillAmount;
            yield return null;
        }
        
    }
}
