using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class buttonFeedback : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject buttonFrameOn;
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonFrameOn.SetActive(!buttonFrameOn.activeSelf);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonFrameOn.SetActive(!buttonFrameOn.activeSelf);
    }
}