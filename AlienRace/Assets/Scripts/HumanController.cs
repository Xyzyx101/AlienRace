using UnityEngine;
using System.Collections;

public class HumanController : MonoBehaviour {

    public float FacingAxis;
    public Vector3 MoveVector;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        FacingAxis = Input.GetAxis("Facing");
        MoveVector = Input.GetAxis("Vertical") * Vector3.forward + Input.GetAxis("Horizontal") * Vector3.right;
	}

    public Vector3 GetMoveVector()
    {
        return MoveVector;
    }

    public float GetFacingAxis()
    {
        return FacingAxis;
    }
}
