using UnityEngine;
using System.Collections;

public class RepairBotSpawn : MonoBehaviour {
    public GameObject StuckWhatsit;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void CallStuckWhatsit(GameObject car)
    {
        GameObject stuckWhatsitObj = (GameObject)Instantiate(StuckWhatsit, transform.position, transform.rotation);
        StuckWhatsit stuckWhatsit = stuckWhatsitObj.GetComponent<StuckWhatsit>();
        stuckWhatsit.SetTarget(car);
    }
}
