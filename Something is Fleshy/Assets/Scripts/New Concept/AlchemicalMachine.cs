using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemicalMachine : MonoBehaviour
{
    public static AlchemicalMachine instance;

    public bool redOn;
    public bool yellowOn;
    public bool blueOn;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(instance.gameObject);
    }

    public Ressource GetRessource()
    {
        if (redOn)
        {
            if (yellowOn)
            {
                if (blueOn)
                    return Ressource.black;
                return Ressource.orange;
            }
            if (blueOn)
                return Ressource.purple;
            return Ressource.red;
        }
        if (yellowOn)
        {
            if (blueOn)
                return Ressource.green;
            return Ressource.yellow;
        }
        if (blueOn)
            return Ressource.blue;
        return Ressource.none;
    }

    public Color RessourceToColor(Ressource ressource)
    {
        switch (ressource)
        {
            case Ressource.red:
                return Color.red;
            case Ressource.yellow:
                return Color.yellow;
            case Ressource.blue:
                return Color.blue;
            case Ressource.orange:
                return new Color(1, .67f, 0);
            case Ressource.green:
                return Color.green;
            case Ressource.purple:
                return new Color(.8f, .35f, .73f);
            case Ressource.black:
                return Color.black;
        }
        return Color.white;
    }

    public enum Ressource
    {
        none, red, yellow, blue, orange, green, purple, black
    }
}
