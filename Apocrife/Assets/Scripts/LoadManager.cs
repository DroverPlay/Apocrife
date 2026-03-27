using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadManager : MonoBehaviour
{
    [Header("Настройка загрузки")]
    [SerializeField] private Sprite[] _loadingSprite;
    [SerializeField] private float _imageDisplayTime = 3f;
    [SerializeField] private float _fadeDuration = 1f;
    [SerializeField] private Slider _loadingBar;
    [SerializeField] private TMP_Text _statusText;

    [Header("Настройки")]
    public float minLoadingTime = 2f;
    public string targetSceneName = "Game";

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
    }

    // Основной метод для загрузки сцены
    public void StartLoading(string sceneName)
    {
        if (_isLoading) return;

        _isLoading = true;
        _loadingStartTime = Time.time;
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
        StartCoroutine(LoadSceneRoutine());
    }

    // Старый метод для совместимости
    public void StartLocalLoading(int sceneIndex)
    {
        StartLoading(SceneManager.GetSceneByBuildIndex(sceneIndex).name);
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

    IEnumerator LoadSceneRoutine()
    {
        float progress = 0f;
        float elapsedTime = 0f;
        string lastStatus = "";

        UpdateStatus("Подготовка...");

        // Имитация загрузки с минимальным временем показа
        while (elapsedTime < minLoadingTime)
        {
            elapsedTime += Time.deltaTime;
            progress = Mathf.Clamp01(elapsedTime / minLoadingTime);

            if (_loadingBar != null)
                _loadingBar.value = progress;

            string newStatus = GetLoadingStatus(progress);
            if (newStatus != lastStatus)
            {
                UpdateStatus(newStatus);
                lastStatus = newStatus;
            }

            yield return null;
        }

        // Загружаем сцену
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
            asyncLoad.allowSceneActivation = false;

            // Обновляем прогресс-бар во время загрузки
            while (!asyncLoad.isDone)
            {
                progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // Unity загружает до 0.9

                if (_loadingBar != null)
                    _loadingBar.value = progress;

                if (asyncLoad.progress >= 0.9f)
                {
                    UpdateStatus("Завершение загрузки...");
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

    private string GetLoadingStatus(float progress)
    {
        if (progress <= 0.2f)
            return "Подготовка...";
        else if (progress <= 0.4f)
            return "Загрузка ресурсов...";
        else if (progress <= 0.7f)
            return "Инициализация...";
        else if (progress <= 0.9f)
            return "Завершение...";
        else
            return "Готово!";
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

    // Для обратной совместимости
    public void StartNetworkLoading(string sceneName)
    {
        StartLoading(sceneName);
    }
}