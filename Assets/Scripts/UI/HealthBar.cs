using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public Gradient color;

    public Image fill;

    public CameraController cam;

    public float moveSpeed;

    public float onChangeRatio;

    private float originalY;

    public bool showHealthBar = false;

    public void Start()
    {
        originalY = transform.position.y;
    }

    public void Update()
    {
        if (transform.position.y < originalY + (Screen.height / onChangeRatio) && showHealthBar)
        {
            transform.position =
                Vector3
                    .Lerp(transform.position,
                    new Vector3(transform.position.x,
                        originalY + (Screen.height / onChangeRatio),
                        transform.position.z),
                    Time.deltaTime * moveSpeed);
        }
        else if(transform.position.y > originalY)
        {
            transform.position =
               Vector3
                   .Lerp(transform.position,
                   new Vector3(transform.position.x,
                       originalY,
                       transform.position.z),
                   Time.deltaTime * moveSpeed);
        }
    }

    public void setMaxHealth(int maxhealth)
    {
        slider.maxValue = maxhealth;
        slider.value = maxhealth;
        color.Evaluate(1f);
    }

    // Start is called before the first frame update
    public void setHealth(int health)
    {
        slider.value = health;
        fill.color = color.Evaluate(slider.normalizedValue);
    }
}
