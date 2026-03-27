using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ConnectToLobby : MonoBehaviour
{
    public string gameSceneName = "Game";
    [SerializeField] private Button _startGameBtn;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private GameObject _loadScene;

    [Header("Настройки")]
    public float loadDelay = 2f; // Задержка перед загрузкой сцены

    void Start()
    {
        Application.targetFrameRate = 144;
        UpdateStatus("Нажмите для подключения");

        if (_startGameBtn != null)
            _startGameBtn.interactable = true;
    }

    public void StartGame()
    {

        _loadScene.SetActive(true);

        // Запускаем экран загрузки
        if (LoadManager.Instance != null)
        {
            LoadManager.Instance.StartNetworkLoading(gameSceneName);
        }

        // Блокируем кнопку
        if (_startGameBtn != null)
            _startGameBtn.interactable = false;

        UpdateStatus("Подготовка...");

        // Загружаем игровую сцену через задержку
        Invoke("LoadGameScene", loadDelay);
    }

    private void LoadGameScene()
    {
        Debug.Log("Загрузка игровой сцены...");

        // Просто загружаем сцену без Photon
        SceneManager.LoadScene(gameSceneName);
    }

    private void UpdateStatus(string message)
    {
        Debug.Log($"Статус: {message}");
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    // Для кнопки "Выход"
    public void QuitGame()
    {
        Debug.Log("Выход из игры");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}