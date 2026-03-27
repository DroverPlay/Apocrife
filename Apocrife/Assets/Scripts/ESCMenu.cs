using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Muryotaisu;

public class ESCMenu : MonoBehaviour
{
    [SerializeField] private GameObject _menu;
    [SerializeField] private string _menuSceneName = "MainMenu";

    private bool _isOpen = false;
    private CameraController _cameraController;
    private MuryotaisuController _playerController;

    void Start()
    {
        // Находим контроллеры
        _cameraController = FindObjectOfType<CameraController>();
        _playerController = FindObjectOfType<MuryotaisuController>();

        // Скрываем меню при старте
        if (_menu != null)
            _menu.SetActive(false);

        // Блокируем курсор в начале игры
        LockCursor();
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

        if (_isOpen)
        {
            // Меню открыто - пауза и разблокировка курсора
            Time.timeScale = 0f;
            UnlockCursor();

            // Отключаем управление камерой и персонажем
            DisableControls();
        }
        else
        {
            // Меню закрыто - возобновление и блокировка курсора
            Time.timeScale = 1f;
            LockCursor();

            // Включаем управление камерой и персонажем
            EnableControls();
        }
    }

    private void DisableControls()
    {
        // Отключаем управление камерой
        if (_cameraController != null)
        {
            _cameraController.enabled = false;
        }
        else
        {
            // Ищем снова если не нашли в Start
            _cameraController = FindObjectOfType<CameraController>();
            if (_cameraController != null)
                _cameraController.enabled = false;
        }

        // Отключаем управление персонажем
        if (_playerController != null)
        {
            _playerController.enabled = false;
        }
        else
        {
            _playerController = FindObjectOfType<MuryotaisuController>();
            if (_playerController != null)
                _playerController.enabled = false;
        }
    }

    private void EnableControls()
    {
        // Включаем управление камерой
        if (_cameraController != null)
        {
            _cameraController.enabled = true;
        }

        // Включаем управление персонажем
        if (_playerController != null)
        {
            _playerController.enabled = true;
        }
    }

    public void BackToMenu()
    {
        ReturnToMenu();
    }

    public void ReturnToMenu()
    {
        Debug.Log("Возвращаемся в главное меню...");

        // Снимаем паузу и включаем управление перед загрузкой
        Time.timeScale = 1f;
        EnableControls();

        // Загружаем главное меню
        SceneManager.LoadScene(_menuSceneName);
    }

    public void ResumeGame()
    {
        ToggleMenu();
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры");

        // Снимаем паузу перед выходом
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Методы для управления курсором
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Автоматически разблокируем курсор при выходе из игры
    void OnApplicationQuit()
    {
        UnlockCursor();
    }
}