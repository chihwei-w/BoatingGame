using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;

public class WSAD : MonoBehaviour
{
    // 移動相關變數
    [SerializeField] float verticalInput, horizontalInput; // 垂直和水平輸入
    [SerializeField] float walkSpeed = 2f; // 走路速度
    [SerializeField] float runSpeed = 5f; // 跑步速度

    // 其他參考
    [SerializeField] Rigidbody playerRigidbody; // 玩家剛體，用於物理驅動
    [SerializeField] Transform rotationAxis; // 控制旋轉方向

    // 狀態變數
    float walkRunRatio = 0f; // 走跑比例
    public bool isMoving = false; // 是否在移動
    public bool isRunning = false; // 是否在跑步
    private void Update()
    {
        // 取得玩家的鍵盤輸入
        verticalInput = Input.GetAxis("Vertical"); // W = 1, S = -1, 不按 = 0
        horizontalInput = Input.GetAxis("Horizontal"); // A = -1, D = 1, 不按 = 0

        // 計算移動方向
        Vector3 moveDirection;
        moveDirection.x = horizontalInput; // 水平移動
        moveDirection.y = 0f; // Y 軸保持不變，防止垂直移動
        moveDirection.z = verticalInput; // 垂直移動

        // 根據角色的旋轉軸轉換移動方向
        moveDirection = rotationAxis.TransformDirection(moveDirection);

        // 檢測跑步狀態
        if (Input.GetKey(KeyCode.LeftShift))
        {
            walkRunRatio += Time.deltaTime * 5f; // 按住 Shift 時，走跑比例增加
        }
        else
        {
            walkRunRatio -= Time.deltaTime * 5f; // 放開 Shift 時，走跑比例減少
        }

        // 限制走跑比例在 0 到 1 之間
        walkRunRatio = Mathf.Clamp01(walkRunRatio);

        // 根據走跑比例動態計算速度
        moveDirection *= Mathf.Lerp(walkSpeed, runSpeed, walkRunRatio);

        // 保留垂直方向速度（例如重力影響）
        moveDirection.y = playerRigidbody.velocity.y;

        // 將移動方向應用到剛體
        playerRigidbody.velocity = moveDirection;

        // 判斷是否在移動
        isMoving = Mathf.Abs(verticalInput) > 0.1f || Mathf.Abs(horizontalInput) > 0.1f;
        // 判斷是否在跑步
        isRunning = walkRunRatio > 0.1f && isMoving;
    }
}
