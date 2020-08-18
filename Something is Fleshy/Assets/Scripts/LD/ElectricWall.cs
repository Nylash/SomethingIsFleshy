using UnityEngine.U2D;
using UnityEngine;

public class ElectricWall : MonoBehaviour
{
    #region CONFIGURATION
#pragma warning disable 0649
    [Header("PARAMETERS")]
    [SerializeField] bool updateWhilePlaying;
    [SerializeField] GameObject emitter1;
    [SerializeField] GameObject emitter2;
    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    [SerializeField] GameObject emitterPrefab;
    [SerializeField] Material lineMaterial;
#pragma warning restore 0649
    #endregion
    SpriteRenderer startRenderer;
    SpriteRenderer endRenderer;
    BoxCollider2D startCollider;
    BoxCollider2D endCollider;
    LineRenderer line;
    BoxCollider2D lineCollider;

    private void Awake()
    {
        startRenderer = emitter1.GetComponent<SpriteRenderer>();
        endRenderer = emitter2.GetComponent<SpriteRenderer>();
        startCollider = emitter1.GetComponent<BoxCollider2D>();
        endCollider = emitter2.GetComponent<BoxCollider2D>();
        startRenderer.color = GetComponent<SpriteShapeRenderer>().color;
        endRenderer.color = GetComponent<SpriteShapeRenderer>().color;

        line = new GameObject("Line").AddComponent<LineRenderer>();
        line.material = lineMaterial;
        line.startWidth = .1f;
        line.endWidth = .1f;
        line.SetPosition(0, emitter1.transform.position);
        line.SetPosition(1, emitter2.transform.position);
        line.startColor = startRenderer.color;
        line.endColor = endRenderer.color;
        line.sortingLayerName = startRenderer.sortingLayerName;
        line.sortingOrder = startRenderer.sortingOrder;
        line.gameObject.tag = "ElectricPlatform";
        lineCollider = line.gameObject.AddComponent<BoxCollider2D>();

        lineCollider.size = new Vector3(Vector3.Distance(emitter1.transform.position, emitter2.transform.position), .1f, 0);
        lineCollider.transform.position = (line.GetPosition(0) + line.GetPosition(1)) / 2;
        lineCollider.transform.rotation = Quaternion.FromToRotation(Vector3.right, (emitter2.transform.position - emitter1.transform.position).normalized);
        lineCollider.offset = Vector2.zero;

        float angleStart = Vector2.SignedAngle(emitter1.transform.up, (emitter2.transform.position - emitter1.transform.position).normalized);
        emitter1.transform.Rotate(new Vector3(0, 0, angleStart));
        float angleEnd = Vector2.SignedAngle(emitter2.transform.up, (emitter1.transform.position - emitter2.transform.position).normalized);
        emitter2.transform.Rotate(new Vector3(0, 0, angleEnd));

    }

    private void Update()
    {
        if (updateWhilePlaying)
        {
            line.SetPosition(0, emitter1.transform.position);
            line.SetPosition(1, emitter2.transform.position);
            lineCollider.size = new Vector3(Vector3.Distance(emitter1.transform.position, emitter2.transform.position), .1f, 0);
            lineCollider.transform.position = (line.GetPosition(0) + line.GetPosition(1)) / 2;
            lineCollider.transform.rotation = Quaternion.FromToRotation(Vector3.right, (emitter2.transform.position - emitter1.transform.position).normalized);
            lineCollider.offset = Vector2.zero;
            float angleStart = Vector2.SignedAngle(emitter1.transform.up, (emitter2.transform.position - emitter1.transform.position).normalized);
            emitter1.transform.Rotate(new Vector3(0, 0, angleStart));
            float angleEnd = Vector2.SignedAngle(emitter2.transform.up, (emitter1.transform.position - emitter2.transform.position).normalized);
            emitter2.transform.Rotate(new Vector3(0, 0, angleEnd));
        }
    }

    public void Activate()
    {
        gameObject.tag = "ElectricPlatform";
        emitter1.tag = "ElectricPlatform";
        emitter2.tag = "ElectricPlatform";
        line.gameObject.tag = "ElectricPlatform";
        line.enabled = true;
        startCollider.enabled = true;
        endCollider.enabled = true;
        lineCollider.enabled = true;
    }

    public void Desactivate()
    {
        gameObject.tag = "Untagged";
        emitter1.tag = "Untagged";
        emitter2.tag = "Untagged";
        line.gameObject.tag = "Untagged";
        line.enabled = false;
        startCollider.enabled = false;
        endCollider.enabled = false;
        lineCollider.enabled = false;
    }

    public void SpawnEmitter()
    {
        Instantiate(emitterPrefab, transform.position, Quaternion.identity);
    }
}
