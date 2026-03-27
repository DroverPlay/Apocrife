using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModulesShaker : MonoBehaviour
{
    [Header("Package details:")]
    [Space]
    public string package_mark = "AA";
    public int first_set_number = 0;
    public int last_set_number = 10;

    [Space]
    public string set_numeration = "001";

    [HideInInspector]
    public string[] Gender = new string[] { "Male", "Female" };
    [HideInInspector]
    public int gender_idx = 0;

    [Space]
    [Header("UI References (Optional):")]
    public TMP_Text genderText;
    public Button changeGenderButton;

    [Space]
    [Header("Randomize Buttons (Optional):")]
    public Button randomizeAllButton;
    public Button randomizeAllOtherButton;
    [Space]
    // Кнопки для отдельных элементов
    public Button randomizeBeltButton;
    public Button randomizeCapeButton;
    public Button randomizeElbowsButton;
    public Button randomizeEyebrowsButton;
    public Button randomizeKneesButton;
    public Button randomizePauldronsButton;
    public Button randomizeArmsButton;
    public Button randomizeCalvesButton;
    public Button randomizeFeetButton;
    public Button randomizeForearmsButton;
    public Button randomizeHairButton;
    public Button randomizeHandsButton;
    public Button randomizeHeadButton;
    public Button randomizeLegsButton;
    public Button randomizeTorsoButton;
    public Button randomizeFacialHairButton;

    [Space]
    [Header("Other modular packages:")]
    public string[] other_packages = new string[] { };

    [Space]
    [Space]
    [Header("Body elements:")]
    [Space]

    // Унисекс элементы
    public GameObject unisex_belt;
    public GameObject unisex_cape;
    public GameObject unisex_elbow_l;
    public GameObject unisex_elbow_r;
    public GameObject unisex_eyebrows;
    public GameObject unisex_knee_l;
    public GameObject unisex_knee_r;
    public GameObject unisex_pauldron_l;
    public GameObject unisex_pauldron_r;

    // Female elements
    public GameObject arm_f_l;
    public GameObject arm_f_r;
    public GameObject calf_f_l;
    public GameObject calf_f_r;
    public GameObject foot_f_l;
    public GameObject foot_f_r;
    public GameObject forearm_f_l;
    public GameObject forearm_f_r;
    public GameObject hair_f;
    public GameObject hand_f_l;
    public GameObject hand_f_r;
    public GameObject head_f;
    public GameObject legs_f;
    public GameObject torso_f;

    // Male elements
    public GameObject arm_m_l;
    public GameObject arm_m_r;
    public GameObject calf_m_l;
    public GameObject calf_m_r;
    public GameObject facial_hair;
    public GameObject foot_m_l;
    public GameObject foot_m_r;
    public GameObject forearm_m_l;
    public GameObject forearm_m_r;
    public GameObject hair_m;
    public GameObject hand_m_l;
    public GameObject hand_m_r;
    public GameObject head_m;
    public GameObject legs_m;
    public GameObject torso_m;

    void Start()
    {
        InitializeUIButtons();
        InitializeAllElements();
        UpdateGenderUI();
        UpdateFacialHairButtonVisibility();
    }

    void InitializeUIButtons()
    {

        if (randomizeAllButton != null)
            randomizeAllButton.onClick.AddListener(RandomizeAll);

        // Инициализация кнопок отдельных элементов
        if (randomizeBeltButton != null)
            randomizeBeltButton.onClick.AddListener(RandomizeBelt);

        if (randomizeCapeButton != null)
            randomizeCapeButton.onClick.AddListener(RandomizeCape);

        if (randomizeElbowsButton != null)
            randomizeElbowsButton.onClick.AddListener(RandomizeBothElbows);

        if (randomizeEyebrowsButton != null)
            randomizeEyebrowsButton.onClick.AddListener(RandomizeEyebrows);

        if (randomizeKneesButton != null)
            randomizeKneesButton.onClick.AddListener(RandomizeBothKnees);

        if (randomizePauldronsButton != null)
            randomizePauldronsButton.onClick.AddListener(RandomizeBothPauldrons);

        if (randomizeArmsButton != null)
            randomizeArmsButton.onClick.AddListener(RandomizeBothArms);

        if (randomizeCalvesButton != null)
            randomizeCalvesButton.onClick.AddListener(RandomizeBothCalves);

        if (randomizeFeetButton != null)
            randomizeFeetButton.onClick.AddListener(RandomizeBothFeet);

        if (randomizeForearmsButton != null)
            randomizeForearmsButton.onClick.AddListener(RandomizeBothForearms);

        if (randomizeHairButton != null)
            randomizeHairButton.onClick.AddListener(RandomizeHair);

        if (randomizeHandsButton != null)
            randomizeHandsButton.onClick.AddListener(RandomizeBothHands);

        if (randomizeHeadButton != null)
            randomizeHeadButton.onClick.AddListener(RandomizeHead);

        if (randomizeLegsButton != null)
            randomizeLegsButton.onClick.AddListener(RandomizeLegs);

        if (randomizeTorsoButton != null)
            randomizeTorsoButton.onClick.AddListener(RandomizeTorso);

        if (randomizeFacialHairButton != null)
            randomizeFacialHairButton.onClick.AddListener(RandomizeFacialHair);
    }

    void InitializeAllElements()
    {
        SetAll(set_numeration);
    }

    
    #region Гендерные методы
    public void SetGender(int genderIndex)
    {
        if (genderIndex >= 0 && genderIndex < Gender.Length)
        {
            gender_idx = genderIndex;
            SetAll(set_numeration);
            UpdateGenderUI();
            UpdateFacialHairButtonVisibility();
        }
    }

    public void SetGender(string genderName)
    {
        for (int i = 0; i < Gender.Length; i++)
        {
            if (Gender[i].Equals(genderName, StringComparison.OrdinalIgnoreCase))
            {
                SetGender(i);
                return;
            }
        }
        Debug.LogWarning($"Пол '{genderName}' не найден");
    }

    public string GetCurrentGenderName()
    {
        if (gender_idx >= 0 && gender_idx < Gender.Length)
        {
            return Gender[gender_idx];
        }
        return "Unknown";
    }

    void UpdateGenderUI()
    {
        if (genderText != null)
        {
            genderText.text = $"Пол: {GetCurrentGenderName()}";
        }
    }

    void UpdateFacialHairButtonVisibility()
    {
        if (randomizeFacialHairButton != null)
        {
            randomizeFacialHairButton.gameObject.SetActive(gender_idx == 0);
        }
    }
    #endregion

    #region Методы рандомизации отдельных элементов
    public void RandomizeBelt() => RandomizeElement("Belt");
    public void RandomizeCape() => RandomizeElement("Cape");
    public void RandomizeElbowLeft() => RandomizeElement("Elbow_L");
    public void RandomizeElbowRight() => RandomizeElement("Elbow_R");
    public void RandomizeBothElbows() { RandomizeElbowLeft(); RandomizeElbowRight(); }
    public void RandomizeEyebrows() => RandomizeElement("Eyebrows");
    public void RandomizeKneeLeft() => RandomizeElement("Knee_L");
    public void RandomizeKneeRight() => RandomizeElement("Knee_R");
    public void RandomizeBothKnees() { RandomizeKneeLeft(); RandomizeKneeRight(); }
    public void RandomizePauldronLeft() => RandomizeElement("Pauldron_L");
    public void RandomizePauldronRight() => RandomizeElement("Pauldron_R");
    public void RandomizeBothPauldrons() { RandomizePauldronLeft(); RandomizePauldronRight(); }

    public void RandomizeArmLeft() => RandomizeElement($"Arm_{getGender()}_L");
    public void RandomizeArmRight() => RandomizeElement($"Arm_{getGender()}_R");
    public void RandomizeBothArms() { RandomizeArmLeft(); RandomizeArmRight(); }

    public void RandomizeCalfLeft() => RandomizeElement($"Calf_{getGender()}_L");
    public void RandomizeCalfRight() => RandomizeElement($"Calf_{getGender()}_R");
    public void RandomizeBothCalves() { RandomizeCalfLeft(); RandomizeCalfRight(); }

    public void RandomizeFootLeft() => RandomizeElement($"Foot_{getGender()}_L");
    public void RandomizeFootRight() => RandomizeElement($"Foot_{getGender()}_R");
    public void RandomizeBothFeet() { RandomizeFootLeft(); RandomizeFootRight(); }

    public void RandomizeForearmLeft() => RandomizeElement($"Forearm_{getGender()}_L");
    public void RandomizeForearmRight() => RandomizeElement($"Forearm_{getGender()}_R");
    public void RandomizeBothForearms() { RandomizeForearmLeft(); RandomizeForearmRight(); }

    public void RandomizeHair() => RandomizeElement($"Hair_{getGender()}");

    public void RandomizeHandLeft() => RandomizeElement($"Hand_{getGender()}_L");
    public void RandomizeHandRight() => RandomizeElement($"Hand_{getGender()}_R");
    public void RandomizeBothHands() { RandomizeHandLeft(); RandomizeHandRight(); }

    public void RandomizeHead() => RandomizeElement($"Head_{getGender()}");
    public void RandomizeLegs() => RandomizeElement($"Legs_{getGender()}");
    public void RandomizeTorso() => RandomizeElement($"Torso_{getGender()}");

    public void RandomizeFacialHair()
    {
        if (gender_idx == 0) RandomizeElement("Facial_Hair");
    }

    public void RandomizeElement(string element, bool fromOtherPackages = false)
    {
        string randomNumeration = getRandomNumeration();
        SetElement(element, randomNumeration, fromOtherPackages, !fromOtherPackages);
        Debug.Log($"Рандомизирован: {element}");
    }
    #endregion

    #region Основные методы кастомизации
    public void SetAll(string numeration)
    {
        // Унисекс элементы
        SetElement("Belt", numeration);
        SetElement("Cape", numeration);
        SetElement("Elbow_L", numeration);
        SetElement("Elbow_R", numeration);
        SetElement("Eyebrows", numeration);
        SetElement("Knee_L", numeration);
        SetElement("Knee_R", numeration);
        SetElement("Pauldron_L", numeration);
        SetElement("Pauldron_R", numeration);

        string currentGender = getGender();

        // Гендерные элементы
        SetElement($"Arm_{currentGender}_L", numeration);
        SetElement($"Arm_{currentGender}_R", numeration);
        SetElement($"Calf_{currentGender}_L", numeration);
        SetElement($"Calf_{currentGender}_R", numeration);
        SetElement($"Foot_{currentGender}_L", numeration);
        SetElement($"Foot_{currentGender}_R", numeration);
        SetElement($"Forearm_{currentGender}_L", numeration);
        SetElement($"Forearm_{currentGender}_R", numeration);
        SetElement($"Hand_{currentGender}_L", numeration);
        SetElement($"Hand_{currentGender}_R", numeration);

        SetElement($"Hair_{currentGender}", numeration);
        SetElement($"Head_{currentGender}", numeration);
        SetElement($"Legs_{currentGender}", numeration);
        SetElement($"Torso_{currentGender}", numeration);

        if (currentGender == "M")
        {
            SetElement("Facial_Hair", numeration);
        }
        else if (facial_hair != null)
        {
            facial_hair.SetActive(false);
        }
    }

    public void RandomizeAll()
    {

        SetElement("Belt", getRandomNumeration(), false, true);
        SetElement("Cape", getRandomNumeration(), false, true);
        SetElement("Elbow_L", getRandomNumeration(), false, true);
        SetElement("Elbow_R", getRandomNumeration(), false, true);
        SetElement("Eyebrows", getRandomNumeration(), false, true);
        SetElement("Knee_L", getRandomNumeration(), false, true);
        SetElement("Knee_R", getRandomNumeration(), false, true);
        SetElement("Pauldron_L", getRandomNumeration(), false, true);
        SetElement("Pauldron_R", getRandomNumeration(), false, true);

        string gender = getGender();

        SetElement($"Arm_{gender}_L", getRandomNumeration(), false, true);
        SetElement($"Arm_{gender}_R", getRandomNumeration(), false, true);
        SetElement($"Calf_{gender}_L", getRandomNumeration(), false, true);
        SetElement($"Calf_{gender}_R", getRandomNumeration(), false, true);

        if (gender == "M")
        {
            SetElement("Facial_Hair", getRandomNumeration(), false, true);
        }
        else
        {
            if (facial_hair != null) facial_hair.SetActive(false);
        }

        SetElement($"Foot_{gender}_L", getRandomNumeration(), false, true);
        SetElement($"Foot_{gender}_R", getRandomNumeration(), false, true);
        SetElement($"Forearm_{gender}_L", getRandomNumeration(), false, true);
        SetElement($"Forearm_{gender}_R", getRandomNumeration(), false, true);
        SetElement($"Hair_{gender}", getRandomNumeration(), false, true);
        SetElement($"Hand_{gender}_L", getRandomNumeration(), false, true);
        SetElement($"Hand_{gender}_R", getRandomNumeration(), false, true);
        SetElement($"Head_{gender}", getRandomNumeration(), false, true);
        SetElement($"Legs_{gender}", getRandomNumeration(), false, true);
        SetElement($"Torso_{gender}", getRandomNumeration(), false, true);
    }

    public void RandomizeAllOther()
    {
        if (other_packages.Length == 0)
        {
            Debug.LogError("Нет других пакетов в секции 'Other_packages'");
            return;
        }

        SetElement("Belt", getRandomNumeration(), true, false);
        SetElement("Cape", getRandomNumeration(), true, false);
        SetElement("Elbow_L", getRandomNumeration(), true, false);
        SetElement("Elbow_R", getRandomNumeration(), true, false);
        SetElement("Eyebrows", getRandomNumeration(), true, false);
        SetElement("Knee_L", getRandomNumeration(), true, false);
        SetElement("Knee_R", getRandomNumeration(), true, false);
        SetElement("Pauldron_L", getRandomNumeration(), true, false);
        SetElement("Pauldron_R", getRandomNumeration(), true, false);

        string gender = getGender();

        SetElement($"Arm_{gender}_L", getRandomNumeration(), true, false);
        SetElement($"Arm_{gender}_R", getRandomNumeration(), true, false);
        SetElement($"Calf_{gender}_L", getRandomNumeration(), true, false);
        SetElement($"Calf_{gender}_R", getRandomNumeration(), true, false);

        if (gender == "M")
        {
            SetElement("Facial_Hair", getRandomNumeration(), true, false);
        }
        else
        {
            if (facial_hair != null) facial_hair.SetActive(false);
        }

        SetElement($"Foot_{gender}_L", getRandomNumeration(), true, false);
        SetElement($"Foot_{gender}_R", getRandomNumeration(), true, false);
        SetElement($"Forearm_{gender}_L", getRandomNumeration(), true, false);
        SetElement($"Forearm_{gender}_R", getRandomNumeration(), true, false);
        SetElement($"Hair_{gender}", getRandomNumeration(), true, false);
        SetElement($"Hand_{gender}_L", getRandomNumeration(), true, false);
        SetElement($"Hand_{gender}_R", getRandomNumeration(), true, false);
        SetElement($"Head_{gender}", getRandomNumeration(), true, false);
        SetElement($"Legs_{gender}", getRandomNumeration(), true, false);
        SetElement($"Torso_{gender}", getRandomNumeration(), true, false);
    }
    #endregion

    #region Вспомогательные методы
    public void SetElement(string element, string numeration, bool randomOtherPackages = false, bool randomSamePackage = false)
    {
        SkinnedMeshRenderer smr = null;
        string from_package = package_mark;
        bool random = randomOtherPackages || randomSamePackage;

        // Определяем SkinnedMeshRenderer и управляем активностью объектов
        // (эта часть остается как в оригинальном коде)
        if (element == "Belt") smr = unisex_belt.GetComponent<SkinnedMeshRenderer>();
        else if (element == "Cape") smr = unisex_cape.GetComponent<SkinnedMeshRenderer>();
        else if (element == "Elbow_L") smr = unisex_elbow_l.GetComponent<SkinnedMeshRenderer>();
        else if (element == "Elbow_R") smr = unisex_elbow_r.GetComponent<SkinnedMeshRenderer>();
        else if (element == "Eyebrows") smr = unisex_eyebrows.GetComponent<SkinnedMeshRenderer>();
        else if (element == "Knee_L") smr = unisex_knee_l.GetComponent<SkinnedMeshRenderer>();
        else if (element == "Knee_R") smr = unisex_knee_r.GetComponent<SkinnedMeshRenderer>();
        else if (element == "Pauldron_L") smr = unisex_pauldron_l.GetComponent<SkinnedMeshRenderer>();
        else if (element == "Pauldron_R") smr = unisex_pauldron_r.GetComponent<SkinnedMeshRenderer>();

        // Женские элементы
        else if (element == "Arm_F_L") { smr = arm_f_l.GetComponent<SkinnedMeshRenderer>(); arm_f_l.SetActive(true); arm_m_l.SetActive(false); }
        else if (element == "Arm_F_R") { smr = arm_f_r.GetComponent<SkinnedMeshRenderer>(); arm_f_r.SetActive(true); arm_m_r.SetActive(false); }
        else if (element == "Calf_F_L") { smr = calf_f_l.GetComponent<SkinnedMeshRenderer>(); calf_f_l.SetActive(true); calf_m_l.SetActive(false); }
        else if (element == "Calf_F_R") { smr = calf_f_r.GetComponent<SkinnedMeshRenderer>(); calf_f_r.SetActive(true); calf_m_r.SetActive(false); }
        else if (element == "Foot_F_L") { smr = foot_f_l.GetComponent<SkinnedMeshRenderer>(); foot_f_l.SetActive(true); foot_m_l.SetActive(false); }
        else if (element == "Foot_F_R") { smr = foot_f_r.GetComponent<SkinnedMeshRenderer>(); foot_f_r.SetActive(true); foot_m_r.SetActive(false); }
        else if (element == "Forearm_F_L") { smr = forearm_f_l.GetComponent<SkinnedMeshRenderer>(); forearm_f_l.SetActive(true); forearm_m_l.SetActive(false); }
        else if (element == "Forearm_F_R") { smr = forearm_f_r.GetComponent<SkinnedMeshRenderer>(); forearm_f_r.SetActive(true); forearm_m_r.SetActive(false); }
        else if (element == "Hair_F") { smr = hair_f.GetComponent<SkinnedMeshRenderer>(); hair_f.SetActive(true); hair_m.SetActive(false); }
        else if (element == "Hand_F_L") { smr = hand_f_l.GetComponent<SkinnedMeshRenderer>(); hand_f_l.SetActive(true); hand_m_l.SetActive(false); }
        else if (element == "Hand_F_R") { smr = hand_f_r.GetComponent<SkinnedMeshRenderer>(); hand_f_r.SetActive(true); hand_m_r.SetActive(false); }
        else if (element == "Head_F") { smr = head_f.GetComponent<SkinnedMeshRenderer>(); head_f.SetActive(true); head_m.SetActive(false); }
        else if (element == "Legs_F") { smr = legs_f.GetComponent<SkinnedMeshRenderer>(); legs_f.SetActive(true); legs_m.SetActive(false); }
        else if (element == "Torso_F") { smr = torso_f.GetComponent<SkinnedMeshRenderer>(); torso_f.SetActive(true); torso_m.SetActive(false); }

        // Мужские элементы
        else if (element == "Arm_M_L") { smr = arm_m_l.GetComponent<SkinnedMeshRenderer>(); arm_m_l.SetActive(true); arm_f_l.SetActive(false); }
        else if (element == "Arm_M_R") { smr = arm_m_r.GetComponent<SkinnedMeshRenderer>(); arm_m_r.SetActive(true); arm_f_r.SetActive(false); }
        else if (element == "Calf_M_L") { smr = calf_m_l.GetComponent<SkinnedMeshRenderer>(); calf_m_l.SetActive(true); calf_f_l.SetActive(false); }
        else if (element == "Calf_M_R") { smr = calf_m_r.GetComponent<SkinnedMeshRenderer>(); calf_m_r.SetActive(true); calf_f_r.SetActive(false); }
        else if (element == "Facial_Hair") { smr = facial_hair.GetComponent<SkinnedMeshRenderer>(); facial_hair.SetActive(true); }
        else if (element == "Foot_M_L") { smr = foot_m_l.GetComponent<SkinnedMeshRenderer>(); foot_m_l.SetActive(true); foot_f_l.SetActive(false); }
        else if (element == "Foot_M_R") { smr = foot_m_r.GetComponent<SkinnedMeshRenderer>(); foot_m_r.SetActive(true); foot_f_r.SetActive(false); }
        else if (element == "Forearm_M_L") { smr = forearm_m_l.GetComponent<SkinnedMeshRenderer>(); forearm_m_l.SetActive(true); forearm_f_l.SetActive(false); }
        else if (element == "Forearm_M_R") { smr = forearm_m_r.GetComponent<SkinnedMeshRenderer>(); forearm_m_r.SetActive(true); forearm_f_r.SetActive(false); }
        else if (element == "Hair_M") { smr = hair_m.GetComponent<SkinnedMeshRenderer>(); hair_m.SetActive(true); hair_f.SetActive(false); }
        else if (element == "Hand_M_L") { smr = hand_m_l.GetComponent<SkinnedMeshRenderer>(); hand_m_l.SetActive(true); hand_f_l.SetActive(false); }
        else if (element == "Hand_M_R") { smr = hand_m_r.GetComponent<SkinnedMeshRenderer>(); hand_m_r.SetActive(true); hand_f_r.SetActive(false); }
        else if (element == "Head_M") { smr = head_m.GetComponent<SkinnedMeshRenderer>(); head_m.SetActive(true); head_f.SetActive(false); }
        else if (element == "Legs_M") { smr = legs_m.GetComponent<SkinnedMeshRenderer>(); legs_m.SetActive(true); legs_f.SetActive(false); }
        else if (element == "Torso_M") { smr = torso_m.GetComponent<SkinnedMeshRenderer>(); torso_m.SetActive(true); torso_f.SetActive(false); }

        if (randomOtherPackages)
        {
            from_package = getRandomOtherPackage();
        }

        if (smr != null)
        {
            smr.sharedMesh = findMesh(element, from_package, numeration, random, randomOtherPackages, 0);
            if (smr.sharedMesh == null)
            {
                if (numeration != "000")
                {
                    smr.gameObject.SetActive(false);
                }
            }
            else
            {
                smr.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError($"SkinnedMeshRenderer не найден для элемента: {element}");
        }
    }

    string getRandomOtherPackage()
    {
        if (other_packages.Length == 0) return package_mark;
        int random = UnityEngine.Random.Range(0, other_packages.Length);
        return other_packages[random];
    }

    string getRandomNumeration()
    {
        int random = UnityEngine.Random.Range(first_set_number, last_set_number + 1);
        return random.ToString().PadLeft(3, '0');
    }

    string getGender()
    {
        return gender_idx == 0 ? "M" : "F";
    }

    Mesh findMesh(string element, string from_package, string numeration, bool random, bool randomOtherPackages, int tries)
    {
        Mesh[] meshes = Resources.FindObjectsOfTypeAll<Mesh>();
        string meshName = element + "_" + from_package + "_" + numeration;

        foreach (Mesh mesh in meshes)
        {
            if (mesh.name == meshName)
            {
                return mesh;
            }
        }

        if (random && tries < 100)
        {
            string newNumeration = getRandomNumeration();
            string newPackage = randomOtherPackages ? getRandomOtherPackage() : from_package;
            return findMesh(element, newPackage, newNumeration, random, randomOtherPackages, tries + 1);
        }

        Debug.LogWarning($"Меш не найден: {meshName}");
        return null;
    }
    #endregion

    #region UI методы
    public void ApplyPreset(string presetNumeration)
    {
        if (presetNumeration.Length < 3)
        {
            presetNumeration = presetNumeration.PadLeft(3, '0');
        }

        set_numeration = presetNumeration;
        SetAll(presetNumeration);
        Debug.Log($"Применен пресет: {presetNumeration}");
    }

    public string GetCurrentPreset()
    {
        return set_numeration;
    }

    public void ResetCharacter()
    {
        set_numeration = "001";
        gender_idx = 0;
        SetAll("001");
        UpdateGenderUI();
        UpdateFacialHairButtonVisibility();
        Debug.Log("Персонаж сброшен");
    }
    public CharacterSaveData GetCurrentCharacterData(string characterName = "MyCharacter")
    {
        CharacterSaveData data = new CharacterSaveData();
        data.characterName = characterName;
        data.saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        data.gender = gender_idx;
        data.packageMark = package_mark;
        data.otherPackages = other_packages;

        // Получаем текущие пресеты для всех частей тела
        // Для этого нужно проанализировать текущие меши
        CollectCurrentPresets(data);

        return data;
    }

    private void CollectCurrentPresets(CharacterSaveData data)
    {
        // Собираем пресеты из SkinnedMeshRenderer
        // Унисекс части
        AddPresetIfExists(data, "Belt", unisex_belt);
        AddPresetIfExists(data, "Cape", unisex_cape);
        AddPresetIfExists(data, "Elbow_L", unisex_elbow_l);
        AddPresetIfExists(data, "Elbow_R", unisex_elbow_r);
        AddPresetIfExists(data, "Eyebrows", unisex_eyebrows);
        AddPresetIfExists(data, "Knee_L", unisex_knee_l);
        AddPresetIfExists(data, "Knee_R", unisex_knee_r);
        AddPresetIfExists(data, "Pauldron_L", unisex_pauldron_l);
        AddPresetIfExists(data, "Pauldron_R", unisex_pauldron_r);

        // Части по полу
        if (gender_idx == 0) // Male
        {
            AddPresetIfExists(data, "Arm_L", arm_m_l);
            AddPresetIfExists(data, "Arm_R", arm_m_r);
            AddPresetIfExists(data, "Calf_L", calf_m_l);
            AddPresetIfExists(data, "Calf_R", calf_m_r);
            AddPresetIfExists(data, "Foot_L", foot_m_l);
            AddPresetIfExists(data, "Foot_R", foot_m_r);
            AddPresetIfExists(data, "Forearm_L", forearm_m_l);
            AddPresetIfExists(data, "Forearm_R", forearm_m_r);
            AddPresetIfExists(data, "Hair", hair_m);
            AddPresetIfExists(data, "Hand_L", hand_m_l);
            AddPresetIfExists(data, "Hand_R", hand_m_r);
            AddPresetIfExists(data, "Head", head_m);
            AddPresetIfExists(data, "Legs", legs_m);
            AddPresetIfExists(data, "Torso", torso_m);
            AddPresetIfExists(data, "Facial_Hair", facial_hair);
        }
        else // Female
        {
            AddPresetIfExists(data, "Arm_L", arm_f_l);
            AddPresetIfExists(data, "Arm_R", arm_f_r);
            AddPresetIfExists(data, "Calf_L", calf_f_l);
            AddPresetIfExists(data, "Calf_R", calf_f_r);
            AddPresetIfExists(data, "Foot_L", foot_f_l);
            AddPresetIfExists(data, "Foot_R", foot_f_r);
            AddPresetIfExists(data, "Forearm_L", forearm_f_l);
            AddPresetIfExists(data, "Forearm_R", forearm_f_r);
            AddPresetIfExists(data, "Hair", hair_f);
            AddPresetIfExists(data, "Hand_L", hand_f_l);
            AddPresetIfExists(data, "Hand_R", hand_f_r);
            AddPresetIfExists(data, "Head", head_f);
            AddPresetIfExists(data, "Legs", legs_f);
            AddPresetIfExists(data, "Torso", torso_f);
        }
    }

    private void AddPresetIfExists(CharacterSaveData data, string partName, GameObject partObject)
    {
        if (partObject == null || !partObject.activeSelf)
        {
            data.partPresets[partName] = "000"; // По умолчанию
            return;
        }

        SkinnedMeshRenderer smr = partObject.GetComponent<SkinnedMeshRenderer>();
        if (smr != null && smr.sharedMesh != null)
        {
            string meshName = smr.sharedMesh.name;
            // Извлекаем номер пресета из имени меша
            // Формат: "Arm_F_AA_001" -> берем "001"
            string[] parts = meshName.Split('_');
            if (parts.Length >= 4)
            {
                data.partPresets[partName] = parts[3]; // Последняя часть - номер
            }
            else
            {
                data.partPresets[partName] = "000";
            }
        }
        else
        {
            data.partPresets[partName] = "000";
        }
    }
    #endregion
}