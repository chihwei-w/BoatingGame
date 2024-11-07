using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteHitBox : MonoBehaviour
{
    private RhythmManager rhythmManager;

    void Start()
    {
        rhythmManager = FindObjectOfType<RhythmManager>();
        if (rhythmManager == null)
        {
            Debug.LogError("找不到 RhythmManager，請確認場景中有此組件。");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        RhythmNote note = other.GetComponent<RhythmNote>();
        if (note != null)
        {
            note.isInHitBox = true; // 標記音符已進入判定區域
            Debug.Log("音符進入判定區域: " + note.gameObject.name);
            if (rhythmManager != null)
            {
                rhythmManager.ShowHitCue();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        RhythmNote note = other.GetComponent<RhythmNote>();
        if (note != null)
        {
            note.isInHitBox = false; // 標記音符已離開判定區域
            Debug.Log("音符已離開判定區域: " + note.gameObject.name);

            if (rhythmManager != null)
            {
                rhythmManager.HideHitCue();
            }
        }
    }
}
