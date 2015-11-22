using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class CameraChase : MonoBehaviour
{
    public Transform TargetCar;
    public Vector3 MinCameraOffset;
    public Vector3 MaxCameraOffset;
    public float MinTargetOffset;
    public float MaxTargetOffset;
    public float PositionLerp;
    public float RotationLerp;
    public float FOVLerp;
    public float FOVMin;
    public float FOVMax;
    public float SpeedRange;
    private Rigidbody RB;
    private Camera thisCamera;
    private Vector3 PreviousPosition;
    private TiltShift MyTiltShift;

    void Start()
    {
        RB = TargetCar.gameObject.GetComponent<Rigidbody>();
        thisCamera = GetComponent<Camera>();
        MyTiltShift = GetComponent<TiltShift>();
        PreviousPosition = transform.position;
    }

    void LateUpdate()
    {
        float speed = RB.velocity.magnitude;
        float speedFactor = Mathf.Clamp01(speed / SpeedRange);
        MyTiltShift.maxBlurSize = speedFactor;

        Vector3 targetPos = TargetCar.position + TargetCar.forward * Mathf.Lerp(MinTargetOffset, MaxTargetOffset, speedFactor);
        Quaternion newRot = Quaternion.LookRotation(targetPos - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRot, RotationLerp * Time.deltaTime);

        Vector3 localCameraOffset = Vector3.Lerp(MinCameraOffset, MaxCameraOffset, speedFactor);
        Vector3 worldPosition = TargetCar.TransformPoint(localCameraOffset);
        Vector3 averagePosition = Vector3.Lerp(PreviousPosition, worldPosition, 0.5f);
        PreviousPosition = worldPosition;

        transform.position = Vector3.Lerp(transform.position, averagePosition, PositionLerp * Time.deltaTime);

        float FOVTarget = FOVMin + (FOVMax - FOVMin) * Mathf.Clamp01(speed / SpeedRange);
        thisCamera.fieldOfView = Mathf.Lerp(thisCamera.fieldOfView, FOVTarget, FOVLerp * Time.deltaTime);
    }
}
