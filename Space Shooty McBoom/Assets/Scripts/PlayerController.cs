using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Ship Controls")]
    private Playercontrols playerControls;
    [SerializeField] InputAction move;
    [SerializeField] float controlSpeed = 10f;
    [SerializeField] float xRange = 10f;
    [SerializeField] float yRange = 7f;
    [SerializeField] float maxZRange = 3f;
    [SerializeField] float zRange = 0f;
    [SerializeField] float storedZPosition = 0f;
    [SerializeField] float positionPitchFactor = -2f;
    [SerializeField] float controlPitchFactor = -10f;
    [SerializeField] float strafePower = -10f;
    [SerializeField] float boostPower = 3f;
    [SerializeField] float positionYawFactor = 2f;
    [SerializeField] float controlRollFactor = -20f;

    public bool isShipRolling = false;
    public bool isShipStrafing = false;
    public bool isShipBoosting = false;

    public CinemachineDollyCart cart;
    [Header("Roll Tuning")]
    [SerializeField] float xThrow;
    [SerializeField] float yThrow;
    [SerializeField] float rollDirection;
    [SerializeField] float rollFactor = 0f;
    [SerializeField] float maxRollFactor = -360f;
    [SerializeField] float smoothTime = 10f;
    [SerializeField] float minRollFactor = 0f;

    [Header("Weapons")]
    public GameObject[] lasers;
    public GameObject[] missiles;

    // Change into slotting systems after tests... i.e laserDamage => slotOneDamage, missileDamage =>slotTwoDamage
    [SerializeField] int laserDamage;
    [SerializeField] int missileDamage;
    
    public TrailRenderer[] rollTrails;
    public TrailRenderer[] strafeTrails;
    private void Awake()
    {
        DeactivateRollTrail();
        playerControls = new Playercontrols();
    }
    private void OnEnable()
    {
        
        move = playerControls.Player.Move;

        move.Enable();
        playerControls.Player.Strafe.performed += DoStrafe;
        playerControls.Player.Strafe.Enable();
        playerControls.Player.Roll.performed += DoRoll;
        playerControls.Player.Roll.Enable();
        playerControls.Player.Boost.performed += DoBoost;
        playerControls.Player.Boost.Enable();
        playerControls.Player.FireMains.performed += FireMains;
        playerControls.Player.FireMains.Enable();
        playerControls.Player.FireSecondary.performed += FireSecondary;
        playerControls.Player.FireSecondary.Enable();
    }
    private void OnDisable()
    {
        move.Disable();
        playerControls.Player.Strafe.Disable();
        playerControls.Player.Roll.Disable();
        playerControls.Player.Boost.Disable();
        playerControls.Player.FireMains.Disable();
        playerControls.Player.FireSecondary.Disable();

    }
    void LateUpdate()
    {
        
        ProcessTranslation();
        ProcessRotation();
        if (playerControls.Player.Strafe.ReadValue<float>() <= 0) { DeactivateStrafeTrail(); isShipStrafing = false; }
        if (playerControls.Player.Boost.ReadValue<float>() < 0.5f) { isShipBoosting = false; }
        if (playerControls.Player.FireMains.ReadValue<float>() < 0.5f) { ShutDownMains(); }
        if (playerControls.Player.FireSecondary.ReadValue<float>() ==0f) { ShutDownSecondary(); }
    }



    void ProcessRotation()
    {
        if (!isShipRolling)
        {
            DeactivateRollTrail();
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
            ActivateRollTrail();
            rollDirection = playerControls.Player.Move.ReadValue<Vector2>().x;
            rollFactor = Mathf.Lerp(rollFactor, maxRollFactor, smoothTime*Time.deltaTime);
            float pitchDueToPosition = transform.localPosition.y * positionPitchFactor;
            float pitchDueToControlThrow = yThrow * controlPitchFactor;
            float pitch = pitchDueToPosition + pitchDueToControlThrow;

            float yaw = transform.localPosition.x * positionYawFactor;

            float roll =  rollFactor;//rollDirection *
            Quaternion targetRotation = Quaternion.Euler(pitch, yaw, roll);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, 0.1f);
            if (rollFactor >= (maxRollFactor-1)) { rollFactor = Mathf.Lerp(0, minRollFactor, smoothTime*Time.deltaTime); isShipRolling = false;}
        }
    }

    void ProcessTranslation()
    {
        xThrow = playerControls.Player.Move.ReadValue<Vector2>().x;
        yThrow = playerControls.Player.Move.ReadValue<Vector2>().y;

        float xOffset = xThrow * Time.deltaTime * controlSpeed;
        float rawXPos = transform.localPosition.x + xOffset;
        if (isShipStrafing) {rawXPos = transform.localPosition.x + xOffset*strafePower; }
        float clampedXPos = Mathf.Clamp(rawXPos, -xRange, xRange);

        float yOffset = yThrow * Time.deltaTime * controlSpeed;
        float rawYPos = transform.localPosition.y + yOffset;
        float clampedYPos = Mathf.Clamp(rawYPos, -yRange, yRange);
        
        Vector3 targetTranslation = new Vector3(clampedXPos, clampedYPos, transform.localPosition.z);
        if (isShipBoosting)
        {
            storedZPosition = transform.localPosition.z;
            float zOffset = boostPower;
            float rawZPos = transform.localPosition.z + zOffset;
            float clampedZPos = Mathf.Clamp(rawZPos, -zRange, zRange);
            targetTranslation = new Vector3(clampedXPos, clampedYPos, clampedZPos);
            zRange = Mathf.Lerp(zRange,maxZRange,0.1f);
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetTranslation, 0.8f);
        if (!isShipBoosting) { targetTranslation = new Vector3(clampedXPos, clampedYPos,storedZPosition -storedZPosition ); transform.localPosition = Vector3.Lerp(transform.localPosition, targetTranslation, 0.1f); }
    }
    private void DoRoll(InputAction.CallbackContext obj)
    {
        isShipRolling = true;
        ActivateRollTrail();
    }
    private void DoStrafe(InputAction.CallbackContext obj)
    {
        isShipStrafing = true;
        ActivateStrafeTrail();
    }

    private void ActivateStrafeTrail()
    {
        foreach (TrailRenderer trail in strafeTrails)
        {
            //trail.forceRenderingOff = false;
            trail.emitting = true;
        }
    }

    private void DeactivateStrafeTrail()
    {
        foreach (TrailRenderer trail in strafeTrails)
        {
            //trail.forceRenderingOff = false;
            trail.emitting = false;
        }
    }
    private void ActivateRollTrail()
    {
        foreach (TrailRenderer trail in rollTrails)
        {
            //trail.forceRenderingOff = false;
            trail.emitting = true;
        }
    }

    private void DeactivateRollTrail()
    {
        foreach (TrailRenderer trail in rollTrails)
        {
            //trail.forceRenderingOff = true;
            trail.emitting = false;
        }

    }

    private void DoBoost(InputAction.CallbackContext obj)
    {
        isShipBoosting = true;
    }
    void FireMains(InputAction.CallbackContext obj)
    {
        SetLasersActive(true);
    }
    void ShutDownMains()
    {
        SetLasersActive(false);
    }
    void FireSecondary(InputAction.CallbackContext obj)
    {
        SetMissilesActive(true);
    }
    void ShutDownSecondary()
    {
        SetMissilesActive(false);
    }
    void SetLasersActive(bool isActive)
    {
        foreach (GameObject laser in lasers)
        {
            var emissionModule = laser.GetComponent<ParticleSystem>().emission;
            emissionModule.enabled = isActive;
        }
    }

    void SetMissilesActive(bool isActive)
    {
        foreach (GameObject missile in missiles)
        {
            var emissionModule = missile.GetComponent<ParticleSystem>().emission;
            emissionModule.enabled = isActive;
        }
    }
}



