using System.Collections;
using UnityEngine;

namespace Muryotaisu
{
    public class CameraController : MonoBehaviour
    {
        [Header("Цели и настройки")]
        public Transform player;
        public Transform firstPersonCameraPos;
        public Transform thirdPersonCameraPos;

        [Header("Настройки камеры")]
        public float mouseSensitivity = 2f;
        public float cameraSmoothness = 5f;
        public float minVerticalAngle = -80f;
        public float maxVerticalAngle = 80f;

        [Header("Настройки третьего лица")]
        public float thirdPersonDistance = 3f;
        public float thirdPersonHeight = 1.5f;

        [Header("Режимы камеры")]
        public bool firstPersonMode = false;
        public KeyCode toggleCameraKey = KeyCode.V;

        private Camera cam;
        private float currentXRotation = 0f;
        private float currentYRotation = 0f;
        private Vector3 currentVelocity;

        // Для плавного переключения
        private bool isTransitioning = false;
        private float transitionProgress = 0f;
        private Vector3 transitionStartPos;
        private Quaternion transitionStartRot;

        // Новые переменные для плавности
        private Vector3 smoothVelocity = Vector3.zero;
        private Quaternion smoothCameraRotation;
        private bool isCameraInitialized = false;

        private bool _isPaused = false;

        void Start()
        {
            cam = GetComponent<Camera>();
            if (cam == null)
                cam = Camera.main;

            if (firstPersonMode)
                transform.SetParent(firstPersonCameraPos);
            else
                transform.SetParent(thirdPersonCameraPos);

            // Инициализируем начальный поворот камеры
            smoothCameraRotation = transform.rotation;
            isCameraInitialized = true;

            ResetCameraPosition();
        }

        private void OnPauseStateChanged(bool isPaused)
        {
            _isPaused = isPaused;
            Debug.Log($"CameraController: Получено состояние паузы = {isPaused}");
        }
        void Update()
        {
            //if (!IsInputEnabled()) return;


            if (_isPaused) return;

            if (Input.GetKeyDown(toggleCameraKey) && !isTransitioning)
            {
                StartCoroutine(SwitchCameraMode());
            }

            if (!isTransitioning)
            {
                HandleMouseLook();
            }
        }


        void LateUpdate()
        {
            //if (!IsInputEnabled()) return;


            if (!isTransitioning)
            {
                if (firstPersonMode)
                {
                    UpdateFirstPersonCamera();
                }
                else
                {
                    UpdateThirdPersonCamera();
                }
            }
        }

        //private bool IsInputEnabled()
        //{
        //    return InputManager.Instance != null && InputManager.Instance.IsInputEnabled;
        //}

        void HandleMouseLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            if (firstPersonMode)
            {
                // ПЕРВОЕ ЛИЦО
                player.Rotate(Vector3.up * mouseX);

                currentXRotation -= mouseY;
                currentXRotation = Mathf.Clamp(currentXRotation, minVerticalAngle, maxVerticalAngle);
                transform.localRotation = Quaternion.Euler(currentXRotation, 0, 0);
            }
            else
            {
                // ТРЕТЬЕ ЛИЦО - плавное вращение камеры
                if (Mathf.Abs(mouseX) > 0.01f || Mathf.Abs(mouseY) > 0.01f)
                {
                    currentYRotation += mouseX;
                    currentXRotation -= mouseY;
                    currentXRotation = Mathf.Clamp(currentXRotation, minVerticalAngle, maxVerticalAngle);

                    // Сразу применяем поворот при движении мышью
                    Quaternion targetRotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);
                    smoothCameraRotation = targetRotation;
                }
                else
                {
                    // Плавная интерполяция когда мышь не двигается
                    Quaternion targetRotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);
                    smoothCameraRotation = Quaternion.Slerp(smoothCameraRotation, targetRotation, cameraSmoothness * Time.deltaTime);
                }
            }
        }

        void UpdateFirstPersonCamera()
        {
            if (firstPersonCameraPos != null)
            {
                transform.position = Vector3.SmoothDamp(transform.position, firstPersonCameraPos.position, ref currentVelocity, 0.1f);
            }
        }

        void UpdateThirdPersonCamera()
        {
            if (player != null && isCameraInitialized)
            {
                // Используем сглаженный поворот камеры
                Vector3 direction = smoothCameraRotation * Vector3.back;
                Vector3 desiredPosition = player.position + direction * thirdPersonDistance + Vector3.up * thirdPersonHeight;

                // Проверяем коллизии с улучшенным сглаживанием
                RaycastHit hit;
                Vector3 actualPosition = desiredPosition;
                if (Physics.Linecast(player.position + Vector3.up * 1.5f, desiredPosition, out hit))
                {
                    actualPosition = hit.point - direction * 0.3f;
                }

                // Более плавное перемещение камеры
                transform.position = Vector3.SmoothDamp(transform.position, actualPosition, ref smoothVelocity, 0.1f);

                // Плавный взгляд на персонажа
                Vector3 lookTarget = player.position + Vector3.up * 1.5f;
                Quaternion targetLookRotation = Quaternion.LookRotation(lookTarget - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetLookRotation, cameraSmoothness * Time.deltaTime);
            }
        }

        IEnumerator SwitchCameraMode()
        {
            isTransitioning = true;
            transitionProgress = 0f;

            transitionStartPos = transform.position;
            transitionStartRot = transform.rotation;

            firstPersonMode = !firstPersonMode;

            if (firstPersonMode)
                transform.SetParent(firstPersonCameraPos);
            else
                transform.SetParent(null);

            while (transitionProgress < 1f)
            {
                transitionProgress += Time.deltaTime * 3f;

                if (firstPersonMode)
                {
                    transform.position = Vector3.Lerp(transitionStartPos, firstPersonCameraPos.position, transitionProgress);
                    transform.rotation = Quaternion.Lerp(transitionStartRot, firstPersonCameraPos.rotation, transitionProgress);
                }
                else
                {
                    Vector3 direction = smoothCameraRotation * Vector3.back;
                    Vector3 targetPos = player.position + direction * thirdPersonDistance + Vector3.up * thirdPersonHeight;
                    transform.position = Vector3.Lerp(transitionStartPos, targetPos, transitionProgress);

                    Vector3 lookTarget = player.position + Vector3.up * 1.5f;
                    transform.LookAt(lookTarget);
                }

                yield return null;
            }

            ResetCameraPosition();
            isTransitioning = false;
        }

        void ResetCameraPosition()
        {
            if (firstPersonMode && firstPersonCameraPos != null)
            {
                transform.position = firstPersonCameraPos.position;
                transform.rotation = firstPersonCameraPos.rotation;
            }
            else
            {
                UpdateThirdPersonCamera();
            }
        }

        // Метод для обновления поворота камеры из внешнего скрипта
        public void UpdateCameraRotation(float yaw, float pitch)
        {
            currentYRotation = yaw;
            currentXRotation = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);
            smoothCameraRotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);
        }

        public void ToggleCursorLock()
        {
            if (GameNetworkManager.Instance != null)
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    GameNetworkManager.Instance.UnlockCursor();
                }
                else
                {
                    GameNetworkManager.Instance.LockCursor();
                }
            }
            else
            {
                // Fallback если GameNetworkManager не существует
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }

        //private bool IsGamePaused()
        //{
        //    Debug.Log("CameraController: Update пропущен из-за паузы");
        //    return GameStateManager.Instance != null && GameStateManager.Instance.IsPaused;
        //}
    }
}