using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Standards;
using UnityEngine;

public enum NoteType
{
    EMPTY,
    NOTE,
    HOLD,
    OBSTACLE,
    COLLECTIBLE
}

public class MIDIReader : MonoBehaviour
{
    /*

     ----------------x----x-----x-----x-----                                > piano (64)
     ------x-x---x-----x--x--------x--x-----                > guitar (60)   > piano (63)
     --x-x-----x-x-----------x--x--x--x-----   > drum (57)  > guitar (59)   > piano (62)
     x---x---x-x-x-x-x-x--x--x--x--x--x-----   > drum (56)  > guitar (58)   > piano (61)
     1 2 3 4 5 6 7 8 9 10 11 12 13 14 15

    */
    public Spine spine;
    public Conductor conductor;

    public TracksController tracksController;

    TempoMap tempoMap;

    int BEATS_PER_BAR;
    int SPOTS_PER_BEAT = 4;

    int SixteenthLength = 119;

    byte[] oneNotes = new byte[] { 64 };
    byte[] twoNotes = new byte[] { 60, 63 };
    byte[] threeNotes = new byte[] { 57, 59, 62 };
    byte[] fourNotes = new byte[] { 56, 58, 61 };

    private MidiFile midiFile;

    /*
        A Midi file has 32 bits per note. Here is how it's laid out.
        xxxx  xxxx  xxxx  xxxx  xxxx  xxxx  xxxx  xxxx 
        < number >  <velocity>  <       length       >
    */
    public struct NoteElement
    {
        /* 
         DRUM: 56 (G), 57 (A)
         GUITAR: 58 (A#), 59 (B), 60 (C)
         PIANO: 61 (C#), 62(D), 63 (D#), 64 (E)
        */
        public byte number;

        public ushort[] pos;

        // in units of 16th note length of 119, for a quarter note, length = 4
        public ushort length;

        /*
            64: Hit note
            72: Hold note
            80: obstacles
            88: collectible
            96: health
            104: ...
        */
        public byte velocity;
    }
    List<NoteElement> trackInfo = new List<NoteElement>();

    public struct SpotElement
    {
        public NoteElement four;

        public NoteElement three;

        public NoteElement two;

        public NoteElement one;
    }
    public SpotElement[] SpotTrack;

    public List<NoteType[]> beatmap;

    public void Initialize(string path)
    {
        midiFile = MidiFile.Read(path);
        InitNoteElements();
        InitSpotTrack();
        InitBeatmap();

        index = 0;
    }

    void InitNoteElements()
    {
        var notes = midiFile.GetNotes();
        tempoMap = midiFile.GetTempoMap();
        trackInfo.Clear();

        foreach (var note in notes)
        {
            NoteElement newNote = new NoteElement();

            // track number and instrument type
            newNote.number = note.NoteNumber;

            // note position
            var pos =
                note.TimeAs(TimeSpanType.BarBeatTicks, tempoMap).ToString();
            var temp = pos.Split(".");
            newNote.pos = new ushort[3];
            for (int i = 0; i < temp.Length; ++i)
            {
                newNote.pos[i] = Convert.ToUInt16(temp[i]);
            }

            // note length for press & hold
            newNote.length = ((ushort)(note.Length / SixteenthLength));

            // note types
            newNote.velocity = note.Velocity;
            trackInfo.Add (newNote);
        }
    }

    void InitSpotTrack()
    {
        var firstTrackChunk = midiFile.GetTrackChunks().First();
        var firstTimeSignatureEvent =
            firstTrackChunk
                .Events
                .OfType<TimeSignatureEvent>()
                .FirstOrDefault();

        BEATS_PER_BAR = firstTimeSignatureEvent.Numerator;

        var barBeatTimeOfLastEvent =
            midiFile
                .GetTimedEvents()
                .Last()
                .TimeAs<BarBeatTicksTimeSpan>(tempoMap);
        var totalBars = barBeatTimeOfLastEvent.Bars;
        if (barBeatTimeOfLastEvent.Beats > 0 || barBeatTimeOfLastEvent.Ticks > 0
        ) totalBars = barBeatTimeOfLastEvent.Bars + 1;

        SpotTrack = new SpotElement[totalBars * BEATS_PER_BAR * SPOTS_PER_BEAT];

        foreach (NoteElement note in trackInfo)
        {
            // ticks: 0 -> 1 120->2, 240->3, 360->4
            int index =
                (note.pos[0] - 1) * BEATS_PER_BAR * SPOTS_PER_BEAT +
                (note.pos[1] - 1) * SPOTS_PER_BEAT +
                (note.pos[2] / SixteenthLength) +
                1;

            if (oneNotes.Contains(note.number))
            {
                SpotTrack[index].one = note;
            }
            else if (twoNotes.Contains(note.number))
            {
                SpotTrack[index].two = note;
            }
            else if (threeNotes.Contains(note.number))
            {
                SpotTrack[index].three = note;
            }
            else if (fourNotes.Contains(note.number))
            {
                SpotTrack[index].four = note;
            }
        }
    }

    public void InitBeatmap() {
        beatmap = new List<NoteType[]>();

        foreach(SpotElement element in SpotTrack) {
            NoteType[] current_spot_notes = new NoteType[4];

            // track 1
            if (element.one.velocity == 64)
                current_spot_notes[0] = NoteType.NOTE;
            else if (element.one.velocity == 72)
                current_spot_notes[0] = NoteType.HOLD;
            else if (element.one.velocity == 80)
                current_spot_notes[0] = NoteType.OBSTACLE;
            else if (element.one.velocity == 88)
                current_spot_notes[0] = NoteType.COLLECTIBLE;

            // track 2
            if (element.two.velocity == 64)
                current_spot_notes[1] = NoteType.NOTE;
            else if (element.two.velocity == 72)
                current_spot_notes[1] = NoteType.HOLD;
            else if (element.two.velocity == 80)
                current_spot_notes[1] = NoteType.OBSTACLE;
            else if (element.two.velocity == 88)
                current_spot_notes[1] = NoteType.COLLECTIBLE;

            // track 3
            if (element.three.velocity == 64)
                current_spot_notes[2] = NoteType.NOTE;
            else if (element.three.velocity == 72)
                current_spot_notes[2] = NoteType.HOLD;
            else if (element.three.velocity == 80)
                current_spot_notes[2] = NoteType.OBSTACLE;
            else if (element.three.velocity == 88)
                current_spot_notes[2] = NoteType.COLLECTIBLE;

            // track 4
            if (element.four.velocity == 64)
                current_spot_notes[3] = NoteType.NOTE;
            else if (element.four.velocity == 72)
                current_spot_notes[3] = NoteType.HOLD;
            else if (element.four.velocity == 80)
                current_spot_notes[3] = NoteType.OBSTACLE;
            else if (element.four.velocity == 88)
                current_spot_notes[3] = NoteType.COLLECTIBLE;

            beatmap.Add(current_spot_notes);
        }
    }

    public int index; // TODO: BRICK THIS SHIT

    public void updateTrackState()
    {
        // NOTE (Alex): This is a weird bug, for some reason the conductor is
        // off by one sometimes and gives us the wrong conductor spot twice in a
        // row. For now, incrementing works fine.
        /*
        index =
            conductor.barNumber *
            Conductor.BEATS_PER_BAR *
            Conductor.SPOTS_PER_BEAT +
            conductor.beatNumber * Conductor.SPOTS_PER_BEAT +
            conductor.spotNumber;
            */

        if(index >= SpotTrack.Length - 8)
        {
            conductor.should_end_section = true;
            if(conductor.ready_for_dialogue)
                spine.DialogueStart();
            return;
        }
        index += 1;


        // TODO: Pull this out into the spine.
        SpotElement curVal = SpotTrack[index];
        if (curVal.four.velocity == 1)
        {
            StartCoroutine(tracksController.switchTrack(0.15f, 1));
        }
        else if (curVal.four.velocity == 2)
        {
            StartCoroutine(tracksController.switchTrack(0.15f, 2));
        }
        else if (curVal.four.velocity == 3)
        {
            StartCoroutine(tracksController.switchTrack(0.15f, 3));
        }
    }
}
