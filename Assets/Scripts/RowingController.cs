using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RowingController : MonoBehaviour
{
    // 公開變數
    public OVRInput.Button actionButton = OVRInput.Button.One; // Meta Quest Pro 的 A 按鈕
    public bool enableActionButton = false; // 是否啟用按鍵 A 來測試打擊功能
    public bool enableSpaceKey = false; // 是否啟用空白鍵來測試打擊功能
    private bool hasRowed = false; // 紀錄是否已經執行了一次划船動作

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
        OVRInput.Update(); // 更新 OVRInput 狀態，以確保初始化
        Debug.Log("OVRInput 在場景載入後重新初始化成功");
    }

    void Update()
    {
        // 控制器A按鍵
        if (enableActionButton && OVRInput.GetDown(actionButton))
        {
            OnRowingAction(); // 執行划船動作
        }

        // 空白建
        if (enableSpaceKey && Input.GetKeyDown(KeyCode.Space))
        {
            OnRowingAction(); // 執行划船動作
        }
    }

    // 控制器判定
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("進入判定區域的物件: " + other.name); // 檢查進入的物件名稱
        OnRowingAction(); // 執行划船動作
    }

    // 當划船動作被觸發時執行的函數
    void OnRowingAction()
    {
        // 找到 RhythmManager 並觸發打擊事件
        RhythmManager rhythmManager = FindObjectOfType<RhythmManager>();
        if (rhythmManager != null)
        {
            rhythmManager.TriggerBeat(); // 通知節奏管理器進行打擊判定
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
