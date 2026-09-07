using UnityEngine;

public class CharacterSaveSystem : MonoBehaviour
{
    public void Save(CharacterSaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("PLAYER_CHARACTER", json);
        PlayerPrefs.Save();
    }

    public CharacterSaveData Load()
    {
        if (!PlayerPrefs.HasKey("PLAYER_CHARACTER"))
            return new CharacterSaveData();

        string json = PlayerPrefs.GetString("PLAYER_CHARACTER");
        return JsonUtility.FromJson<CharacterSaveData>(json);
    }
}