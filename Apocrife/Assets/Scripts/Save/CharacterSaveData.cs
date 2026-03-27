using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterSaveData
{
    public string version = "1.0";
    public string characterName;
    public string saveDate;

    // Основные настройки
    public int gender; // 0 = Male, 1 = Female
    public string packageMark;
    public string[] otherPackages;

    // Все пресеты частей тела
    public Dictionary<string, string> partPresets = new Dictionary<string, string>();

    // Дополнительные данные
    public Vector3 position;
    public Quaternion rotation;
    public float scale = 1f;

    // Методы для удобства
    public string ToJson()
    {
        return JsonUtility.ToJson(this, true);
    }

    public static CharacterSaveData FromJson(string json)
    {
        return JsonUtility.FromJson<CharacterSaveData>(json);
    }
}