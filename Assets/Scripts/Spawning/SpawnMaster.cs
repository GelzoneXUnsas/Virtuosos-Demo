using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMaster : MonoBehaviour
{
    public Spawner spawner1;
    public Spawner spawner2;
    public Spawner spawner3;
    public Spawner spawner4;
    public MIDIReader MIDIReader;
    public Conductor conductor;
    public int noteSpeed;
    private MIDIReader.SpotElement curVal;
    public ushort[] lengths;

    void Start()
    {
        lengths = new ushort[4];
    }

    public void SpawnNotes()
    {
        int index = MIDIReader.index;
        // TODO: Alex --- This should just query the beatmap too...
        curVal = MIDIReader.SpotTrack[(index+noteSpeed) % MIDIReader.SpotTrack.Length];

        //Setting all the hold note lengths so note trigger can use them to calculate score
        if (curVal.one.velocity == 72)
        {
            lengths[0] = curVal.one.length;
        }
        if (curVal.two.velocity == 72)
        {
            lengths[1] = curVal.two.length;
        }
        if (curVal.three.velocity == 72)
        {
            lengths[2] = curVal.three.length;
        }
        if (curVal.four.velocity == 72)
        {
            lengths[3] = curVal.four.length;
        }

        // Note 1
        if (curVal.one.velocity == 64)
            spawner1.Spawn(1, 0, index);
        else if (curVal.one.velocity == 72)
            spawner1.Spawn(2, curVal.one.length, index);
        else if (curVal.one.velocity == 80)
            spawner1.Spawn(3, 0, index);
        else if (curVal.one.velocity == 88)
            spawner1.Spawn(4, 0, index);

        // Note 2
        if (curVal.two.velocity == 64) {
            spawner2.Spawn(1, 0, index);
        }
        else if (curVal.two.velocity == 72) {
            spawner2.Spawn(2, curVal.two.length, index);
        }
        else if (curVal.two.velocity == 80)
            spawner2.Spawn(3, 0, index);
        else if (curVal.two.velocity == 88)
            spawner2.Spawn(4, 0, index);

        // Note 3
        if (curVal.three.velocity == 64) {
            spawner3.Spawn(1, 0, index);
        }
        else if (curVal.three.velocity == 72) {
            spawner3.Spawn(2, curVal.three.length, index);
        }
        else if (curVal.three.velocity == 80) {
            spawner3.Spawn(3, 0, index);
        }
        else if (curVal.three.velocity == 88)
            spawner3.Spawn(4, 0, index);

        // Note 4
        if (curVal.four.velocity == 64)
            spawner4.Spawn(1, -1, index);
        else if (curVal.four.velocity == 72)
            spawner4.Spawn(2, curVal.four.length, index);
        else if (curVal.four.velocity == 80)
            spawner4.Spawn(3, 0, index);
        else if (curVal.four.velocity == 88)
            spawner4.Spawn(4, 0, index);
    }
}
