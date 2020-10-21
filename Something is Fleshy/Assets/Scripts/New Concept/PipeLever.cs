using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PipeLever : MonoBehaviour
{
    public Pipe pipe;
    public BasicRessource ressource;

    public bool isOpen;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        pipe.shapeRenderer = pipe.pipeObject.GetComponent<SpriteShapeRenderer>();
        pipe.shapeController = pipe.pipeObject.GetComponent<SpriteShapeController>();
    }

    private void Start()
    {
        pipe.shapeRenderer.color = Color.gray;
        pipe.shapeController.spriteShape = GameManager.instance.pipeCloseShape;
    }

    public void Interact()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            switch (ressource)
            {
                case BasicRessource.red:
                    AlchemicalMachine.instance.redOn = true;
                    pipe.shapeRenderer.color = Color.red;
                    break;
                case BasicRessource.yellow:
                    AlchemicalMachine.instance.yellowOn = true;
                    pipe.shapeRenderer.color = Color.yellow;
                    break;
                case BasicRessource.blue:
                    AlchemicalMachine.instance.blueOn = true;
                    pipe.shapeRenderer.color = Color.blue;
                    break;
            }
            pipe.shapeController.spriteShape = GameManager.instance.pipeOpenShape;
            animator.SetTrigger("ToRight");
        }
        else
        {
            switch (ressource)
            {
                case BasicRessource.red:
                    AlchemicalMachine.instance.redOn = false;
                    break;
                case BasicRessource.yellow:
                    AlchemicalMachine.instance.yellowOn = false;
                    break;
                case BasicRessource.blue:
                    AlchemicalMachine.instance.blueOn = false;
                    break;
            }
            pipe.shapeRenderer.color = Color.gray;
            pipe.shapeController.spriteShape = GameManager.instance.pipeCloseShape;
            animator.SetTrigger("ToLeft");
        }
    }

    public enum BasicRessource
    {
        red, yellow, blue
    }

    [System.Serializable]
    public struct Pipe
    {
        public GameObject pipeObject;
        [HideInInspector]
        public SpriteShapeRenderer shapeRenderer;
        [HideInInspector]
        public SpriteShapeController shapeController;
    }
}
