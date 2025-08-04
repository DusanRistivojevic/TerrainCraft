using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerrainCraft;
namespace TerrainCraft
{
    public class TimeOfDay : MonoBehaviour
    {

        private Light dayNightLight;
        [SerializeField]
        private Color dayColor;
        [SerializeField]
        private Color nightColor;
        [SerializeField]
        private float duration = 20;

        private float timer = 0;
        private bool isDay;
        // Start is called before the first frame update
        void Start()
        {
            dayNightLight = GetComponent<Light>();
            timer = duration;
            dayNightLight.color = dayColor;
        }

        // Update is called once per frame
        void Update()
        {
            if (isDay)
            {
                timer -= Time.deltaTime;
                if (timer < 0)
                {
                    timer = 0;
                    isDay = false;
                }
                dayNightLight.color = Color.Lerp(dayNightLight.color, dayColor, 0.3f * Time.deltaTime);
            }
            else
            {
                timer += Time.deltaTime;
                if (timer > duration)
                {
                    timer = duration;
                    isDay = true;
                }
                dayNightLight.color = Color.Lerp(dayNightLight.color, nightColor, 0.3f * Time.deltaTime);
            }

        }
    }
}
