using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class LoadManager : MonoBehaviour
{
    [Header("Настройка загрузки")]
    [SerializeField] private Sprite[] _loadingSprite;
    [SerializeField] private float _imageDisplayTime = 3f;
    [SerializeField] private float _fadeDuration = 1f;
    [SerializeField] private Slider _loadingBar;
    [SerializeField] private TMP_Text _statusText;

    [Header("Сетевая загрузка")]
    public bool useNetworkLoading = false;
    public float minLoadingTime = 2f;
    public string targetSceneName = "Game"; // Добавляем имя сцены

    private Image _displayImage;
    private int _currentImageIndex = 0;
    private CanvasGroup _canvasGroup;
    private int _lastImageIndex;
    private bool _isLoading = false;
    private float _loadingStartTime;

    public static LoadManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _displayImage = GetComponent<Image>();
        _canvasGroup = GetComponent<CanvasGroup>();

        // Скрываем экран загрузки при старте
        //if (_canvasGroup != null)
        //    _canvasGroup.alpha = 0;

        //if (_loadingBar != null)
        //    _loadingBar.gameObject.SetActive(false);
    }

    // Новый метод для сетевой загрузки по имени сцены
    public void StartNetworkLoading(string sceneName)
    {
        if (_isLoading) return;

        _isLoading = true;
        _loadingStartTime = Time.time;
        useNetworkLoading = true;
        targetSceneName = sceneName;

        // Показываем экран загрузки
        if (_canvasGroup != null)
            _canvasGroup.alpha = 1;

        if (_loadingBar != null)
        {
            _loadingBar.gameObject.SetActive(true);
            _loadingBar.value = 0;
        }

        // Запускаем корутины
        StartCoroutine(ImageTransitionRoutine());
        StartCoroutine(LoadNetworkSceneAsync());
    }

    // Старый метод для загрузки по индексу (оставляем для совместимости)
    public void StartLocalLoading(int sceneIndex)
    {
        if (_isLoading) return;

        _isLoading = true;
        useNetworkLoading = false;

        if (_canvasGroup != null)
            _canvasGroup.alpha = 1;

        if (_loadingBar != null)
            _loadingBar.gameObject.SetActive(true);

        StartCoroutine(ImageTransitionRoutine());
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }

    IEnumerator ImageTransitionRoutine()
    {
        if (_loadingSprite.Length == 0) yield break;

        while (_isLoading)
        {
            // Плавное появление
            yield return StartCoroutine(FadeImage(1f));
            yield return new WaitForSeconds(_imageDisplayTime);

            // Плавное исчезновение
            yield return StartCoroutine(FadeImage(0f));

            // Выбор следующей картинки (исключая повторения)
            int newIndex;
            do
            {
                newIndex = Random.Range(0, _loadingSprite.Length);
            }
            while (newIndex == _lastImageIndex && _loadingSprite.Length > 1);

            _currentImageIndex = newIndex;
            _lastImageIndex = _currentImageIndex;
            _displayImage.sprite = _loadingSprite[_currentImageIndex];
            _displayImage.preserveAspect = true;
        }
    }

    IEnumerator FadeImage(float targetAlpha)
    {
        if (_canvasGroup == null) yield break;

        float startAlpha = _canvasGroup.alpha;
        float time = 0;

        while (time < _fadeDuration)
        {
            if (!_isLoading) yield break;

            time += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / _fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = targetAlpha;
    }

    // Измененный метод для сетевой загрузки
    IEnumerator LoadNetworkSceneAsync()
    {
        float progress = 0f;
        float elapsedTime = 0f;
        string lastStatus = "";

        UpdateStatus("Подключение к серверу...");

        // Ждем подключения к Photon и минимальное время загрузки
        while (elapsedTime < minLoadingTime || progress < 0.9f)
        {
            elapsedTime += Time.deltaTime;

            // Прогресс на основе времени и состояния сети
            float timeProgress = Mathf.Clamp01(elapsedTime / minLoadingTime);
            float networkProgress = GetNetworkProgress();

            progress = Mathf.Max(timeProgress, networkProgress);

            if (_loadingBar != null)
                _loadingBar.value = progress;

            string newStatus = GetLoadingStatus(progress);
            if(newStatus != lastStatus)
            {
                UpdateStatus(newStatus);
                lastStatus = newStatus;
            }
            //UpdateLoadingStatus(progress);
            yield return null;
        }

        // Все готово - загружаем сцену по имени
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
            asyncLoad.allowSceneActivation = false;

            // Ждем завершения загрузки сцены
            while (!asyncLoad.isDone)
            {
                if (_loadingBar != null)
                    _loadingBar.value = asyncLoad.progress;

                if (asyncLoad.progress >= 0.9f)
                {
                    UpdateStatus("Завершение подключения...");
                    asyncLoad.allowSceneActivation = true;
                }

                yield return null;
            }
        }
        else
        {
            Debug.LogError("Не указано имя сцены для загрузки!");
        }

        CompleteLoading();
    }

    // Старый метод для локальной загрузки по индексу
    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false;

        UpdateStatus("Загрузка...");

        while (asyncLoad.progress < 0.9f)
        {
            if (_loadingBar != null)
                _loadingBar.value = asyncLoad.progress;

            yield return null;
        }

        yield return new WaitForSeconds(1f);
        asyncLoad.allowSceneActivation = true;
        CompleteLoading();
    }

    private float GetNetworkProgress()
    {
        if (!PhotonNetwork.IsConnected)
            return 0.2f; // Подключение к серверу

        if (!PhotonNetwork.InLobby)
            return 0.4f; // Подключение к лобби

        if (!PhotonNetwork.InRoom)
            return 0.7f; // Поиск/создание комнаты

        return 0.9f; // Подключение к игре
    }

    private void UpdateLoadingStatus(float progress)
    {
        string status = "Подключение к серверу...";

        if (progress > 0.2f && progress <= 0.4f)
            status = "Соединение установлено...";
        else if (progress > 0.4f && progress <= 0.7f)
            status = "Поиск игровой сессии...";
        else if (progress > 0.7f)
            status = "Подключение к комнате...";

        UpdateStatus(status);
    }

    public void UpdateStatus(string status)
    {
        if (_statusText != null)
            _statusText.text = status;

        Debug.Log($"Статус загрузки: {status}");
    }

    private void CompleteLoading()
    {
        _isLoading = false;

        // Плавно скрываем экран загрузки
        if (_canvasGroup != null)
            StartCoroutine(FadeOutLoadingScreen());
    }

    private IEnumerator FadeOutLoadingScreen()
    {
        yield return StartCoroutine(FadeImage(0f));

        if (_loadingBar != null)
            _loadingBar.gameObject.SetActive(false);
    }

    public void ShowError(string errorMessage)
    {
        UpdateStatus($"Ошибка: {errorMessage}");
        _isLoading = false;

        // Автоматическое скрытие через 3 секунды
        Invoke("HideLoadingScreen", 3f);
    }

    private void HideLoadingScreen()
    {
        if (_canvasGroup != null)
            _canvasGroup.alpha = 0;

        if (_loadingBar != null)
            _loadingBar.gameObject.SetActive(false);
    }
    private string GetLoadingStatus(float progress)
    {
        if (progress <= 0.2f)
            return "Подключение к серверу...";
        else if (progress <= 0.4f)
            return "Соединение установлено...";
        else if (progress <= 0.7f)
            return "Поиск игровой сессии...";
        else if (progress <= 0.9f)
            return "Подключение к комнате...";
        else
            return "Завершение подключения...";
    }
}