using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
public class PlayerController : MonoBehaviour
{
    [Header("Ship Controls")]
    private Playercontrols playerControls;
    [SerializeField] InputAction move;
    [SerializeField] float controlSpeed = 10f;
    [SerializeField] float xRange = 10f;
    [SerializeField] float yRange = 7f;

    [SerializeField] float positionPitchFactor = -2f;
    [SerializeField] float controlPitchFactor = -10f;
    [SerializeField] float positionYawFactor = 2f;
    [SerializeField] float controlRollFactor = -20f;

    private bool isShipRolling = false;

    public CinemachineDollyCart cart;
    [Header("Roll Tuning")]
    [SerializeField] float xThrow;
    [SerializeField] float yThrow;
    [SerializeField] float rollDirection;
    [SerializeField] float rollFactor = 0f;
    [SerializeField] float maxRollFactor = 360f;
    [SerializeField] float smoothTime = 10f;
    [SerializeField] float minRollFactor = 0f;

    [Header("Weapons")]
    public GameObject[] lasers;
       // Change into slotting systems after tests... i.e laserDamage => slotOneDamage, missileDamage =>slotTwoDamage
    [SerializeField] int laserDamage; 
    [SerializeField] int missileDamage;
    private void Awake()
    {
        playerControls = new Playercontrols();
    }
    private void OnEnable()
    {
        move = playerControls.Player.Move;

        move.Enable();
        playerControls.Player.Roll.performed += DoRoll;
        playerControls.Player.Roll.Enable();
        playerControls.Player.FireMains.performed += FireMains;
        playerControls.Player.FireMains.Enable();

    }
    private void OnDisable()
    {
        move.Disable();
        playerControls.Player.Roll.Disable();
        playerControls.Player.FireMains.Disable();

    }
    void LateUpdate()
    {
        
        ProcessTranslation();
        ProcessRotation();
        if (playerControls.Player.FireMains.ReadValue<float>() < 0.5f) { ShutDownMains(); }
    }

    void ProcessRotation()
    {
        if (!isShipRolling)
        {
            float pitchDueToPosition = transform.localPosition.y * positionPitchFactor;
            float pitchDueToControlThrow = yThrow * controlPitchFactor;

            float pitch = pitchDueToPosition + pitchDueToControlThrow;
            float yaw = transform.localPosition.x * positionYawFactor;

            float roll = xThrow * controlRollFactor;
            Quaternion targetRotation = Quaternion.Euler(pitch, yaw, roll);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, 0.1f);
        }else
        if (isShipRolling)
        {

            rollDirection = playerControls.Player.Move.ReadValue<Vector2>().x;
            rollFactor = Mathf.Lerp(rollFactor, maxRollFactor, smoothTime * Time.deltaTime);
            float pitchDueToPosition = transform.localPosition.y * positionPitchFactor;
            float pitchDueToControlThrow = yThrow * controlPitchFactor;
            float pitch = pitchDueToPosition + pitchDueToControlThrow;

            float yaw = transform.localPosition.x * positionYawFactor;

            float roll = rollDirection * rollFactor;
            Quaternion targetRotation = Quaternion.Euler(pitch, yaw, roll);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, 0.1f);
            if (rollFactor >= 350f) { rollFactor = Mathf.Lerp(0, minRollFactor, smoothTime * Time.deltaTime); isShipRolling = false; }
        }
    }

    void ProcessTranslation()
    {
        xThrow = playerControls.Player.Move.ReadValue<Vector2>().x;
        yThrow = playerControls.Player.Move.ReadValue<Vector2>().y;

        float xOffset = xThrow * Time.deltaTime * controlSpeed;
        float rawXPos = transform.localPosition.x + xOffset;
        float clampedXPos = Mathf.Clamp(rawXPos, -xRange, xRange);

        float yOffset = yThrow * Time.deltaTime * controlSpeed;
        float rawYPos = transform.localPosition.y + yOffset;
        float clampedYPos = Mathf.Clamp(rawYPos, -yRange, yRange);
        Vector3 targetTranslation = new Vector3(clampedXPos, clampedYPos, transform.localPosition.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetTranslation, 0.8f);
    }
    private void DoRoll(InputAction.CallbackContext obj)
    {
        isShipRolling = true;
       //if (Input.GetButtonDown("Fire1"))
       //{
       //    isShipRolling = true;
       //}
       //if (Input.GetButtonUp("Fire1"))
       //{
       //    rollFactor = Mathf.Lerp(0, minRollFactor, smoothTime * Time.deltaTime);
       //    isShipRolling = false;
       //}
    }

    void FireMains(InputAction.CallbackContext obj)
    {
        SetLasersActive(true);
       
    }
    void ShutDownMains()
    {
        SetLasersActive(false);
    }
    void SetLasersActive(bool isActive)
    {
        foreach (GameObject laser in lasers)
        {
            var emissionModule = laser.GetComponent<ParticleSystem>().emission;
            emissionModule.enabled = isActive;
        }
    }
}



