using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Narrative : MonoBehaviour
{
    public TextMeshProUGUI textComponment;

    public TextMeshProUGUI name_field;

    public GameObject image;

    public string[] lines;

    public string[] characters;

    public Sprite[] images;

    public Sprite[] images2;

    public Sprite[] images3;

    public Image vento_image;

    public Image bronte_image;

    public Image dolce_image;

    public Image fadePanel;

    public int[] sectionBreak;

    public int currentSection = 0;

    public float textSpeed;

    public int narrative_index = -1;

    public int image_index = 0;

    public float fadeTime = 2.0f;

    public float fadeTime_2 = 1.0f;

    private float currentTime = 0.0f;

    private bool sceneStarting = true;

    private string dialoguePath;

    public AudioSource audioSource;

    public CutSceneSFX sfx;

    public Image cont;

    private bool leadin;

    public Image textbox;

    public Image dialoguebox;

    // Start is called before the first frame update
    void Start()
    {
        vento_image.enabled = false;
        bronte_image.enabled = false;
        dolce_image.enabled = false;

        dialoguePath = Application.streamingAssetsPath + "/Dialogue/";

        textComponment.text = string.Empty;
        name_field.text = string.Empty;
        fadePanel.color =
            new Color(fadePanel.color.r,
                fadePanel.color.g,
                fadePanel.color.b,
                1.0f);

        leadin = false;

        if (1 == PlayerPrefs.GetInt("level_number"))
        {
            readNarrativeFile("cutscene_1.txt");
            image.GetComponent<Image>().sprite = images[0];
        }
        if (2 == PlayerPrefs.GetInt("level_number"))
        {
            readNarrativeFile("cutscene_2.txt");
            image.GetComponent<Image>().sprite = images2[0];
        }
        if (3 == PlayerPrefs.GetInt("level_number"))
        {
            readNarrativeFile("cutscene_3.txt");
            image.GetComponent<Image>().sprite = images3[0];
        }

        dialoguebox.enabled = false;
        textbox.enabled = false;
    }

    private void readNarrativeFile(string file_name)
    {
        List<string> list = new List<string>();
        List<string> character_list = new List<string>();
        StreamReader inp_stm = new StreamReader(dialoguePath + file_name);
        while (!inp_stm.EndOfStream)
        {
            string inp_ln = inp_stm.ReadLine();
            if (inp_ln.Length != 0 && (inp_ln[0] != '#'))
            {
                if (inp_ln.Contains(':'))
                {
                    string[] parts = inp_ln.Split(':');
                    list.Add(parts[1]);
                    if (parts[0] == "DOLCE")
                    {
                        character_list.Add("Dolce");
                    }
                    else if (parts[0] == "BRONTE")
                    {
                        character_list.Add("Bronte");
                    }
                    else if (parts[0] == "VENTO")
                    {
                        character_list.Add("Vento");
                    }
                }
                else
                {
                    list.Add (inp_ln);
                    character_list.Add("");
                }
            }
        }
        inp_stm.Close();

        lines = list.ToArray();
        characters = character_list.ToArray();
    }

    public void Enable()
    {
        vento_image.enabled = false;
        bronte_image.enabled = false;
        dolce_image.enabled = false;

        textComponment.enabled = true;
        name_field.enabled = true;
        textbox.enabled = true;
        dialoguebox.enabled = true;

        if (currentSection != 0)
        {
            narrative_index = sectionBreak[currentSection - 1] - 1;
        }
        else
        {
            narrative_index = -1;
            image_index = 0;
        }
        NextLine();
    }

    public void Disable()
    {
        vento_image.enabled = false;
        bronte_image.enabled = false;
        dolce_image.enabled = false;

        textComponment.enabled = false;
        name_field.enabled = false;
        textbox.enabled = false;
        dialoguebox.enabled = false;
        StopAllCoroutines();
        textComponment.text = string.Empty;
        name_field.text = string.Empty;
    }

    public int NextLine()
    {
        cont.GetComponent<Image>().enabled = false;

        if (narrative_index < lines.Length - 2)
        {
            narrative_index++;
            if (lines[narrative_index] == "---")
            {
                NextImage();
                narrative_index++;
            }

            Debug.Log (narrative_index);
            if (1 == PlayerPrefs.GetInt("level_number") && narrative_index == 4)
            {
                sfx.Playfire();
            }

            if (1 == PlayerPrefs.GetInt("level_number") && narrative_index == 12
            )
            {
                sfx.Stopfire();
            }

            // crashing
            if (1 == PlayerPrefs.GetInt("level_number") && narrative_index == 32
            )
            {
                sfx.Playcrashing();
            }

            if (characters[narrative_index] != "")
            {
                textComponment.margin = new Vector4(0, 0, -600, -131);
                if (characters[narrative_index] == "Dolce")
                {
                    dolce_image.enabled = true;
                    vento_image.enabled = false;
                    bronte_image.enabled = false;
                }
                else if (characters[narrative_index] == "Bronte")
                {
                    bronte_image.enabled = true;
                    vento_image.enabled = false;
                    dolce_image.enabled = false;
                }
                else if (characters[narrative_index] == "Vento")
                {
                    vento_image.enabled = true;
                    bronte_image.enabled = false;
                    dolce_image.enabled = false;
                }

                name_field.text = characters[narrative_index];
                dialoguebox.enabled = true;
                textbox.enabled = false;
            }
            else
            {
                textComponment.margin = new Vector4(0, 0, -1400, -131);
                name_field.text = string.Empty;
                dialoguebox.enabled = false;
                textbox.enabled = true;
                dolce_image.enabled = false;
                bronte_image.enabled = false;
                vento_image.enabled = false;
            }

            textComponment.text = string.Empty;
            StopAllCoroutines();
            StartCoroutine(EmitLine());
            return narrative_index;
        }
        return -1;
    }

    public IEnumerator EmitLine()
    {
        char[] curLine = lines[narrative_index].ToCharArray();
        foreach (char c in curLine)
        {
            textComponment.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        cont.GetComponent<Image>().enabled = true;
    }

    public bool NextImage()
    {
        if (1 == PlayerPrefs.GetInt("level_number"))
        {
            if (image_index < images.Length - 1)
            {
                image_index++;
                image.GetComponent<Image>().sprite = images[image_index];
                return true;
            }
            return false;
        }
        else if (2 == PlayerPrefs.GetInt("level_number"))
        {
            if (image_index < images2.Length - 1)
            {
                image_index++;
                image.GetComponent<Image>().sprite = images2[image_index];
                return true;
            }
            return false;
        }
        else
        {
            if (image_index < images3.Length - 1)
            {
                image_index++;
                image.GetComponent<Image>().sprite = images3[image_index];
                return true;
            }
            return false;
        }
    }

    public void FadeToClear()
    {
        // Reduce the alpha of the panel by the amount of time that has passed
        fadePanel.color =
            Color
                .Lerp(fadePanel.color,
                Color.clear,
                Time.deltaTime / fadeTime_2);

        // Once the alpha is close to zero, disable the panel and stop fading
        if (fadePanel.color.a <= 0.05f)
        {
            fadePanel.color = Color.clear;
        }
    }

    public void FadeToBlack()
    {
        // Increase the alpha of the panel by the amount of time that has passed
        fadePanel.color =
            Color
                .Lerp(fadePanel.color,
                Color.black,
                Time.deltaTime / (fadeTime_2 / 2.0f));

        audioSource.volume =
            Mathf
                .Lerp(audioSource.volume,
                0.0f,
                Time.deltaTime / (fadeTime_2 / 2.0f));

        // Once the alpha is close to one, load the next scene
        if (fadePanel.color.a >= 0.95f)
        {
            if (PlayerPrefs.GetInt("level_number") != 3)
            {
                SceneManager.LoadScene("Game");
            }
            else
            {
                SceneManager.LoadScene("Start");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!leadin)
        {
            NextLine();
            leadin = true;
        }
        if (sceneStarting)
        {
            // Fade the panel out over the specified duration
            FadeToClear();
        }
        else
        {
            FadeToBlack();
        }

        currentTime += Time.deltaTime;

        // Calculate the alpha value based on the current time and fade time
        float alpha = currentTime / fadeTime;
        alpha = Mathf.Clamp01(alpha);

        Color color = textbox.GetComponent<Image>().color;
        color.a = alpha;

        if (color.a < 0.75f)
        {
            textbox.GetComponent<Image>().color = color;
        }

        if (
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.Joystick1Button0) ||
            Input.GetKeyDown(KeyCode.Joystick1Button1)
        )
        {
            sfx.Playdialog();
            int result = NextLine();
            if (result == -1)
            {
                Disable();
                sceneStarting = false;
            }
        }
    }
}
