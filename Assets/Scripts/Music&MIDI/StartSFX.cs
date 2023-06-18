using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartSFX : MonoBehaviour
{
    public AudioSource buttonClick;
    public AudioSource buttonHover;
    public AudioSource zoom;

    public Slider sfxSlider;

    void Start()
    {
        buttonClick.volume = sfxSlider.value;
        buttonHover.volume = sfxSlider.value;
        zoom.volume = sfxSlider.value;
    }
    public void PlaybuttonClick()
    {
        buttonClick.Play();
    }

    public void PlaybuttonHover()
    {
        buttonHover.Play();
    }

    public void Playzoom()
    {
        zoom.Play();
    }

}
