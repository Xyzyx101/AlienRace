using UnityEngine;
using System.Collections;

public abstract class Controller : MonoBehaviour {
    protected float FacingAxis;
    protected Vector3 MoveVector;

    public Vector3 GetMoveVector()
    {
        return MoveVector;
    }

    public float GetFacingAxis()
    {
        return FacingAxis;
    }
}
