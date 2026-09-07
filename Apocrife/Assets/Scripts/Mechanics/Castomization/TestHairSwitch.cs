using UnityEngine;

public class TestHairSwitch : MonoBehaviour
{
    public UniversalCharacterMeshCustomizer customizer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            customizer.Next(CharacterPartCategory.Hair);
        }
    }
}