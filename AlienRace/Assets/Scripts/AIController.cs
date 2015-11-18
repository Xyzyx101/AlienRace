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

    public LayerMask CapsuleMask;
    public Vector3 CapsulePoint1;
    public Vector3 CapsulePoint2;
    public float CapsuleRadius;
    public float CapsuleDistance;

    public RepairBotSpawn RepairBot;
    public float MaxStuckTime;
    public float StuckTime;
    public float StuckDistanceSqr;
    private Vector3 LastPosition;
    private bool RepairBotComing;

    private Rigidbody RB;
    private bool SlowMode;
    private float SlowTarget;

    // Use this for initialization
    void Start()
    {
        RB = GetComponent<Rigidbody>();
        SlowMode = false;
        RepairBotComing = false;
        LastPosition = transform.position;
        StuckTime = 20f;
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

        // Waypoint Turning
        float wayDirAngle = Mathf.Atan2(wayDir.y, wayDir.x) * Mathf.Rad2Deg;
        float facingAngle = Mathf.Atan2(projDir.y, projDir.x) * Mathf.Rad2Deg;
        float deltaAngle = Mathf.DeltaAngle(wayDirAngle, facingAngle);
        FacingAxis = Mathf.Clamp(deltaAngle, -MaxTurnAngle, MaxTurnAngle) / MaxTurnAngle;

        // Raycast Steering
        float forwardDeceleration = 0f;
        float horizontalMove = 0f;
        RaycastHit hit;
        Vector3 castDir = Quaternion.AngleAxis(-15, transform.right) * transform.forward;
        if (Physics.CapsuleCast(transform.TransformPoint(CapsulePoint1), transform.TransformPoint(CapsulePoint2), CapsuleRadius, castDir, out hit, CapsuleDistance, CapsuleMask, QueryTriggerInteraction.Ignore))
        {
            // Deceleration

            if (hit.distance > 3f)
            {
                forwardDeceleration = 1f - (hit.distance / CapsuleDistance);
            }
            else
            {
                forwardDeceleration = 3f - hit.distance;
            }

            // Move Horizontal
            Vector2 projCarDir = new Vector2(transform.forward.x, transform.forward.z);
            Vector3 hitDir = hit.point - transform.position;
            Vector2 projHitDir = new Vector2(hitDir.x, hitDir.z);
            float hitDirAngle = Mathf.Atan2(projHitDir.y, projHitDir.x) * Mathf.Rad2Deg;
            float hitDeltaAngle = Mathf.DeltaAngle(facingAngle, hitDirAngle);
            horizontalMove = Mathf.Clamp(hitDeltaAngle * 0.2f, -1f, 1f);
        }

        // Turn Correction
        //Vector3 moveDir = RB.velocity - transform.position;
        Vector2 projMoveDir = new Vector2(RB.velocity.x, RB.velocity.z);
        float moveDirAngle = Mathf.Atan2(projMoveDir.y, projMoveDir.x) * Mathf.Rad2Deg;
        float moveDeltaAngle = Mathf.DeltaAngle(facingAngle, moveDirAngle);
        float correctionMove = Mathf.Clamp(moveDeltaAngle / 15f, -1f, 1f);
        Debug.DrawRay(transform.position + 2f * Vector3.up, new Vector3(projDir.x, 0f, projDir.y), Color.cyan, Time.deltaTime);
        Debug.DrawRay(transform.position + 2f * Vector3.up, new Vector3(projMoveDir.x, 0f, projMoveDir.y), Color.yellow, Time.deltaTime);

        float combinedForward = Mathf.Clamp(forwardFactor - forwardDeceleration, -1f, 1f);
        float combinedSideways = Mathf.Clamp(horizontalMove + correctionMove, -1f, 1f);
        MoveVector = new Vector3(combinedSideways, 0f, combinedForward);


        if (SlowMode && Vector3.Dot(RB.velocity, transform.forward) > SlowTarget)
        {
            MoveVector.z = -1f;
        }
        else
        {
            SlowMode = false;
        }
        Debug.DrawRay(transform.position + 2f * Vector3.up, transform.right * FacingAxis, Color.green, Time.deltaTime);
        Debug.DrawRay(transform.position + 2f * Vector3.up, MoveVector, Color.blue, Time.deltaTime);

        // Stuck Timer
        if ((transform.position - LastPosition).sqrMagnitude > StuckDistanceSqr)
        {
            StuckTime = MaxStuckTime;
            LastPosition = transform.position;
            RepairBotComing = false;
        }
        else
        {
            StuckTime -= Time.deltaTime;
            if (StuckTime < 0f && RepairBotComing == false)
            {
                RepairBot.CallStuckWhatsit(gameObject);
                RepairBotComing = true;
            }
        }

        // Ugly Hack to fic falling off the map
        if (transform.position.y < -10f)
        {
            RB.velocity = Vector3.zero;
        }
    }

    public void SlowDown(float TargetSpeed)
    {
        SlowTarget = TargetSpeed;
        SlowMode = true;
    }
}
