using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LatencyCalibrator : MonoBehaviour
{
    public AudioSource audioSource;

    public double startTime;

    public double bufferSchedulingOffset = 1.0;

    public double tempo = 110.0;

    double beatLength;

    float[]
        latencies =
        { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };

    int cur_index = 0;

    public float latency_offset = 0.0f;

    public float amount = 0.08f;

    Vector3 oldScale;

    Vector3 newScale;

    bool doLerp;

    float time, duration;

    public GameObject trigger;

    public TextMeshProUGUI latencyText;

    private Vector3 left;

    private Vector3 right;

    double GetCalibrationTime()
    {
        return AudioSettings.dspTime - startTime;
    }

    void Start()
    {
        startTime = (double) AudioSettings.dspTime + bufferSchedulingOffset;
        beatLength = 60 / tempo;

        //audioSource = GetComponent<AudioSource>();
        audioSource.PlayScheduled (startTime);

        oldScale =
            new Vector3(transform.GetChild(0).gameObject.transform.localScale.x,
                transform.GetChild(0).gameObject.transform.localScale.y,
                transform.GetChild(0).gameObject.transform.localScale.z);
        newScale =
            oldScale +
            new Vector3(oldScale.x * amount,
                oldScale.y * amount,
                oldScale.z * amount);
        time = 0.1f;
        duration = 0.1f;

        left = transform.GetChild(0).gameObject.transform.position;
        right = transform.GetChild(1).gameObject.transform.position;

        //latencyText.text = string.Empty;
    }

    float GetDesiredLatencyOffset()
    {
        float average = 0.0f;
        for (int i = 0; i < 10; ++i)
        {
            average += latencies[i];
        }
        return -(average / 10.0f);
    }

    float toInterp(float x)
    {
        x *= 2;
        if (x > 1.0f)
        {
            return 2.0f - x;
        }
        return x;
    }

    void Update()
    {
        double beat_with_fraction = GetCalibrationTime() / beatLength / 2;
        int beat = (int) beat_with_fraction % 2;
        float fractional_part =
            (float)(beat_with_fraction - (int) beat_with_fraction);

        if (
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Joystick1Button1) ||
            Input.GetKeyDown(KeyCode.Joystick1Button1)
        )
        {
            double current_beat = GetCalibrationTime() / beatLength;
            float off;
            if (fractional_part >= 0.5)
                off = fractional_part - 1;
            else
                off = fractional_part;
            latencies[cur_index] = off;
            ++cur_index;
            if (cur_index == 2) cur_index = 0;

            latency_offset = GetDesiredLatencyOffset();
            latencyText.text = latency_offset.ToString("0.00000");
            //PlayerPrefs.SetFloat("latency_offset", latency_offset);
            //Debug.Log(latency_offset);
        }

        double beat_with_fraction_latency =
            GetCalibrationTime() / beatLength / 4 + latency_offset;
        int beat_latency = (int) beat_with_fraction_latency % 2;
        double fractional_part_latency =
            beat_with_fraction_latency - (int) beat_with_fraction_latency;

        // Display the user's tempo.
        // Only getting s1,s2
        for (int i = 0; i < transform.childCount - 2; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (
                fractional_part_latency < 0.03 ||
                fractional_part_latency > 0.97 ||
                (
                fractional_part_latency > 0.47 && fractional_part_latency < 0.53
                )
            )
            {
                child.transform.localScale = newScale;
                doLerp = true;
                time = 0;
            }

            if (doLerp)
            {
                child.transform.localScale =
                    Vector3.Lerp(newScale, oldScale, time / duration);
                time += Time.deltaTime;
                if (time / duration >= 1.0f) doLerp = false;
            }
        }

        Vector3 newPosition =
            Vector3
                .Lerp(left, right, toInterp((float) fractional_part_latency));

        trigger.transform.position = newPosition;
    }
}
