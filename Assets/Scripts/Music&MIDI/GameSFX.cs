using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSFX : MonoBehaviour
{
    public AudioSource hit_1;
    public AudioSource hit_2;
    public AudioSource hold_hit;
    public AudioSource miss;
    public AudioSource collect;
    public AudioSource streetlight;
    public AudioSource dialog;
    public AudioSource count;
    public AudioSource count_done;

    public void Playhit_1(){
        hit_1.Play();
    }

    public void Playhit_2(){
        hit_2.Play();
    }

    public void Playhold_hit(){
        hold_hit.Play();
    }

    public void Stophold_hit(){
        hold_hit.Stop();
    }

    public void Playmiss(){
        miss.Play();
    }

    public void Playcollect(){
        collect.Play();
    }

    public void Playstreetlight(){
        streetlight.Play();
    }

    public void Playdialog(){
        dialog.Play();
    }

    public void Playcount(){
        count.Play();
    }

    public void Playcount_done(){
        count_done.Play();
    }


}
