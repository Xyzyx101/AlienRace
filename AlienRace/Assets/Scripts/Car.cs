﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Car : MonoBehaviour
{
    public GameObject[] FrontEngines;
    public GameObject[] BackEngines;
    private List<Transform> AllEngines;

    public float TurnMaxAngle;
    public float TiltSidewaysMaxAngle;
    public float TiltFrontMaxAngle;
    public float EngineTiltSpeed;

    public float EngineForce;
    public float NormalizedUpForce;
    public float MainThrustForce;

    public float TurnAmount;
    public Vector3 StabilizationVector;
    public float StabilizationAmount;

    public Transform COG;

    private HumanController Controller;
    private Rigidbody RB;
    private List<CarForce> Forces;

    void Start()
    {
        Controller = GetComponent<HumanController>();
        RB = GetComponent<Rigidbody>();
        RB.centerOfMass = COG.localPosition;
        AllEngines = new List<Transform>(FrontEngines.Length + BackEngines.Length);
        for (int i = 0; i < FrontEngines.Length; ++i)
        {
            AllEngines.Add(FrontEngines[i].transform);
        }
        for (int i = 0; i < BackEngines.Length; ++i)
        {
            AllEngines.Add(BackEngines[i].transform);
        }
        Forces = new List<CarForce>(6);

        StabilizationVector = transform.forward - (Vector3.Dot(transform.forward, Vector3.up) * Vector3.up);
    }

    void Update()
    {
        // Move
        float targetEngineForwardRot = 0f;
        float targetEngineSidewaysRot = 0f;
        Vector3 moveVector = Controller.GetMoveVector();
        targetEngineForwardRot = moveVector.z * TiltFrontMaxAngle;
        targetEngineSidewaysRot = moveVector.x * -TiltSidewaysMaxAngle;
        float forwardTilt = targetEngineForwardRot;
        float frontSidewaysTilt = targetEngineSidewaysRot;
        float backSidewaysTilt = targetEngineSidewaysRot;

        // Turn
        float turnAngle = Controller.GetFacingAxis() * TurnMaxAngle;
        frontSidewaysTilt -= turnAngle;
        backSidewaysTilt += turnAngle;
        frontSidewaysTilt = Mathf.Clamp(frontSidewaysTilt, -TiltFrontMaxAngle, TiltSidewaysMaxAngle);
        backSidewaysTilt = Mathf.Clamp(backSidewaysTilt, -TiltSidewaysMaxAngle, TiltSidewaysMaxAngle);

        float stabilizationTurn = Controller.GetFacingAxis() * TurnAmount * Time.deltaTime;
        StabilizationVector = Quaternion.AngleAxis(stabilizationTurn, Vector3.up) * StabilizationVector;

        // Apply Tilt
        Vector3 currentFrontTilt = FrontEngines[0].transform.localRotation.eulerAngles;
        Vector3 currentBackTilt = BackEngines[0].transform.localRotation.eulerAngles;
        forwardTilt = Mathf.LerpAngle(currentFrontTilt.x, forwardTilt, EngineTiltSpeed * Time.deltaTime);
        frontSidewaysTilt = Mathf.LerpAngle(currentFrontTilt.z, frontSidewaysTilt, EngineTiltSpeed * Time.deltaTime);
        backSidewaysTilt = Mathf.LerpAngle(currentBackTilt.z, backSidewaysTilt, EngineTiltSpeed * Time.deltaTime);
        for (int i = 0; i < FrontEngines.Length; ++i)
        {
            FrontEngines[i].transform.localRotation = Quaternion.Euler(forwardTilt + Random.value * 2 - 1f, 0f, frontSidewaysTilt + Random.value * 2 - 1f);
        }
        for (int i = 0; i < BackEngines.Length; ++i)
        {
            BackEngines[i].transform.localRotation = Quaternion.Euler(forwardTilt + Random.value * 2 - 1f, 0f, backSidewaysTilt + Random.value * 2 - 1f);
        }

        // Engine Forces
        Forces.Clear();
        LayerMask groundMask = LayerMask.GetMask(new string[] { "Ground" });
        for (int i = 0; i < AllEngines.Count; ++i)
        {
            Ray ray = new Ray(AllEngines[i].position, -AllEngines[i].up);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10, groundMask))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.blue);
                Vector3 point = AllEngines[i].position;
                float forceMag = EngineForce * 1f / (hit.distance * hit.distance);
                Vector3 force = AllEngines[i].up * forceMag;
                Forces.Add(new CarForce(force, point));
            }
        }

        // Normalized Up Force
        float upForceTotal = 0;
        // The normailzed force is calculated assuming Forces only has the engine forces in it so far
        for (int i = 0; i < Forces.Count; ++i)
        {
            upForceTotal += Vector3.Dot(Forces[i].Force, transform.up);
        }

        float upForceMag = Mathf.Clamp(NormalizedUpForce - upForceTotal, 0, NormalizedUpForce);
        Forces.Add(new CarForce(Vector3.up * upForceMag, transform.position));

        // Main Engine Thrust
        float forwardForce = MainThrustForce * moveVector.z;
        Debug.Log(forwardForce);
        Forces.Add(new CarForce(transform.forward * forwardForce, COG.position));
    }

    void FixedUpdate()
    {
        for (int i = 0; i < Forces.Count; ++i)
        {
            Vector3 point = transform.TransformPoint(Forces[i].Point);
            RB.AddForceAtPosition(Forces[i].Force, Forces[i].Point, ForceMode.Force);
            Debug.DrawRay(Forces[i].Point, Forces[i].Force * 0.002f, Color.red, Time.fixedDeltaTime);
        }
        Quaternion stable = Quaternion.LookRotation(StabilizationVector, Vector3.up);
        Quaternion newRot = Quaternion.Lerp(transform.rotation, stable, StabilizationAmount * Time.fixedDeltaTime);
        RB.MoveRotation(newRot);

        Debug.DrawRay(transform.position, 5f * transform.forward, Color.green);
        Debug.DrawRay(transform.position, 5f * StabilizationVector, Color.yellow);
    }

}

public struct CarForce
{
    public CarForce(Vector3 force, Vector3 point)
    {
        Force = force;
        Point = point;
    }
    public Vector3 Force;
    public Vector3 Point;
}
