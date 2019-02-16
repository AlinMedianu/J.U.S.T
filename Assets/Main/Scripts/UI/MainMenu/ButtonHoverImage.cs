using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite defaultImage;
    public Sprite hoverImage;

    public void OnPointerEnter(PointerEventData eventData)
    {

        GetComponent<Image>().sprite = hoverImage;
		GetComponent<Button>().targetGraphic = GetComponent<Image>();
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        GetComponent<Image>().sprite = defaultImage;
		GetComponent<Button>().targetGraphic = GetComponent<Image>();
    }
}
