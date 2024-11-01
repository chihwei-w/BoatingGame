using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteHitBox : MonoBehaviour
{
    private RhythmManager rhythmManager;

    void Start()
    {
        rhythmManager = FindObjectOfType<RhythmManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        RhythmNote note = other.GetComponent<RhythmNote>();
        if (note != null)
        {
            note.isInHitBox = true; // 標記音符已進入判定區域
            //Debug.Log("音符已進入判定區域");

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
            //Debug.Log("音符已離開判定區域");

            if (rhythmManager != null)
            {
                rhythmManager.HideHitCue();
            }
        }
    }
}
