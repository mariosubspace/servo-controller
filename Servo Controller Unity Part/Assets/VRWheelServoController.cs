using UnityEngine;

public class VRWheelServoController : MonoBehaviour
{
    public ServoClient client;
    public float cooldownSeconds = 0.01f;
    Vector3 initialForwardDirection;

    float lastCommandTime;

    void Start()
    {
        initialForwardDirection = transform.TransformDirection(transform.forward);
    }
	
	void Update()
    {
        ChangeAngle(Vector3.SignedAngle(initialForwardDirection, transform.TransformDirection(transform.forward), transform.TransformDirection(Vector3.up)));
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
