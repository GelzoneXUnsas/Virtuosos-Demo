using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    private Conductor conductor;
    Vector3 oldScale;
    Vector3 newScale;
    public float amount = 0.2f;
    int num;
    bool doLerp;
    float time, duration;
    // Start is called before the first frame update
    void Start()
    {
        conductor = (Conductor)GameObject.Find("/Conductor").GetComponent("Conductor");
        num = 0;
        oldScale = transform.localScale;
        newScale = oldScale + new Vector3(amount, amount, amount);
        time = 0.1f;
        duration = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (num == conductor.beatNumber)
        {
            transform.localScale = newScale;
            doLerp = true;
            time = 0;
            num++;
            if (num >= 4)
                num = 0;
        }

        if (doLerp)
        {
            transform.localScale = Vector3.Lerp(newScale, oldScale, time / duration);
            time += Time.deltaTime;
            if (time / duration >= 1.0f)
                doLerp = false;
        }
        
    }
}
