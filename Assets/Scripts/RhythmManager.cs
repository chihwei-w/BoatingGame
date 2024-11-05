using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RhythmManager : MonoBehaviour
{
    // 音樂和譜面相關的變數
    public AudioSource audioSource; // 音源，用於播放音樂
    public JsonParser jsonParser; // JSON 解析器，用於解析音符的譜面數據
    public List<TextAsset> jsonFiles; // 存儲所有譜面 JSON 文件
    public List<AudioClip> audioClips; // 存儲所有音樂剪輯
    public TextMeshProUGUI healthText; // 顯示玩家血量的 UI 文字
    public int playerHealth = 100; // 玩家初始血量
    private int currentSongIndex = 0; // 當前播放的歌曲索引
    private List<Note> notes; // 存儲音符信息
    public Slider healthBar; // 血量條 UI
    public float hitWindow = 10.0f; // 判定窗口，用於確定玩家打擊音符的誤差範圍

    // 遊戲結束和勝利面板
    public GameObject gameOver; // 顯示遊戲結束的面板
    public GameObject win; // 顯示遊戲勝利的面板

    // 音符生成相關的變數
    public GameObject notePrefab; // 音符的預製件
    public Transform noteSpawnPoint; // 音符生成的位置
    public Transform noteParent; // 音符的父物件，用於組織音符
    public Transform hitBoxTarget; // 判定點的位置
    public float noteAdvanceTime = 2.0f; // 音符提前生成的時間
    private int totalNotesCount; // 總共的音符數量

    // 船的控制和移動相關
    public BoatController boatController; // 船的控制器
    private float totalBoatMoveDistance; // 總共需要移動的距離
    public Transform endPoint; // 終點位置
    private bool isSongPlaying = false; // 標記是否正在播放音樂

    // 倒數計時和提示相關的 UI
    public TextMeshProUGUI countdownText; // 倒數計時的 UI 文字
    public TextMeshProUGUI hitCueText; // 打擊提示的 UI 文字
    public GameObject preGamePrompt; // 遊戲開始前的提示面板
    public Button confirmButton; // 確認開始遊戲的按鈕
    private float moveDistancePerHit; // 每次打擊成功後的移動距離
    private int remainingBeats; // 剩餘的節奏數量
    private float initialMultiplier = 1.8f; // 初始移動距離的倍率

    // 控制器震動相關變數
    public float vibrationIntensity = 0.7f; // 震動強度（範圍是 0 到 1）
    public float vibrationDuration = 0.2f; // 震動持續時間（秒）

    // 控制器輸入相關的變數
    public OVRInput.Button actionButton = OVRInput.Button.One; // Meta Quest Pro 的 A 按鈕
    public bool enableActionButton = false; // 是否啟用 A 按鈕來測試打擊功能
    public bool enableSpaceKey = false; // 是否啟用空白鍵來測試打擊功能

    void OnEnable()
    {
        // 註冊場景載入事件，當場景載入時調用 OnSceneLoaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // 取消場景載入事件註冊
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 當場景載入時初始化音符生成和控制器
        Initialize();
        InitializeOVRInput();
    }

    void Start()
    {
        // 初始載入時調用初始化
        Initialize();
    }

    void Initialize()
    {
        // 檢查所有必要的變數是否已設置
        if (audioSource == null || jsonFiles.Count == 0 || audioClips.Count == 0)
        {
            Debug.LogError("未分配 AudioSource、JSON 文件或音頻剪輯。");
            return;
        }
        UpdateHealthUI(); // 更新血量 UI
        ShowPreGamePrompt(); // 顯示遊戲開始提示
        hitCueText.enabled = false; // 隱藏打擊提示
        Debug.Log("RhythmManager 已初始化");
    }

    void InitializeOVRInput()
    {
        // 初始化 OVRInput
        OVRInput.Update();
        Debug.Log("OVRInput 初始化成功");
    }

    void Update()
    {
        // 每幀更新 OVRInput 狀態
        OVRInput.Update();

        // 檢測 A 按鈕輸入
        if (enableActionButton && OVRInput.GetDown(actionButton))
        {
            OnRowingAction();
        }

        // 檢測空白鍵輸入
        if (enableSpaceKey && Input.GetKeyDown(KeyCode.Space))
        {
            OnRowingAction();
        }
    }

    void ShowPreGamePrompt()
    {
        // 顯示遊戲開始前的提示面板，並設置按鈕監聽
        preGamePrompt.SetActive(true);
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);
    }

    void OnConfirmButtonClicked()
    {
        // 隱藏提示面板並開始新的關卡
        preGamePrompt.SetActive(false);
        StartNewLevel(currentSongIndex);
    }

    IEnumerator StartCountdownAndPlaySong(int songIndex)
    {
        // 倒數計時並播放音樂
        int countdown = 3;
        while (countdown > 0)
        {
            countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }
        countdownText.text = "";
        PlaySong(songIndex);
    }

    void UpdateHealthUI()
    {
        // 更新玩家血量 UI
        healthText.text = "血量: " + playerHealth.ToString();
        healthBar.value = playerHealth;
    }

    public void PlaySong(int index)
    {
        // 播放音樂並解析譜面
        if (isSongPlaying)
            return;
        if (index >= 0 && index < audioClips.Count && index < jsonFiles.Count)
        {
            currentSongIndex = index;
            audioSource.clip = audioClips[index];
            audioSource.Play();
            jsonParser.ParseJson(jsonFiles[index].text);
            notes = jsonParser.GetNotes();
            totalNotesCount = notes.Count;
            remainingBeats = notes.Count;

            // 計算船的移動距離
            totalBoatMoveDistance = Mathf.Abs(
                boatController.transform.position.z - endPoint.position.z
            );
            moveDistancePerHit = (totalBoatMoveDistance / totalNotesCount) * initialMultiplier;
            boatController.SetInitialMoveDistance(moveDistancePerHit);

            StartCoroutine(SpawnNotes());
            isSongPlaying = true;
            Invoke(nameof(SongFinished), audioSource.clip.length);
        }
        else
        {
            Debug.LogError("歌曲或 JSON 文件索引無效");
        }
    }

    // 顯示打擊提示
    public void ShowHitCue()
    {
        hitCueText.enabled = true; // 顯示打擊提示 UI
        hitCueText.text = "划！"; // 提示文字，可以根據需求調整
        // 顯示提示時觸發震動
        StartCoroutine(TriggerVibration());
    }

    // 隱藏打擊提示
    public void HideHitCue()
    {
        hitCueText.enabled = false; // 隱藏打擊提示 UI
    }

    // 控制震動的 Coroutine
    IEnumerator TriggerVibration()
    {
        // 開始震動，左手和右手控制器均震動
        OVRInput.SetControllerVibration(1, vibrationIntensity, OVRInput.Controller.RTouch);
        OVRInput.SetControllerVibration(1, vibrationIntensity, OVRInput.Controller.LTouch);

        // 持續指定的時間
        yield return new WaitForSeconds(vibrationDuration);

        // 停止震動
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
    }

    IEnumerator SpawnNotes()
    {
        // 根據節奏生成音符
        foreach (Note note in notes)
        {
            float beatTime = note.num / (float)note.LPB * 60f / jsonParser.GetBPM();
            float spawnTime = beatTime - noteAdvanceTime;
            float delay = spawnTime - audioSource.time;
            if (delay > 0)
                yield return new WaitForSeconds(delay);
            SpawnNote();
        }
    }

    void SpawnNote()
    {
        // 生成音符並設置目標位置
        GameObject note = Instantiate(
            notePrefab,
            noteSpawnPoint.position,
            Quaternion.identity,
            noteParent
        );
        RhythmNote noteScript = note.GetComponent<RhythmNote>();
        noteScript.SetTarget(hitBoxTarget.position, noteAdvanceTime);
        noteScript.onMissed = MissedBeat;
    }

    void OnRowingAction()
    {   
        Debug.Log("划船動作被觸發！"); // 添加這行檢查按鍵是否被檢測到
        // 檢測划船動作並進行打擊判定
        RhythmManager rhythmManager = FindObjectOfType<RhythmManager>();
        if (rhythmManager != null)
        {
            rhythmManager.TriggerBeat();
        }
    }

    public void TriggerBeat()
    {
        // 判定是否成功打擊音符
        foreach (GameObject note in GameObject.FindGameObjectsWithTag("Note"))
        {
            RhythmNote noteScript = note.GetComponent<RhythmNote>();
            if (
                noteScript.isInHitBox
                && Mathf.Abs(noteScript.GetTimeDifference(audioSource.time)) <= hitWindow
            )
            {
                noteScript.Hit();
                boatController?.MoveForward();
                HideHitCue();
                remainingBeats--;
                Debug.Log("Nice! 剩餘節奏：" + remainingBeats + "，船移動：" + moveDistancePerHit);
                return;
            }
        }
    }

    public void MissedBeat()
    {
        // 處理打擊失敗邏輯
        playerHealth -= 5;
        UpdateHealthUI();
        HideHitCue(); // 錯過音符後隱藏打擊提示
        remainingBeats--;
        Debug.Log("Miss！當前血量：" + playerHealth + "，節奏數：" + remainingBeats);

        if (remainingBeats > 0)
        {
            moveDistancePerHit += moveDistancePerHit / remainingBeats;
            boatController.SetInitialMoveDistance(moveDistancePerHit);
        }
        if (playerHealth <= 0)
            GameOver();
    }

    void GameOver()
    {
        // 遊戲結束邏輯
        audioSource.Stop();
        gameOver.SetActive(true);
        StartCoroutine(ReturnToLevelSelectScene());
    }

    IEnumerator ReturnToLevelSelectScene()
    {
        // 返回選擇關卡場景
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Level05");
    }

    void StartNewLevel(int levelIndex)
    {
        StartCoroutine(StartCountdownAndPlaySong(levelIndex));
    }

    void SongFinished()
    {
        // 處理歌曲播放結束邏輯
        isSongPlaying = false;
        if (playerHealth > 0)
        {
            win.SetActive(true);
            StartCoroutine(ReturnToLevelSelectScene());
        }
    }
}
