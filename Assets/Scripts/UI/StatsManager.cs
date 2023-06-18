using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using System.Linq;
using static UnityEngine.Rendering.DebugUI;

public class StatsManager : MonoBehaviour
{
    public NoteTrigger noteTrigger;
    public ScoreManager scoreManager;
    public CollectableUI Collectable;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI perfectText;
    public TextMeshProUGUI goodText;
    public TextMeshProUGUI missText;
    public TextMeshProUGUI collectText;
    public TextMeshProUGUI maxcomboText;
    public TextMeshProUGUI accuracyText;
    public Image grade;

    public Sprite S;
    public Sprite A;
    public Sprite B;
    public Sprite C;
    public Sprite D;
    public Sprite F;


    public float onChangePannelRatio;
    public float onChangeLeftRatio;
    public float onChangeRightRatio;

    private float targetPannelPos;
    private float targetLeftPos;
    private float targetRightPos;

    void Start()
    {
        targetPannelPos = Screen.width * onChangePannelRatio;
        targetLeftPos = Screen.width * onChangeLeftRatio;
        targetRightPos = Screen.width * onChangeRightRatio;
        //DisableStats();
    }

    public void EnableStats()
    {
        foreach (GameObject child in gameObject.transform)
        {
            foreach (GameObject text in child.transform)
            {
                // Disable the child GameObject
                if (text.GetComponent<TextMeshProUGUI>() != null)
                {
                    text.GetComponent<TextMeshProUGUI>().enabled = true;
                }
            }
        }

    }

    public void DisableStats()
    {
        foreach (GameObject child in gameObject.transform)
        {
            foreach (GameObject text in child.transform)
            {
                // Disable the child GameObject
                if (text.GetComponent<TextMeshProUGUI>() != null)
                {
                    text.GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
        }
    }

    public IEnumerator displayPannel(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);

            for (int i = 0; i < 2; i++)
            {
                Vector3 startPannelPosition = gameObject.transform.GetChild(i).transform.position;
                Vector3 endPannelPosition = new Vector3(targetPannelPos,
                                                gameObject.transform.GetChild(i).transform.position.y,
                                                startPannelPosition.z);
                gameObject.transform.GetChild(i).transform.position = Vector3.Lerp(startPannelPosition, endPannelPosition, t);
            }

            yield return null;
        }
    }

    public IEnumerator displayRight(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);

            for (int i = 2; i < 6; i++)
            {
                Vector3 startLeftPosition = gameObject.transform.GetChild(i).transform.position;
                Vector3 endLeftPosition = new Vector3(targetLeftPos,
                                                    gameObject.transform.GetChild(i).transform.position.y,
                                                    startLeftPosition.z);
                gameObject.transform.GetChild(i).transform.position = Vector3.Lerp(startLeftPosition, endLeftPosition, t);
            }

            yield return null;
        }
    }

    public IEnumerator displayLeft(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);

            for (int i = 6; i < 9; i++)
            {
                Vector3 startRightPosition = gameObject.transform.GetChild(i).transform.position;
                Vector3 endRightPosition = new Vector3(targetRightPos,
                                                    gameObject.transform.GetChild(i).transform.position.y,
                                                    startRightPosition.z);
                gameObject.transform.GetChild(i).transform.position = Vector3.Lerp(startRightPosition, endRightPosition, t);
            }

            yield return null;
        }
    }

    public IEnumerator displayGrade(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);


                Vector3 startRightPosition = gameObject.transform.GetChild(9).transform.position;
                Vector3 endRightPosition = new Vector3(targetRightPos + 150.0f,
                                                    gameObject.transform.GetChild(9).transform.position.y,
                                                    startRightPosition.z);
                gameObject.transform.GetChild(9).transform.position = Vector3.Lerp(startRightPosition, endRightPosition, t);


            yield return null;
        }
    }



    public IEnumerator displayStatsUI(float t1, float t2, float t3, float t4)
    {
        scoreText.text = scoreManager.score.ToString();
        perfectText.text = noteTrigger.perfectNum.ToString();
        goodText.text = noteTrigger.goodNum.ToString();
        missText.text = noteTrigger.missNum.ToString();
        collectText.text = Collectable.collectableNum.ToString() + " / 10";
        maxcomboText.text = noteTrigger.maxCombo.ToString();
        accuracyText.text = noteTrigger.accuracy.ToString("0.#") + " %";
        if (noteTrigger.accuracy >= 95) 
        {
            grade.sprite = S;
        }
        else if (noteTrigger.accuracy >= 90)
        {
            grade.sprite = A;
        }
        else if (noteTrigger.accuracy >= 85)
        {
            grade.sprite = B;
        }
        else if (noteTrigger.accuracy >= 80)
        {
            grade.sprite = C;
        }
        else if (noteTrigger.accuracy >= 75)
        {
            grade.sprite = D;
        }
        else
        {
            grade.sprite = F;
        }

        yield return new WaitForSeconds(t4);
        StartCoroutine(displayPannel(t1));
        yield return new WaitForSeconds(t1);
        StartCoroutine(displayRight(t2));
        yield return new WaitForSeconds(t2);
        StartCoroutine(displayLeft(t3));
        yield return new WaitForSeconds(t3);
        StartCoroutine(displayGrade(t1));
        yield return new WaitForSeconds(t1);
    }
}
