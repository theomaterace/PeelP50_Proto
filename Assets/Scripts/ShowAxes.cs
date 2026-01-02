using UnityEngine;
public class ShowAxes : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Vector3 p = transform.position + Vector3.up * 0.3f;
        Gizmos.color = Color.red; Gizmos.DrawLine(p, p + transform.right * 0.7f);   // X+
        Gizmos.color = Color.green; Gizmos.DrawLine(p, p + transform.up * 0.7f);      // Y+
        Gizmos.color = Color.blue; Gizmos.DrawLine(p, p + transform.forward * 1.2f); // Z+ (przód)
    }
}
