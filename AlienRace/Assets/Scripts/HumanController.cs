using UnityEngine;
using System.Collections;

public class HumanController : Controller {
    
	// Update is called once per frame
	void Update () {
        FacingAxis = Input.GetAxis("Facing");
        MoveVector = Input.GetAxis("Vertical") * Vector3.forward + Input.GetAxis("Horizontal") * Vector3.right;
	}
}
