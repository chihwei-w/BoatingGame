using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterScene : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("GoScene"))
        {
            SceneInfo sceneInfo = collision.gameObject.GetComponent<SceneInfo>();
            if (sceneInfo != null && !string.IsNullOrEmpty(sceneInfo.SceneName))
            {
                Debug.Log("加載場景: " + sceneInfo.SceneName);
                SceneManager.LoadScene(sceneInfo.SceneName);
            }
            else
            {
                Debug.LogWarning("SceneInfo 組件未找到或場景名稱為空");
            }
        }
    }
}
