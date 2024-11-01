using System.Collections;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    public float moveSpeed = 5f; // 船的移動速度
    private bool isMoving = false;
    private float moveDistancePerHit; // 每次成功打擊後的初始移動距離

    // 設置初始移動距離
    public void SetInitialMoveDistance(float distance)
    {
        moveDistancePerHit = distance;
        Debug.Log("初始移動距離：" + moveDistancePerHit);
    }

    // 控制船向前移動一段固定距離
    public void MoveForward()
    {
        if (!isMoving && moveDistancePerHit > 0)
        {
            StartCoroutine(MoveBoatSmoothly(moveDistancePerHit));
        }
        else
        {
            Debug.LogWarning("無法移動 - 正在移動中或移動距離為 0");
        }
    }

    // 使用 Coroutine 平滑移動船
    IEnumerator MoveBoatSmoothly(float distance)
    {
        isMoving = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + transform.forward * distance;

        float elapsedTime = 0f;
        float duration = distance / moveSpeed;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        isMoving = false;
    }
}
