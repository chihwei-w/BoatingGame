using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OVRInputTest : MonoBehaviour
{
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            Debug.Log("按鍵 One 被偵測到！");
        }

        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            Debug.Log("按鍵 Two 被偵測到！");
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            Debug.Log("食指觸發器被偵測到！");
        }
    }
}
