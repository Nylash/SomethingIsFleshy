using UnityEngine;
using UnityEngine.U2D;

public class InversionBlockAnimMethod : MonoBehaviour
{
    PolygonCollider2D coll;
    SpriteShapeRenderer shapeRenderer;

    private void Start()
    {
        coll = GetComponent<PolygonCollider2D>();
        shapeRenderer = GetComponent<SpriteShapeRenderer>();
    }

    void ActivateCollider()
    {
        coll.enabled = true;
    }

    void ActivateRenderer()
    {
        shapeRenderer.enabled = true;
    }

    void DeactivateColliderAndRenderer()
    {
        shapeRenderer.enabled = false;
        coll.enabled = false;
    }
}
