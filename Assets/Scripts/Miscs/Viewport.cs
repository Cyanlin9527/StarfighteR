using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

/// <summary>
/// 通过世界视口中心点的坐标来控制移动的范围。
/// </summary>
public class Viewport : Singleton<Viewport>
{
    float minX,minY,maxX,maxY,middlex;
    public float MaxX => maxX;
    private void Start() 
    {
        Camera mainCamera = Camera.main;

        Vector2 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0f,0f));
        Vector2 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1f,1f));
        middlex = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f)).x;

        maxX = topRight.x;
        maxY = topRight.y;
        minX = bottomLeft.x;
        minY = bottomLeft.y;

    }
    public Vector3 PlayerMoveablePosition(Vector3 playerPosition, float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Clamp(playerPosition.x, minX + paddingX, maxX - paddingY);
        position.y = Mathf.Clamp(playerPosition.y, minY + paddingY, maxY - paddingY);

        return position;
    } 
    public Vector3 RandomEnemySpawnPosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;
        position.x = maxX + paddingX;
        position.y = Random.Range(minY + paddingY, maxY - paddingY);
        return position;

    }
    public Vector3 RandomRightHalfPosition(float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;
        position.x = Random.Range(middlex, maxX - paddingX);
        position.y = Random.Range(minY + paddingY, maxY - paddingY);
        return position;
    }
}
