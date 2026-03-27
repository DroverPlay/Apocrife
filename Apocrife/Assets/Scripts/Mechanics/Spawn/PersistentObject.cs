using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PersistentObject : MonoBehaviour
{
    [Header("Быстрая загрузка (для кнопок UI, если нужно)")]
    public string targetScene;
    public string targetSpawnID;

    private static PersistentObject instance;
    private static string pendingSpawnID = "";

    public static PersistentObject Instance => instance;

    private CharacterController characterController;
    private PlayerAnimationController playerAnimationController;
    //private PlayerController playerController;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SetSpawnAfterLoad());
    }

    private IEnumerator SetSpawnAfterLoad()
    {
        yield return new WaitForEndOfFrame();

        if (string.IsNullOrWhiteSpace(pendingSpawnID))
        {
            yield break;
        }

        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>(true);
        SpawnPoint targetSpawn = null;

        foreach (var sp in spawnPoints)
        {
            if (sp.spawnID == pendingSpawnID)
            {
                targetSpawn = sp;
                break;
            }
        }

        if (targetSpawn != null)
        {
            if (characterController == null)
                characterController = GetComponent<CharacterController>();

            if (characterController != null)
                characterController.enabled = false;

            transform.SetPositionAndRotation(
                targetSpawn.transform.position,
                targetSpawn.transform.rotation
            );

            if (characterController != null)
            {
                characterController.enabled = true;
                //playerController.canControl = true;
            }

            if (playerAnimationController == null)
                playerAnimationController = GetComponent<PlayerAnimationController>();

            if (playerAnimationController != null)
                playerAnimationController.enabled = true;

            Debug.Log($"[PersistentObject] Spawned at '{pendingSpawnID}' in scene '{SceneManager.GetActiveScene().name}'");
        }
        else
        {
            Debug.LogWarning($"[PersistentObject] SpawnPoint '{pendingSpawnID}' not found in scene '{SceneManager.GetActiveScene().name}'");
        }

        pendingSpawnID = "";
    }

    public void LoadScene(string sceneName, string spawnID)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("[PersistentObject] Scene name is empty.");
            return;
        }

        if (!IsSceneInBuild(sceneName))
        {
            Debug.LogError($"[PersistentObject] Scene '{sceneName}' is not in Build Settings.");
            return;
        }

        pendingSpawnID = spawnID;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadAssignedScene()
    {
        LoadScene(targetScene, targetSpawnID);
    }

    private bool IsSceneInBuild(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (name == sceneName)
                return true;
        }

        return false;
    }
}