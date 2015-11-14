using UnityEngine;
using System.Collections;

public class SplashTrigger : MonoBehaviour {

    public GameObject SplashPrefab;

	void OnTriggerEnter(Collider other)
    {
        Debug.Log("Splash" + other.transform.position);
        Instantiate(SplashPrefab, other.transform.position, Quaternion.identity);
    }
}
