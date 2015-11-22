using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Speedometer : MonoBehaviour
{
    public Image Marker;
    public Text SpeedText;
    public float MarkerLerpSpeed = 3f;

    private Rigidbody RB;

    void Start()
    {
        RB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // The displayed velocity is bullshit.  This math is just o make it look good.
        float velocity = Vector3.Dot(RB.velocity, transform.forward);
        if (velocity > 0)
        {
            velocity = Mathf.Pow(velocity * 4f, 1.1f);
        }
        else
        {
            velocity *= 3f;
        }
        SpeedText.text = Mathf.Floor(velocity).ToString() + " km/h";

        const float minAngle = 270f;
        const float maxAngle = -90f;
        const float maxSpeedo = 300f;
        float velFraction = Mathf.Clamp01(velocity / maxSpeedo);

        Color color = Color.Lerp(Color.green, Color.red, velFraction);
        Marker.color = color;
        SpeedText.color = color;

        velFraction = -(1f / (5f * (velFraction + 0.2f))) + 1f;

        const float minScale = 0.05f;
        const float maxScale = 1.35f;
        float markerScale = (maxScale - minScale) * velFraction + minScale;
        Marker.rectTransform.localScale = Vector3.Lerp(Marker.rectTransform.localScale, new Vector3(markerScale * 2, markerScale, 1f), Time.deltaTime * MarkerLerpSpeed);

        float markerRot = (maxAngle - minAngle) * velFraction + minAngle;
        Marker.rectTransform.localRotation = Quaternion.Lerp(Marker.rectTransform.localRotation, Quaternion.Euler(0f, 0f, markerRot), Time.deltaTime * MarkerLerpSpeed);


    }
}
