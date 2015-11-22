using UnityEngine;
using System.Collections;

public class HumanController : Controller
{
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

    void Start()
    {
        RB = GetComponent<Rigidbody>();
        LastPosition = transform.position;
        StuckTime = 20f;
    }

    // Update is called once per frame
    void Update()
    {
        FacingAxis = Input.GetAxis("Facing");
        MoveVector = Input.GetAxis("Vertical") * Vector3.forward + Input.GetAxis("Horizontal") * Vector3.right;

        //RaycastHit hit;
        //Vector3 castDir = Quaternion.AngleAxis(-15, transform.right) * transform.forward;
        //if (Physics.CapsuleCast(transform.TransformPoint(CapsulePoint1), transform.TransformPoint(CapsulePoint2), CapsuleRadius, castDir, out hit, CapsuleDistance, CapsuleMask, QueryTriggerInteraction.Ignore))
        //{
        //    Debug.DrawLine(transform.position + 2f * Vector3.up, hit.point, Color.black, Time.deltaTime);
        //    // Deceleration
        //    float forwardDeceleration = 0f;
        //    if (hit.distance > 3f)
        //    {
        //        forwardDeceleration = 1f - (hit.distance / CapsuleDistance);
        //    }
        //    else
        //    {
        //        forwardDeceleration = 3f - hit.distance;
        //    }

        //    // Move Horizontal
        //    Vector2 projCarDir = new Vector2(transform.forward.x, transform.forward.z);
        //    float facingAngle = Mathf.Atan2(projCarDir.y, projCarDir.x) * Mathf.Rad2Deg;
        //    Vector3 hitDir = hit.point - transform.position;
        //    Vector2 projHitDir = new Vector2(hitDir.x, hitDir.z);
        //    float hitDirAngle = Mathf.Atan2(projHitDir.y, projHitDir.x) * Mathf.Rad2Deg;
        //    float deltaAngle = Mathf.DeltaAngle(facingAngle, hitDirAngle);
        //    float horizontalMove = Mathf.Clamp(deltaAngle * 0.2f, -1f, 1f);

        //    Debug.DrawRay(transform.position + 2f * Vector3.up, transform.forward * forwardDeceleration + transform.right * horizontalMove, Color.blue, Time.deltaTime);
        //}

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

        // Ugly Hack to fix falling off the map
        if (transform.position.y < -10f)
        {
            RB.velocity = Vector3.zero;
        }

    }
}
