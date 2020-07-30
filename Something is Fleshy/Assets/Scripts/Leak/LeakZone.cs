using UnityEngine;

public class LeakZone : MonoBehaviour
{
    Vector3 pos0;
    Vector3 pos1;
    Bounds leakZoneBounds;

    private void Start()
    {
        pos0 = transform.GetChild(0).transform.position;
        pos1 = transform.GetChild(1).transform.position;
        leakZoneBounds.SetMinMax(pos0, pos1);
    }

    public Vector3 GetRandomLeakPosition()
    {
        return new Vector3(Random.Range(pos0.x, pos1.x), Random.Range(pos0.y, pos1.y), 0);
    }

    private void OnDrawGizmos()
    {
        pos0 = transform.GetChild(0).transform.position;
        pos1 = transform.GetChild(1).transform.position;
        leakZoneBounds.SetMinMax(pos0, pos1);
        DrawBounds(leakZoneBounds);
    }

    void DrawBounds(Bounds b, float delay = 0)
    {
        // bottom
        var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
        var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
        var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
        var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

        Debug.DrawLine(p1, p2, Color.red, delay);
        Debug.DrawLine(p2, p3, Color.red, delay);
        Debug.DrawLine(p3, p4, Color.red, delay);
        Debug.DrawLine(p4, p1, Color.red, delay);

        // top
        var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
        var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
        var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
        var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

        Debug.DrawLine(p5, p6, Color.red, delay);
        Debug.DrawLine(p6, p7, Color.red, delay);
        Debug.DrawLine(p7, p8, Color.red, delay);
        Debug.DrawLine(p8, p5, Color.red, delay);

        // sides
        Debug.DrawLine(p1, p5, Color.red, delay);
        Debug.DrawLine(p2, p6, Color.red, delay);
        Debug.DrawLine(p3, p7, Color.red, delay);
        Debug.DrawLine(p4, p8, Color.red, delay);
    }
}
