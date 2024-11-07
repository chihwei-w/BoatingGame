using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteParentController : MonoBehaviour
{
    public float moveSpeed = 1.0f; // 移動速度
    private bool isPaused = false; // 是否暫停移動

    void Update()
    {
        if (!isPaused)
        {
            // 控制所有音符的父物件移動
            transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
        }
    }

    // 暫停移動的方法
    public void PauseMovement()
    {
        isPaused = true;
        Debug.Log("音符移動已暫停");
    }

    // 恢復移動的方法
    public void ResumeMovement()
    {
        isPaused = false;
        Debug.Log("音符移動已恢復");
    }
}
