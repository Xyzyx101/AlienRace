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
    public float SidewaysThrustForce;

    public float TurnAmount;
    public Vector3 StabilizationVector;
    public float StabilizationAmount;
    private Vector3 StabilizationNormal;

    public Transform COG;
    public Vector3 AeroDynamicResistance;

    public ParticleSystem SplashParticles;
    public AudioSource SplashSound;
    public float SplashDelay;
    private float SplashDelayTimer;

    private Controller Controller;
    private Rigidbody RB;
    private List<CarForce> Forces;

    void Start()
    {
        Controller = GetComponent<Controller>();
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
        Frozen();
        SplashParticles.enableEmission = false;
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

        // Apply Tilt
        Vector3 currentFrontTilt = FrontEngines[0].transform.localRotation.eulerAngles;
        Vector3 currentBackTilt = BackEngines[0].transform.localRotation.eulerAngles;
        forwardTilt = Mathf.LerpAngle(currentFrontTilt.x, forwardTilt, EngineTiltSpeed * Time.deltaTime);
        frontSidewaysTilt = Mathf.LerpAngle(currentFrontTilt.z, frontSidewaysTilt, EngineTiltSpeed * Time.deltaTime);
        backSidewaysTilt = Mathf.LerpAngle(currentBackTilt.z, backSidewaysTilt, EngineTiltSpeed * Time.deltaTime);
        for (int i = 0; i < FrontEngines.Length; ++i)
        {
            FrontEngines[i].transform.localRotation = Quaternion.Euler(forwardTilt + Random.value * 2 - 1f, 0f, frontSidewaysTilt + Random.value * 10 - 5f);
        }
        for (int i = 0; i < BackEngines.Length; ++i)
        {
            BackEngines[i].transform.localRotation = Quaternion.Euler(forwardTilt + Random.value * 2 - 1f, 0f, backSidewaysTilt + Random.value * 10 - 5f);
        }

        // Engine Forces
        Forces.Clear();
        LayerMask groundMask = LayerMask.GetMask(new string[] { "Ground", "Water" });
        for (int i = 0; i < AllEngines.Count; ++i)
        {
            Ray ray = new Ray(AllEngines[i].position, -AllEngines[i].up);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10, groundMask))
            {
                Vector3 point = AllEngines[i].position;
                float forceMag = EngineForce * 1f / Mathf.Pow(hit.distance, 1.5f);
                Vector3 force = AllEngines[i].up * forceMag;
                Forces.Add(new CarForce(force, point));

                // Stabilization
                float stabilizationTurn = Controller.GetFacingAxis() * TurnAmount * Time.deltaTime;
                StabilizationVector = Quaternion.AngleAxis(stabilizationTurn, Vector3.up) * StabilizationVector;
                StabilizationNormal = hit.normal;
            }
        }

        Ray groundTouchRay = new Ray(transform.position, -Vector3.up);
        RaycastHit groundHit;
        if (Physics.Raycast(groundTouchRay, out groundHit, 3f, groundMask))
        {
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
            Forces.Add(new CarForce(transform.forward * forwardForce, COG.position));

            float sidewaysForce = SidewaysThrustForce * moveVector.x;
            Forces.Add(new CarForce(transform.right * sidewaysForce, COG.position));

            if (groundHit.collider.tag == "PitWater")
            {
                SplashParticles.enableEmission = true;
                SplashDelayTimer -= Time.deltaTime;
                if (SplashDelayTimer < 0f)
                {
                    SplashSound.Play((ulong)(Random.value * 0.5f));
                    SplashDelayTimer = SplashDelay;
                }
            }
            else
            {
                SplashParticles.enableEmission = false;
            }
        }

        // Aerodynamic Force
        float forwardVelocity = Vector3.Dot(RB.velocity, transform.forward);
        float rightVelocity = Vector3.Dot(RB.velocity, transform.right);
        float upVelocity = Vector3.Dot(RB.velocity, transform.up);
        Vector3 resistance =
            -transform.forward * Mathf.Sign(forwardVelocity) * forwardVelocity * forwardVelocity * AeroDynamicResistance.z +
            -transform.right * Mathf.Sign(rightVelocity) * rightVelocity * rightVelocity * AeroDynamicResistance.x +
            -transform.up * Mathf.Sign(upVelocity) * upVelocity * upVelocity * AeroDynamicResistance.y;
        Forces.Add(new CarForce(resistance, COG.position));
    }

    void FixedUpdate()
    {
        for (int i = 0; i < Forces.Count; ++i)
        {
            RB.AddForceAtPosition(Forces[i].Force, Forces[i].Point, ForceMode.Force);
        }
        Quaternion stable = Quaternion.LookRotation(StabilizationVector, StabilizationNormal);
        Quaternion newRot = Quaternion.Lerp(transform.rotation, stable, StabilizationAmount * Time.fixedDeltaTime);
        RB.MoveRotation(newRot);
    }

    public void Frozen()
    {
        RB.constraints = RigidbodyConstraints.FreezeAll;
        RB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePosition;
    }

    public void Mobile()
    {
        RB.constraints = RigidbodyConstraints.None;
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
