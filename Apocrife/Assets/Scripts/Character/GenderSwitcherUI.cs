using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GenderSwitcherUI : MonoBehaviour
{
    [Header("References")]
    public ModulesShaker modulesShaker; // Ссылка на основной скрипт

    [Header("UI Elements")]
    public Button maleButton;
    public Button femaleButton;
    public Button randomizeButton;
    public TMP_Text genderText;

    [Header("Settings")]
    public bool randomizeOnGenderSwitch = false; // Автоматически рандомизировать при смене пола

    private void Start()
    {
        // Настраиваем кнопки
        if (maleButton != null)
            maleButton.onClick.AddListener(SetMale);

        if (femaleButton != null)
            femaleButton.onClick.AddListener(SetFemale);

        if (randomizeButton != null)
            randomizeButton.onClick.AddListener(RandomizeCurrentGender);

        // Инициализируем UI текущим полом
        UpdateGenderUI();
    }

    public void SetMale()
    {
        if (modulesShaker == null)
        {
            Debug.LogError("ModulesShaker reference is not set!");
            return;
        }

        modulesShaker.gender_idx = 0; // 0 = Male

        modulesShaker.SetAll("001");

        UpdateGenderUI();
    }

    public void SetFemale()
    {
        if (modulesShaker == null)
        {
            Debug.LogError("ModulesShaker reference is not set!");
            return;
        }

        modulesShaker.gender_idx = 1; // 1 = Female

        modulesShaker.SetAll("001");

        UpdateGenderUI();
    }

    public void RandomizeCurrentGender()
    {
        if (modulesShaker == null)
        {
            Debug.LogError("ModulesShaker reference is not set!");
            return;
        }

        modulesShaker.RandomizeAll();
        UpdateGenderUI();
    }

    public void RandomizeCurrentGenderFromOtherPackages()
    {
        if (modulesShaker == null)
        {
            Debug.LogError("ModulesShaker reference is not set!");
            return;
        }

        modulesShaker.RandomizeAllOther();
        UpdateGenderUI();
    }

    public void SetSpecificPreset(string presetNumber)
    {
        if (modulesShaker == null)
        {
            Debug.LogError("ModulesShaker reference is not set!");
            return;
        }

        modulesShaker.SetAll(presetNumber);
        UpdateGenderUI();
    }

    private void UpdateGenderUI()
    {
        if (modulesShaker == null) return;

        string gender = modulesShaker.gender_idx == 0 ? "Мужской" : "Женский";

        if (genderText != null)
            genderText.text = $"Пол: {gender}";

        // Визуально выделяем активную кнопку
        if (maleButton != null && femaleButton != null)
        {
            ColorBlock maleColors = maleButton.colors;
            ColorBlock femaleColors = femaleButton.colors;

            if (modulesShaker.gender_idx == 0) // Male
            {
                maleColors.normalColor = Color.green;
                femaleColors.normalColor = Color.white;
            }
            else // Female
            {
                maleColors.normalColor = Color.white;
                femaleColors.normalColor = Color.green;
            }

            maleButton.colors = maleColors;
            femaleButton.colors = femaleColors;
        }
    }

    // Для использования в UI через инспектор
    public void SetGenderByIndex(int index)
    {
        if (index == 0)
            SetMale();
        else if (index == 1)
            SetFemale();
    }
}