using UnityEngine;
using System.Collections;

public class AnimatedCookie : MonoBehaviour {
    public float fps = 30.0f;
    public float rotSpeed = 1f;
    public Texture2D[] frames;

    private int frameIndex;
    private Light myLight;

    void Start()
    {
        myLight = GetComponent<Light>();
        NextFrame();
        InvokeRepeating("NextFrame", 1 / fps, 1 / fps);
    }
    void NextFrame()
    {
        myLight.cookie = frames[frameIndex];
        frameIndex = (frameIndex + 1) % frames.Length;
        transform.Rotate(Vector3.up, Time.deltaTime * rotSpeed, Space.World);
    }
}
