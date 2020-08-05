using UnityEngine.U2D;
using UnityEngine;

public class ElectricWall : MonoBehaviour
{
    #region CONFIGURATION
#pragma warning disable 0649
    [Header("PARAMETERS")]
    [SerializeField] bool updateWhilePlaying;
    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    [SerializeField] GameObject start;
    [SerializeField] GameObject end;
    [SerializeField] Material lineMaterial;
#pragma warning restore 0649
    #endregion
    SpriteRenderer startRenderer;
    SpriteRenderer endRenderer;
    BoxCollider2D startCollider;
    BoxCollider2D endCollider;
    LineRenderer line;
    BoxCollider2D lineCollider;

    private void Start()
    {
        startRenderer = start.GetComponent<SpriteRenderer>();
        endRenderer = end.GetComponent<SpriteRenderer>();
        startCollider = start.GetComponent<BoxCollider2D>();
        endCollider = end.GetComponent<BoxCollider2D>();
        startRenderer.color = GetComponent<SpriteShapeRenderer>().color;
        endRenderer.color = GetComponent<SpriteShapeRenderer>().color;

        line = new GameObject("Line").AddComponent<LineRenderer>();
        line.material = lineMaterial;
        line.startWidth = .1f;
        line.endWidth = .1f;
        line.SetPosition(0, start.transform.position);
        line.SetPosition(1, end.transform.position);
        line.startColor = startRenderer.color;
        line.endColor = endRenderer.color;
        line.sortingLayerName = startRenderer.sortingLayerName;
        line.sortingOrder = startRenderer.sortingOrder;
        lineCollider = line.gameObject.AddComponent<BoxCollider2D>();

        lineCollider.size = new Vector3(Vector3.Distance(start.transform.position, end.transform.position), .1f, 0);
        lineCollider.transform.position = (line.GetPosition(0) + line.GetPosition(1)) / 2;
        lineCollider.transform.rotation = Quaternion.FromToRotation(Vector3.right, (end.transform.position - start.transform.position).normalized);
        lineCollider.offset = Vector2.zero;
    }

    public void Activate()
    {
        gameObject.tag = "ElectricPlatform";
        start.tag = "ElectricPlatform";
        end.tag = "ElectricPlatform";
        line.gameObject.tag = "ElectricPlatform";
        line.enabled = true;
        startCollider.enabled = true;
        endCollider.enabled = true;
        lineCollider.enabled = true;
    }

    public void Desactivate()
    {
        gameObject.tag = "Untagged";
        start.tag = "Untagged";
        end.tag = "Untagged";
        line.gameObject.tag = "Untagged";
        line.enabled = false;
        startCollider.enabled = false;
        endCollider.enabled = false;
        lineCollider.enabled = false;
    }
}
