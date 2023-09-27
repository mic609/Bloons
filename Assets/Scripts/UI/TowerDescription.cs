using UnityEngine;
using UnityEngine.EventSystems;

public class TowerDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject fieldToShow;
    public void OnPointerEnter(PointerEventData eventData)
    {
        fieldToShow.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        fieldToShow.SetActive(false);
    }
}
