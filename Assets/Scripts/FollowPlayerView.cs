using System.Collections;
using UnityEngine;

public class FollowPlayerView : MonoBehaviour
{
    public Transform playerCamera; // 設置為 CenterEyeAnchor
    public Vector3 offset = new Vector3(0, -0.5f, 1f); // 調整血條相對於視角的位置

    void Start()
    {
        if (playerCamera == null)
        {
            Debug.LogError("未設置 playerCamera，請分配 CenterEyeAnchor。");
            enabled = false; // 禁用腳本以防止運行錯誤
        }
    }

    void Update()
    {
        if (playerCamera != null)
        {
            // 讓血條跟隨玩家的頭部並保持固定距離
            transform.position =
                playerCamera.position
                + playerCamera.forward * offset.z
                + playerCamera.up * offset.y;
            transform.LookAt(playerCamera.position);
            transform.Rotate(0, 180, 0); // 旋轉血條，使其面向玩家
        }
    }
}
