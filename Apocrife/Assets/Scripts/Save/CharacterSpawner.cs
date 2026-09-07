using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterSpawner : MonoBehaviour
{
    private void Start()
    {
        // Ищем нашего выжившего/созданного персонажа по синглтону кастомизатора
        if (UniversalCharacterMeshCustomizer.Instance != null)
        {
            GameObject character = UniversalCharacterMeshCustomizer.Instance.gameObject;

            // Переносим персонажа в координаты этого объекта-спавнера
            character.transform.position = this.transform.position;
            character.transform.rotation = this.transform.rotation;

            var movement = character.GetComponent<CharacterController>();
            if (movement != null) { movement.enabled = true; }
            var animator = character.GetComponent<PlayerAnimationController>();
            if (animator != null) { animator.enabled = true; }

            Debug.Log("Персонаж успешно перемещен на точку спавна: " + this.transform.position);
        }
        else
        {
            Debug.LogWarning("Персонаж не найден на сцене! Проверь, работает ли DontDestroyOnLoad.");
        }

    }
}
