using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RhythmManager : MonoBehaviour
{
    public AudioSource audioSource; // 音源，用於播放音樂
    public JsonParser jsonParser; // JSON 解析器，用於解析音符的譜面數據
    public List<TextAsset> jsonFiles; // 存儲譜面 JSON 文件
    public List<AudioClip> audioClips; // 存儲音樂剪輯
    public TextMeshProUGUI healthText; // 顯示玩家血量的 UI
    public int playerHealth = 100; // 玩家初始血量
    private int currentSongIndex = 0; // 當前播放的歌曲索引
    private List<Note> notes; // 存儲音符信息
    public Slider healthBar; // 血量條 UI
    public float hitWindow = 10.0f; // 判定窗口，允許玩家打擊音符的誤差時間

    public GameObject gameOver; // 遊戲結束時顯示的面板
    public GameObject win; // 遊戲成功時顯示的面板
    public GameObject notePrefab; // 音符預製件
    public Transform noteSpawnPoint; // 音符生成的位置
    public Transform noteParent; // 音符的父物件
    public Transform hitBoxTarget; // 判定點的位置
    public Transform endPoint; // 終點位置
    public float noteAdvanceTime = 2.0f; // 音符提前生成的時間
    private int totalNotesCount; // 紀錄總共的音符數量

    public BoatController boatController; // 船的控制器，用於控制船的移動
    private float totalBoatMoveDistance; // 總共需要移動的距離
    private bool isSongPlaying = false; // 標記是否正在播放音樂
    public TextMeshProUGUI countdownText; // 倒數計時的 UI 文字
    public TextMeshProUGUI hitCueText; // 打擊提示的 UI 文字
    public GameObject preGamePrompt; // 遊戲開始前的提示面板
    public Button confirmButton; // 確認開始遊戲的按鈕
    private float moveDistancePerHit; // 每次打擊成功後的移動距離
    private int remainingBeats; // 剩餘的節奏數量
    private float initialMultiplier = 1.8f;
    public float vibrationIntensity = 0.7f; // 震動強度，範圍是 0 到 1
    public float vibrationDuration = 0.2f; // 震動持續時間（秒）

    void Start()
    {
        if (audioSource == null || jsonFiles.Count == 0 || audioClips.Count == 0)
        {
            Debug.LogError("未分配 AudioSource、JSON 文件或音頻剪輯。");
            return;
        }
        if (jsonParser == null)
        {
            Debug.LogError("未分配 JSON 解析器。");
            return;
        }
        if (boatController == null || endPoint == null)
        {
            Debug.LogError("未分配船的控制器或終點位置。");
            return;
        }

        UpdateHealthUI();
        ShowPreGamePrompt();
        hitCueText.enabled = false; // 初始化時隱藏打擊提示
    }

    void ShowPreGamePrompt()
    {
        preGamePrompt.SetActive(true);
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);
    }

    void Update()
    {
        // 檢測控制器的 A 鍵按下（假設是 "joystick button 0"）
        if (preGamePrompt.activeSelf && Input.GetButtonDown("Submit"))
        {
            OnConfirmButtonClicked();
        }
    }

    void OnConfirmButtonClicked()
    {
        preGamePrompt.SetActive(false);
        StartNewLevel(currentSongIndex);
    }

    IEnumerator StartCountdownAndPlaySong(int songIndex)
    {
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
        healthText.text = "血量: " + playerHealth.ToString();
        healthBar.value = playerHealth;
    }

    public void PlaySong(int index)
    {
        if (isSongPlaying)
        {
            Debug.Log("當前已在播放歌曲，無法重複調用 PlaySong。");
            return;
        }

        if (index >= 0 && index < audioClips.Count && index < jsonFiles.Count)
        {
            currentSongIndex = index;
            audioSource.clip = audioClips[index];
            audioSource.Play();

            Debug.Log("譜面名稱: " + jsonFiles[index].name);

            jsonParser.ParseJson(jsonFiles[index].text);
            notes = jsonParser.GetNotes();
            totalNotesCount = notes.Count;

            if (notes == null || notes.Count == 0)
            {
                Debug.LogError("未找到音符數據或 JSON 文件解析失敗。");
                return;
            }

            remainingBeats = notes.Count;

            totalBoatMoveDistance = Mathf.Abs(
                boatController.transform.position.z - endPoint.position.z
            );

            if (totalNotesCount > 0)
            {
                moveDistancePerHit = (totalBoatMoveDistance / totalNotesCount) * initialMultiplier;
                boatController.SetInitialMoveDistance(moveDistancePerHit);
            }
            Debug.Log("節奏數：" + remainingBeats + "，初始移動距離：" + moveDistancePerHit);
            StartCoroutine(SpawnNotes());
            isSongPlaying = true;
            Invoke(nameof(SongFinished), audioSource.clip.length);
        }
        else
        {
            Debug.LogError("歌曲或 JSON 文件索引無效");
        }
    }

    IEnumerator SpawnNotes()
    {
        foreach (Note note in notes)
        {
            float beatTime = note.num / (float)note.LPB * 60f / jsonParser.GetBPM();
            float spawnTime = beatTime - noteAdvanceTime;
            float delay = spawnTime - audioSource.time;

            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                Debug.LogWarning("音符生成時間過短，可能導致音符立即生成。");
            }
            SpawnNote();
        }
    }

    void SpawnNote()
    {
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

    public void ShowHitCue()
    {
        hitCueText.enabled = true;
        hitCueText.text = "划！";
        StartCoroutine(TriggerVibration());
    }

    public void HideHitCue()
    {
        hitCueText.enabled = false;
    }

    IEnumerator TriggerVibration()
    {
        OVRInput.SetControllerVibration(1, vibrationIntensity, OVRInput.Controller.RTouch);
        OVRInput.SetControllerVibration(1, vibrationIntensity, OVRInput.Controller.LTouch);
        yield return new WaitForSeconds(vibrationDuration);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
    }

    public void TriggerBeat()
    {
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
        playerHealth -= 5;
        UpdateHealthUI();
        HideHitCue();
        remainingBeats--;
        Debug.Log("Miss！當前血量：" + playerHealth + "，節奏數：" + remainingBeats);

        if (remainingBeats > 0)
        {
            moveDistancePerHit += (moveDistancePerHit / remainingBeats);
            boatController.SetInitialMoveDistance(moveDistancePerHit);
        }

        if (playerHealth <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("遊戲結束");
        audioSource.Stop();
        gameOver.SetActive(true);
        StartCoroutine(ReturnToLevelSelectScene());
    }

    IEnumerator ReturnToLevelSelectScene()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Level1");
    }

    void StartNewLevel(int levelIndex)
    {
        StartCoroutine(StartCountdownAndPlaySong(levelIndex));
    }

    void SongFinished()
    {
        Debug.Log("歌曲播放完畢");
        isSongPlaying = false;

        if (playerHealth > 0)
        {
            Debug.Log("成功！");
            win.SetActive(true);
            StartCoroutine(ReturnToLevelSelectScene());
        }
    }
}
