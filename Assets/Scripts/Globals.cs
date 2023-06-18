using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Globals : MonoBehaviour
{
    public StartManager startManager;
    public LatencyCalibrator lc;
    public Graphic graphic;
    public Persist sounds;

    private const string latencyOffset = "latency_offset";

    private const string screen_width = "screen_width";
    private const string screen_height = "screen_height";
    private const string full_screen = "is_full_screen";
    private const string vsync = "is_vsync";

    private const string master = "master_volume";
    private const string music = "music_volume";
    private const string sfx = "sfx_volume";
    private const string level_number = "level_number";
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (startManager.isLaunching)
        {
            PlayerPrefs.SetFloat(latencyOffset, lc.latency_offset);

            PlayerPrefs.SetInt(screen_width, graphic.resolutions[graphic.selectedRes].horizontal);
            PlayerPrefs.SetInt(screen_height, graphic.resolutions[graphic.selectedRes].vertical);
            PlayerPrefs.SetInt(full_screen, graphic.fullScreenTog.isOn ? 1 : 0);
            PlayerPrefs.SetInt(vsync, graphic.vsyncTog.isOn ? 1 : 0);

            PlayerPrefs.SetFloat(master, sounds.master_volume);
            PlayerPrefs.SetFloat(music, sounds.music_volume);
            PlayerPrefs.SetFloat(sfx, sounds.sfx_volume);

            PlayerPrefs.SetInt(level_number, 1);
        }
    }
}
