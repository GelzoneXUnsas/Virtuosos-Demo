using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracksController : MonoBehaviour
{
    public GameObject Track1;

    public GameObject Track2;

    public GameObject Track3;

    public GameObject Track4;

    public GameObject Joystick1;

    public GameObject Joystick2;

    public PlayerController pc;

    [SerializeField]
    public float[] DrumPositions;

    [SerializeField]
    public float[] GuitarPositions;

    [SerializeField]
    public float[] PianoPositions;

    public int currentInstrument = 0;
    public bool spawnBoss = false;
    private bool hasDrummed = false;

    // Start is called before the first frame update
    void Start()
    {
        DrumerTracks();
        currentInstrument = 1;
        Track3.transform.position =
            new Vector3(Track3.transform.position.x,
                DrumPositions[0],
                Track3.transform.position.z);
        Track4.transform.position =
            new Vector3(Track4.transform.position.x,
                DrumPositions[1],
                Track4.transform.position.z);
    }

    public IEnumerator switchTrack(float duration, int toInst)
    {
        // toInst: 1=drum, 2=guitar, 3=piano
        Vector3 startPos3 = Track3.transform.position;
        Vector3 startPos4 = Track4.transform.position;
        Vector3 startPos2 = Track2.transform.position;
        Vector3 startPos1 = Track1.transform.position;
        Vector3 closePos;

        if (currentInstrument == 2)
        {
            // guitar
            closePos =
                new Vector3(Track3.transform.position.x,
                    (
                    GuitarPositions[0] + GuitarPositions[1] + GuitarPositions[2]
                    ) /
                    3.0f,
                    Track3.transform.position.z);
        }
        else if (currentInstrument == 3)
        {
            // piano
            closePos =
                new Vector3(Track3.transform.position.x,
                    (
                    PianoPositions[0] +
                    PianoPositions[1] +
                    PianoPositions[2] +
                    PianoPositions[3]
                    ) /
                    4.0f,
                    Track3.transform.position.z);
        }
        else
        {
            // drum
            closePos =
                new Vector3(Track3.transform.position.x,
                    (DrumPositions[0] + DrumPositions[1]) / 2.0f,
                    Track3.transform.position.z);
        }

        float startTime = Time.time;
        float t = 0f;

        while (t < 1f)
        {
            t = (Time.time - startTime) / duration;
            Track1.transform.position = Vector3.Lerp(startPos1, closePos, t);
            Track2.transform.position = Vector3.Lerp(startPos2, closePos, t);
            Track3.transform.position = Vector3.Lerp(startPos3, closePos, t);
            Track4.transform.position = Vector3.Lerp(startPos4, closePos, t);
            yield return null;
        }
        if (toInst == 1)
        {
            StartCoroutine(drumSwitch(duration));
        }
        else if (toInst == 2)
        {
            StartCoroutine(guitarSwitch(duration));
        }
        else
        {
            StartCoroutine(pianoSwitch(duration));
        }
    }

    public IEnumerator pianoSwitch(float duration)
    {
        pc.drumEndPos = -5;
        pc.guitarEndPos = -5;
        pc.pianoEndPos = 0;
        if(spawnBoss && hasDrummed)
            pc.bossEndPos = 0;
        else
            pc.bossEndPos = 10;
        pc.switching = true;
        PianistTracks();
        currentInstrument = 3;
        float elapsedTime = 0.0f;

        Vector3 startPos =
            new Vector3(Track3.transform.position.x,
                (
                PianoPositions[0] +
                PianoPositions[1] +
                PianoPositions[2] +
                PianoPositions[3]
                ) /
                4.0f,
                Track3.transform.position.z);

        Vector3 endPos1 =
            new Vector3(Track1.transform.position.x,
                PianoPositions[0],
                Track1.transform.position.z);
        Vector3 endPos2 =
            new Vector3(Track2.transform.position.x,
                PianoPositions[1],
                Track2.transform.position.z);
        Vector3 endPos3 =
            new Vector3(Track3.transform.position.x,
                PianoPositions[2],
                Track3.transform.position.z);
        Vector3 endPos4 =
            new Vector3(Track4.transform.position.x,
                PianoPositions[3],
                Track4.transform.position.z);

        while (elapsedTime < duration)
        {
            // Lerp the objects' positions towards the end position
            float t = elapsedTime / duration;
            Track1.transform.position = Vector3.Lerp(startPos, endPos1, t);
            Track2.transform.position = Vector3.Lerp(startPos, endPos2, t);
            Track3.transform.position = Vector3.Lerp(startPos, endPos3, t);
            Track4.transform.position = Vector3.Lerp(startPos, endPos4, t);

            yield return null;
            elapsedTime += Time.deltaTime;
        }
    }

    public IEnumerator guitarSwitch(float duration)
    {
        pc.drumEndPos = -5;
        pc.guitarEndPos = 0.2f;
        pc.pianoEndPos = -5;
        pc.bossEndPos = 10;
        pc.switching = true;
        GuitaristTracks();
        currentInstrument = 2;

        float elapsedTime = 0.0f;

        Vector3 startPos =
            new Vector3(Track3.transform.position.x,
                (GuitarPositions[0] + GuitarPositions[1] + GuitarPositions[2]) /
                3.0f,
                Track3.transform.position.z);

        Vector3 endPos2 =
            new Vector3(Track2.transform.position.x,
                GuitarPositions[0],
                Track2.transform.position.z);
        Vector3 endPos3 =
            new Vector3(Track3.transform.position.x,
                GuitarPositions[1],
                Track3.transform.position.z);
        Vector3 endPos4 =
            new Vector3(Track4.transform.position.x,
                GuitarPositions[2],
                Track4.transform.position.z);

        while (elapsedTime < duration)
        {
            // Lerp the objects' positions towards the end position
            float t = elapsedTime / duration;
            Track2.transform.position = Vector3.Lerp(startPos, endPos2, t);
            Track3.transform.position = Vector3.Lerp(startPos, endPos3, t);
            Track4.transform.position = Vector3.Lerp(startPos, endPos4, t);

            yield return null;
            elapsedTime += Time.deltaTime;
        }
    }

    public IEnumerator drumSwitch(float duration)
    {
        pc.drumEndPos = 0;
        pc.guitarEndPos = -5;
        pc.pianoEndPos = -5;
        if (spawnBoss && hasDrummed)
            pc.bossEndPos = 0;
        else if (spawnBoss)
            hasDrummed = true;
        else
            pc.bossEndPos = 10;
        pc.switching = true;
        DrumerTracks();
        currentInstrument = 1;

        float elapsedTime = 0.0f;

        Vector3 startPos =
            new Vector3(Track3.transform.position.x,
                (DrumPositions[0] + DrumPositions[1]) / 2.0f,
                Track3.transform.position.z);

        Vector3 endPos3 =
            new Vector3(Track3.transform.position.x,
                DrumPositions[0],
                Track3.transform.position.z);
        Vector3 endPos4 =
            new Vector3(Track4.transform.position.x,
                DrumPositions[1],
                Track4.transform.position.z);

        while (elapsedTime < duration)
        {
            // Lerp the objects' positions towards the end position
            float t = elapsedTime / duration;
            Track3.transform.position = Vector3.Lerp(startPos, endPos3, t);
            Track4.transform.position = Vector3.Lerp(startPos, endPos4, t);

            yield return null;
            elapsedTime += Time.deltaTime;
        }
    }

    // public void drumSwitch()
    // {
    //     DrumerTracks();
    //     Track3.transform.position =
    //         new Vector3(Track3.transform.position.x,
    //             DrumPositions[0],
    //             Track3.transform.position.z);
    //     Track4.transform.position =
    //         new Vector3(Track4.transform.position.x,
    //             DrumPositions[1],
    //             Track4.transform.position.z);
    // }
    public void guitarSwitch()
    {
        GuitaristTracks();
        Track2.transform.position =
            new Vector3(Track2.transform.position.x,
                GuitarPositions[0],
                Track2.transform.position.z);
        Track3.transform.position =
            new Vector3(Track3.transform.position.x,
                GuitarPositions[1],
                Track3.transform.position.z);
        Track4.transform.position =
            new Vector3(Track4.transform.position.x,
                GuitarPositions[2],
                Track4.transform.position.z);
    }

    // public void pianoSwitch()
    // {
    //     PianistTracks();
    //     Track1.transform.position =
    //         new Vector3(Track1.transform.position.x,
    //             PianoPositions[0],
    //             Track1.transform.position.z);
    //     Track2.transform.position =
    //         new Vector3(Track2.transform.position.x,
    //             PianoPositions[1],
    //             Track2.transform.position.z);
    //     Track3.transform.position =
    //         new Vector3(Track3.transform.position.x,
    //             PianoPositions[2],
    //             Track3.transform.position.z);
    //     Track4.transform.position =
    //         new Vector3(Track4.transform.position.x,
    //             PianoPositions[3],
    //             Track4.transform.position.z);
    // }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DrumerTracks();
            Track3.transform.position =
                new Vector3(Track3.transform.position.x,
                    DrumPositions[0],
                    Track3.transform.position.z);
            Track4.transform.position =
                new Vector3(Track4.transform.position.x,
                    DrumPositions[1],
                    Track4.transform.position.z);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GuitaristTracks();
            Track2.transform.position =
                new Vector3(Track2.transform.position.x,
                    GuitarPositions[0],
                    Track2.transform.position.z);
            Track3.transform.position =
                new Vector3(Track3.transform.position.x,
                    GuitarPositions[1],
                    Track3.transform.position.z);
            Track4.transform.position =
                new Vector3(Track4.transform.position.x,
                    GuitarPositions[2],
                    Track4.transform.position.z);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PianistTracks();
            Track1.transform.position =
                new Vector3(Track1.transform.position.x,
                    PianoPositions[0],
                    Track1.transform.position.z);
            Track2.transform.position =
                new Vector3(Track2.transform.position.x,
                    PianoPositions[1],
                    Track2.transform.position.z);
            Track3.transform.position =
                new Vector3(Track3.transform.position.x,
                    PianoPositions[2],
                    Track3.transform.position.z);
            Track4.transform.position =
                new Vector3(Track4.transform.position.x,
                    PianoPositions[3],
                    Track4.transform.position.z);
        }
    }

    private void DrumerTracks()
    {
        Track1.SetActive(false);
        Track2.SetActive(false);
        Track3.SetActive(true);
        Track4.SetActive(true);

        Joystick1.SetActive(false);
        Joystick2.SetActive(false);
    }

    private void GuitaristTracks()
    {
        Track1.SetActive(false);
        Track2.SetActive(true);
        Track3.SetActive(true);
        Track4.SetActive(true);

        Joystick1.SetActive(false);
        Joystick2.SetActive(false);
    }

    private void PianistTracks()
    {
        Track1.SetActive(true);
        Track2.SetActive(true);
        Track3.SetActive(true);
        Track4.SetActive(true);

        Joystick1.SetActive(true);
        Joystick2.SetActive(true);
    }
}
