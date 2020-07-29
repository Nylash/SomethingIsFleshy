using UnityEngine;
using UnityEngine.U2D;

public class InversionBlockAnimMethod : MonoBehaviour
{
    PolygonCollider2D coll;
    SpriteShapeRenderer shapeRenderer;
    Animator anim;

    private void Start()
    {
        coll = GetComponent<PolygonCollider2D>();
        shapeRenderer = GetComponent<SpriteShapeRenderer>();
        anim = GetComponent<Animator>();
    }

    void ActivateCollider()
    {
        anim.SetBool("PlayerIn", false);
        coll.isTrigger = false;
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
        coll.isTrigger = false;
        anim.SetBool("PlayerIn", false);
    }

    private void Update()
    {
        if (coll.isActiveAndEnabled)
        {
            if (coll.bounds.Contains(CharacterController2D.instance.transform.position))
            {
                if (!coll.isTrigger)
                {
                    coll.isTrigger = true;
                    anim.SetBool("PlayerIn", true);
                }
            }
            else
            {
                if (coll.isTrigger)
                {
                    coll.isTrigger = false;
                    anim.SetBool("PlayerIn", false);
                }
            } 
        }
    }
}
