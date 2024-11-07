using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JsonParser : MonoBehaviour
{
    public NoteData noteData; // 存儲整個譜面信息

    public void ParseJson(string jsonContent)
    {
        // 解析 JSON 並填充 NoteData 對象
        try
        {
            noteData = JsonUtility.FromJson<NoteData>(jsonContent);
            if (noteData == null || noteData.notes == null || noteData.notes.Count == 0)
            {
                Debug.LogError("解析 JSON 時出現錯誤：格式錯誤或缺少數據。");
            }
            else
            {
                Debug.Log("JSON 解析成功，共有 " + noteData.notes.Count + " 個音符。");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("JSON 解析失敗：" + ex.Message);
        }
    }

    public List<Note> GetNotes()
    {
        return noteData?.notes ?? new List<Note>();
    }

    public int GetBPM()
    {
        return noteData?.BPM ?? 120; // 預設 BPM 為 120
    }

    public int GetOffset()
    {
        return noteData?.offset ?? 0; // 預設偏移為 0
    }
}

[System.Serializable]
public class Note
{
    public int LPB; // 每小節的音符數
    public int num; // 音符的時間點
    public int block; // 音符所屬區塊
    public int type; // 音符類型
}

[System.Serializable]
public class NoteData
{
    public string name; // 譜面名稱
    public int maxBlock; // 最大區塊數
    public int BPM; // 每分鐘節拍數
    public int offset; // 時間偏移
    public List<Note> notes; // 音符列表
}
