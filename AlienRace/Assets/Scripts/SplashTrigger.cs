using UnityEngine;
using System.Collections;

public class SplashTrigger : MonoBehaviour {

    public GameObject SplashPrefab;

	void OnTriggerEnter(Collider other)
    {
        Instantiate(SplashPrefab, other.transform.position, Quaternion.identity);
    }
}
