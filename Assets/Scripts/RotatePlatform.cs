using UnityEngine;

public class RotatePlatform : MonoBehaviour
{
    public float secondsPerRotation = 10f; // <- klasyczny „salon samochodowy”

    void Update()
    {
        float speed = 360f / secondsPerRotation;
        transform.Rotate(0f, speed * Time.deltaTime, 0f);
    }
}
