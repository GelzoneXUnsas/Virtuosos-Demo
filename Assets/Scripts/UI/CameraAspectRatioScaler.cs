using UnityEngine;
using UnityEngine.UI;

public class CameraAspectRatioScaler : MonoBehaviour {

    public Vector2 ReferenceResolution;
    public Vector3 ZoomFactor = Vector3.one;
    [HideInInspector]
    public Vector3 OriginPosition;
    void Start () {

        OriginPosition = transform.position;

        ReferenceResolution.x = PlayerPrefs.GetInt("screen_width");
        ReferenceResolution.y = PlayerPrefs.GetInt("screen_height");

        if (PlayerPrefs.GetInt("is_vsync") == 1)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }

        bool is_fs = PlayerPrefs.GetInt("is_full_screen") != 0;

        Screen.SetResolution(PlayerPrefs.GetInt("screen_width"), PlayerPrefs.GetInt("screen_height"), is_fs);
    }

    void Update () {

        if (ReferenceResolution.y == 0 || ReferenceResolution.x == 0)
            return;

        var refRatio = ReferenceResolution.x / ReferenceResolution.y;
        var ratio = (float)Screen.width / (float)Screen.height;

        transform.position = OriginPosition + transform.forward * (1f - refRatio / ratio) * ZoomFactor.z
                                            + transform.right   * (1f - refRatio / ratio) * ZoomFactor.x
                                            + transform.up      * (1f - refRatio / ratio) * ZoomFactor.y;


    }
}