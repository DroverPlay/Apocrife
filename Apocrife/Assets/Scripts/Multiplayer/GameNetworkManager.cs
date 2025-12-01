using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Настройки")]
    public string menuSceneName = "MainMenu";
    public bool showDebugMessages = true;

    private static GameNetworkManager instance;
    private bool isReturningToMenu = false;

    public static GameNetworkManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameNetworkManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("GameNetworkManager");
                    instance = obj.AddComponent<GameNetworkManager>();
                }
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Всегда разблокируем курсор при создании
        UnlockCursor();
    }

    void Update()
    {
        // Периодическая проверка соединения
        if (!PhotonNetwork.IsConnected && !isReturningToMenu && SceneManager.GetActiveScene().name != menuSceneName)
        {
            Debug.LogWarning("Соединение потеряно в Update!");
            ReturnToMenu("Потеряно соединение с сервером");
        }

        // Разблокируем курсор в меню при нажатии Escape
        if (SceneManager.GetActiveScene().name == menuSceneName && Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }

        // Автоматически блокируем курсор в игровой сцене через 0.5 секунды после загрузки
        if (SceneManager.GetActiveScene().name != menuSceneName && Time.timeSinceLevelLoad < 1f)
        {
            if (Time.timeSinceLevelLoad > 0.5f && Cursor.lockState != CursorLockMode.Locked)
            {
                LockCursor();
            }
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (showDebugMessages)
            Debug.Log($"OnDisconnected вызван: {cause}");

        if (!isReturningToMenu)
        {
            ReturnToMenu($"Отключено: {GetDisconnectMessage(cause)}");
        }
    }

    private string GetDisconnectMessage(DisconnectCause cause)
    {
        switch (cause)
        {
            case DisconnectCause.ClientTimeout:
                return "Таймаут соединения клиента";
            case DisconnectCause.ServerTimeout:
                return "Таймаут соединения сервера";
            case DisconnectCause.Exception:
                return "Ошибка сети";
            case DisconnectCause.DisconnectByServerLogic:
                return "Сервер отключил соединение";
            case DisconnectCause.MaxCcuReached:
                return "Сервер переполнен";
            default:
                return "Соединение разорвано";
        }
    }

    public void ReturnToMenu(string reason = "")
    {
        if (isReturningToMenu) return;

        isReturningToMenu = true;

        Debug.Log($"Возвращаемся в меню. Причина: {reason}");

        // Разблокируем курсор ПЕРЕД загрузкой меню
        UnlockCursor();

        // Останавливаем всю сетевую активность
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }

        // Загружаем меню
        PhotonNetwork.LoadLevel(menuSceneName);
        //SceneManager.LoadScene(menuSceneName);

        // Сбрасываем флаг после загрузки меню
        Invoke("ResetReturnFlag", 2f);
    }

    private void ResetReturnFlag()
    {
        isReturningToMenu = false;
    }

    // Вызывается при загрузке любой сцены
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Загружена сцена: {scene.name}");

        if (scene.name == menuSceneName)
        {
            // В меню - разблокируем курсор
            UnlockCursor();
            isReturningToMenu = false;
        }
        else
        {
            // В игровой сцене - блокируем курсор через небольшую задержку
            Invoke("LockCursorDelayed", 0.5f);

            // Проверяем соединение
            if (!PhotonNetwork.IsConnected && scene.name != menuSceneName)
            {
                Debug.LogWarning("Загружена игровая сцена без соединения!");
                ReturnToMenu("Нет соединения с сервером");
            }
        }
    }
    public void DisconnectToMenu(string reason = "Выход в меню")
    {
        if (isReturningToMenu) return;

        Debug.Log($"Ручное отключение: {reason}");
        ReturnToMenu(reason);
    }
    private void LockCursorDelayed()
    {
        LockCursor();
    }

    // Методы для управления курсором
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (showDebugMessages) Debug.Log("Курсор заблокирован");
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (showDebugMessages) Debug.Log("Курсор разблокирован");
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Метод для кнопки "Выход в меню"
    public void DisconnectToMenu()
    {
        ReturnToMenu("Выход в меню");
    }
}