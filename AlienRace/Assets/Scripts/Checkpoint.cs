using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

    public RaceManager Manager;
    public int CheckpointID;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        Manager.Checkpoint(other.tag, CheckpointID);
    }
}
