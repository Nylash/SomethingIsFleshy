using UnityEngine.U2D;
using UnityEngine;

public class LeakMask : MonoBehaviour
{
    SpriteShapeController shape;
    float initialSize;

    void Start()
    {
        shape = transform.parent.transform.parent.GetComponent<SpriteShapeController>();
        initialSize = transform.localScale.y;
    }

    void Update()
    {
        if(shape.spriteShape == GameManager.instance.pipeOpenShape)
        {
            if (transform.localScale.y != initialSize)
                transform.localScale = new Vector3(transform.localScale.x, initialSize, transform.localScale.z);
        }
        else
        {
            if (transform.localScale.y == initialSize)
                transform.localScale = new Vector3(transform.localScale.x, initialSize / 1.65f, transform.localScale.z);
        }
    }
}
