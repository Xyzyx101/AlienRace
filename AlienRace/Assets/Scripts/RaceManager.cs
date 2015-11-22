using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RaceManager : MonoBehaviour
{
    public Car[] Cars;
    public int Laps;
    public CarData[] data;
    public Checkpoint[] Checkpoints;
    public Checkpoint FinishLine;
    private float StartRaceTime;

    private CarData PlayerCar;
    public Text CurrentLapGUIText;
    public Text RaceTimeGUIText;
    public Text Lap1TimeGUIText;
    public Text Lap2TimeGUIText;
    public Text Lap3TimeGUIText;

    public GameObject WinnerPanel;
    public Text[] WinnerNames;
    public Text[] WinnerTimes;
    private int CarsComplete = 0;

    private bool Paused;
    public GameObject RestartRaceMessage;
    public GameObject PausePanel;

    void Start()
    {
        StartRaceTime = 0f;
        data = new CarData[Cars.Length];
        for (int i = 0; i < Cars.Length; ++i)
        {
            data[i] = new CarData();
            data[i].Tag = Cars[i].tag;
            if (data[i].Tag == "PlayerCar")
            {
                PlayerCar = data[i];
            }
            CurrentLapGUIText.text = "1";
            Lap1TimeGUIText.text = TimeToString(0);
            Lap2TimeGUIText.text = "";
            Lap3TimeGUIText.text = "";
        }
        WinnerPanel.SetActive(false);
        for (int i = 0; i < WinnerNames.Length; ++i)
        {
            WinnerNames[i].text = "";
            WinnerTimes[i].text = "";
        }
        PausePanel.SetActive(false);
        RestartRaceMessage.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            TogglePause();
        }
        if (StartRaceTime == 0f)
        {
            return;
        }
        CurrentLapGUIText.text = (PlayerCar.LapsComplete + 1).ToString();
        RaceTimeGUIText.text = TimeToString(Time.time - StartRaceTime);
        switch (PlayerCar.LapsComplete)
        {
            case 0:
                Lap1TimeGUIText.text = TimeToString(Time.time - StartRaceTime);
                break;
            case 1:
                Lap2TimeGUIText.text = TimeToString(Time.time - StartRaceTime - PlayerCar.LapTimes[0]);
                break;
            case 2:
                Lap3TimeGUIText.text = TimeToString(Time.time - StartRaceTime - PlayerCar.LapTimes[0] - PlayerCar.LapTimes[1]);
                break;
        }
    }

    public void StartRace()
    {
        for (int i = 0; i < Cars.Length; ++i)
        {
            Cars[i].Mobile();
        }
        StartRaceTime = Time.time;
        FinishLine.SetSign(global::Checkpoint.CheckpointSign.Race);
    }

    public void Checkpoint(string CarTag, int Checkpoint)
    {
        for (int i = 0; i < data.Length; ++i)
        {
            if (data[i].Tag == CarTag)
            {
                if (data[i].PassedCheckpoint + 1 == Checkpoint)
                {
                    data[i].PassedCheckpoint = Checkpoint;
                    Debug.Log(CarTag + " passed checkpoint " + Checkpoint);
                }
                if (data[i].PassedCheckpoint == Checkpoints.Length)
                {
                    if (data[i].LapsComplete < Laps)
                    {
                        data[i].LapsComplete++;
                    }
                    float lapTime = Time.time - StartRaceTime;
                    for (int j = 0; j < data[i].LapTimes.Count; ++j)
                    {
                        lapTime -= data[i].LapTimes[j];
                    }
                    data[i].LapTimes.Add(lapTime);
                    data[i].PassedCheckpoint = 0;
                    if (CarTag == "PlayerCar")
                    {
                        if (data[i].LapTimes.Count == 1)
                        {
                            FinishLine.SetSign(global::Checkpoint.CheckpointSign.Lap2);
                        }
                        else if (data[i].LapTimes.Count == 2)
                        {
                            FinishLine.SetSign(global::Checkpoint.CheckpointSign.Finish);
                        }
                    }
                    Debug.Log(CarTag + "completed lap " + data[i].LapsComplete + " in " + TimeToString(lapTime));
                }
                if (data[i].LapsComplete == Laps && data[i].TotalTime == 0)
                {
                    data[i].TotalTime = Time.time - StartRaceTime;
                    WinnerPanel.SetActive(true);
                    WinnerNames[CarsComplete].text = data[i].Tag;
                    WinnerTimes[CarsComplete].text = TimeToString(data[i].TotalTime);
                    CarsComplete++;
                    if (CarTag == "PlayerCar")
                    {
                        RestartRaceMessage.SetActive(true);
                    }
                    Debug.Log(CarTag + " completed race in " + TimeToString(data[i].TotalTime));
                }
                break;
            }
        }
    }

    public void ResumePressed()
    {
        TogglePause();
    }

    public void RestartRacePressed()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void QuitPressed()
    {
        Application.Quit();
    }

    string TimeToString(float time)
    {
        int minutes = (int)time / 60;
        float seconds = Mathf.Floor((time - (float)minutes * 60f) * 100f) * 0.01f;
        return minutes.ToString() + ":" + (seconds < 10f ? "0" : "") + seconds.ToString();
    }

    void TogglePause()
    {
        Paused = !Paused;
        Time.timeScale = Paused ? 0f : 1f;
        PausePanel.SetActive(Paused);
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
