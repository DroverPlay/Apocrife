//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections.Generic;
//using TMPro;

//public class ButtonGenerator : MonoBehaviour
//{
//    public ModulesShaker characterCustomizer;
//    public GameObject buttonPrefab;

//    [Header("Родительские трансформы")]
//    public Transform genderPanel;
//    public Transform mainActionsPanel;
//    public Transform unisexPanel;
//    public Transform bodyPartsPanel;

//    [Header("Тексты для отображения")]
//    public TMP_Text currentGenderText;
//    public TMP_Text currentPresetText;

//    [System.Serializable]
//    public class ButtonInfo
//    {
//        public string buttonName;
//        public string category; // "gender", "main", "unisex", "body"
//        public System.Action action;
//        public Color color = Color.white;
//    }

//    private List<ButtonInfo> allButtons = new List<ButtonInfo>();

//    void Start()
//    {
//        InitializeButtons();
//        GenerateAllButtons();
//        UpdateDisplays();
//    }

//    void InitializeButtons()
//    {
//        // Кнопки для смены пола
//        allButtons.Add(new ButtonInfo
//        {
//            buttonName = "Мужской",
//            category = "gender",
//            action = () => { characterCustomizer.SetGender(0); UpdateDisplays(); },
//            color = new Color(0.2f, 0.4f, 1f) // Синий
//        });

//        allButtons.Add(new ButtonInfo
//        {
//            buttonName = "Женский",
//            category = "gender",
//            action = () => { characterCustomizer.SetGender(1); UpdateDisplays(); },
//            color = new Color(1f, 0.4f, 0.8f) // Розовый
//        });

//        // Основные действия
//        allButtons.Add(new ButtonInfo
//        {
//            buttonName = "Рандомизировать",
//            category = "main",
//            action = () => { characterCustomizer.RandomizeAll(); UpdateDisplays(); },
//            color = new Color(0.5f, 0.8f, 0.3f) // Зеленый
//        });

//        // Унисекс элементы
//        allButtons.Add(new ButtonInfo
//        {
//            buttonName = "Пояс",
//            category = "unisex",
//            action = characterCustomizer.RandomizeBelt
//        });

//        allButtons.Add(new ButtonInfo
//        {
//            buttonName = "Плащ",
//            category = "unisex",
//            action = characterCustomizer.RandomizeCape
//        });

//        allButtons.Add(new ButtonInfo
//        {
//            buttonName = "Локти",
//            category = "unisex",
//            action = characterCustomizer.RandomizeBothElbows
//        });

//        allButtons.Add(new ButtonInfo
//        {
//            buttonName = "Брови",
//            category = "unisex",
//            action = characterCustomizer.RandomizeEyebrows
//        });

//        allButtons.Add(new ButtonInfo
//        {
//            buttonName = "Колени",
//            category = "unisex",
//            action = characterCustomizer.RandomizeBothKnees
//        });

//        allButtons.Add(new ButtonInfo
//        {
//            buttonName = "Наплечники",
//            category = "unisex",
//            action = characterCustomizer.RandomizeBothPauldrons
//        });

//        // Части тела (гендерные)
//        allButtons.Add(new ButtonInfo
//        {
//            buttonName = "Руки",
//            category = "body",
//            action = characterCustomizer.RandomizeBothArms
//        });

//        allButtons.Add(new ButtonInfo
//        {
//            buttonName = "Ноги",
//            category = "body",
//            action = characterCustomizer.RandomizeLegs
//        });

//        allButtons.Add(new ButtonInfo
//        {
//            buttonName = "Торс",
//            category = "body",
//            action = characterCustomizer.RandomizeTorso
//        });

//        allButtons.Add(new ButtonInfo
//        {
//            buttonName = "Голова",
//            category = "body",
//            action = characterCustomizer.RandomizeHead
//        });

//        allButtons.Add(new ButtonInfo
//        {
//            buttonName = "Волосы",
//            category = "body",
//            action = characterCustomizer.RandomizeHair
//        });

//        allButtons.Add(new ButtonInfo
//        {
//            buttonName = "Кисти",
//            category = "body",
//            action = characterCustomizer.RandomizeBothHands
//        });

//        allButtons.Add(new ButtonInfo
//        {
//            buttonName = "Ступни",
//            category = "body",
//            action = characterCustomizer.RandomizeBothFeet
//        });

//        allButtons.Add(new ButtonInfo
//        {
//            buttonName = "Борода",
//            category = "body",
//            action = characterCustomizer.RandomizeFacialHair
//        });
//    }

//    void GenerateAllButtons()
//    {
//        foreach (var buttonInfo in allButtons)
//        {
//            Transform parentPanel = GetParentPanel(buttonInfo.category);
//            if (parentPanel == null) continue;

//            GameObject buttonGO = Instantiate(buttonPrefab, parentPanel);
//            Button button = buttonGO.GetComponent<Button>();
//            TMP_Text buttonText = buttonGO.GetComponentInChildren<TMP_Text>();
//            Image buttonImage = buttonGO.GetComponent<Image>();

//            // Настройка текста
//            if (buttonText != null)
//                buttonText.text = buttonInfo.buttonName;

//            // Настройка цвета
//            if (buttonImage != null && buttonInfo.color != Color.white)
//                buttonImage.color = buttonInfo.color;

//            // Сброс трансформа
//            RectTransform rt = buttonGO.GetComponent<RectTransform>();
//            if (rt != null)
//            {
//                rt.anchoredPosition = Vector2.zero;
//                rt.localScale = Vector3.one;
//            }

//            // Назначение действия
//            if (buttonInfo.action != null)
//                button.onClick.AddListener(() => buttonInfo.action());
//        }
//    }

//    Transform GetParentPanel(string category)
//    {
//        switch (category)
//        {
//            case "gender": return genderPanel;
//            case "main": return mainActionsPanel;
//            case "unisex": return unisexPanel;
//            case "body": return bodyPartsPanel;
//            default: return null;
//        }
//    }

//    void UpdateDisplays()
//    {
//        if (currentGenderText != null)
//        {
//            currentGenderText.text = $"Пол: {characterCustomizer.GetCurrentGenderName()}";
//        }

//        if (currentPresetText != null)
//        {
//            currentPresetText.text = $"Пресет: {characterCustomizer.set_numeration}";
//        }
//    }
//}