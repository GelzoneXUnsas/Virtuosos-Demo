using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;

    public TextMeshProUGUI scoreMesh;

    private float originalX;

    public float onChangeRatio;

    public bool showScoreBar = false;

    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        originalX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        string scoreText = "SCORE: " + score;
        scoreMesh.GetComponent<TMPro.TextMeshProUGUI>().text = scoreText;

        if (transform.position.x < originalX + (Screen.width / onChangeRatio) && showScoreBar)
        {
            transform.position =
                Vector3
                    .Lerp(transform.position,
                    new Vector3(originalX + (Screen.width / onChangeRatio),
                        transform.position.y,
                        transform.position.z),
                    Time.deltaTime * moveSpeed);
        }
        else if (transform.position.x > originalX && !showScoreBar)
        {
            transform.position =
                Vector3
                    .Lerp(transform.position,
                    new Vector3(originalX,
                        transform.position.y,
                        transform.position.z),
                    Time.deltaTime * moveSpeed);
        }
    }
}
