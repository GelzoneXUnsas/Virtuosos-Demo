using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public Spine spine;

    public TextMeshProUGUI textComponment;
    public TextMeshProUGUI name_field;

    public Image vento_image;
    public Image bronte_image;
    public Image dolce_image;
    public Image textbox;
    public Image narrativebox;

    public float textSpeed;

    AudioSource audioSource;

    // Implementation Variables
    public int line_index = -1;

    public Image cont;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        textComponment.text = string.Empty;
        textComponment.enabled = false;
        audioSource.enabled = false;
        textbox.enabled = false;
        narrativebox.enabled = false;

        vento_image.enabled = false;
        bronte_image.enabled = false;
        dolce_image.enabled = false;
    }

    public void Enable()
    {
        textComponment.enabled = true;
        name_field.enabled = true;
        audioSource.enabled = true;
        textbox.enabled = true;
        narrativebox.enabled = false;

        line_index = -1;

        NextLine();
    }

    public void Disable()
    {
        audioSource.enabled = false;
        textComponment.enabled = false;
        textbox.enabled = false;
        narrativebox.enabled = false;
        StopAllCoroutines();
        textComponment.text = string.Empty;
        name_field.enabled = false;
        vento_image.enabled = false;
        bronte_image.enabled = false;
        dolce_image.enabled = false;
    }

    // Returns false if no lines left.
    public int NextLine()
    {
        cont.GetComponent<Image>().enabled = false;
        if (line_index < spine.sections[spine.section_index].conversation.Length - 1)
        {
            if (line_index >= 0) GetImage(spine.sections[spine.section_index].conversation[line_index].character).enabled = false;
            line_index++;

            if(spine.sections[spine.section_index].conversation[line_index].character == Character.NONE) {
                name_field.enabled = false;
                narrativebox.enabled = true;
                textbox.enabled = false;
            }
            else {
                name_field.text = GetName(spine.sections[spine.section_index].conversation[line_index].character);
                GetImage(spine.sections[spine.section_index].conversation[line_index].character).enabled = true;
                name_field.enabled = true;
                narrativebox.enabled = false;
                textbox.enabled = true;
            }

            textComponment.text = string.Empty;
            StopAllCoroutines();
            StartCoroutine(EmitLine());
            return line_index;
        }

        // Conversation is over.
        GetImage(spine.sections[spine.section_index].conversation[line_index - 1].character).enabled = false;
        name_field.enabled = false;
        return -1;
    }

    public IEnumerator EmitLine()
    {
        audioSource.Play();
        char[] curLine = spine.sections[spine.section_index].conversation[line_index].line.ToCharArray();
        foreach (char c in curLine)
        {
            textComponment.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        cont.GetComponent<Image>().enabled = true;
        audioSource.Pause();
    }

    public string GetName(Character character)
    {
        switch (character)
        {
            case Character.VENTO:
                return "VENTO";
            case Character.BRONTE:
                return "BRONTE";
            case Character.DOLCE:
                return "DOLCE";
            default:
                return "NONE";
        }
    }

    public Image GetImage(Character character)
    {
        switch (character)
        {
            case Character.VENTO:
                return vento_image;
            case Character.BRONTE:
                return bronte_image;
            case Character.DOLCE:
                return dolce_image;
            default:
                return bronte_image;
        }
    }
}
