using UnityEngine;
using System.Collections;

public class StuckWhatsit : MonoBehaviour
{

    public float Speed;
    public GameObject Target;
    public float TotalDistanceSqr;
    public Vector3 DropOffPoint;
    private Car TargetCar;
    private bool GotCar;
    private bool JobDone;
    private Waypoints WaypointScript;

    // Use this for initialization
    void Start()
    {
        // FIXME Set target should be called from another script
        SetTarget(Target);
        GotCar = false;
        GameObject WaypointObject = GameObject.Find("Waypoints");
        WaypointScript = WaypointObject.GetComponent<Waypoints>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move To Car
        if (!GotCar)
        {
            transform.position = Vector3.Lerp(transform.position, Target.transform.position, Speed * Time.deltaTime);
            float scale = 1 - (Target.transform.position - transform.position).sqrMagnitude / TotalDistanceSqr;
            transform.localScale = new Vector3(scale, scale, scale);
            transform.rotation = Quaternion.Euler(0f, Target.transform.rotation.eulerAngles.y, 0f);
            return;
        }
        transform.position = Vector3.Lerp(transform.position, DropOffPoint, Speed * Time.deltaTime);

        if (TargetCar == null) // Job Complete
        {
            transform.localScale = transform.localScale * 0.9f;
            if (transform.localScale.x < 0.1f)
            {
                Destroy(gameObject);
            }
        }
        else // Drop Car
        {
            if ((DropOffPoint - transform.position).sqrMagnitude < 0.1f)
            {
                TargetCar.Mobile();
                TargetCar.transform.parent = null;
                TargetCar = null;
            }
        }
    }

    public void SetTarget(GameObject targetObject)
    {
        Target = targetObject;
        TotalDistanceSqr = (Target.transform.position - transform.position).sqrMagnitude;
        TargetCar = Target.GetComponent<Car>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != Target.tag)
        {
            return;
        }
        TargetCar.Frozen();
        TargetCar.transform.parent = transform;
        Target.transform.localPosition = new Vector3(0f, 0f, 0f);
        GotCar = true;
        // The right and forward are because the car gets stuck with 0 velocity right at a waypoint
        DropOffPoint = WaypointScript.GetDropOffPoint(transform.position) + Vector3.up * 3f + Vector3.forward + Vector3.right;
    }
}
