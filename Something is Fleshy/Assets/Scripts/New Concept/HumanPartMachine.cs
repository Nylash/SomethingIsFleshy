using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HumanPartMachine : MonoBehaviour
{
    public HumanPart whatToProduces;
    public List<AlchemicalMachine.Ressource> recipe = new List<AlchemicalMachine.Ressource>();
    public Text caca;
    [Space]
    List<AlchemicalMachine.Ressource> currentRecipe = new List<AlchemicalMachine.Ressource>();

    private void Start()
    {
        currentRecipe.AddRange(recipe);
        
    }

    private void Update()
    {
        caca.text = "";
        foreach (var item in currentRecipe)
        {
            caca.text += item + " ";
        }
        caca.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0,2,0));
    }

    public void Interact(AlchemicalMachine.Ressource ressource)
    {
        if (currentRecipe.Contains(ressource))
        {
            currentRecipe.Remove(ressource);
            if (currentRecipe.Count == 0)
            {
                print(whatToProduces);
                currentRecipe.AddRange(recipe);
            }
        }
        else
        {
            print("Cette ressource n'est pas nécessaire, faire perdre du temps sur le timer ?");
        }
    }

    public enum HumanPart
    {
        head, arm, leg
    }
}
