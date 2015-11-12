using UnityEngine;
using System.Collections;

public class CameraChase : MonoBehaviour
{
    public Transform TargetCar;
    public Vector3 CameraOffset;
    public float TargetOffset;
    public float SpeedFactor;
    public float PositionLerp;
    public float RotationLerp;

    private Rigidbody RB;

    void Start()
    {
        RB = TargetCar.gameObject.GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        float currentSpeedFactor = RB.velocity.magnitude * SpeedFactor;

        Vector3 targetPos = TargetCar.position + TargetCar.forward + TargetCar.forward * TargetOffset * currentSpeedFactor;
        Quaternion newRot = Quaternion.LookRotation(targetPos - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRot, RotationLerp * Time.deltaTime);

        Vector3 localCameraOffset = CameraOffset + currentSpeedFactor * CameraOffset;
        Vector3 worldCameraOffset = TargetCar.TransformPoint(localCameraOffset);
        transform.position = Vector3.Lerp(transform.position, worldCameraOffset, PositionLerp * Time.deltaTime);
    }
}
