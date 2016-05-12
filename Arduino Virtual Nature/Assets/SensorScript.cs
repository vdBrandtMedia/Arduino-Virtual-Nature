using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class SensorScript : MonoBehaviour
{
    SerialPort sp = new SerialPort("COM3", 9600);

    public Light lt1; // light intensity lightpole 1
    public Light lt2; // light intensity lightpole 2
    public Light lt3; // light intensity lightpole 3
    public float amplitude;

    public GameObject lHeatObj; //WarmLight Object
    public Light ltHeat;        //WarmLight intensity

    public GameObject trees; //Tree object group
    public float treesSize;  //Tree object group height

    public WindZone wind;
    public double windSpeed;

    public Particle rainAndSnow;
    public string weatherKind;
    public float weatherValue;

    public GameObject rain;
    public GameObject snow;

    public float a; // Sound
    public float b; // Humidity
    public float c; // Temperature
    public float d; // LDR

    void Start()
    {
        sp.Open();          // Open the COM Port
        sp.ReadTimeout = 1; // Read time-out for lag decrease

        lt1 = GetComponent<Light>();
        lt2 = GetComponent<Light>();
        lt3 = GetComponent<Light>();

        weatherKind = "Clear";
    }

    void Update()
    {
        try
        {
            //print(sp.ReadLine());

            string value = sp.ReadLine();       //Read the information
            string[] vec3 = value.Split(' ');   //Split the information to an array
            a = float.Parse(vec3[0]);
            b = float.Parse(vec3[1]);
            c = float.Parse(vec3[2]);
            d = float.Parse(vec3[3]);

            print(a); // Sound

            if (b > 110) { c = b / 10; }
            print(b); // Humidity

            if (c > 110) { c = c / 10; }
            print(c); // Temperature

            print(d); // LDR

            //Wind trough Sound - A
            windSpeed = (a - 10) * 2;
            if(windSpeed < 0) {windSpeed = 0;}
            wind.windTurbulence = (float)windSpeed;

            // Rain and Snow trough Humidity - B and Temp - C
            weatherValue = (b);
            if (weatherValue < 50) {
                weatherKind = "Clear";
                rain.SetActive(false);
                snow.SetActive(false);
            }

            if (weatherValue > 50 && c > 22) {
                weatherKind = "Rain";
                rain.SetActive(true);
                snow.SetActive(false);
            }

            if (weatherValue > 50 && c < 22) {
                weatherKind = "Snow";
                snow.SetActive(true);
                rain.SetActive(false);
            }

            //Tree Size trough temp - C
            treesSize = c / 24;
            trees.transform.localScale = new Vector3(1, treesSize, 1);

            //Light warmth trough temp - C
            if (c > 26) { lHeatObj.SetActive(true); rain.SetActive(false); }
            if (c < 24) { lHeatObj.SetActive(false); }
            ltHeat.intensity = c - 22;

            //Light intensity trough LDR - D
            amplitude = (d - 150) / 8;
            lt1.intensity = amplitude;
            lt2.intensity = amplitude;
            lt3.intensity = amplitude;

        }
        catch (System.Exception){}
    }
}