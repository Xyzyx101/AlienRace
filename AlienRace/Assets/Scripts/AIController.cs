using UnityEngine;
using System.Collections;

public class AIController : Controller
{
    public Waypoints WaypointScript;
    public int LastWaypoint = 0;

    public float VelocityFactor = 1f;
    public float FullSpeedAngle;
    public float FullBackAngle;
    public float MaxTurnAngle;

    private Rigidbody RB;

    // Use this for initialization
    void Start()
    {
        RB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Acceleration
        Vector2 projPos = new Vector2(transform.position.x, transform.position.z);
        Vector2 wayDir = WaypointScript.GetNearestPoint(ref LastWaypoint, transform.position + RB.velocity * VelocityFactor) - projPos;
        Debug.DrawRay(transform.position + Vector3.up * 2f, new Vector3(wayDir.x, 0f, wayDir.y), Color.red, Time.deltaTime);
        Vector2 projDir = new Vector2(transform.forward.x, transform.forward.z);
        float angle = Vector2.Angle(projDir, wayDir);
        float forwardFactor;
        forwardFactor = Mathf.Clamp(1 - 2 * (angle / (FullBackAngle - FullSpeedAngle)), -1f, 1f);
        MoveVector = new Vector3(0f, 0f, forwardFactor);

        // Turning
        float wayDirAngle = Mathf.Atan2(wayDir.y, wayDir.x) * Mathf.Rad2Deg;
        float facingAngle = Mathf.Atan2(projDir.y, projDir.x) * Mathf.Rad2Deg;
        float deltaAngle = Mathf.DeltaAngle(wayDirAngle, facingAngle);
        FacingAxis = Mathf.Clamp(deltaAngle, -MaxTurnAngle, MaxTurnAngle) / MaxTurnAngle;
    }
}

// MoveVector
// FacingAxis