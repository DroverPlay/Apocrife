//using Photon.Pun;
//using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConnectToLobby : MonoBehaviour
{
    [SerializeField] private int _maxPlayers = 4;
    public string gameSceneName = "Game";
    [SerializeField] private Button quickMatchButton;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private GameObject _loadScene;

    [Header("Настройки переподключения")]
    public bool autoReconnect = true;
    public float reconnectDelay = 3f;
    public int maxReconnectAttempts = 3;

    private int currentReconnectAttempts = 0;
    private bool isConnecting = false;

    void Start()
    {
        Application.targetFrameRate = 144;
        UpdateStatus("Нажмите для подключения");

        if (quickMatchButton != null)
            quickMatchButton.interactable = true;
    }

    public void QuickMatch()
    {
        if (isConnecting) return;

        isConnecting = true;
        currentReconnectAttempts++;
        _loadScene.SetActive(true);

        // Запускаем экран загрузки по ИМЕНИ сцены
        if (LoadManager.Instance != null)
        {
            LoadManager.Instance.StartNetworkLoading(gameSceneName);
        }

        // Блокируем кнопку
        if (quickMatchButton != null)
            quickMatchButton.interactable = false;

        UpdateStatus("Подключение...");

        // Подключаемся к Photon
        //if (!PhotonNetwork.IsConnected)
        //{
        //    PhotonNetwork.ConnectUsingSettings();
        //}
        //else
        //{
        //    PhotonNetwork.JoinRandomRoom();
        //}
    }

    // ... остальные методы остаются без изменений ...
    //public override void OnConnectedToMaster()
    //{
    //    Debug.Log("Подключились к мастер-серверу");

    //    if (LoadManager.Instance != null)
    //    {
    //        LoadManager.Instance.UpdateStatus("Поиск игровой сессии...");
    //    }

    //    PhotonNetwork.JoinRandomRoom();
    //}

    //public override void OnJoinRandomFailed(short returnCode, string message)
    //{
    //    Debug.Log("Не найдено подходящих комнат, создаем новую...");

    //    if (LoadManager.Instance != null)
    //    {
    //        LoadManager.Instance.UpdateStatus("Создание новой комнаты...");
    //    }

    //    CreateRoom();
    //}

    //private void CreateRoom()
    //{
    //    RoomOptions roomOptions = new RoomOptions();
    //    roomOptions.MaxPlayers = _maxPlayers;
    //    PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
    //}

    //public override void OnCreateRoomFailed(short returnCode, string message)
    //{
    //    Debug.LogError($"Ошибка создания комнаты: {message}");

    //    if (LoadManager.Instance != null)
    //    {
    //        LoadManager.Instance.UpdateStatus("Повторная попытка...");
    //    }

    //    RoomOptions roomOptions = new RoomOptions();
    //    roomOptions.MaxPlayers = _maxPlayers;
    //    PhotonNetwork.CreateRoom("Room_" + Random.Range(1000, 9999), roomOptions);
    //}

    //public override void OnJoinedRoom()
    //{
    //    Debug.Log($"Успешно подключились к комнате");

    //    if (LoadManager.Instance != null)
    //    {
    //        LoadManager.Instance.UpdateStatus("Подключение к игровой сессии...");
    //    }
    //}

    //public override void OnDisconnected(DisconnectCause cause)
    //{
    //    isConnecting = false;
    //    string errorMessage = GetShortDisconnectMessage(cause);

    //    Debug.Log($"Отключено: {errorMessage}");

    //    // Показываем ошибку в экране загрузки
    //    if (LoadManager.Instance != null)
    //    {
    //        LoadManager.Instance.ShowError(errorMessage);
    //    }

    //    UpdateStatus($"Отключено: {errorMessage}");

    //    if (autoReconnect && currentReconnectAttempts < maxReconnectAttempts)
    //    {
    //        UpdateStatus($"Переподключение через {reconnectDelay} сек...");
    //        Invoke("Reconnect", reconnectDelay);
    //    }
    //    else
    //    {
    //        UpdateStatus("Не удалось подключиться");
    //        if (quickMatchButton != null)
    //            quickMatchButton.interactable = true;
    //    }
    //}

    //private void Reconnect()
    //{
    //    if (!PhotonNetwork.IsConnected)
    //    {
    //        QuickMatch();
    //    }
    //}

    //private string GetShortDisconnectMessage(DisconnectCause cause)
    //{
    //    switch (cause)
    //    {
    //        case DisconnectCause.ClientTimeout:
    //        case DisconnectCause.ServerTimeout:
    //            return "Таймаут подключения";
    //        case DisconnectCause.Exception:
    //            return "Ошибка сети";
    //        case DisconnectCause.MaxCcuReached:
    //            return "Сервер переполнен";
    //        default:
    //            return "Соединение разорвано";
    //    }
    //}

    private void UpdateStatus(string message)
    {
        Debug.Log($"Статус: {message}");
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    public void ManualReconnect()
    {
        if (!isConnecting)
        {
            currentReconnectAttempts = 0;
            QuickMatch();
        }
    }
}