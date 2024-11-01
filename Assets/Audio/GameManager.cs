using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public RhythmManager rhythmManager;  // 參考 RhythmManager

    void Start()
    {
        // 播放當前關卡的歌曲
        rhythmManager.PlaySong(0);  // 每個場景對應的音樂和 .sus 文件在 RhythmManager 中設置
    }

    public void ChangeSong(int index)
    {
        rhythmManager.PlaySong(index); // 切換歌曲
    }
}

