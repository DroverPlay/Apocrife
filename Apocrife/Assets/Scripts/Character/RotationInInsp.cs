using UnityEngine;
using UnityEngine.UI;

public class RotationInInsp : MonoBehaviour
{
    [SerializeField] private Slider _rotationslider;
    [SerializeField] private GameObject _player;

    private float _standartRotation = 265f;

    private void Start()
    {
        _rotationslider.value = _standartRotation;
        Rotate();
    }

    public void Rotate()
    {
        float rotationAngle = _rotationslider.value;

        _player.transform.rotation = Quaternion.Euler(0f, -rotationAngle, 0f);
    }
}