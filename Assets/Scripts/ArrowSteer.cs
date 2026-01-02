using UnityEngine;

public class ArrowSteer : MonoBehaviour
{
    public float steerValue { get; private set; } = 0f;

    public void LeftDown() { steerValue = -1f; }
    public void RightDown() { steerValue = 1f; }
    public void Up() { steerValue = 0f; }
}
