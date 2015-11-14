using UnityEngine;
using System.Collections;

public class SlowTrigger : MonoBehaviour {
    public float SlowTarget;
   void OnTriggerEnter(Collider other)
    {
        AIController controller = other.gameObject.GetComponent<AIController>();
        if(controller!=null) {
            controller.SlowDown(SlowTarget);
        }
    }
}
