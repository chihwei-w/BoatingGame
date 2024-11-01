using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteParentController : MonoBehaviour
{
    public float moveSpeed = 1.0f; // 移動速度

    void Update()
    {
        // 控制所有音符的父物件移動
        transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
    }
}
