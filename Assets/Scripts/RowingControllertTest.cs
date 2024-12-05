using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowingControllertTest : MonoBehaviour
{
    public Transform leftController; // 左控制器
    public Transform rightController; // 右控制器
    public float rowingThreshold = 0.2f; // 划動距離閾值
    public float forwardForce = 5f; // 推力大小

    private Vector3 leftStartPos;
    private Vector3 rightStartPos;
    private bool isRowing = false;

    private Rigidbody boatRigidbody;

    void Start()
    {
        // 獲取船的剛體
        boatRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 偵測划船動作
        DetectRowingMotion();
    }

    void DetectRowingMotion()
    {
        // 初始化划船起點
        if (!isRowing)
        {
            leftStartPos = leftController.position;
            rightStartPos = rightController.position;
            isRowing = true;
        }

        // 計算左右控制器的划動距離
        float leftDistance = Vector3.Distance(leftController.position, leftStartPos);
        float rightDistance = Vector3.Distance(rightController.position, rightStartPos);

        // 如果划動距離超過閾值，進行推動
        if (leftDistance > rowingThreshold && rightDistance > rowingThreshold)
        {
            Row();
            isRowing = false; // 重置划船狀態
        }
    }

    void Row()
    {
        // 給船施加向前的推力
        Vector3 forwardDirection = transform.forward;
        boatRigidbody.AddForce(forwardDirection * forwardForce, ForceMode.Impulse);

        Debug.Log("Rowing action detected! Boat is moving forward.");
    }
}
