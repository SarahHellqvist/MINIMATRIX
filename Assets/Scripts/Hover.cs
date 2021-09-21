using UnityEngine;
using UnityEngine.EventSystems;

public class Hover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject go;

    private void Start()
    {
        go.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        go.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        go.SetActive(false);
    }
}
