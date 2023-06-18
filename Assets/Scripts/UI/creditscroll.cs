using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using System.Collections;

public class creditscroll : MonoBehaviour
{
    public float scroll_speed;

    private float initial_y_point;


    // Start is called before the first frame update
    public StartManager StartManager;
    public TextMeshProUGUI credit;
    public float changeRatio;
    public InputActionReference interact;

    public GameObject clip;
    VideoPlayer CreditClip;

    private bool leadin;

    void Start()
    {
        initial_y_point = credit.transform.position.y;
        CreditClip = clip.GetComponentInChildren<VideoPlayer>();
        CreditClip.source = VideoSource.Url;
        CreditClip.url = Application.streamingAssetsPath + "/videos/credits.mp4";

        leadin = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (credit.transform.position.y < Screen.height * changeRatio && StartManager.currentPage == CURRENTPAGE.ABOUT)
        {
            clip.GetComponent<Image>().enabled = true;
            CreditClip.enabled = true;
            if (!leadin)
            {
                StartCoroutine(FadeInCoroutine(clip));
                leadin = true;
            }
            CreditClip.Play();
            float cur_speed = scroll_speed;
            if (interact.action.ReadValue<float>() > 0)
            {
                cur_speed *= 10.0f;
            }
            credit.transform.Translate(0, cur_speed * Time.deltaTime * 10.0f, 0);
        }
        else
        {
            StartManager.backButton.GetComponent<TextMeshProUGUI>().enabled = true;
            StartManager.backButton.GetComponent<Button>().interactable = true;
            StartManager.cursorImage.SetActive(true);
            clip.GetComponent<Image>().enabled = false;
            CreditClip.Stop();
            CreditClip.enabled = false;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(StartManager.curBackButton);
            leadin = false;
        }
    }

    public void Reset()
    {
        StartManager.backButton.GetComponent<TextMeshProUGUI>().enabled = false;
        StartManager.backButton.GetComponent<Button>().interactable = false;
        credit.transform.position = new Vector3(credit.transform.position.x, initial_y_point, credit.transform.position.y);
    }

    private IEnumerator FadeInCoroutine(GameObject clip)
    {
        // Calculate the target alpha value
        float targetAlpha = 1f;
        float elapsedTime = 0f;

        // Perform the fade-in effect over time
        while (elapsedTime < 2.0f)
        {
            // Calculate the current alpha value based on the elapsed time and duration
            float currentAlpha = Mathf.Lerp(0f, targetAlpha, elapsedTime / 2.0f);

            // Update the alpha value of the material color
            Color newColor1 = clip.GetComponent<Image>().color;
            newColor1.a = currentAlpha;
            clip.GetComponent<Image>().color = newColor1;

            Color newColor2 = clip.GetComponentInChildren<RawImage>().color;
            newColor2.a = currentAlpha;
            clip.GetComponentInChildren<RawImage>().color = newColor2;

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the final alpha value is set to 1
        Color finalColor = clip.GetComponent<Image>().color;
        finalColor.a = targetAlpha;
        clip.GetComponent<Image>().color = finalColor;
        clip.GetComponentInChildren<RawImage>().color = finalColor;

    }
}
