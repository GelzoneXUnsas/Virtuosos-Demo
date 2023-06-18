using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickControl : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 1.0f;

    public float maxDistanceFromCenter = 5.0f;

    public Vector3 joystickInput;

    float verticalInput;

    public int joystickNum = 1;

    public GameObject trackAbove;

    public GameObject trackBelow;

    private Vector3 initialPosition;

    public InputAction leftJoystick;
    public InputAction rightJoystick;
    float timer1 = 0;
    float timer2 = 0;
    float timer3 = 0;
    float timer4 = 0;

    private void OnEnable()
    {
        leftJoystick.Enable();
        rightJoystick.Enable();
    }

    private void OnDisable()
    {
        leftJoystick.Disable();
        rightJoystick.Disable();
    }

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        initialPosition =
            (trackAbove.transform.position + trackBelow.transform.position) / 2;
        maxDistanceFromCenter =
            Vector3.Distance(initialPosition, trackAbove.transform.position);
        if (joystickNum == 1)
            verticalInput = leftJoystick.ReadValue<float>();
        else
            verticalInput = rightJoystick.ReadValue<float>();

        if (Input.GetKey(KeyCode.H) && joystickNum == 1)
        {
            verticalInput = Mathf.Lerp(0, 1, timer1 / 0.05f);
            timer1 += Time.deltaTime;
        }
        else
            timer1 = 0;
        if (Input.GetKey(KeyCode.J) && joystickNum == 1)
        {
            verticalInput = Mathf.Lerp(0, -1, timer2 / 0.05f);
            timer2 += Time.deltaTime;
        }
        else
            timer2 = 0;
        if (Input.GetKey(KeyCode.K) && joystickNum == 2)
        {
            verticalInput = Mathf.Lerp(0, 1, timer3 / 0.05f);
            timer3 += Time.deltaTime;
        }
        else
            timer3 = 0;
        if (Input.GetKey(KeyCode.L) && joystickNum == 2)
        {
            verticalInput = Mathf.Lerp(0, -1, timer4 / 0.05f);
            timer4 += Time.deltaTime;
        }
        else
            timer4 = 0;
        joystickInput = new Vector3(0, verticalInput, 0);

        Vector3 newPosition =
            initialPosition + joystickInput * maxDistanceFromCenter;

        transform.position =
            Vector3
                .Lerp(transform.position, newPosition, speed * Time.deltaTime);
    }
}
