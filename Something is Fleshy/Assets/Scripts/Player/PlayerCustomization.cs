using UnityEngine;

public class PlayerCustomization : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteOut = null;
    [SerializeField] SpriteRenderer spriteIn = null;

    public void SetColor(Color color)
    {
        spriteOut.color = color;
        spriteIn.color = color;
    }
}
