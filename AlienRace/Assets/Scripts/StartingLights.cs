using UnityEngine;
using System.Collections;

public class StartingLights : MonoBehaviour
{
    public float StartTime = 5f;
    public RaceManager Manager;
    private float Timer;
    private Material LightMat;
    public float PingPongTimer = 20;
    private float CurrentPingPongTimer;
    // Use this for initialization
    void Start()
    {
        Timer = StartTime;
        LightMat = GetComponent<Renderer>().material;
        LightMat.shader = Shader.Find("Standard");
        LightMat.color = Color.grey;
        CurrentPingPongTimer = PingPongTimer;
    }

    // Update is called once per frame
    void Update()
    {
        Timer -= Time.deltaTime;
        float emission = Mathf.PingPong(Time.time * CurrentPingPongTimer, 1.0f);
        Color baseColor = Color.grey;
        if (Timer < 0f)
        {
            baseColor = Color.green;
            emission = 1f;
            Manager.StartRace();
            Destroy(this);
        }
        else if (Timer < 1f)
        {
            baseColor = Color.yellow;
            CurrentPingPongTimer = PingPongTimer * 0.25f;
        }
        else if (Timer < 3f)
        {
            baseColor = Color.red;
            CurrentPingPongTimer = PingPongTimer * 0.5f;
        }
        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);
        LightMat.SetColor("_EmissionColor", finalColor);
    }
}
