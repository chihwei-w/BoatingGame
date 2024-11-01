using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerView : MonoBehaviour
{
    public Transform playerCamera; // 設置為CenterEyeAnchor
    public Vector3 offset = new Vector3(0, -0.5f, 1f); // 調整血條相對於視角的位置

    void Update()
    {
        // 讓血條跟隨玩家的頭部並保持固定距離
        transform.position =
            playerCamera.position + playerCamera.forward * offset.z + playerCamera.up * offset.y;
        transform.LookAt(playerCamera.position);
        transform.Rotate(0, 180, 0); // 旋轉血條，使其面向玩家
    }
}
