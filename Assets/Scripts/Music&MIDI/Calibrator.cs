using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibrator : MonoBehaviour
{
    public Conductor conductor;

    double running_offset = 0.0;
    double[] last = {0.0, 0.0, 0.0, 0.0, 0.0, 0.0};

    // Start is called before the first frame update
    void Start()
    {
    } 

    // Update is called once per frame
    void Update()
    {
        if (checkHit(KeyCode.Joystick1Button0, KeyCode.K))
        {
            GetComponent<TMPro.TextMeshProUGUI>().text = conductor.nextSpotTime.ToString();

            last[0] = last[1]; last[1] = last[2]; last[2] = last[3]; last[3] = last[4]; last[4] = last[5]; 
            last[5] = (conductor.spotLength - conductor.nextSpotTime);
            running_offset = (last[0]+last[1]+last[2]+last[3]+last[4]+last[5]) / 6.0;

            //GetComponent<TMPro.TextMeshProUGUI>().text = running_offset.ToString();
        }
        if (checkHit(KeyCode.Joystick1Button1, KeyCode.J))
        {
            conductor.offset = running_offset;
        }
    }

    // Checks for a hit on a given keycode.
    private bool
    checkHit(KeyCode kc, KeyCode kb)
    {
        if (Input.GetKeyDown(kc) || Input.GetKeyDown(kb))
            return true;
        return false;
    }
}