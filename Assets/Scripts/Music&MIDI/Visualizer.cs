using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Visualizer : MonoBehaviour
{
    public float minHeight;

    public float maxHeight;

    public float updateSenstivity;

    public Color visualizerColor;

    public Color hurtColor;

    [Space(12)]
    public bool loop = true;

    [Space(12), Range(64, 8192)]
    public int visualizerSamples = 64;

    public VisualizerObject[] visualizerObjects;

    AudioSource BGM_1;
    AudioSource BGM_2;
    AudioSource BGM_3;
    AudioSource BGM_4;
    AudioSource BGM_5;
    AudioSource BGM_6;
    AudioSource BGM_7;
    AudioSource BGM_8;
    AudioSource BGM_9;
    AudioSource BGM_10;
    AudioSource BGM_11;

    private NoteTrigger notetrigger;

    private AnimatorStateInfo stateInfo;

    // Start is called before the first frame update
    void Start()
    {
        visualizerObjects = GetComponentsInChildren<VisualizerObject>();
        BGM_1 = GameObject.Find("Conductor").GetComponents<AudioSource>()[0];
        BGM_2 = GameObject.Find("Conductor").GetComponents<AudioSource>()[1];
        BGM_3 = GameObject.Find("Conductor").GetComponents<AudioSource>()[5];
        BGM_4 = GameObject.Find("Conductor").GetComponents<AudioSource>()[6];
        BGM_5 = GameObject.Find("Conductor").GetComponents<AudioSource>()[7];
        BGM_6 = GameObject.Find("Conductor").GetComponents<AudioSource>()[8];
        BGM_7 = GameObject.Find("Conductor").GetComponents<AudioSource>()[9];
        BGM_8 = GameObject.Find("Conductor").GetComponents<AudioSource>()[10];
        BGM_9 = GameObject.Find("Conductor").GetComponents<AudioSource>()[11];
        BGM_10 = GameObject.Find("Conductor").GetComponents<AudioSource>()[12];
        BGM_11 = GameObject.Find("Conductor").GetComponents<AudioSource>()[13];
        notetrigger =
            (NoteTrigger)
            GameObject.Find("/Tracks/NoteTrigger").GetComponent("NoteTrigger");
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        stateInfo = notetrigger.anim.GetCurrentAnimatorStateInfo(0);

        float[] spectrumData_1 =
            BGM_1.GetSpectrumData(visualizerSamples, 0, FFTWindow.Rectangular);
        float[] spectrumData_2 =
            BGM_2.GetSpectrumData(visualizerSamples, 0, FFTWindow.Rectangular);
        float[] spectrumData_3 =
            BGM_3.GetSpectrumData(visualizerSamples, 0, FFTWindow.Rectangular);
        float[] spectrumData_4 =
            BGM_4.GetSpectrumData(visualizerSamples, 0, FFTWindow.Rectangular);
        float[] spectrumData_5 =
            BGM_5.GetSpectrumData(visualizerSamples, 0, FFTWindow.Rectangular);
        float[] spectrumData_6 =
            BGM_6.GetSpectrumData(visualizerSamples, 0, FFTWindow.Rectangular);
        float[] spectrumData_7 =
            BGM_7.GetSpectrumData(visualizerSamples, 0, FFTWindow.Rectangular);
        float[] spectrumData_8 =
            BGM_8.GetSpectrumData(visualizerSamples, 0, FFTWindow.Rectangular);
        float[] spectrumData_9 =
            BGM_9.GetSpectrumData(visualizerSamples, 0, FFTWindow.Rectangular);
        float[] spectrumData_10 =
            BGM_10.GetSpectrumData(visualizerSamples, 0, FFTWindow.Rectangular);
        float[] spectrumData_11 =
            BGM_11.GetSpectrumData(visualizerSamples, 0, FFTWindow.Rectangular);

        float[] spectrumData = new float[spectrumData_1.Length];
        for (int i = 0; i < spectrumData_1.Length; i++)
        {
            spectrumData[i] = spectrumData_1[i] + spectrumData_2[i] + spectrumData_3[i] +
                              spectrumData_4[i] + spectrumData_5[i] + spectrumData_6[i] + 
                              spectrumData_7[i] + spectrumData_8[i] + spectrumData_9[i] + 
                              spectrumData_10[i] + spectrumData_11[i];
        }

        float averageVolume = 0f;
        for (int i = 0; i < spectrumData.Length; i++)
        {
            averageVolume += spectrumData[i];
        }
        averageVolume /= spectrumData.Length;

        for (int i = 0; i < visualizerObjects.Length; i++)
        {
            Vector3 newSize = visualizerObjects[i].transform.localScale;

            newSize.y =
                Mathf
                    .Clamp(Mathf
                        .Lerp(newSize.y,
                        minHeight +
                        (averageVolume * (maxHeight - minHeight) * 50),
                        updateSenstivity),
                    minHeight,
                    maxHeight);
            visualizerObjects[i].transform.localScale =
                new Vector3(visualizerObjects[i].transform.localScale.x,
                    newSize.y,
                    visualizerObjects[i].transform.localScale.z);
            visualizerObjects[i].GetComponent<SpriteRenderer>().color =
                visualizerColor;

            // Apply noise to the sprite height
            float noise = Mathf.PerlinNoise(Time.time, i * 0.1f) * 0.2f - 0.1f;
            newSize.y += noise;
            visualizerObjects[i].transform.localScale =
                new Vector3(visualizerObjects[i].transform.localScale.x,
                    newSize.y,
                    visualizerObjects[i].transform.localScale.z);
            if (stateInfo.IsName("hurt"))
            {
                visualizerObjects[i].GetComponent<SpriteRenderer>().color =
                    hurtColor;
            }
        }
    }
}
