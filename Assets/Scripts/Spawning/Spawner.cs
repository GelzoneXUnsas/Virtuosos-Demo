using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private Conductor conductor;
    private MIDIReader midiReader;
    private Transform parentTransform;

    public NoteObject note;
    public NoteObject groundNote;
    public NoteObject obstacle;
    public NoteObject collectible;
    public NoteObject hold;
    public NoteObject holdsquare;

    public int spawnNum = 0;
    public int holdNum;
    public bool incremeted;
    public int which_track;

    public int index;
    public int newIndex;

    private void Start()
    {
        incremeted = false;
        parentTransform = GetComponent<Transform>();
        conductor = (Conductor)GameObject.Find("/Conductor").GetComponent("Conductor");
        midiReader = (MIDIReader)GameObject.Find("/MIDIReader").GetComponent("MIDIReader");
        index = midiReader.index;
        newIndex = index;
    }
    private void Update()
    {
        newIndex = midiReader.index;

        if (midiReader.index == 0)
            index = midiReader.index;

        if (holdNum >= 1 && newIndex > index)
        {
            NoteObject clone = (NoteObject)Instantiate(holdsquare, transform);
            clone.transform.position = transform.position;
            clone.transform.rotation = Quaternion.identity;
            clone.which_track = which_track;
            clone.index = newIndex;
            holdNum--;
            index = newIndex;
        }
    }

    private void SetupNoteObject(NoteObject obj, int which_track, int index)
    {
        NoteObject clone = (NoteObject)Instantiate(obj, transform);
        clone.transform.position = transform.position;
        clone.transform.rotation = Quaternion.identity;
        clone.which_track = which_track;
        clone.index = index;
        clone.GetComponent<SpriteRenderer>().enabled = true;
        spawnNum++;
    }

    public void Spawn(int spawn_type, int spawn_length, int index)
    {
        if (spawn_type == 1)
        {
            if (spawn_length == -1)
                SetupNoteObject(groundNote, which_track, index);
            else
                SetupNoteObject(note, which_track, index);
        }
        if (spawn_type == 2)
        {
            SetupNoteObject(hold, which_track, index);
            holdNum = spawn_length;
        }
        else if (spawn_type == 3)
            SetupNoteObject(obstacle, which_track, index);
        else if (spawn_type == 4)
            SetupNoteObject(collectible, which_track, index);
    }
}
