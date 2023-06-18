using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum HitCategory
{
    MISS,
    WEAK,
    GOOD
}

public class NoteTrigger : MonoBehaviour
{
    public Spine spine;

    public Conductor conductor;

    public MIDIReader midiReader;

    public PlayerController character;

    public TracksController trackController;

    ParticleSystem.MainModule psmain;
    ParticleSystem.MainModule npsmain;

    public ParticleSystem particles;
    public ParticleSystem circleParticle;
    public ParticleSystem nutParticle;

    public ParticleSystem hittext;

    public TMPro.TextMeshProUGUI text;

    public SpawnMaster spawnMaster;

    public Animator anim;

    public Animator anim2;

    public Animator anim3;

    public JoystickControl controllerControl;

    public SpriteRenderer top;

    public SpriteRenderer high;

    public SpriteRenderer low;

    public SpriteRenderer bot;

    public Color perfect_color;

    public Color weak_color;

    public Color fail_color;

    public Color drum_perfect_color;

    public Color guitar_perfect_color;

    public Color piano_perfect_color;

    public GameSFX sfx;

    private bool[] updateHold = { false, false, false, false };

    public bool[] goodHold = { false, false, false, false };

    private ushort[] holdLengths;

    private int holdScore = 1;

    public ScoreManager scoreManager;

    public ComboManager comboManager;

    public CollectableUI collectableUI;

    public float last_spot;

    public float next_spot;

    public float perfect;

    public float good;

    public int perfectNum;
    public int goodNum;
    public int missNum;
    public int maxCombo;
    public float goodWeight;
    public float accuracy;

    public int totalNote;

    //int transpose = 0;

    int[] notes = new int[] { 7, 4, 0, -12 };

    public List<bool[]> hit_notes;

    public int index;

    private bool flickUpLeft = false;

    private bool flickDownLeft = false;

    private bool flickUpRight = false;

    private bool flickDownRight = false;

    private bool hold_sfx_playing = false;

    public InputActionReference

            bottomControls,
            lowControls,
            topControls,
            highControls,
            leftJoystick,
            rightJoystick;

    void Start()
    {
        holdLengths = new ushort[4];
        psmain = particles.main;
        npsmain = nutParticle.main;

        hit_notes = new List<bool[]>();
        foreach (NoteType[] notes in midiReader.beatmap)
        {
            bool[] current_spot_hit =
                new bool[4] { false, false, false, false };

            hit_notes.Add (current_spot_hit);
        }
        HideHitbox();
        // Lucas: This is double called also in spine
        //Reset();
    }

    public void Reset()
    {
        last_spot = conductor.spotLength;
        next_spot = conductor.spotLength + conductor.spotLength;
        index = 0;

        hit_notes = new List<bool[]>();
        foreach (NoteType[] notes in midiReader.beatmap)
        {
            bool[] current_spot_hit =
                new bool[4] { false, false, false, false };

            foreach (NoteType note in notes)
            {
                if (note == NoteType.NOTE || note == NoteType.HOLD)
                {
                    totalNote++;
                }
            }

            hit_notes.Add (current_spot_hit);
        }
    }

    public void StatsReset()
    {
        scoreManager.score = 0;
        totalNote = 0;
        perfectNum = 0;
        goodNum = 0;
        collectableUI.collectableNum = 0;
        missNum = 0;
        accuracy = 0;
    }

    void Update()
    {
        if (spine.state != InterfaceState.GAMEPLAY) return;

        checkHit(topControls, 0, top, 0.7f);
        checkHit(highControls, 1, high, 0.7f);
        checkHit(lowControls, 2, low, 0.7f);
        checkHit(bottomControls, 3, bot, 0.7f);

        if (trackController.currentInstrument == 0)
        {
            // default
            perfect_color = drum_perfect_color;
        }
        else if (trackController.currentInstrument == 1)
        {
            //drum
            perfect_color = drum_perfect_color;
        }
        else if (trackController.currentInstrument == 2)
        {
            //guitar
            perfect_color = guitar_perfect_color;
        }
        else if (trackController.currentInstrument == 3)
        {
            //piano
            perfect_color = piano_perfect_color;
        }

        if (
            (topControls.action.ReadValue<float>() > 0.0f ||
            leftJoystick.action.ReadValue<float>() >= 0.7) && anim3.GetBool("isHold1") == false
        )
        {
            anim3.SetBool("isHold1", true);
            anim3.SetTrigger("hold1");
        }
        else if(leftJoystick.action.ReadValue<float>() <= 0.7 && !(topControls.action.ReadValue<float>() > 0.0f))
        {
            anim3.SetBool("isHold1", false);
        }
        if (
            (highControls.action.ReadValue<float>() > 0.0f ||
            lowControls.action.ReadValue<float>() > 0.0f ||
            -leftJoystick.action.ReadValue<float>() >= 0.7 ||
            rightJoystick.action.ReadValue<float>() >= 0.7) && anim3.GetBool("isHold2") == false
        )
        {
            anim3.SetBool("isHold2", true);
            anim3.SetTrigger("hold2");
            anim2.SetBool("isHold2", true);
            anim2.SetTrigger("hold2");
        }
        else if (!(highControls.action.ReadValue<float>() > 0.0f) &&
            !(lowControls.action.ReadValue<float>() > 0.0f) &&
            -leftJoystick.action.ReadValue<float>() <= 0.7 &&
            rightJoystick.action.ReadValue<float>() <= 0.7)
        {
            anim3.SetBool("isHold2", false);
            anim2.SetBool("isHold2", false);
        }
        if (
            (bottomControls.action.ReadValue<float>() > 0.0f ||
            -rightJoystick.action.ReadValue<float>() >= 0.7) && anim3.GetBool("isHold3") == false
        )
        {
            anim3.SetBool("isHold3", true);
            anim3.SetTrigger("hold3");
            anim2.SetBool("isHold1", true);
            anim2.SetTrigger("hold1");
        }
        else if(!(bottomControls.action.ReadValue<float>() > 0.0f) &&
            -rightJoystick.action.ReadValue<float>() <= 0.7)
        {
            anim3.SetBool("isHold3", false);
            anim2.SetBool("isHold1", false);
        }


        //Update Hold Note Score
        if (
            updateHold[0] &&
            holdLengths[0] >= 1 &&
            (
            topControls.action.ReadValue<float>() > 0.0f ||
            leftJoystick.action.ReadValue<float>() >= 0.7
            )
        )
        {
            if (!hold_sfx_playing){
                sfx.Playhold_hit();
                hold_sfx_playing = true;
            }
            var em =
                top
                    .gameObject
                    .transform
                    .Find("HitSpot/HoldParticle")
                    .GetComponent<ParticleSystem>()
                    .emission;
            em.enabled = true;
            if (goodHold[0] == false) holdScore += 1;
            goodHold[0] = true;
        }
        else
        {

            var em =
                top
                    .gameObject
                    .transform
                    .Find("HitSpot/HoldParticle")
                    .GetComponent<ParticleSystem>()
                    .emission;
            em.enabled = false;

            // if (goodHold[0] == true) holdScore -= 1;
            goodHold[0] = false;
            updateHold[0] = false;
        }
        if (
            updateHold[1] &&
            holdLengths[1] >= 1 &&
            (
            highControls.action.ReadValue<float>() > 0.0f ||
            -leftJoystick.action.ReadValue<float>() >= 0.7
            )
        )
        {
            if (!hold_sfx_playing){
                sfx.Playhold_hit();
                hold_sfx_playing = true;
            }
            var em =
                high
                    .gameObject
                    .transform
                    .Find("HitSpot/HoldParticle")
                    .GetComponent<ParticleSystem>()
                    .emission;
            em.enabled = true;
            if (goodHold[1] == false) holdScore += 1;
            goodHold[1] = true;
        }
        else
        {

            var em =
                high
                    .gameObject
                    .transform
                    .Find("HitSpot/HoldParticle")
                    .GetComponent<ParticleSystem>()
                    .emission;
            em.enabled = false;

            // if (goodHold[1] == true) holdScore -= 1;
            goodHold[1] = false;
            updateHold[1] = false;
        }
        if (
            updateHold[2] &&
            holdLengths[2] >= 1 &&
            (
            lowControls.action.ReadValue<float>() > 0.0f ||
            rightJoystick.action.ReadValue<float>() >= 0.7
            )
        )
        {
            if (!hold_sfx_playing){
                sfx.Playhold_hit();
                hold_sfx_playing = true;
            }
            var em =
                low
                    .gameObject
                    .transform
                    .Find("HitSpot/HoldParticle")
                    .GetComponent<ParticleSystem>()
                    .emission;
            em.enabled = true;
            if (goodHold[2] == false) holdScore += 1;
            goodHold[2] = true;
        }
        else
        {

            var em =
                low
                    .gameObject
                    .transform
                    .Find("HitSpot/HoldParticle")
                    .GetComponent<ParticleSystem>()
                    .emission;
            em.enabled = false;

            // if (goodHold[2] == true) holdScore -= 1;
            goodHold[2] = false;
            updateHold[2] = false;
        }
        if (
            updateHold[3] &&
            holdLengths[3] >= 1 &&
            (
            bottomControls.action.ReadValue<float>() > 0.0f ||
            -rightJoystick.action.ReadValue<float>() >= 0.7
            )
        )
        {
            if (!hold_sfx_playing){
                sfx.Playhold_hit();
                hold_sfx_playing = true;
            }
            var em =
                bot
                    .gameObject
                    .transform
                    .Find("HitSpot/HoldParticle")
                    .GetComponent<ParticleSystem>()
                    .emission;
            em.enabled = true;
            if (goodHold[3] == false) holdScore += 1;
            goodHold[3] = true;
        }
        else
        {

            var em =
                bot
                    .gameObject
                    .transform
                    .Find("HitSpot/HoldParticle")
                    .GetComponent<ParticleSystem>()
                    .emission;
            em.enabled = false;

            // if (goodHold[3] == true) holdScore -= 1;
            goodHold[3] = false;
            updateHold[3] = false;
        }


        if (midiReader.index == 0)
        {
            index = midiReader.index;
            for (int i = 0; i < 4; i++)
            {
                goodHold[i] = false;
                updateHold[i] = false;
            }
        }

        if (goodHold.Any(b => b))
        {
            int newIndex = midiReader.index;

            if (newIndex > index)
            {
                holdLengths[0]--;
                holdLengths[1]--;
                holdLengths[2]--;
                holdLengths[3]--;
                scoreManager.score += holdScore;
                index = newIndex;
            }
        }
        else
        {
            sfx.Stophold_hit();
            hold_sfx_playing = false;
        }

        // Set the next beat when current beat is over.
        if (conductor.GetSongPosition() > next_spot)
        {
            last_spot += conductor.spotLength;
            next_spot += conductor.spotLength;

            // BELOW: Consequences for missing things
            switch (midiReader.beatmap[midiReader.index][0])
            {
                case NoteType.NOTE:
                    {
                        if (!hit_notes[midiReader.index][0])
                            ResolveMiss(top, 0);
                    }
                    break;
                case NoteType.OBSTACLE:
                    {
                        if (character.current_track_position == 3)
                            ResolveHitObstacle(top, 0);
                    }
                    break;
                case NoteType.HOLD:
                    {
                        if (!hit_notes[midiReader.index][0])
                            ResolveMiss(top, 0);
                    }
                    break;
            }
            switch (midiReader.beatmap[midiReader.index][1])
            {
                case NoteType.NOTE:
                    {
                        if (!hit_notes[midiReader.index][1])
                            ResolveMiss(high, 1);
                    }
                    break;
                case NoteType.OBSTACLE:
                    {
                        if (character.current_track_position == 2)
                            ResolveHitObstacle(high, 1);
                    }
                    break;
                case NoteType.HOLD:
                    {
                        if (!hit_notes[midiReader.index][1])
                            ResolveMiss(high, 1);
                    }
                    break;
            }
            switch (midiReader.beatmap[midiReader.index][2])
            {
                case NoteType.NOTE:
                    {
                        if (!hit_notes[midiReader.index][2])
                            ResolveMiss(low, 2);
                    }
                    break;
                case NoteType.OBSTACLE:
                    {
                        if (character.current_track_position == 1)
                            ResolveHitObstacle(low, 2);
                    }
                    break;
                case NoteType.HOLD:
                    {
                        if (!hit_notes[midiReader.index][2])
                            ResolveMiss(low, 2);
                    }
                    break;
            }
            switch (midiReader.beatmap[midiReader.index][3])
            {
                case NoteType.NOTE:
                    {
                        if (!hit_notes[midiReader.index][3])
                            ResolveMiss(bot, 3);
                    }
                    break;
                case NoteType.OBSTACLE:
                    {
                        if (character.current_track_position == 0)
                            ResolveHitObstacle(bot, 3);
                    }
                    break;
                case NoteType.HOLD:
                    {
                        if (!hit_notes[midiReader.index][3])
                            ResolveMiss(bot, 3);
                    }
                    break;
            }

            midiReader.updateTrackState();
            spawnMaster.SpawnNotes();
            accuracy = (perfectNum + (goodNum * goodWeight)) / totalNote * 100;
        }
    }

    private void ResolveHit(
        SpriteRenderer sprite,
        int trackNumber,
        HitCategory hitCategory,
        bool isHold
    )
    {
        if (hitCategory == HitCategory.WEAK && isHold)
        {
            updateHold[trackNumber] = true;
            holdScore = 1;
            holdLengths[trackNumber] = spawnMaster.lengths[trackNumber];
            holdLengths[trackNumber]++;
        }
        else if (hitCategory == HitCategory.GOOD && isHold)
        {
            updateHold[trackNumber] = true;
            holdScore = 2;
            holdLengths[trackNumber] = spawnMaster.lengths[trackNumber];
            holdLengths[trackNumber]++;
        }

        if (hitCategory == HitCategory.WEAK)
        {
            sfx.Playhit_2();
            psmain.startColor = weak_color;
            npsmain.startColor = weak_color * 1.2f;
            ParticleSystem clone =
                (ParticleSystem)
                Instantiate(particles,
                new Vector3(sprite.transform.position.x,
                    sprite.transform.position.y,
                    -6),
                Quaternion.identity);
            text.text = "GOOD";
            ParticleSystem clone2 =
                (ParticleSystem)
                Instantiate(hittext,
                new Vector3(sprite.transform.position.x,
                    sprite.transform.position.y,
                    -6),
                Quaternion.identity);
            ParticleSystem clone3 =
                (ParticleSystem)
                Instantiate(circleParticle,
                new Vector3(sprite.transform.position.x,
                    sprite.transform.position.y,
                    -6),
                Quaternion.identity);
            ParticleSystem clone4 =
                (ParticleSystem)
                Instantiate(nutParticle,
                new Vector3(sprite.transform.position.x,
                    sprite.transform.position.y,
                    -6),
                Quaternion.identity);
            sprite.color = weak_color;
            Destroy(clone.gameObject, 0.5f);
            Destroy(clone2.gameObject, 1.0f);
            Destroy(clone3.gameObject, 1.0f);
            Destroy(clone4.gameObject, 1.0f);
            scoreManager.score += 1;
            goodNum++;
        }
        else if (hitCategory == HitCategory.GOOD)
        {
            sfx.Playhit_1();
            perfect_color.a = 1f;
            psmain.startColor = perfect_color;
            npsmain.startColor = perfect_color * 1.2f;
            ParticleSystem clone =
                (ParticleSystem)
                Instantiate(particles,
                new Vector3(sprite.transform.position.x,
                    sprite.transform.position.y,
                    -6),
                Quaternion.identity);
            text.text = "PERFECT";
            ParticleSystem clone2 =
                (ParticleSystem)
                Instantiate(hittext,
                new Vector3(sprite.transform.position.x,
                    sprite.transform.position.y,
                    -6),
                Quaternion.identity);
            ParticleSystem clone3 =
                (ParticleSystem)
                Instantiate(circleParticle,
                new Vector3(sprite.transform.position.x,
                    sprite.transform.position.y,
                    -6),
                Quaternion.identity);
            ParticleSystem clone4 =
                (ParticleSystem)
                Instantiate(nutParticle,
                new Vector3(sprite.transform.position.x,
                    sprite.transform.position.y,
                    -6),
                Quaternion.identity);
            perfect_color.a = 0.75f;
            sprite.color = perfect_color;
            Destroy(clone.gameObject, 0.5f);
            Destroy(clone2.gameObject, 1.0f);
            Destroy(clone3.gameObject, 1.0f);
            Destroy(clone4.gameObject, 1.0f);
            scoreManager.score += 2;
            perfectNum++;
        }
        else if (hitCategory == HitCategory.MISS)
        {
            ResolveMiss (sprite, trackNumber);
            return;
        }

        // Note: this only applies if you don't miss. See return above.
        comboManager.comboNumber++;
    }

    private void ResolveMiss(SpriteRenderer sprite, int trackNumber)
    {
        anim.SetTrigger("hurt");
        anim2.SetTrigger("hurt");
        anim3.SetTrigger("hurt");
        missNum++;
        if (comboManager.comboNumber > maxCombo)
        {
            maxCombo = comboManager.comboNumber;
        }
        comboManager.comboNumber = 0;
        character.curHealth -= 2;
        character.hb.setHealth(character.curHealth);
        if (character.curHealth <= 0) character.Die();

        sfx.Playmiss();
        psmain.startColor = fail_color;
        ParticleSystem clone =
            (ParticleSystem)
            Instantiate(particles,
            sprite.transform.position,
            Quaternion.identity);
        text.text = "MISS";
        ParticleSystem clone2 =
            (ParticleSystem)
            Instantiate(hittext,
            new Vector3(sprite.transform.position.x,
                sprite.transform.position.y,
                -6),
            Quaternion.identity);
        sprite.color = fail_color;
        Destroy(clone.gameObject, 0.5f);
        Destroy(clone2.gameObject, 1.0f);
    }

    private void ResolveHitObstacle(SpriteRenderer sprite, int trackNumber)
    {
        anim.SetTrigger("hurt");
        anim2.SetTrigger("hurt");
        anim3.SetTrigger("hurt");
        character.curHealth -= 5;
        character.hb.setHealth(character.curHealth);
        if (character.curHealth <= 0) character.Die();

        sfx.Playmiss();
        psmain.startColor = fail_color;
        ParticleSystem clone =
            (ParticleSystem)
            Instantiate(particles,
            sprite.transform.position,
            Quaternion.identity);
        text.text = "OUCH";
        ParticleSystem clone2 =
            (ParticleSystem)
            Instantiate(hittext,
            new Vector3(sprite.transform.position.x,
                sprite.transform.position.y,
                -6),
            Quaternion.identity);
        sprite.color = fail_color;
        Destroy(clone.gameObject, 0.5f);
        Destroy(clone2.gameObject, 1.0f);
    }

    // Checks for a hit on a given keycode.
    // Returns true if hit, false if not.
    private void checkHit(
        InputActionReference controls,
        int trackNumber,
        SpriteRenderer sprite,
        float pianoThreshold
    )
    {
        float joystickAxis = 0.0f;

        bool pianoBool = true;
        if (trackNumber == 0)
        {
            pianoBool = flickUpLeft;
            joystickAxis = leftJoystick.action.ReadValue<float>();
        }
        else if (trackNumber == 1)
        {
            pianoBool = flickDownLeft;
            joystickAxis = -leftJoystick.action.ReadValue<float>();
        }
        else if (trackNumber == 2)
        {
            pianoBool = flickUpRight;
            joystickAxis = rightJoystick.action.ReadValue<float>();
        }
        else if (trackNumber == 3)
        {
            pianoBool = flickDownRight;
            joystickAxis = -rightJoystick.action.ReadValue<float>();
        }

        if (joystickAxis <= pianoThreshold)
        {
            pianoBool = false;
            if (trackNumber == 0)
                flickUpLeft = false;
            else if (trackNumber == 1)
                flickDownLeft = false;
            else if (trackNumber == 2)
                flickUpRight = false;
            else if (trackNumber == 3) flickDownRight = false;
        }
        if (
            controls.action.WasPressedThisFrame() ||
            (joystickAxis >= pianoThreshold && !pianoBool)
        )
        {
            pianoBool = true;
            if (trackNumber == 0)
                flickUpLeft = true;
            else if (trackNumber == 1)
                flickDownLeft = true;
            else if (trackNumber == 2)
                flickUpRight = true;
            else if (trackNumber == 3) flickDownRight = true;

            int[] to_check;
            double song_position = conductor.GetSongPosition();
            double since_last_spot = song_position - last_spot;
            double till_next_spot = next_spot - song_position;
            if (since_last_spot < till_next_spot)
            {
                // -2 -1  1  2
                //  3  1  2  4
                to_check = new int[] { 0, 1, -1, 2 };
            }
            else
            {
                // -2 -1  1  2
                //  4  2  1  3
                to_check = new int[] { 1, 0, 2, -1 };
            }

            // TODO: Guard against multiple presses
            foreach (int i in to_check)
            {
                int index = midiReader.index + i;
                if (i < 0 || i > midiReader.beatmap.Count()) continue; // Array bounds-checking

                float time_away_from_this_hit = 0.0f;
                if (i == 0)
                    time_away_from_this_hit = (float) since_last_spot;
                else if (i == 1)
                    time_away_from_this_hit = (float) till_next_spot;
                else if (i == -1)
                    time_away_from_this_hit =
                        (float) since_last_spot + conductor.spotLength;
                else if (i == 2)
                    time_away_from_this_hit =
                        (float) till_next_spot + conductor.spotLength;
                else
                {
                    Debug.Assert(false, "Should not get here!\n");
                }
                HitCategory hit_category;
                if (time_away_from_this_hit < perfect * conductor.spotLength)
                {
                    hit_category = HitCategory.GOOD;
                }
                else if (time_away_from_this_hit < good * conductor.spotLength)
                {
                    hit_category = HitCategory.WEAK;
                }
                else
                {
                    // never get here
                    hit_category = HitCategory.MISS;
                }

                switch (midiReader.beatmap[index][trackNumber])
                {
                    case NoteType.NOTE:
                        {
                            if (
                                hit_category != HitCategory.MISS &&
                                !hit_notes[index][trackNumber]
                            )
                            {
                                ResolveHit(sprite,
                                trackNumber,
                                hit_category,
                                false);
                                hit_notes[index][trackNumber] = true;
                            }
                        }
                        break;
                    case NoteType.HOLD:
                        {
                            if (
                                hit_category != HitCategory.MISS &&
                                !hit_notes[index][trackNumber]
                            )
                            {
                                ResolveHit(sprite,
                                trackNumber,
                                hit_category,
                                true);
                                hit_notes[index][trackNumber] = true;
                            }
                        }
                        break;
                }
            }
        }
    }

    public void showHitbox()
    {
        top.GetComponent<SpriteRenderer>().enabled = true;
        top.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        high.GetComponent<SpriteRenderer>().enabled = true;
        high.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        low.GetComponent<SpriteRenderer>().enabled = true;
        low.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        bot.GetComponent<SpriteRenderer>().enabled = true;
        bot.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
    }

    public void HideHitbox()
    {
        top.GetComponent<SpriteRenderer>().enabled = false;
        top.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        high.GetComponent<SpriteRenderer>().enabled = false;
        high.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        low.GetComponent<SpriteRenderer>().enabled = false;
        low.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        bot.GetComponent<SpriteRenderer>().enabled = false;
        bot.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
    }
}
