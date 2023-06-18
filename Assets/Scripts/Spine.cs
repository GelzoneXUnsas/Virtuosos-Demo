using System.Collections;
using System.Collections.Generic;
//using UnityEngine.UIElements;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public enum InterfaceState
{
    GAMEPLAY,
    DIALOGUE,
    TUTORIAL,
    RESULTS,
    GAME_OVER
}

public enum GameplayAudio
{
    DRUMS,
    PIANO,
    GUITAR,
    S1,
    S2,
    END
}

public enum Character
{
    NONE,
    BRONTE,
    VENTO,
    DOLCE
}

public class DialogueLine
{
    public Character character;

    public string line;

    public DialogueLine(Character char_in, string line_in)
    {
        character = char_in;
        line = line_in;
    }
}

public class TutorialClip
{
    public string name;

    public string video_path;

    public TutorialClip(string name_in, string video_in)
    {
        name = name_in;
        video_path = video_in;
    }
}

public struct Section
{
    public DialogueLine[] conversation;

    public TutorialClip[] tutorialVideos;

    public string midi_path;

    public GameplayAudio audio_to_play;

    public int l2_background_audio;

    public bool background_audio;

    public int beats_till_first_note;

    public Section(
        DialogueLine[] conv_in,
        TutorialClip[] video_in,
        string midi_in,
        GameplayAudio audio_in,
        int l2_background_audio_in,
        bool background_audio_in,
        int beats_till_in
    )
    {
        conversation = conv_in;
        tutorialVideos = video_in;
        midi_path = midi_in;
        audio_to_play = audio_in;
        l2_background_audio = l2_background_audio_in;
        background_audio = background_audio_in;
        beats_till_first_note = beats_till_in;
    }
}

public class Spine : MonoBehaviour
{
    public int section_index = -1;

    public Section[] sections;

    public InterfaceState state = InterfaceState.DIALOGUE;

    public Conductor conductor;

    public MIDIReader midiReader;

    public CameraController cam;

    public Dialogue dialogue;

    public PlayerController player;

    public NoteTrigger noteTrigger;

    public TracksController tracksController;

    public HealthBar healthBar;

    public ScoreManager scoreManager;

    public ComboManager comboManager;

    public StatsManager statsManager;

    public GameObject fade;

    public Tutorial tutorial;

    public string midiPath;

    public string videoPath;

    public string dialoguePath;

    private Image image;

    private Color color;

    DialogueLine[] first_dialogue = new DialogueLine[0];

    DialogueLine[] second_dialogue = new DialogueLine[0];

    DialogueLine[] third_dialogue = new DialogueLine[0];

    TutorialClip[] first_tutorial = new TutorialClip[0];

    TutorialClip[] second_tutorial = new TutorialClip[0];

    TutorialClip[] third_tutorial = new TutorialClip[0];

    TutorialClip[] fourth_tutorial = new TutorialClip[0];

    TutorialClip[] fifth_tutorial = new TutorialClip[0];

    void Awake()
    {
        midiPath = Application.streamingAssetsPath + "/MIDIs/";
        videoPath = Application.streamingAssetsPath + "/videos/";
        dialoguePath = Application.streamingAssetsPath + "/Dialogue/";
    }

    void Start()
    {
        if (1 == PlayerPrefs.GetInt("level_number"))
        {
            initDialogue1();
            initTutorial_level1();
            initLevel1();
        }
        else
        {
            initDialogue2();
            initTutorial_level2();
            initLevel2();
        }

        image = fade.GetComponent<Image>();
        color = image.color;

        DialogueStart();
    }

    private bool fading_out;

    public void Update()
    {
        if (section_index == 2 && PlayerPrefs.GetInt("level_number") == 2)
            tracksController.spawnBoss = true;
        if (state == InterfaceState.RESULTS)
        {
            //hideStatsUI
            if (
                Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.Joystick1Button0) ||
                Input.GetKeyDown(KeyCode.Joystick1Button1)
            )
            {
                fading_out = true;
            }

            if (fading_out)
            {
                bool done = conductor.FadeAudioOut();

                if (done)
                {
                    if(PlayerPrefs.GetInt("level_number") == 1) {
                        GoToLevel2();
                    }
                    else {
                        GoToLevel3();
                    }
                    noteTrigger.StatsReset();
                }
            }
        }
    }

    public void GoToLevel2()
    {
        PlayerPrefs.SetInt("level_number", 2);
        SceneManager.LoadScene("CutScene");
    }

    public void GoToLevel3()
    {
        PlayerPrefs.SetInt("level_number", 3);
        SceneManager.LoadScene("CutScene");
    }

    public void DialogueStart()
    {
        if (section_index > 1 && PlayerPrefs.GetInt("level_number") == 2 ||
            section_index > 2)
        {
            state = InterfaceState.RESULTS;

            // TODO: Lucas – Here's your stats UI page!
            if (comboManager.comboNumber > noteTrigger.maxCombo)
            {
                noteTrigger.maxCombo = comboManager.comboNumber;
            }
            if(PlayerPrefs.GetInt("level_number") != 2)
                StartCoroutine(statsManager.displayStatsUI(0.5f, 0.5f, 0.5f, 0.0f));
            else
            {
                player.animBoss.SetTrigger("die");
                player.endGame = true;
                player.switching = true;
                cam.isMoving = false;
                player.anim.SetTrigger("idle");
                StartCoroutine(statsManager.displayStatsUI(0.5f, 0.5f, 0.5f, 2.0f));
            }
            return;
        }

        color.a = 0.4f;
        image.color = color;
        healthBar.showHealthBar = false;
        scoreManager.showScoreBar = false;
        comboManager.showComboBar = false;
        noteTrigger.HideHitbox();
        if (section_index >= sections.Length)
        {
            // END LEVEL HERE
            section_index = 0;
        }

        // NOTE: This is just for the sake of having background playing after a
        // no-background section.
        if (sections[section_index].background_audio)
            conductor.playBackground = true;

        if (sections[section_index].l2_background_audio == 1)
            conductor.playL2BG1 = true;
        if (sections[section_index].l2_background_audio == 2)
            conductor.playL2BG2 = true;
        if (sections[section_index].l2_background_audio == 3)
            conductor.playL2BG3 = true;

        state = InterfaceState.DIALOGUE;
        dialogue.Enable();
        midiReader.Initialize(sections[section_index].midi_path);

        cam.isMoving = false;
    }

    public void TutorialStart()
    {
        state = InterfaceState.TUTORIAL;
        tutorial.Enable();
        dialogue.Disable();
    }

    public void GameOverStart()
    {
        state = InterfaceState.GAME_OVER;
        comboManager.showComboBar = false;
        cam.isMoving = false;
    }

    public void GameplayStart()
    {
        color.a = 0.0f;
        image.color = color;
        healthBar.showHealthBar = true;
        scoreManager.showScoreBar = true;
        comboManager.showComboBar = true;
        noteTrigger.showHitbox();
        state = InterfaceState.GAMEPLAY;

        conductor.Reset();
        conductor.beats_till_first_note =
            sections[section_index].beats_till_first_note;
        switch (sections[section_index].audio_to_play)
        {
            case GameplayAudio.DRUMS:
                {
                    conductor.playDrumsNextBar = true;
                }
                break;
            case GameplayAudio.PIANO:
                {
                    conductor.playPianoNextBar = true;
                }
                break;
            case GameplayAudio.GUITAR:
                {
                    conductor.playGuitarNextBar = true;
                }
                break;
            case GameplayAudio.S1:
                {
                    conductor.playL2Section1 = true;
                }
                break;
            case GameplayAudio.S2:
                {
                    conductor.playL2Section2 = true;
                }
                break;
            case GameplayAudio.END:
                {
                    conductor.playL2End = true;
                }
                break;
        }
        if (sections[section_index].background_audio)
            conductor.playBackground = true;
        else
            conductor.playBackground = false;

        if (sections[section_index].l2_background_audio == 1)
            conductor.playL2BG1 = true;

        if (sections[section_index].l2_background_audio == 2)
            conductor.playL2BG2 = true;

        if (sections[section_index].l2_background_audio == 3)
            conductor.playL2BG3 = true;

        noteTrigger.Reset();
        tutorial.Disable();
        cam.isMoving = true;

        ++section_index;
    }

    private List<DialogueLine> readDialogueFile(string file_name)
    {
        List<DialogueLine> dialogueList =
            new List<DialogueLine>(first_dialogue.Length);

        StreamReader inp_stm = new StreamReader(dialoguePath + file_name);
        while (!inp_stm.EndOfStream)
        {
            string inp_ln = inp_stm.ReadLine();
            if (inp_ln.Length != 0 && (inp_ln[0] != '#'))
            {
                if (inp_ln.Contains(':'))
                {
                    var splitted = inp_ln.Split(':', 2);
                    Character c = Character.NONE;
                    if (splitted[0] == "VENTO")
                        c = Character.VENTO;
                    else if (splitted[0] == "BRONTE")
                        c = Character.BRONTE;
                    else if (splitted[0] == "DOLCE")
                        c = Character.DOLCE;
                    else
                        Debug
                            .Log("Unknown name: " +
                            splitted[0] +
                            " in dialogue file: " +
                            file_name);
                    string text = splitted[1].Trim('\n');

                    dialogueList.Add(new DialogueLine(c, text.Trim(' ')));
                }
                else
                {
                    dialogueList
                        .Add(new DialogueLine(Character.NONE,
                            inp_ln.Trim(' ')));
                }
            }
        }

        inp_stm.Close();

        return dialogueList;
    }

    private void initDialogue1()
    {
        List<DialogueLine> dialogueList_1 = readDialogueFile("dialogue_1.txt");
        first_dialogue = dialogueList_1.ToArray();

        List<DialogueLine> dialogueList_2 = readDialogueFile("dialogue_2.txt");
        second_dialogue = dialogueList_2.ToArray();

        List<DialogueLine> dialogueList_3 = readDialogueFile("dialogue_3.txt");
        third_dialogue = dialogueList_3.ToArray();
    }

    private void initDialogue2()
    {
        List<DialogueLine> dialogueList_1 = readDialogueFile("dialogue_4.txt");
        first_dialogue = dialogueList_1.ToArray();

        List<DialogueLine> dialogueList_2 = readDialogueFile("dialogue_5.txt");
        second_dialogue = dialogueList_2.ToArray();
    }

    private void initTutorial_level1()
    {
        List<TutorialClip> tutorialList_1 =
            new List<TutorialClip>(first_tutorial);

        tutorialList_1
            .Add(new TutorialClip("OBSTACLE", videoPath + "t1.mp4"));
        tutorialList_1.Add(new TutorialClip("DRUM", videoPath + "t2.mp4"));
        tutorialList_1
            .Add(new TutorialClip("SCREW", videoPath + "t3.mp4"));

        first_tutorial = tutorialList_1.ToArray();

        List<TutorialClip> tutorialList_2 =
            new List<TutorialClip>(second_tutorial);

        tutorialList_2
            .Add(new TutorialClip("PIANO", videoPath + "t4.mp4"));

        second_tutorial = tutorialList_2.ToArray();

        List<TutorialClip> tutorialList_3 =
            new List<TutorialClip>(third_tutorial);

        tutorialList_3
            .Add(new TutorialClip("GUITAR", videoPath + "t5.mp4"));

        third_tutorial = tutorialList_3.ToArray();
    }

    private void initTutorial_level2()
    {
        List<TutorialClip> tutorialList_4 =
            new List<TutorialClip>(fourth_tutorial);

        tutorialList_4
            .Add(new TutorialClip("SWITCH", videoPath + "t6.mp4"));


        fourth_tutorial = tutorialList_4.ToArray();

        List<TutorialClip> tutorialList_5 =
            new List<TutorialClip>(fifth_tutorial);

        tutorialList_5
            .Add(new TutorialClip("BOSS", videoPath + "t7.mp4"));

        fifth_tutorial = tutorialList_5.ToArray();

    }

    private void initLevel1()
    {
        sections =
            new Section[3]
            {
                new Section(first_dialogue,
                    first_tutorial,
                    midiPath + "DV_L1_drum.mid",
                    GameplayAudio.DRUMS,
                    0,
                    true,
                    -17),
                new Section(second_dialogue,
                    second_tutorial,
                    midiPath + "DV_L1_piano.mid",
                    GameplayAudio.PIANO,
                    0,
                    false,
                    -17),
                new Section(third_dialogue,
                    third_tutorial,
                    midiPath + "DV_L1_guitar.mid",
                    GameplayAudio.GUITAR,
                    0,
                    true,
                    -17)
            };
    }

    private void initLevel2()
    {
        sections =
            new Section[2]
            {
                new Section(first_dialogue,
                    fourth_tutorial,
                    midiPath + "DV_L2_Section_1.mid",
                    GameplayAudio.S1,
                    1,
                    false,
                    -17),
                new Section(second_dialogue,
                    fifth_tutorial,
                    midiPath + "DV_L2_Section_2.mid",
                    GameplayAudio.S2,
                    2,
                    false,
                    -1),
            };
    }
}
