using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    Material material;
    [SerializeField] Vector2 scrollVertor2;//偏移量
    // Start is called before the first frame update
    void Awake() 
    {
        material = GetComponent<Renderer>().material;//n通过渲染器组件取得材质的值；
    }
    // Update is called once per frame
    /*void Update()
    {
        material.mainTextureOffset += scrollVertor2 * Time.deltaTime;
    }*/
    IEnumerator Start()
    {
        while(GameManager.GameState != GameState.GameOver)
        {
            material.mainTextureOffset += scrollVertor2 * Time.deltaTime;

            yield return null;
        }
    }
}
