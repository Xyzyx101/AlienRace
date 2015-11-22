using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{

    public enum CheckpointSign
    {
        Race,
        Checkpoint,
        Lap1,
        Lap2,
        Finish
    }

    public RaceManager Manager;
    public int CheckpointID;
    public GameObject ActiveSign;
    public Material PassedMat;
    public Material NotPassedMat;

    public GameObject RaceSign;
    public GameObject CheckpointSignObject;
    public GameObject Lap1Sign;
    public GameObject Lap2Sign;
    public GameObject FinishSign;

    private float CheckpointTime = 10f;
    private float CheckpointTimer;
    private bool PlayerTouched;

    void Update()
    {
        if (PlayerTouched)
        {
            CheckpointTimer -= Time.deltaTime;
            if (CheckpointTimer < 0f)
            {
                PlayerTouched = false;
                if (ActiveSign == RaceSign)
                {
                    SetSign(CheckpointSign.Lap1);
                }
                ActiveSign.GetComponent<Renderer>().material = NotPassedMat;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Manager.Checkpoint(other.tag, CheckpointID);
        if (other.tag == "PlayerCar")
        {
            ActiveSign.GetComponent<Renderer>().material = PassedMat;
            PlayerTouched = true;
            CheckpointTimer = CheckpointTime;
        }
    }

    public void SetSign(CheckpointSign sign)
    {
        if (ActiveSign)
        {
            ActiveSign.GetComponent<Renderer>().material = NotPassedMat;
            ActiveSign.SetActive(false);
        }

        switch (sign)
        {
            case CheckpointSign.Race:
                ActiveSign = RaceSign;
                break;
            case CheckpointSign.Checkpoint:
                ActiveSign = CheckpointSignObject;
                break;
            case CheckpointSign.Lap1:
                ActiveSign = Lap1Sign;
                break;
            case CheckpointSign.Lap2:
                ActiveSign = Lap2Sign;
                break;
            case CheckpointSign.Finish:
                ActiveSign = FinishSign;
                break;
        }
        ActiveSign.GetComponent<Renderer>().material = NotPassedMat;
        ActiveSign.SetActive(true);
    }
}
