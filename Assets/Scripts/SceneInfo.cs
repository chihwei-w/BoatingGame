using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInfo : MonoBehaviour
{
    public string SceneName = null; // 場景名稱

    void Start()
    {
        if (string.IsNullOrEmpty(SceneName))
        {
            Debug.LogWarning("SceneInfo 組件的 SceneName 未設置或為空，請確認正確設置。");
        }
        else
        {
            Debug.Log("SceneInfo 初始化完成，場景名稱為: " + SceneName);
        }
    }
}
