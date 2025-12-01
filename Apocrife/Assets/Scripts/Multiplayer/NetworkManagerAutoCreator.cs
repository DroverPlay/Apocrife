using Photon.Pun;
using UnityEngine;

public class NetworkManagerAutoCreator : MonoBehaviour
{
    void Start()
    {
        // Создаем GameNetworkManager если его нет
        if (GameNetworkManager.Instance == null)
        {
            Debug.Log("Создаем GameNetworkManager на сцене Game");
        }

        // Проверяем соединение при старте сцены
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("Нет соединения с Photon при загрузке игровой сцены!");
            GameNetworkManager.Instance.ReturnToMenu("Нет соединения");
        }
    }

    void Update()
    {
        // Дополнительная проверка каждые 5 секунд
        if (Time.frameCount % 300 == 0) // ~5 секунд при 60 FPS
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogError("Обнаружена потеря соединения во время игры!");
                GameNetworkManager.Instance.ReturnToMenu("Потеряно соединение во время игры");
            }
        }
    }
}