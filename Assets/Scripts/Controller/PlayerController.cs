using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Spine spine;

    public Conductor conductor;

    public MIDIReader midiReader;

    public NoteTrigger noteTrigger;

    public TracksController tracksController;

    public Dialogue dialogue;

    public Tutorial tutorial;

    public GameObject collector;

    public JoystickControl controllerControl;

    public float movePower = 2f;

    public int maxHealth;

    public int curHealth;

    public HealthBar hb;

    private Rigidbody2D rb;

    public Animator anim;

    public Animator anim2;

    public Animator anim3;

    public Animator animBoss;

    public GameObject guitarist;
    public GameObject pianist;
    public GameObject boss;

    public GameSFX sfx;

    public float drumStartPos = 0;
    public float guitarStartPos = -5f;
    public float pianoStartPos = -5;
    public float bossStartPos = 10;
    public float drumEndPos = 0;
    public float guitarEndPos = -5;
    public float pianoEndPos = -5;
    public float bossEndPos = 10;
    float drumXVal = 0;
    float pianoXVal = -5;
    float guitarXVal = -5;
    float bossXVal = 10;

    public bool isDead = false;
    public bool endGame = false;

    private float timer = 0;

    public bool switching = false;

    public bool isBoss = false;

    bool isJumping = false;

    public float startX;

    public bool reset = false;

    public uint current_track_position;

    public float playerHeightOffset = 1.2f;

    private double lastNoteHitTime;

    public InputActionReference bottom, low, top, high, interact;

    public float fallSpeed = 1f;
    private float t = 0f;

    private bool partSpawned;

    public ParticleSystem explosion;
    public ParticleSystem sparks;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startX = rb.transform.position.x;
        reset = true;
        curHealth = maxHealth;
        hb.setMaxHealth (maxHealth);
        partSpawned = false;
    }

    private void Update()
    {
        if (switching && !isDead && !endGame)
        {
            drumXVal = Mathf.Lerp(drumStartPos, drumEndPos, timer);
            pianoXVal = Mathf.Lerp(pianoStartPos, pianoEndPos, timer);
            guitarXVal = Mathf.Lerp(guitarStartPos, guitarEndPos, timer);
            bossXVal = Mathf.Lerp(bossStartPos, bossEndPos, timer);
            timer += Time.deltaTime * 0.5f;
            transform.position = new Vector3(drumXVal, -3.2f, transform.position.z);
            pianist.transform.position = new Vector3(pianoXVal, -0.45f, 0);
            guitarist.transform.position = new Vector3(guitarXVal, -3.0f, guitarist.transform.position.z);
            boss.transform.position = new Vector3(bossXVal, boss.transform.position.y, boss.transform.position.z);
        }
        else if (isDead && switching)
        {
            transform.position = Vector3.Lerp(new Vector3(drumXVal, transform.position.y, transform.position.z), new Vector3(drumXVal, -3.65f, 0), timer);
            pianist.transform.position = Vector3.Lerp(new Vector3(pianoXVal, pianist.transform.position.y, transform.position.z), new Vector3(pianoXVal, -3.2f, 0), timer);
            guitarist.transform.position = Vector3.Lerp(new Vector3(guitarXVal, guitarist.transform.position.y, transform.position.z), new Vector3(guitarXVal, -3.2f, 0), timer);
            boss.transform.position = Vector3.Lerp(new Vector3(bossXVal, boss.transform.position.y, transform.position.z), new Vector3(bossXVal, -3.2f, 0), timer);
            timer += Time.deltaTime * 0.5f;
        }
        else if(endGame && switching)
        {
            boss.transform.position = Vector3.Lerp(new Vector3(bossXVal, boss.transform.position.y, transform.position.z), new Vector3(bossXVal, -2.0f, 0), timer);
            timer += Time.deltaTime * 0.5f;
            if(timer > 1)
            {
                if (!partSpawned)
                {
                    ParticleSystem clone =
                    (ParticleSystem)
                    Instantiate(explosion,
                    new Vector3(13.5f,
                        -0.8f,
                        -6),
                    Quaternion.identity);
                    Destroy(clone.gameObject, 5.0f);
                    ParticleSystem clone2 =
                    (ParticleSystem)
                    Instantiate(sparks,
                    new Vector3(13.5f,
                        -0.8f,
                        -6),
                    Quaternion.identity);
                    Destroy(clone2.gameObject, 5.0f);
                    partSpawned = true;
                }
            }
        }
        if(timer/2 >= 1)
        {
            drumStartPos = drumEndPos;
            guitarStartPos = guitarEndPos;
            pianoStartPos = pianoEndPos;
            bossStartPos = bossEndPos;
            timer = 0;
            switching = false;
        }

        switch (spine.state)
        {
            case InterfaceState.GAMEPLAY:
                {
                    anim.SetBool("isRun", true);
                    anim2.SetBool("isRun", true);
                    Attack();
                    Jump();
                    Run();
                    if (Input.GetKeyDown(KeyCode.P)) spine.DialogueStart();
                }
                break;
            case InterfaceState.DIALOGUE:
                {
                    anim.SetBool("isRun", false);
                    anim2.SetBool("isRun", false);
                    anim2.SetTrigger("idle");
                    if (interact.action.WasPressedThisFrame())
                    {
                        sfx.Playdialog();
                        int result = dialogue.NextLine();
                        if (result == -1) spine.TutorialStart();
                    }
                }
                break;
            case InterfaceState.TUTORIAL:
                {
                    if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Joystick1Button5))
                    {
                        tutorial.NextVideo();
                    }
                    else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Joystick1Button4))
                    {
                        tutorial.PrevVideo();
                    }
                    else if (interact.action.WasPressedThisFrame() && tutorial.cont.activeSelf)
                    {
                        spine.GameplayStart();
                    }
                }
                break;
            case InterfaceState.GAME_OVER:
                {
                }
                break;
        }
    }

    public void Die()
    {
        Debug.Log("YOU DIED!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        anim.SetBool("isJump", false);
    }

    void Run()
    {
        Vector3 moveVelocity = Vector3.right;
        if (!anim.GetBool("isJump")) anim.SetBool("isRun", true);
    }

    void Jump()
    {
        if (
            (Input.GetButtonDown("Jump") || Input.GetAxisRaw("Vertical") > 0) &&
            !anim.GetBool("isJump")
        )
        {
            isJumping = true;
            anim.SetBool("isJump", true);
        }
        if (!isJumping)
        {
            return;
        }

        rb.velocity = Vector2.zero;
        isJumping = false;
    }

    void Attack()
    {
        uint prev_track_pos = current_track_position;
        if (top.action.WasPressedThisFrame() || (current_track_position != 3 && top.action.ReadValue<float>() > 0.0f && guitarEndPos == 0.2f))
        {
            transform.position = new Vector3(drumXVal, tracksController.Track1.transform.position.y - playerHeightOffset, 0);
            guitarist.transform.position = new Vector3(guitarXVal, tracksController.Track1.transform.position.y - playerHeightOffset, 0);
            collector.transform.position = new Vector3(collector.transform.position.x, tracksController.Track1.transform.position.y, collector.transform.position.z);
            current_track_position = 3;
            lastNoteHitTime = conductor.GetSongPosition();

            if (prev_track_pos == current_track_position)
            {
                float randomValue = Random.value;
                string chosenString = randomValue < 0.5f ? "kickAttack" : "attack";
                anim.SetTrigger(chosenString);
                anim2.SetTrigger("attack2");
                anim3.SetTrigger("attack");
            }
            else if (prev_track_pos < current_track_position)
            {
                anim.SetTrigger("attack");
                anim2.SetTrigger("attack1");
                anim3.SetTrigger("attack");
            }
            else if (prev_track_pos > current_track_position)
            {
                anim.SetTrigger("downAttack");
                anim2.SetTrigger("attack1");
                anim3.SetTrigger("attack");
            }
        }
        if (high.action.WasPressedThisFrame() || (current_track_position != 2 && high.action.ReadValue<float>() > 0.0f && guitarEndPos == 0.2f))
        {
            transform.position = new Vector3(drumXVal, tracksController.Track2.transform.position.y - playerHeightOffset, 0);
            guitarist.transform.position = new Vector3(guitarXVal, tracksController.Track2.transform.position.y - playerHeightOffset, 0);
            collector.transform.position = new Vector3(collector.transform.position.x, tracksController.Track2.transform.position.y, collector.transform.position.z);
            current_track_position = 2;
            lastNoteHitTime = conductor.GetSongPosition();

            if (prev_track_pos == current_track_position)
            {
                float randomValue = Random.value;
                string chosenString = randomValue < 0.5f ? "kickAttack" : "attack";
                anim.SetTrigger(chosenString);
                anim2.SetTrigger("attack2");
                anim3.SetTrigger("attack");
            }
            else if (prev_track_pos < current_track_position)
            {
                anim.SetTrigger("attack");
                anim2.SetTrigger("attack1");
                anim3.SetTrigger("attack");
            }
            else if (prev_track_pos > current_track_position)
            {
                anim.SetTrigger("downAttack");
                anim2.SetTrigger("attack1");
                anim3.SetTrigger("attack");
            }
        }
        if (low.action.WasPressedThisFrame() || (current_track_position != 1 && low.action.ReadValue<float>() > 0.0f && guitarEndPos == 0.2f))
        {
            transform.position = new Vector3(drumXVal, tracksController.Track3.transform.position.y - playerHeightOffset, 0);
            guitarist.transform.position = new Vector3(guitarXVal, tracksController.Track3.transform.position.y - playerHeightOffset, 0);
            collector.transform.position =
                new Vector3(collector.transform.position.x,
                    tracksController.Track3.transform.position.y,
                    collector.transform.position.z);
            current_track_position = 1;
            lastNoteHitTime = conductor.GetSongPosition();

            if (prev_track_pos == current_track_position)
            {
                float randomValue = Random.value;
                string chosenString = randomValue < 0.5f ? "kickAttack" : "attack";
                anim.SetTrigger(chosenString);
                anim2.SetTrigger("attack2");
                anim3.SetTrigger("attack");
            }
            else if (prev_track_pos < current_track_position)
            {
                anim.SetTrigger("attack");
                anim2.SetTrigger("attack1");
                anim3.SetTrigger("attack");
            }
            else if (prev_track_pos > current_track_position)
            {
                anim.SetTrigger("downAttack");
                anim2.SetTrigger("attack1");
                anim3.SetTrigger("attack");
            }
        }
        if (bottom.action.WasPressedThisFrame() || (current_track_position != 0 && bottom.action.ReadValue<float>() > 0.0f && guitarEndPos == 0.2f))
        {
            transform.position = new Vector3(drumXVal, tracksController.Track4.transform.position.y - playerHeightOffset, 0);
            guitarist.transform.position = new Vector3(guitarXVal, tracksController.Track4.transform.position.y - playerHeightOffset, 0);
            collector.transform.position =
                new Vector3(collector.transform.position.x,
                    tracksController.Track4.transform.position.y,
                    collector.transform.position.z);
            current_track_position = 0;
            lastNoteHitTime = conductor.GetSongPosition();

            if (prev_track_pos == current_track_position)
            {
                float randomValue = Random.value;
                string chosenString = randomValue < 0.5f ? "kickAttack" : "attack";
                anim.SetTrigger(chosenString);
                anim2.SetTrigger("attack2");
                anim3.SetTrigger("attack");
            }
            else if (prev_track_pos < current_track_position)
            {
                anim.SetTrigger("attack");
                anim2.SetTrigger("attack1");
                anim3.SetTrigger("attack");
            }
            else if (prev_track_pos > current_track_position)
            {
                anim.SetTrigger("downAttack");
                anim2.SetTrigger("attack1");
                anim3.SetTrigger("attack");
            }
        }

        if (
            conductor.GetSongPosition() - (conductor.spotLength * 4) >=
            lastNoteHitTime &&
            ((!(top.action.ReadValue<float>() > 0.0f) &&
            !(high.action.ReadValue<float>() > 0.0f) &&
            !(low.action.ReadValue<float>() > 0.0f) &&
            !(bottom.action.ReadValue<float>() > 0.0f)) || guitarEndPos != 0.2f)
        )
        {
            t = 0;
            t += fallSpeed * Time.deltaTime;
            t = Mathf.Clamp01(t);
            //float curveT = Mathf.SmoothStep(0f, 1f, t);
            //transform.position = new Vector3(0, -3.2f, 0);
            transform.position = Vector3.Lerp(transform.position, new Vector3(drumXVal, -3.2f, 0), t);
            guitarist.transform.position = Vector3.Lerp(guitarist.transform.position, new Vector3(guitarXVal, -3f, 0), t);
            collector.transform.position =
                new Vector3(collector.transform.position.x,
                    tracksController.Track4.transform.position.y,
                    collector.transform.position.z);
            current_track_position = 0;
        }
    }
}
