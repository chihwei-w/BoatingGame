using System.Collections;
using UnityEngine;

public class CrystalBallMusicPlayer : MonoBehaviour
{
    public AudioSource audioSource; // 當前水晶球的音樂播放器
    public AudioClip musicClip; // 當前水晶球要播放的音樂

    // 靜態變數來存儲當前正在播放的音樂播放器
    private static AudioSource currentlyPlayingAudioSource = null;

    private void Start()
    {
        // 檢查 audioSource 是否被指派，若未指派則嘗試獲取
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError(
                    "AudioSource 未分配，且未能自動找到，請在 Inspector 中設置 audioSource。"
                );
            }
            else
            {
                Debug.Log("AudioSource 已自動獲取。");
            }
        }
    }

    // 當控制器進入水晶球的觸發範圍時
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("碰撞物件名稱：" + other.gameObject.name); // 打印進入觸發範圍的物件名稱

        if (other.CompareTag("PlayerController") || other.CompareTag("Untagged"))
        {
            // 如果有其他水晶球正在播放音樂，先停止它
            if (currentlyPlayingAudioSource != null && currentlyPlayingAudioSource != audioSource)
            {
                currentlyPlayingAudioSource.Stop();
                Debug.Log("停止上一首音樂");
            }

            // 播放當前水晶球的音樂
            if (audioSource != null && musicClip != null)
            {
                if (!audioSource.isPlaying) // 防止重複播放
                {
                    audioSource.clip = musicClip;
                    audioSource.Play();
                    currentlyPlayingAudioSource = audioSource; // 更新正在播放的音樂播放器
                    Debug.Log("音樂已播放");
                }
            }
            else
            {
                Debug.LogWarning("音樂播放器或音樂片段未分配");
            }
        }
    }

    // 在腳本被禁用時停止音樂
    private void OnDisable()
    {
        if (audioSource != null && currentlyPlayingAudioSource == audioSource)
        {
            audioSource.Stop();
            currentlyPlayingAudioSource = null;
            Debug.Log("音樂播放器在禁用時停止");
        }
    }
}
