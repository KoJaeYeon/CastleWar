using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectSlot : MonoBehaviour
{
    [SerializeField] int id;
    Image image;
    Button button;

    private void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if(image.sprite == null)
        {
            image.sprite = DatabaseManager.Instance.GetSpriteData(id);
        }
    }
}
