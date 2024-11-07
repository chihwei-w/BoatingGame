using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmNote : MonoBehaviour
{
    private Vector3 startPosition; // 音符的生成位置
    private Vector3 targetPosition; // 音符的目標位置
    private float travelTime; // 音符移動的時間
    private float spawnTime; // 音符生成時的時間
    public Action onMissed; // 當音符超過時間沒打中時的回調
    private bool isHit = false; // 音符是否被打擊過
    public bool isInHitBox = false; // 確認音符是否在判定區域內

    // 設定音符的目標位置和移動時間
    public void SetTarget(Vector3 target, float time)
    {
        startPosition = transform.position; // 紀錄音符的初始生成位置
        targetPosition = target; // 紀錄音符的目標位置
        travelTime = time * 1.5f; // 設定音符需要的移動時間
        spawnTime = Time.time; // 紀錄生成時的時間
        isHit = false; // 初始化為未被打擊狀態
        Debug.Log("音符已生成，目標位置設置為: " + targetPosition);
    }

    void Update()
    {
        if (isHit)
            return; // 如果音符已經被打擊，則不再執行後續邏輯

        float elapsedTime = (Time.time - spawnTime) / travelTime;
        float smoothStep = Mathf.SmoothStep(0, 1, elapsedTime);
        transform.position = Vector3.Lerp(startPosition, targetPosition, smoothStep);

        // 當音符到達目標點且未被打擊時，觸發 onMissed 回調
        if (elapsedTime >= 1f && !isHit)
        {
            if (onMissed != null)
            {
                onMissed.Invoke();
            }
            else
            {
                Debug.LogWarning("onMissed 回調未設置或為空");
            }
            Destroy(gameObject); // 音符到達目標點後銷毀
        }
    }

    // 當音符被打擊時調用
    public void Hit()
    {
        if (!isHit)
        {
            isHit = true; // 設定音符為已被打擊
            Debug.Log("音符已被打擊，銷毀音符: " + gameObject.name);
            Destroy(gameObject); // 被打擊後立即銷毀音符
        }
    }

    // 獲取音符與當前音樂時間的差異
    public float GetTimeDifference(float currentTime)
    {
        return Mathf.Abs(currentTime - (spawnTime + travelTime));
    }
}
