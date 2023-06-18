using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    public int comboNumber = 0;

    public GameObject combo;

    public GameObject comment;

    public GameObject comboHub;

    public float moveSpeed = 1.0f;

    private float originalX;

    public float onChangeRatio;

    public bool showComboBar = false;

    string comboText;

    string commentText;

    // Start is called before the first frame update
    void Start()
    {
        originalX = comboHub.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (comboNumber != 0 && showComboBar)
        {
            comboHub.transform.position =
                Vector3
                    .Lerp(comboHub.transform.position,
                    new Vector3(originalX + Screen.width / onChangeRatio,
                        comboHub.transform.position.y,
                        comboHub.transform.position.z),
                    Time.deltaTime * moveSpeed);
            comboText = comboNumber.ToString();
            if (comboNumber >= 150)
            {
                commentText = "MAESTRO!";
            }
            else if (comboNumber >= 100)
            {
                commentText = "VIRTUOSO!";
            }
            else if (comboNumber >= 75)
            {
                commentText = "GROOVIE!";
            }
            else if (comboNumber >= 50)
            {
                commentText = "ELITE!";
            }
            else if (comboNumber >= 30)
            {
                commentText = "RHYTHM!";
            }
            else if (comboNumber >= 15)
            {
                commentText = "FINESSE!";
            }
            else
            {
                commentText = "NOVICE!";
            }
        }
        else
        {
            // Vector3 targetPosition = originalPosition;
            // targetPosition.x -= 700.0f;
            comboHub.transform.position =
                Vector3
                    .Lerp(comboHub.transform.position,
                    new Vector3(originalX,
                        comboHub.transform.position.y,
                        comboHub.transform.position.z),
                    Time.deltaTime * moveSpeed);
            comboText = "";
            commentText = "";
        }
        combo.GetComponent<TMPro.TextMeshProUGUI>().text = comboText;
        comment.GetComponent<TMPro.TextMeshProUGUI>().text = commentText;
    }
}
