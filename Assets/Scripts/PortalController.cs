using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  // 引用 SceneManager 所需的命名空間

public class PortalController : MonoBehaviour
{
    // 設定目標場景名稱
    public string targetSceneName;

    // 當玩家進入傳送門的範圍時觸發
    private void OnTriggerEnter(Collider other)
    {
        // 判斷碰到的是不是玩家
        if (other.CompareTag("Player"))
        {
            // 切換到指定的場景
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
