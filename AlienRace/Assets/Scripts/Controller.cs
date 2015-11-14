using UnityEngine;
using System.Collections;

public abstract class Controller : MonoBehaviour {
    //FIXME These should be protected
    public float FacingAxis;
    public Vector3 MoveVector;

    public Vector3 GetMoveVector()
    {
        return MoveVector;
    }

    public float GetFacingAxis()
    {
        return FacingAxis;
    }
}
