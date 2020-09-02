using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PopUpCanvas : MonoBehaviour
{
    public GameObject firstSelected;

    Canvas canvas;
    public List<Button> childs = new List<Button>();

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        foreach (Button item in GetComponentsInChildren<Button>())
            childs.Add(item);
    }

    private void Update()
    {
        if (canvas.enabled)
        {
            if (!childs[0].enabled)
            {
                foreach (Button item in childs)
                {
                    item.enabled = true;
                }
                EventSystem.current.SetSelectedGameObject(firstSelected);
            }
        }
        else
        {
            if (childs[0].enabled)
            {
                foreach (Button item in childs)
                {
                    item.enabled = false;
                }
            }
        }
    }
}
