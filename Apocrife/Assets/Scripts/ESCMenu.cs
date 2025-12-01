using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ESCMenu : MonoBehaviour
{
    [SerializeField] private GameObject _menu;
    [SerializeField] private string _menuSceneName = "MainMenu";

    private GameNetworkManager _gameNetworkManager;
    private bool _isOpen = false;

    void Start()
    {
        // Находим GameNetworkManager
        _gameNetworkManager = GameNetworkManager.Instance;

        // Скрываем меню при старте
        if (_menu != null)
            _menu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        _isOpen = !_isOpen;

        if (_menu != null)
            _menu.SetActive(_isOpen);

        // Управление курсором
        if (_gameNetworkManager != null)
        {
            if (_isOpen)
            {
                _gameNetworkManager.UnlockCursor();
            }
            else
            {
                _gameNetworkManager.LockCursor();
            }
        }
        else
        {
            // Fallback если GameNetworkManager не найден
            if (_isOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void BackToMenu()
    {
        DisconnectAndReturnToMenu();
    }

    public void DisconnectAndReturnToMenu()
    {
        Debug.Log("Отключаемся от сервера и возвращаемся в меню...");

        // Используем GameNetworkManager для корректного отключения
        if (_gameNetworkManager != null)
        {
            _gameNetworkManager.ReturnToMenu("Выход в меню");
        }
        else
        {
            // Fallback если GameNetworkManager не найден
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
            PhotonNetwork.LoadLevel(_menuSceneName);
            SceneManager.LoadScene(_menuSceneName);
        }
    }

    public void ResumeGame()
    {
        ToggleMenu();
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры");

        // Корректно отключаемся от сервера перед выходом
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}