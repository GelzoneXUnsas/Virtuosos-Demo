using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public GameObject parent;

    public Transform parentTransform;

    private Conductor conductor;

    private NoteTrigger notetrigger;

    private SpawnMaster spawnmaster;

    private CollectableUI collectables;

    float localSpot;

    private float rotSpeed = 200f;

    public int index;

    public int which_track;

    public GameObject holdFollower;

    private GameObject spawner;

    private float spawnOffset = 0.25f;

    public float yOffset;

    private GameObject[] followers = new GameObject[4];

    public float interpRatio;

    public bool hit_mode = false;

    //ParticleSystem.MainModule psmain;

    public ParticleSystem particles;

    public GameSFX sfx;

    // Start is called before the first frame update
    void Start()
    {
        //psmain = particles.main;
        parent = transform.parent.gameObject;
        conductor =
            (Conductor) GameObject.Find("/Conductor").GetComponent("Conductor");
        notetrigger =
            (NoteTrigger)
            GameObject.Find("/Tracks/NoteTrigger").GetComponent("NoteTrigger");
        spawnmaster =
            (SpawnMaster)
            GameObject.Find("/Tracks/SpawnMaster").GetComponent("SpawnMaster");
        collectables =
            (CollectableUI)
            GameObject
                .Find("/Canvas/Gameplay/HealthBar/Collectables")
                .GetComponent("CollectableUI");
        spawner = GameObject.Find("/Tracks/BottomTrack/Spawner4");

        sfx = (GameSFX)GameObject.Find("/SFX").GetComponent("GameSFX");
        parentTransform = transform.parent;
        parentTransform = transform.parent;
        localSpot = notetrigger.last_spot;
        if (gameObject.CompareTag("HoldSquare"))
        {
            SpawnObject(0, spawnOffset);
            SpawnObject(1, spawnOffset * 2);
            SpawnObject(2, spawnOffset * 3);
            SpawnObject(3, spawnOffset * 4);
        }
        hit_mode = false;
    }

    // Update is called once per frame
    void Update()
    {
        interpRatio =
            ((float) conductor.GetSongPosition() - localSpot) /
            (conductor.spotLength * spawnmaster.noteSpeed);

        if (hit_mode)
        {
            float interpRatio2 = (interpRatio - 1) * 5;
            float interpedPostionX =
                Mathf
                    .Lerp(notetrigger.transform.position.x,
                    parentTransform.position.x,
                    interpRatio2);
            float interpedPostionY =
                cos_interp(parentTransform.position.y,
                parentTransform.transform.position.y + 5f,
                interpRatio2);

            interpedPostionY *= Random.Range(0.65f, 0.85f);

            transform.position =
                new Vector3(interpedPostionX, interpedPostionY + yOffset, 0.0f);

            // Make the object spin around the y-axis
            transform.Rotate(Vector3.forward, rotSpeed * Time.deltaTime);
            rotSpeed += 0.3f * Time.deltaTime;
        }
        else
        {
            if (
                notetrigger
                    .hit_notes[index + spawnmaster.noteSpeed][which_track]
            )
            {
                hit_mode = true;
            }

            Vector3 interpedPostion =
                Vector3
                    .Lerp(parentTransform.position,
                    new Vector3(notetrigger.transform.position.x,
                        parentTransform.position.y,
                        0f),
                    interpRatio);
            transform.position =
                new Vector3(interpedPostion.x,
                    interpedPostion.y + yOffset,
                    interpedPostion.z);

            if (gameObject != null && gameObject.CompareTag("Collect"))
            {
                // Make the object spin around the y-axis
                transform.Rotate(Vector3.forward, rotSpeed * Time.deltaTime);
                rotSpeed += 0.03f * Time.deltaTime;
            }

            if (interpRatio > 1.0f)
            {
                GetComponent<SpriteRenderer>().enabled = false;

                if (followers[0] != null && notetrigger.goodHold[which_track])
                {
                    float interpRatio2 = (interpRatio - 1) * 10;

                    Vector3 interpedPostionF1 =
                        Vector3
                            .Lerp(followers[0].transform.position,
                            new Vector3(notetrigger.transform.position.x,
                                followers[0].transform.position.y,
                                0f),
                            interpRatio2);
                    Vector3 interpedPostionF2 =
                        Vector3
                            .Lerp(followers[1].transform.position,
                            new Vector3(notetrigger.transform.position.x,
                                followers[1].transform.position.y,
                                0f),
                            interpRatio2);
                    Vector3 interpedPostionF3 =
                        Vector3
                            .Lerp(followers[2].transform.position,
                            new Vector3(notetrigger.transform.position.x,
                                followers[2].transform.position.y,
                                0f),
                            interpRatio2);
                    Vector3 interpedPostionF4 =
                        Vector3
                            .Lerp(followers[3].transform.position,
                            new Vector3(notetrigger.transform.position.x,
                                followers[3].transform.position.y,
                                0f),
                            interpRatio2);
                    followers[0].transform.position = interpedPostionF1;
                    followers[1].transform.position = interpedPostionF2;
                    followers[2].transform.position = interpedPostionF3;
                    followers[3].transform.position = interpedPostionF4;
                    GetComponent<SpriteRenderer>().enabled = false;
                }

                // obstacle should go pass
                if (gameObject.CompareTag("Obstacle"))
                {
                    GetComponent<SpriteRenderer>().enabled = true;
                    float interpRatio2 = interpRatio - 1;
                    Vector3 interpedPostionBehind =
                        Vector3
                            .Lerp(new Vector3(notetrigger.transform.position.x,
                                parentTransform.position.y,
                                0f),
                            new Vector3(notetrigger.transform.position.x -
                                (
                                parentTransform.position.x -
                                notetrigger.transform.position.x
                                ),
                                parentTransform.position.y,
                                0f),
                            interpRatio2);
                    transform.position =
                        new Vector3(interpedPostionBehind.x,
                            interpedPostionBehind.y + yOffset,
                            interpedPostionBehind.z);
                }
                else // note should go off
                if (gameObject.CompareTag("Note") || gameObject.CompareTag("Collect"))
                {
                    GetComponent<SpriteRenderer>().enabled = true;
                    float interpRatio2 = interpRatio - 1;
                    Vector3 interpedPostionBehind =
                        Vector3
                            .Lerp(new Vector3(notetrigger.transform.position.x,
                                parentTransform.position.y,
                                0f),
                            new Vector3(notetrigger.transform.position.x -
                                (
                                parentTransform.position.x -
                                notetrigger.transform.position.x
                                ),
                                parentTransform.position.y,
                                0f),
                            interpRatio2);
                    transform.position =
                        new Vector3(interpedPostionBehind.x,
                            interpedPostionBehind.y + yOffset,
                            interpedPostionBehind.z);
                }
                else if (gameObject.CompareTag("HoldNote"))
                {
                    GetComponent<SpriteRenderer>().enabled = true;
                    float interpRatio2 = interpRatio - 1;
                    Vector3 interpedPostionBehind =
                        Vector3
                            .Lerp(new Vector3(notetrigger.transform.position.x,
                                parentTransform.position.y,
                                0f),
                            new Vector3(notetrigger.transform.position.x -
                                (
                                parentTransform.position.x -
                                notetrigger.transform.position.x
                                ),
                                parentTransform.position.y,
                                0f),
                            interpRatio2);
                    transform.position =
                        new Vector3(interpedPostionBehind.x,
                            interpedPostionBehind.y + yOffset,
                            interpedPostionBehind.z);
                }
                else if (gameObject.CompareTag("HoldSquare"))
                {
                    GetComponent<SpriteRenderer>().enabled = true;
                    float interpRatio2 = interpRatio - 1;
                    Vector3 interpedPostionBehind =
                        Vector3
                            .Lerp(new Vector3(notetrigger.transform.position.x,
                                parentTransform.position.y,
                                0f),
                            new Vector3(notetrigger.transform.position.x -
                                (
                                parentTransform.position.x -
                                notetrigger.transform.position.x
                                ),
                                parentTransform.position.y,
                                0f),
                            interpRatio2);
                    transform.position =
                        new Vector3(interpedPostionBehind.x,
                            interpedPostionBehind.y + yOffset,
                            interpedPostionBehind.z);
                }
            }

            if (followers[0] != null)
            {
                followers[0].transform.position =
                    new Vector3(transform.position.x + spawnOffset,
                        followers[0].transform.position.y,
                        followers[0].transform.position.z);
                followers[1].transform.position =
                    new Vector3(transform.position.x + spawnOffset * 2,
                        followers[1].transform.position.y,
                        followers[1].transform.position.z);
                followers[2].transform.position =
                    new Vector3(transform.position.x + spawnOffset * 3,
                        followers[2].transform.position.y,
                        followers[2].transform.position.z);
                followers[3].transform.position =
                    new Vector3(transform.position.x + spawnOffset * 4,
                        followers[3].transform.position.y,
                        followers[3].transform.position.z);
            }
        }

        if (!notetrigger.goodHold[which_track])
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (interpRatio > 1.0f)
        {
            if (followers[0] != null)
            {
                Destroy(followers[0]);
                Destroy(followers[1]);
                Destroy(followers[2]);
                Destroy(followers[3]);
                Destroy (gameObject);
            }
        }

        if (interpRatio > 2.0f)
        {
            Destroy(followers[0]);
            Destroy(followers[1]);
            Destroy(followers[2]);
            Destroy(followers[3]);
            Destroy (gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        collectables.collectableNum++;
        collectables.updateCollectables();
        if (gameObject.CompareTag("Collect"))
        {
            //psmain.startColor = notetrigger.perfect_color;
            ParticleSystem clone =(ParticleSystem)Instantiate(particles, new Vector3(notetrigger.transform.position.x, transform.position.y, -6), Quaternion.identity);
            Destroy(gameObject);
        }

        //PLAY SFX
        sfx.Playcollect();
        //SPAWN PARTICLES
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    void SpawnObject(int index, float offset)
    {
        Vector3 spawnPosition =
            new Vector3(transform.position.x - offset,
                transform.position.y + Random.Range(-0.07f, 0.07f),
                transform.position.z);
        followers[index] =
            Instantiate(holdFollower, spawnPosition, Quaternion.identity);
    }

    float cos_interp(float a, float b, float t)
    {
        float ct =
            -(
            1.0f - Mathf.Cos((1.0f - Mathf.Clamp(t, 0, 1)) * Mathf.PI * 0.5f)
            );
        return a * (1 - ct) + b * ct + 5.0f;
    }
}
