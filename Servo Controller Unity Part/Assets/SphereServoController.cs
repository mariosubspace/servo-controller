using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereServoController : MonoBehaviour
{
    public ServoClient client;
    public float cooldownSeconds = 0.01f;
    Vector3 initialForwardDirection;

    float lastCommandTime;

    void Start()
    {
        initialForwardDirection = transform.forward;
    }
	
	void Update()
    {
        ChangeAngle(Vector3.SignedAngle(initialForwardDirection, transform.forward, Vector3.up));
	}

    void ChangeAngle(float angleDeviation)
    {
        float currentTime = Time.time;
        float elapsedTime = currentTime - lastCommandTime;
        if (elapsedTime > cooldownSeconds)
        {
            int remappedAngle = (int)Mathf.Clamp(158.5f - angleDeviation, 67f, 250f);
            lastCommandTime = currentTime;
            client.SetAngle(remappedAngle);
        }
    }
}
