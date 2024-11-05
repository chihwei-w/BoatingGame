using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("GoScene"))
        {
            SceneManager.LoadScene(collision.gameObject.GetComponent<SceneInfo>().SceneName);
        }
    }
}
