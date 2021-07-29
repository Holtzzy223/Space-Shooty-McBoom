using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PlayerController : MonoBehaviour
{
    [SerializeField] float controlSpeed = 10f;
    [SerializeField] float xRange = 10f;
    [SerializeField] float yRange = 7f;

    [SerializeField] float positionPitchFactor = -2f;
    [SerializeField] float controlPitchFactor = -10f;
    [SerializeField] float positionYawFactor = 2f;
    [SerializeField] float controlRollFactor = -20f;

    private bool isShipRolling = false;
  
    public CinemachineDollyCart cart;
    [SerializeField] float xThrow, yThrow;
    [SerializeField] float rollDirection;
    [SerializeField] float rollFactor = 0f;
    [SerializeField] float maxRollFactor = 360f;
    [SerializeField] float smoothTime = 10f;
    [SerializeField] float minRollFactor = 0f;
    [SerializeField] float barrelSmoothTime = 6f;
    void Update()
    {
        ProcessTranslation();
        ProcessRotation();
        if(Input.GetButtonDown("Fire1"))
        {
            isShipRolling = true;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            rollFactor = Mathf.Lerp(0, minRollFactor, smoothTime * Time.deltaTime);
            isShipRolling = false;

        }

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
}

