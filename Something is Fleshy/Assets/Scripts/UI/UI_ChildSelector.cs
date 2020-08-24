using UnityEngine;

public class UI_ChildSelector : MonoBehaviour
{
    [Header("REFERENCES")]
#pragma warning disable 0649
    [SerializeField] GameObject[] childs;
#pragma warning restore 0649
    [Header("Variables")]
    [Header("⚠ DON'T TOUCH BELOW ⚠")]
    public int currentIndex;

    public bool UpdateChilds()
    {
        if(currentIndex < childs.Length - 1)
        {
            childs[currentIndex].SetActive(false);
            currentIndex++;
            childs[currentIndex].SetActive(true);
            return false;
        }
        return true;
    }
}
