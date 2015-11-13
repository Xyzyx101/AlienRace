using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaceManager : MonoBehaviour
{
    public Car[] Cars;
    public int Laps;
    public CarData[] data;
    public Checkpoint[] Checkpoints;
    private float StartRaceTime;

    // Use this for initialization
    void Start()
    {
        data = new CarData[Cars.Length];
        for (int i = 0; i < Cars.Length; ++i)
        {
            data[i] = new CarData();
            data[i].Tag = Cars[i].tag;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartRace()
    {
        for (int i = 0; i < Cars.Length; ++i)
        {
            Cars[i].Mobile();
        }
        StartRaceTime = Time.time;
    }

    public void Checkpoint(string CarTag, int Checkpoint)
    {
        for (int i = 0; i < data.Length; ++i)
        {
            if(data[i].Tag == CarTag)
            {
                if(data[i].PassedCheckpoint + 1 == Checkpoint)
                {
                    data[i].PassedCheckpoint = Checkpoint;
                    Debug.Log(CarTag + " passed checkpoint " + Checkpoint);
                }
                if(data[i].PassedCheckpoint == Checkpoints.Length)
                {
                    data[i].LapsComplete++;
                    float lapTime = Time.time - StartRaceTime;
                    for(int j = 0; i<data[j].LapTimes.Count; ++j)
                    {
                        lapTime -= data[i].LapTimes[j];
                    }
                    data[i].LapTimes.Add(lapTime);
                    data[i].PassedCheckpoint = 0;
                    Debug.Log(CarTag + "completed lap " + data[i].LapsComplete + " in " + ParseTime(lapTime));
                }
                if(data[i].LapsComplete == Laps)
                {
                    data[i].TotalTime = Time.time - StartRaceTime;
                    Debug.Log(CarTag + " completed race in " + ParseTime(data[i].TotalTime));
                }
                break;
            }
        }
    }

    string ParseTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        return minutes.ToString() + "m " + seconds.ToString() + "s";
    }
}

[System.Serializable]
public class CarData
{
    public CarData()
    {
        LapTimes = new List<float>();
    }
    public string Tag;
    public int PassedCheckpoint;
    public int LapsComplete;
    public List<float> LapTimes;
    public float TotalTime;
}
