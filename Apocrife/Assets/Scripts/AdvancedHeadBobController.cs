using UnityEngine;
using Muryotaisu;

public class AdvancedHeadBobController : MonoBehaviour
{
    [Header("Основные настройки")]
    public bool enableHeadBob = true;
    
    [Header("Дыхание в покое")]
    public float breathSpeed = 1f;
    public float breathAmount = 0.005f;
    
    [Header("Покачивания при движении")]
    public float walkBobSpeed = 14f;
    public float walkBobAmount = 0.05f;
    public float runBobSpeed = 18f;
    public float runBobAmount = 0.08f;
    
    [Header("Эффекты шагов")]
    public float stepTiltAmount = 1f;
    public float stepTiltSpeed = 2f;
    
    [Header("Эффекты приземления")]
    public float landBobAmount = 0.15f;
    public float landBobRecovery = 3f;
    
    [Header("Плавность")]
    public float smoothness = 2f;

    private CameraController cameraController;
    private MuryotaisuController playerController;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    private float bobTimer = 0;
    private float landBobTimer = 0;
    private float breathTimer = 0;
    private float tiltTimer = 0;
    private bool wasGrounded = true;

    void Start()
    {
        cameraController = GetComponent<CameraController>();
        playerController = FindObjectOfType<MuryotaisuController>();
        
        if (cameraController != null && cameraController.firstPersonCameraPos != null)
        {
            originalCameraPosition = cameraController.firstPersonCameraPos.localPosition;
            originalCameraRotation = cameraController.firstPersonCameraPos.localRotation;
        }
    }

    void Update()
    {
        if (!enableHeadBob || cameraController == null || !cameraController.firstPersonMode) 
            return;

        HandleBreathing();
        HandleMovementBob();
        HandleStepTilt();
        HandleLandingBob();
    }

    private void HandleBreathing()
    {
        // Дыхание в покое
        breathTimer += Time.deltaTime * breathSpeed;
        float breathOffset = Mathf.Sin(breathTimer) * breathAmount;
        
        Vector3 breathPosition = originalCameraPosition + new Vector3(0, breathOffset, 0);
        
        if (cameraController.firstPersonCameraPos != null)
        {
            cameraController.firstPersonCameraPos.localPosition = Vector3.Lerp(
                cameraController.firstPersonCameraPos.localPosition,
                breathPosition,
                smoothness * Time.deltaTime
            );
        }
    }

    private void HandleMovementBob()
    {
        if (playerController == null) return;

        bool isMoving = IsPlayerMoving();
        bool isRunning = playerController.IsRunning();
        bool isGrounded = playerController.controller.isGrounded;

        if (isMoving && isGrounded)
        {
            float bobSpeed = isRunning ? runBobSpeed : walkBobSpeed;
            float bobAmount = isRunning ? runBobAmount : walkBobAmount;

            bobTimer += Time.deltaTime * bobSpeed;
            
            // Вертикальное покачивание (синус)
            float verticalBob = Mathf.Sin(bobTimer) * bobAmount;
            
            // Горизонтальное покачивание (косинус со смещением)
            float horizontalBob = Mathf.Cos(bobTimer * 0.5f) * bobAmount * 0.4f;
            
            // Боковой наклон (для шагов)
            float tiltBob = Mathf.Sin(bobTimer * 0.25f) * bobAmount * 10f;

            Vector3 targetPosition = originalCameraPosition + new Vector3(horizontalBob, verticalBob, 0);
            Quaternion targetRotation = originalCameraRotation * Quaternion.Euler(0, 0, tiltBob);

            if (cameraController.firstPersonCameraPos != null)
            {
                cameraController.firstPersonCameraPos.localPosition = Vector3.Lerp(
                    cameraController.firstPersonCameraPos.localPosition,
                    targetPosition,
                    smoothness * Time.deltaTime
                );
                
                cameraController.firstPersonCameraPos.localRotation = Quaternion.Lerp(
                    cameraController.firstPersonCameraPos.localRotation,
                    targetRotation,
                    smoothness * Time.deltaTime
                );
            }
        }
        else
        {
            // Плавный возврат к исходному положению
            if (cameraController.firstPersonCameraPos != null)
            {
                cameraController.firstPersonCameraPos.localPosition = Vector3.Lerp(
                    cameraController.firstPersonCameraPos.localPosition,
                    originalCameraPosition,
                    smoothness * Time.deltaTime
                );
                
                cameraController.firstPersonCameraPos.localRotation = Quaternion.Lerp(
                    cameraController.firstPersonCameraPos.localRotation,
                    originalCameraRotation,
                    smoothness * Time.deltaTime
                );
            }
            
            if (!isMoving) bobTimer = 0;
        }
    }

    private void HandleStepTilt()
    {
        if (playerController == null || !playerController.controller.isGrounded) return;

        // Легкий наклон при шагах
        if (IsPlayerMoving())
        {
            tiltTimer += Time.deltaTime * stepTiltSpeed;
            float tilt = Mathf.Sin(tiltTimer) * stepTiltAmount;
            
            if (cameraController.firstPersonCameraPos != null)
            {
                Quaternion targetTilt = originalCameraRotation * Quaternion.Euler(0, 0, tilt);
                cameraController.firstPersonCameraPos.localRotation = Quaternion.Lerp(
                    cameraController.firstPersonCameraPos.localRotation,
                    targetTilt,
                    stepTiltSpeed * Time.deltaTime
                );
            }
        }
    }

    private void HandleLandingBob()
    {
        if (playerController == null) return;

        bool isGrounded = playerController.controller.isGrounded;
        
        // Обнаружение приземления
        if (!wasGrounded && isGrounded)
        {
            landBobTimer = landBobAmount;
        }
        wasGrounded = isGrounded;

        // Эффект приземления
        if (landBobTimer > 0)
        {
            float shake = Mathf.Sin(Time.time * 25f) * landBobTimer;
            Vector3 shakeOffset = new Vector3(0, shake, 0);
            
            if (cameraController.firstPersonCameraPos != null)
            {
                cameraController.firstPersonCameraPos.localPosition += shakeOffset;
            }

            landBobTimer -= Time.deltaTime * landBobRecovery;
        }
    }

    private bool IsPlayerMoving()
    {
        if (playerController == null) return false;
        return playerController.IsMovementInputPressed();
    }

    // Метод для внешнего вызова (например, при получении урона)
    public void AddCameraShake(float amount, float duration)
    {
        StartCoroutine(CameraShake(amount, duration));
    }

    private System.Collections.IEnumerator CameraShake(float amount, float duration)
    {
        float timer = duration;
        Vector3 originalPos = cameraController.firstPersonCameraPos.localPosition;
        
        while (timer > 0)
        {
            float shakeX = Random.Range(-1f, 1f) * amount;
            float shakeY = Random.Range(-1f, 1f) * amount;
            
            cameraController.firstPersonCameraPos.localPosition = originalPos + new Vector3(shakeX, shakeY, 0);
            
            timer -= Time.deltaTime;
            yield return null;
        }
        
        // Возврат к исходной позиции
        cameraController.firstPersonCameraPos.localPosition = originalPos;
    }
}