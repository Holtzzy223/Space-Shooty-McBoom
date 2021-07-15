using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float forwardThrust = 25f, strafeThrust = 8f, hoverThrust = 3f ;
    private float actForThrust, actStrafeThrust, actHoverThrust;
    private float forwardAccel = 2.5f, strafeAccel = 1.75f, hoverAccel = 2f;
    public float lookRate = 90f;
    private Vector2 lookInput, screenCenter, mouseDist;
    private float rollInput;
    public float rollSpeed = 90f, rollAccel = 3.75f;
    // Start is called before the first frame update
    void Start()
    {
        //center
        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;
        //lock cursor
        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        ShipControl();
    }

    private void ShipControl()
    {
        lookInput.x = Input.mousePosition.x;
        lookInput.y = Input.mousePosition.y;

        mouseDist.x = (lookInput.x - screenCenter.x) / screenCenter.y;
        mouseDist.y = (lookInput.y - screenCenter.y) / screenCenter.y;

        mouseDist = Vector2.ClampMagnitude(mouseDist, 1f);

        rollInput = Mathf.Lerp(rollInput, Input.GetAxisRaw("Roll"), rollAccel * Time.deltaTime);

        transform.Rotate(
                            -mouseDist.y * lookRate * Time.deltaTime,
                            mouseDist.x*lookRate*Time.deltaTime,
                            rollInput*rollSpeed*Time.deltaTime,
                            Space.Self
                        );
        actForThrust = Mathf.Lerp(actForThrust,Input.GetAxisRaw("Vertical") * forwardThrust,forwardAccel*Time.deltaTime);
        actStrafeThrust = Mathf.Lerp(actStrafeThrust,Input.GetAxisRaw("Horizontal") * strafeThrust,strafeAccel*Time.deltaTime);
        actHoverThrust = Mathf.Lerp(actHoverThrust,Input.GetAxisRaw("Hover") * hoverThrust,hoverAccel*Time.deltaTime);

        transform.position += transform.forward * actForThrust * Time.deltaTime;
        transform.position += (transform.right * actStrafeThrust * Time.deltaTime) + (transform.up * actHoverThrust * Time.deltaTime);

    }
}
