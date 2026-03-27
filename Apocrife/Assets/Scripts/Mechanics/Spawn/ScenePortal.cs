using UnityEngine;
using TMPro;

public class ScenePortal : MonoBehaviour
{
    [Header("Переход")]
    public string targetScene;
    public string targetSpawnID;

    [Header("Взаимодействие")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

    [Header("Подсказка UI")]
    [Tooltip("Текст-подсказка, например: 'Нажмите E, чтобы войти'")]
    public GameObject promptUI;

    private bool playerInside = false;

    private void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    private void Update()
    {
        if (!playerInside) return;

        if (Input.GetKeyDown(interactKey))
        {
            Teleport();
        }
    }

    public void Teleport()
    {
        if (PersistentObject.Instance == null)
        {
            Debug.LogError("[ScenePortal] PersistentObject not found.");
            return;
        }

        PersistentObject.Instance.LoadScene(targetScene, targetSpawnID);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInside = true;

        if (promptUI != null)
            promptUI.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInside = false;

        if (promptUI != null)
            promptUI.SetActive(false);
    }
}