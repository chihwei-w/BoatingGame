using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RowingController : MonoBehaviour
{
    public OVRInput.Button actionButton = OVRInput.Button.One; // Meta Quest Pro 的 A 按鈕
    private bool isInitialized = false; // 紀錄是否已經初始化

    void Start()
    {
        InitializeOVRInput(); // 在 Start 中初始化控制器
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // 註冊場景載入事件
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 取消場景載入事件
    }

    // 每次場景載入完成時重新初始化控制器
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeOVRInput(); // 場景載入後初始化控制器
    }

    // 自定義初始化方法，確保控制器在場景載入時被初始化
    void InitializeOVRInput()
    {
        if (!isInitialized)
        {
            OVRInput.Update(); // 更新 OVRInput 狀態，以確保初始化
            Debug.Log("OVRInput 在場景載入後重新初始化成功");
            isInitialized = true;
        }
        else
        {
            Debug.Log("OVRInput 已經初始化，跳過重複初始化");
        }
    }

    void Update()
    {
        // 控制器 A 按鍵
        if (OVRInput.GetDown(actionButton))
        {
            Debug.Log("控制器 A 按鈕被按下");
            OnRowingAction(); // 執行划船動作
        }

        // 鍵盤空白鍵
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("鍵盤空白鍵被按下");
            OnRowingAction(); // 執行划船動作
        }
    }

    // 控制器判定
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("進入判定區域的物件: " + other.name);
        OnRowingAction(); // 執行划船動作
    }

    // 當划船動作被觸發時執行的函數
    void OnRowingAction()
    {
        RhythmManager rhythmManager = FindObjectOfType<RhythmManager>();
        if (rhythmManager != null)
        {
            Debug.Log("觸發划船動作，通知 RhythmManager 檢查打擊");
            rhythmManager.TriggerBeat(); // 通知 RhythmManager 檢查打擊
        }
        else
        {
            Debug.LogWarning("找不到 RhythmManager，無法觸發打擊");
        }
    }
}
