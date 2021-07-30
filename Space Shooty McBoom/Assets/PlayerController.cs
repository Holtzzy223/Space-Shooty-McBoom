using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PlayerController : MonoBehaviour
{
    [Header("Ship Controls")] 
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

    void Update()
    {
        RollCheck();
        ProcessTranslation();
        ProcessRotation();
        ProcessFiring();

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
            rollDirection = Input.GetAxis("Roll");
            rollFactor = Mathf.Lerp(rollFactor, maxRollFactor, smoothTime * Time.deltaTime);
            float pitchDueToPosition = transform.localPosition.y * positionPitchFactor;
            float pitchDueToControlThrow = yThrow * controlPitchFactor;
            float pitch = pitchDueToPosition + pitchDueToControlThrow;

            float yaw = transform.localPosition.x * positionYawFactor;

            float roll = rollDirection * rollFactor;
            Quaternion targetRotation = Quaternion.Euler(pitch, yaw, roll);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, 0.1f);
        }
    }

    void ProcessTranslation()
    {
        xThrow = Input.GetAxis("Horizontal");
        yThrow = Input.GetAxis("Vertical");

        float xOffset = xThrow * Time.deltaTime * controlSpeed;
        float rawXPos = transform.localPosition.x + xOffset;
        float clampedXPos = Mathf.Clamp(rawXPos, -xRange, xRange);

        float yOffset = yThrow * Time.deltaTime * controlSpeed;
        float rawYPos = transform.localPosition.y + yOffset;
        float clampedYPos = Mathf.Clamp(rawYPos, -yRange, yRange);
        Vector3 targetTranslation = new Vector3(clampedXPos, clampedYPos, transform.localPosition.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetTranslation, 0.8f);
    }
    void RollCheck()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            isShipRolling = true;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            rollFactor = Mathf.Lerp(0, minRollFactor, smoothTime * Time.deltaTime);
            isShipRolling = false;
        }
    }

    void ProcessFiring()
    {
        if (Input.GetButton("Fire1"))
        {
            Debug.Log("I'm shooting");
            SetLasersActive(true);
        }
        else
        {
            Debug.Log("I'm not shooting");
            SetLasersActive(false);
        }
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



