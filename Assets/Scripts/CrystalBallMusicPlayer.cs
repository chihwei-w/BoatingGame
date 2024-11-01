using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalBallMusicPlayer : MonoBehaviour
{
    public AudioSource audioSource; // 當前水晶球的音樂播放器
    public AudioClip musicClip; // 當前水晶球要播放的音樂

    // 靜態變數來存儲當前正在播放的音樂播放器
    private static AudioSource currentlyPlayingAudioSource = null;

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
                audioSource.clip = musicClip;
                audioSource.Play();
                currentlyPlayingAudioSource = audioSource; // 更新正在播放的音樂播放器
                Debug.Log("音樂已播放");
            }
        }
    }
}
