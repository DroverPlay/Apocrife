using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CharacterMeshSlot
{
    public CharacterPartCategory category;
    public SkinnedMeshRenderer renderer;
    public Mesh[] meshOptions;
    public int currentIndex = 0;
}

public class UniversalCharacterMeshCustomizer : MonoBehaviour
{
    public GameObject _cust1; //первая часть кастомизации (основная)
    public GameObject _cust2; //вторая часть кастомизации (допы)
    // Синглтон для легкого доступа из других скриптов и сцен
    public static UniversalCharacterMeshCustomizer Instance { get; private set; }

    [Header("Gender Roots")]
    public GameObject maleRoot;
    public GameObject femaleRoot;

    [Header("Current Gender")]
    public CharacterGender currentGender = CharacterGender.Male;

    [Header("Male Parts")]
    public CharacterMeshSlot[] maleParts;

    [Header("Female Parts")]
    public CharacterMeshSlot[] femaleParts;

    [Header("Unisex Parts")]
    public CharacterMeshSlot[] unisexParts;

    [Header("Debug Keys")]
    public bool enableDebugKeys = true;

    [Header("Save Settings")]
    [SerializeField] private string saveKeyPrefix = "CharCustomizer_";
    [SerializeField] private bool autoLoadOnStart = true;

    private void Awake()
    {
        // Настройка Синглтона, чтобы персонаж не уничтожался при переходе в игровой мир
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (autoLoadOnStart)
        {
            LoadCharacter(); // Автоматически загружаем сохраненного персонажа при старте
        }
        else
        {
            ApplyGender(currentGender);
            ApplyAllCurrentMeshes();
        }
    }

    private void Update()
    {
        if (!enableDebugKeys) return;

        // --- DEBUG TEST KEYS ---
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetMale();
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetFemale();
        if (Input.GetKeyDown(KeyCode.H)) Next(CharacterPartCategory.Hair);
        if (Input.GetKeyDown(KeyCode.B)) Next(CharacterPartCategory.FacialHair);
        if (Input.GetKeyDown(KeyCode.T)) Next(CharacterPartCategory.Torso);
        if (Input.GetKeyDown(KeyCode.L)) Next(CharacterPartCategory.Legs);
        if (Input.GetKeyDown(KeyCode.G)) Next(CharacterPartCategory.Head);
        if (Input.GetKeyDown(KeyCode.R)) Randomize();

        // Быстрые клавиши для проверки сохранения/загрузки в редакторе
        if (Input.GetKeyDown(KeyCode.S)) SaveCharacter();
        if (Input.GetKeyDown(KeyCode.O)) LoadCharacter();
    }

    // -------------------------
    // СОХРАНЕНИЕ И ЗАГРУЗКА (PlayerPrefs)
    // -------------------------

    public void SaveCharacter()
    {
        // Сохраняем пол (0 - Male, 1 - Female)
        PlayerPrefs.SetInt(saveKeyPrefix + "Gender", (int)currentGender);

        // Сохраняем индексы мужских частей
        for (int i = 0; i < maleParts.Length; i++)
        {
            PlayerPrefs.SetInt(saveKeyPrefix + "Male_" + maleParts[i].category.ToString(), maleParts[i].currentIndex);
        }

        // Сохраняем индексы женских частей
        for (int i = 0; i < femaleParts.Length; i++)
        {
            PlayerPrefs.SetInt(saveKeyPrefix + "Female_" + femaleParts[i].category.ToString(), femaleParts[i].currentIndex);
        }

        // Сохраняем индексы унисекс частей
        for (int i = 0; i < unisexParts.Length; i++)
        {
            PlayerPrefs.SetInt(saveKeyPrefix + "Unisex_" + unisexParts[i].category.ToString(), unisexParts[i].currentIndex);
        }

        PlayerPrefs.Save();
        Debug.Log("Внешний вид персонажа успешно СОХРАНЕН!");
    }

    public void LoadCharacter()
    {
        // Если сохранений еще нет, оставляем дефолтные настройки
        if (!PlayerPrefs.HasKey(saveKeyPrefix + "Gender"))
        {
            Debug.Log("Сохранений персонажа не найдено. Загружены настройки по умолчанию.");
            ApplyGender(currentGender);
            return;
        }

        // Загружаем пол
        currentGender = (CharacterGender)PlayerPrefs.GetInt(saveKeyPrefix + "Gender");

        // Загружаем индексы мужских частей
        for (int i = 0; i < maleParts.Length; i++)
        {
            string key = saveKeyPrefix + "Male_" + maleParts[i].category.ToString();
            if (PlayerPrefs.HasKey(key))
                maleParts[i].currentIndex = PlayerPrefs.GetInt(key);
        }

        // Загружаем индексы женских частей
        for (int i = 0; i < femaleParts.Length; i++)
        {
            string key = saveKeyPrefix + "Female_" + femaleParts[i].category.ToString();
            if (PlayerPrefs.HasKey(key))
                femaleParts[i].currentIndex = PlayerPrefs.GetInt(key);
        }

        // Загружаем индексы унисекс частей
        for (int i = 0; i < unisexParts.Length; i++)
        {
            string key = saveKeyPrefix + "Unisex_" + unisexParts[i].category.ToString();
            if (PlayerPrefs.HasKey(key))
                unisexParts[i].currentIndex = PlayerPrefs.GetInt(key);
        }

        // Применяем загруженные меши на персонажа
        ApplyGender(currentGender);
        Debug.Log("Внешний вид персонажа успешно ЗАГРУЖЕН!");
    }

    // -------------------------
    // GENDER
    // -------------------------

    public void SetMale() => ApplyGender(CharacterGender.Male);
    public void SetFemale() => ApplyGender(CharacterGender.Female);

    public void ApplyGender(CharacterGender gender)
    {
        currentGender = gender;

        if (maleRoot != null) maleRoot.SetActive(gender == CharacterGender.Male);
        if (femaleRoot != null) femaleRoot.SetActive(gender == CharacterGender.Female);

        ApplyAllCurrentMeshes();
    }

    // -------------------------
    // PART SWITCHING
    // -------------------------

    public void Next(CharacterPartCategory category)
    {
        CharacterMeshSlot slot = GetSlot(category);
        if (slot == null || slot.meshOptions == null || slot.meshOptions.Length == 0) return;

        slot.currentIndex = (slot.currentIndex + 1) % slot.meshOptions.Length;
        ApplySlot(slot);
    }

    public void Previous(CharacterPartCategory category)
    {
        CharacterMeshSlot slot = GetSlot(category);
        if (slot == null || slot.meshOptions == null || slot.meshOptions.Length == 0) return;

        slot.currentIndex--;
        if (slot.currentIndex < 0) slot.currentIndex = slot.meshOptions.Length - 1;

        ApplySlot(slot);
    }

    public void SetIndex(CharacterPartCategory category, int index)
    {
        CharacterMeshSlot slot = GetSlot(category);
        if (slot == null || slot.meshOptions == null || slot.meshOptions.Length == 0) return;

        slot.currentIndex = Mathf.Clamp(index, 0, slot.meshOptions.Length - 1);
        ApplySlot(slot);
    }

    public int GetIndex(CharacterPartCategory category)
    {
        CharacterMeshSlot slot = GetSlot(category);
        return slot != null ? slot.currentIndex : 0;
    }

    // -------------------------
    // APPLY
    // -------------------------

    public void ApplyAllCurrentMeshes()
    {
        CharacterMeshSlot[] activeParts = GetActiveGenderParts();

        if (activeParts != null)
        {
            for (int i = 0; i < activeParts.Length; i++) ApplySlot(activeParts[i]);
        }

        if (unisexParts != null)
        {
            for (int i = 0; i < unisexParts.Length; i++) ApplySlot(unisexParts[i]);
        }
    }

    private void ApplySlot(CharacterMeshSlot slot)
    {
        if (slot == null || slot.renderer == null || slot.meshOptions == null || slot.meshOptions.Length == 0) return;

        if (slot.currentIndex < 0 || slot.currentIndex >= slot.meshOptions.Length) slot.currentIndex = 0;

        if (!slot.renderer.gameObject.activeSelf) slot.renderer.gameObject.SetActive(true);

        if (slot.renderer.transform.parent != null && !slot.renderer.transform.parent.gameObject.activeSelf)
            slot.renderer.transform.parent.gameObject.SetActive(true);

        slot.renderer.sharedMesh = slot.meshOptions[slot.currentIndex];
    }

    // -------------------------
    // RANDOM
    // -------------------------

    public void Randomize()
    {
        CharacterMeshSlot[] activeParts = GetActiveGenderParts();

        if (activeParts != null)
        {
            for (int i = 0; i < activeParts.Length; i++)
            {
                CharacterMeshSlot slot = activeParts[i];
                if (slot == null || slot.meshOptions == null || slot.meshOptions.Length == 0) continue;
                slot.currentIndex = Random.Range(0, slot.meshOptions.Length);
                ApplySlot(slot);
            }
        }

        if (unisexParts != null)
        {
            for (int i = 0; i < unisexParts.Length; i++)
            {
                CharacterMeshSlot slot = unisexParts[i];
                if (slot == null || slot.meshOptions == null || slot.meshOptions.Length == 0) continue;
                slot.currentIndex = Random.Range(0, slot.meshOptions.Length);
                ApplySlot(slot);
            }
        }
    }

    private CharacterMeshSlot GetSlot(CharacterPartCategory category)
    {
        CharacterMeshSlot[] activeParts = GetActiveGenderParts();

        if (activeParts != null)
        {
            for (int i = 0; i < activeParts.Length; i++)
            {
                if (activeParts[i].category == category) return activeParts[i];
            }
        }

        if (unisexParts != null)
        {
            for (int i = 0; i < unisexParts.Length; i++)
            {
                if (unisexParts[i].category == category) return unisexParts[i];
            }
        }
        return null;
    }
    public void NextCustomSetting()
    {
        _cust1.SetActive(false);
        _cust2.SetActive(true);
    }

    public void PrevCustomSettings()
    {
        _cust1.SetActive(true);
        _cust2.SetActive(false);
    }
    public void GoToGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    private CharacterMeshSlot[] GetActiveGenderParts() => currentGender == CharacterGender.Male ? maleParts : femaleParts;
}
