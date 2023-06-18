using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneSFX : MonoBehaviour
{
    public AudioSource fire;
    public AudioSource crashing;
    public AudioSource dialog;

    public void Playdialog()
    {
        dialog.Play();
    }

    public void Playcrashing()
    {
        crashing.Play();
    }

    public void Playfire()
    {
        fire.Play();
    }

    public void Stopfire()
    {
        fire.Stop();
    }

}
