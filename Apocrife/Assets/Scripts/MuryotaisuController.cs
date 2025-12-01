using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Muryotaisu
{
    public class MuryotaisuController : MonoBehaviourPunCallbacks
    {
        private Animator animator;

        [Header("Скорости движения")]
        public float walkSpeed = 2f;
        public float runSpeed = 5f;
        public float jumpSpeed = 2f;
        public float gravity = 1f;
        public float rotationSpeed = 5f;

        [Header("Настройки бега")]
        public KeyCode runKey = KeyCode.LeftShift;
        public float stamina = 100f;
        public float maxStamina = 100f;
        public float staminaDrainRate = 20f;
        public float staminaRegenRate = 15f;
        public float minStaminaToRun = 10f;

        [Header("Другие настройки")]
        public float startKocchi = 2f;

        private float second;
        public CharacterController controller;
        private Vector3 moveDirection = Vector3.zero;
        private Vector3 inputDirection = Vector3.zero;
        private bool isJumping = false;
        private Vector3 horizontalVelocity = Vector3.zero;
        private bool isRunning = false;
        private float currentSpeed;

        [Header("Камера")]
        public Transform firstPersonCameraPosition;
        public Transform thirdPersonCameraPosition;

        private CameraController cameraController;
        private PhotonView photonView;

        public CharacterController Controller => controller;
        public bool IsGrounded => controller != null && controller.isGrounded;
        public bool IsMoving => IsMovementInputPressed();

        void Start()
        {
            animator = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
            photonView = GetComponent<PhotonView>();

            if (photonView != null && !photonView.IsMine)
            {
                controller.enabled = false;
                enabled = false;
                return;
            }

            cameraController = Camera.main.GetComponent<CameraController>();
            currentSpeed = walkSpeed;
        }

        void Update()
        {

            if (!controller.enabled) return;
            if (photonView != null && !photonView.IsMine) return;

            HandleInput();
            HandleStamina();
            HandleAnimations();
            HandleMovement();
        }

        private void HandleInput()
        {
            // Сбрасываем направление ввода
            inputDirection = Vector3.zero;

            // Собираем все направления ввода
            if (Input.GetKey("up") || Input.GetKey("w")) inputDirection += Vector3.forward;
            if (Input.GetKey("down") || Input.GetKey("s")) inputDirection += Vector3.back;
            if (Input.GetKey("right") || Input.GetKey("d")) inputDirection += Vector3.right;
            if (Input.GetKey("left") || Input.GetKey("a")) inputDirection += Vector3.left;

            // Обработка бега
            bool wantsToRun = Input.GetKey(runKey) && inputDirection != Vector3.zero;

            if (wantsToRun && stamina > minStaminaToRun && !isRunning)
            {
                isRunning = true;
                currentSpeed = runSpeed;
            }
            else if ((!wantsToRun || stamina <= 0) && isRunning)
            {
                isRunning = false;
                currentSpeed = walkSpeed;
            }

            // Обработка прыжка
            if (Input.GetKeyDown("space") && controller.isGrounded && !isJumping)
            {
                moveDirection.y = jumpSpeed;
                isJumping = true;
            }
        }

        private void HandleStamina()
        {
            if (isRunning && inputDirection != Vector3.zero)
            {
                // Тратим выносливость при беге
                stamina = Mathf.Max(0, stamina - staminaDrainRate * Time.deltaTime);

                // Если выносливость закончилась - переходим на ходьбу
                if (stamina <= 0)
                {
                    isRunning = false;
                    currentSpeed = walkSpeed;
                }
            }
            else
            {
                // Восстанавливаем выносливость когда не бежим
                stamina = Mathf.Min(maxStamina, stamina + staminaRegenRate * Time.deltaTime);
            }
        }

        private void HandleAnimations()
        {
            // Smile
            animator.SetBool("smileFlag", Input.GetKey("q"));

            // Kocchiminna
            float dist = Vector3.Distance(transform.position, Camera.main.transform.position);
            animator.SetBool("kocchiFlag", dist < startKocchi);

            // Анимации движения
            if (controller.isGrounded)
            {
                if (isJumping)
                {
                    isJumping = false;
                    animator.SetBool("jumpFlag", false);
                }

                second += Time.deltaTime;

                if (IsMovementInputPressed())
                {
                    if (!isJumping)
                    {
                        animator.SetBool("walkFlag", !isRunning);
                        animator.SetBool("runFlag", isRunning);
                        animator.SetBool("idleFlag", false);
                    }
                }
                else if (second >= 15 && !isJumping)
                {
                    animator.SetBool("walkFlag", false);
                    animator.SetBool("runFlag", false);
                    animator.SetBool("idleFlag", false);
                    animator.SetTrigger("idleBFlag");
                    second = 0;
                }
                else if (!isJumping)
                {
                    animator.SetBool("walkFlag", false);
                    animator.SetBool("runFlag", false);
                    animator.SetBool("idleFlag", true);
                }
            }
            else
            {
                // В воздухе
                if (inputDirection != Vector3.zero && isJumping)
                {
                    animator.SetBool("walkFlag", !isRunning);
                    animator.SetBool("runFlag", isRunning);
                }
                else if (isJumping)
                {
                    animator.SetBool("walkFlag", false);
                    animator.SetBool("runFlag", false);
                }
            }

            // Прыжок
            animator.SetBool("jumpFlag", isJumping);
        }

        private void HandleMovement()
        {
            if (controller.isGrounded)
            {
                if (inputDirection != Vector3.zero)
                {
                    ProcessMovement();
                }
                else
                {
                    horizontalVelocity = Vector3.zero;
                }
            }
            else
            {
                // В воздухе
                if (inputDirection != Vector3.zero)
                {
                    ProcessMovement();
                }
            }

            // Применяем гравитацию и движение
            if (controller.enabled)
            {
                moveDirection.y -= gravity * Time.deltaTime;
                Vector3 finalMove = horizontalVelocity * Time.deltaTime + moveDirection * Time.deltaTime;
                controller.Move(finalMove);
            }
        }

        private void ProcessMovement()
        {
            if (!controller.enabled) return;
            if (photonView != null && !photonView.IsMine) return;

            inputDirection.Normalize();

            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 worldDirection = cameraForward * inputDirection.z + cameraRight * inputDirection.x;

            if (worldDirection.magnitude > 1f)
                worldDirection.Normalize();

            horizontalVelocity = worldDirection * currentSpeed;

            // УЛУЧШЕННАЯ ЛОГИКА ПОВОРОТОВ:
            if (worldDirection.magnitude > 0.1f && !IsInFirstPersonMode())
            {
                // Определяем тип движения
                bool isPureBackward = inputDirection.z < -0.7f && Mathf.Abs(inputDirection.x) < 0.3f; // Только S
                bool isStrafe = Mathf.Abs(inputDirection.x) > 0.5f; // В основном A/D
                bool isDiagonalBackward = inputDirection.z < -0.3f && Mathf.Abs(inputDirection.x) > 0.3f; // S+A или S+D

                if (isPureBackward)
                {
                    // При чистом движении назад - плавно разворачиваем на 180 градусов
                    Vector3 cameraBackDirection = -cameraForward;
                    Quaternion targetRotation = Quaternion.LookRotation(cameraBackDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * 0.7f * Time.deltaTime);
                }
                else if (isDiagonalBackward)
                {
                    // При диагональном движении назад (S+A или S+D) - поворачиваем в сторону движения
                    Quaternion targetRotation = Quaternion.LookRotation(worldDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
                else
                {
                    // При движении вперед или строго вбок - обычный поворот
                    Quaternion targetRotation = Quaternion.LookRotation(worldDirection);
                    float lerpSpeed = isRunning ? rotationSpeed * 1.5f : rotationSpeed;
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpSpeed * Time.deltaTime);
                }
            }
        }


        private bool IsInFirstPersonMode()
        {
            if (cameraController != null)
            {
                return cameraController.firstPersonMode;
            }
            return false;
        }

        public bool IsMovementInputPressed()
        {
            return Input.GetKey("up") || Input.GetKey("right") || Input.GetKey("down") || Input.GetKey("left") ||
                   Input.GetKey("w") || Input.GetKey("d") || Input.GetKey("s") || Input.GetKey("a");
        }

        // Публичные методы для доступа к состоянию бега (можно использовать в UI)
        public bool IsRunning()
        {
            return isRunning;
        }

        public float GetStamina()
        {
            return stamina;
        }

        public float GetStaminaPercentage()
        {
            return stamina / maxStamina;
        }
    }
}