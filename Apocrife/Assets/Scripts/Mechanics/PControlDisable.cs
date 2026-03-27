using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PControlDisable : MonoBehaviour
{
    [SerializeField] private GameObject _gameObject;
    private CharacterController characterController;
    private PlayerAnimationController playerAnimationController;

    void Start()
    {
        characterController = _gameObject.GetComponent<CharacterController>();
        playerAnimationController = _gameObject.GetComponent<PlayerAnimationController>();

        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        if (characterController != null)
            characterController.enabled = false;

        if (playerAnimationController == null)
            playerAnimationController = GetComponent<PlayerAnimationController>();

        if(playerAnimationController != null)
            playerAnimationController.enabled = false;
    }
}
