using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider; // 參考音量調整的滑桿
    public GameObject settingsMenuGroup; // 參考設定選單的群組
    public AudioSource bgmSource; // 參考背景音樂的 AudioSource
    public Button SettingsButton; // 確認按鈕
    public Button ExitButton; // 退出按鈕

    private bool isSettingsOpen = false; // 用來追踪設定選單的狀態
    public float minVolume = 0.1f; // 最小音量值

    void Start()
    {
        // 從 PlayerPrefs 加載音量設置，如果沒有設置，則默認為 1（最大音量）
        if (PlayerPrefs.HasKey("volume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("volume");
            bgmSource.volume = Mathf.Lerp(minVolume, 1f, savedVolume);
            volumeSlider.value = savedVolume; // 更新滑桿的值
        }
        else
        {
            bgmSource.volume = 1f; // 默認最大音量
        }

        // 開始時隱藏設定選單
        settingsMenuGroup.SetActive(false);

        // 綁定確認按鈕的點擊事件
        SettingsButton.onClick.AddListener(OnConfirmButtonClicked);
        ExitButton.onClick.AddListener(OnExitButtonClicked); //退出按鈕的點擊事件
    }

    void Update()
    {
        // 檢測三條線按鈕 (≡) 或按下的「Submit」鍵來開啟或關閉設定選單
        if (Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            OnConfirmButtonClicked();
        }
        // 檢測 X 鍵關閉應用程式
        if (Input.GetKeyDown(KeyCode.JoystickButton2))
        {
            OnExitButtonClicked();
        }
        // 使用控制器的水平軸來調整音量滑桿
        float horizontalInput = Input.GetAxis("Horizontal");
        if (isSettingsOpen && Mathf.Abs(horizontalInput) > 0.1f)
        {
            AdjustVolume(horizontalInput);
        }
    }

    // 調整音量的自定義方法
    void AdjustVolume(float input)
    {
        // 根據控制器的輸入值增減滑桿的值
        float newVolume = Mathf.Clamp(volumeSlider.value + input * 0.01f, 0f, 1f);
        volumeSlider.value = newVolume;
        SetVolume(newVolume); // 更新音量
    }

    // 當滑桿值改變時，設置背景音樂音量並保存
    public void SetVolume(float volume)
    {
        bgmSource.volume = Mathf.Lerp(minVolume, 1f, volume);
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.Save(); // 強制保存 PlayerPrefs
    }

    // 當確認按鈕或 Y 鍵點擊時打開或關閉設定選單
    void OnConfirmButtonClicked()
    {
        isSettingsOpen = !isSettingsOpen;
        settingsMenuGroup.SetActive(isSettingsOpen);
        Debug.Log("設定選單已" + (isSettingsOpen ? "打開" : "關閉"));
    }

    // 當退出按鈕或 X 鍵點擊時退出遊戲
    void OnExitButtonClicked()
    {
        Debug.Log("退出遊戲");
        Application.Quit();
    }
}
