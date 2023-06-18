using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScheduler : MonoBehaviour
{
    [SerializeField]
    private GameObject Map;

    [SerializeField]
    private GameObject foreground;

    [SerializeField]
    private GameObject Background;

    [SerializeField]
    private GameObject FarBackground;

    public CameraController cam;

    private GameObject clone;

    private GameObject clone2;

    private GameObject foreground_clone;

    private GameObject foreground_clone2;

    private GameObject Background_clone;

    private GameObject Background_clone2;

    private GameObject Far_Background_clone;

    private GameObject Far_Background_clone2;

    public GameObject plane;

    public float MapSpeed;

    public float ForegroundSpeed;

    public float BackgroundSpeed;

    public float FarBackgroundSpeed;

    float map_dx = 60f;

    float for_dx = 60f;

    float back_dx = 60f;

    float far_back_dx = 90f;

    float background_offset_x = 10.0f;

    private float map_end;

    private float for_end;

    private float b_end;

    private float fb_end;

    void Start()
    {
        map_end = plane.transform.position.x;
        clone =
            Instantiate(Map,
            new Vector3(map_end, Map.transform.position.y, Map.transform.position.z),
            Quaternion.identity);
        map_end += map_dx;
        clone2 =
            Instantiate(Map,
            new Vector3(map_end, Map.transform.position.y, Map.transform.position.z),
            Quaternion.identity);
        map_end += map_dx;

        b_end = plane.transform.position.x;
        Background_clone =
            Instantiate(Background,
            new Vector3(b_end + background_offset_x,
                Background.transform.position.y,
                Background.transform.position.z),
            Quaternion.identity);
        b_end += back_dx;
        Background_clone2 =
            Instantiate(Background,
            new Vector3(b_end + background_offset_x,
                Background.transform.position.y,
                Background.transform.position.z),
            Quaternion.identity);
        b_end += back_dx;

        fb_end = plane.transform.position.x;
        Far_Background_clone =
            Instantiate(FarBackground,
            new Vector3(fb_end +
                background_offset_x,
                FarBackground.transform.position.y,
                FarBackground.transform.position.z),
            Quaternion.identity);
        fb_end += far_back_dx;
        Far_Background_clone2 =
            Instantiate(FarBackground,
            new Vector3(fb_end +
                background_offset_x,
                FarBackground.transform.position.y,
                FarBackground.transform.position.z),
            Quaternion.identity);
        fb_end += far_back_dx;

        for_end = plane.transform.position.x;
        foreground_clone = Instantiate(foreground,
            new Vector3(for_end +
                + background_offset_x,
                foreground.transform.position.y,
                foreground.transform.position.z),
            Quaternion.identity);
        for_end += for_dx;
        foreground_clone2 = Instantiate(foreground,
            new Vector3(for_end +
                + background_offset_x,
                foreground.transform.position.y,
                foreground.transform.position.z),
            Quaternion.identity);
        for_end += for_dx;
    }

    void Update()
    {
        if (cam.isMoving)
        {
            float map_speed = MapSpeed * Time.deltaTime;
            float back_speed = BackgroundSpeed * Time.deltaTime;
            float far_back_speed = FarBackgroundSpeed * Time.deltaTime;
            float for_speed = ForegroundSpeed * Time.deltaTime;;
            if (clone != null)
            {
                clone.transform.Translate(new Vector3(-map_speed, 0, 0));
            }
            if (clone2 != null)
            {
                clone2.transform.Translate(new Vector3(-map_speed, 0, 0));
            }
            if (Background_clone != null)
            {
                Background_clone
                    .transform
                    .Translate(new Vector3(-back_speed, 0, 0));
            }
            if (Background_clone2 != null)
            {
                Background_clone2
                    .transform
                    .Translate(new Vector3(-back_speed, 0, 0));
            }
            if (Far_Background_clone != null)
            {
                Far_Background_clone
                    .transform
                    .Translate(new Vector3(-far_back_speed, 0, 0));
            }
            if (Far_Background_clone2 != null)
            {
                Far_Background_clone2
                    .transform
                    .Translate(new Vector3(-far_back_speed, 0, 0));
            }
            if (foreground_clone != null)
            {
                foreground_clone
                    .transform
                    .Translate(new Vector3(-for_speed, 0, 0));
            }
            if (foreground_clone2 != null)
            {
                foreground_clone2
                    .transform
                    .Translate(new Vector3(-for_speed, 0, 0));
            }
            map_end -= map_speed;
            b_end -= back_speed;
            fb_end -= far_back_speed;
            for_end -= for_speed;
        }

        if (clone2 == null)
        {
            clone2 =
                Instantiate(Map,
                new Vector3(map_end, Map.transform.position.y, 0f),
                Quaternion.identity);
            map_end += map_dx;
        }
        if (clone == null)
        {
            clone =
                Instantiate(Map,
                new Vector3(map_end, Map.transform.position.y, 0f),
                Quaternion.identity);
            map_end += map_dx;
        }
        if (clone != null)
        {
            if (clone.transform.position.x + map_dx < 0)
            {
                Destroy(clone.gameObject, 0f);
            }
        }
        if (clone2 != null)
        {
            if (clone2.transform.position.x + map_dx < 0)
            {
                Destroy(clone2.gameObject, 0f);
            }
        }

        if (Background_clone2 == null)
        {
            Background_clone2 =
                Instantiate(Background,
                new Vector3(b_end, Background.transform.position.y, 0f),
                Quaternion.identity);
            b_end += back_dx;
        }
        if (Background_clone == null)
        {
            Background_clone =
                Instantiate(Background,
                new Vector3(b_end, Background.transform.position.y, 0f),
                Quaternion.identity);
            b_end += back_dx;
        }
        if (Background_clone != null)
        {
            if (Background_clone.transform.position.x + back_dx < 0)
            {
                Destroy(Background_clone.gameObject, 0f);
            }
        }
        if (Background_clone2 != null)
        {
            if (Background_clone2.transform.position.x + back_dx < 0)
            {
                Destroy(Background_clone2.gameObject, 0f);
            }
        }

        if (Far_Background_clone2 == null)
        {
            Far_Background_clone2 =
                Instantiate(FarBackground,
                new Vector3(fb_end, FarBackground.transform.position.y, 0f),
                Quaternion.identity);
            fb_end += far_back_dx;
        }
        if (Far_Background_clone == null)
        {
            Far_Background_clone =
                Instantiate(FarBackground,
                new Vector3(fb_end, FarBackground.transform.position.y, 0f),
                Quaternion.identity);
            fb_end += far_back_dx;
        }
        if (Far_Background_clone != null)
        {
            if (Far_Background_clone.transform.position.x + far_back_dx < 0)
            {
                Destroy(Far_Background_clone.gameObject, 0f);
            }
        }
        if (Far_Background_clone2 != null)
        {
            if (Far_Background_clone2.transform.position.x + far_back_dx < 0)
            {
                Destroy(Far_Background_clone2.gameObject, 0f);
            }
        }
        
        if (foreground_clone2 == null)
        {
            foreground_clone2 =
                Instantiate(foreground,
                new Vector3(for_end, foreground.transform.position.y, 0f),
                Quaternion.identity);
            for_end += for_dx;
        }
        if (foreground_clone == null)
        {
            foreground_clone =
                Instantiate(foreground,
                new Vector3(for_end, foreground.transform.position.y, 0f),
                Quaternion.identity);
            for_end += for_dx;
        }
        if (foreground_clone != null)
        {
            if (foreground_clone.transform.position.x + for_dx < 0)
            {
                Destroy(foreground_clone.gameObject, 0f);
            }
        }
        if (foreground_clone2 != null)
        {
            if (foreground_clone2.transform.position.x + for_dx < 0)
            {
                Destroy(foreground_clone2.gameObject, 0f);
            }
        }
    }
}
