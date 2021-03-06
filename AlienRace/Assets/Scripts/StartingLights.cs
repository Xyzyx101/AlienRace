﻿using UnityEngine;
using System.Collections;

public class StartingLights : MonoBehaviour
{
    public float StartTime = 6f;
    public RaceManager Manager;
    public Light[] Lights;
    public Color Red;
    public Color Yellow;
    public Color Green;

    private float Timer;
    private Material LightMat;
    private bool RaceStarted;

    void Start()
    {
        Timer = StartTime;
        RaceStarted = false;
        SetLightColor(Color.black);
    }

    // Update is called once per frame
    void Update()
    {
        Timer -= Time.deltaTime;
        if (RaceStarted)
        {
            if (Timer < -10f)
            {
                DestroyLights();
                Destroy(this);
            }
        }
        else
        {
            if (Timer < 0f)
            {
                SetLightColor(Green);
                Manager.StartRace();
                RaceStarted = true;
            }
            else if (Timer < 2f)
            {
                SetLightColor(Yellow);
            }
            else if (Timer < 4f)
            {
                SetLightColor(Red);
            }
        }
    }

    void SetLightColor(Color color)
    {
        for (int i = 0; i < Lights.Length; ++i)
        {
            Lights[i].color = color;
        }
    }

    void DestroyLights()
    {
        for (int i = 0; i < Lights.Length; ++i)
        {
            Destroy(Lights[i].gameObject);
        }
    }
}
