using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectSlot : MonoBehaviour
{
    public int id { get; private set; }

    private void Awake()
    {
        id = 4;
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClicked_Slot);
    }

    private void OnEnable()
    {
        Image image = GetComponent<Image>();
        if (image.sprite == null)
        {
            image.sprite = DatabaseManager.Instance.OnGetSpriteData(id);
        }
    }

    public void OnCalled_Add()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        Button button = GetComponent<Button> ();
        button.interactable =false;
    }
    void OnClicked_Slot()
    {
        SearchAddPanel().GetComponent<AddPanel>().OnCalled_UnitSelctSlot(this);
    }

    Transform SearchAddPanel()
    {
        return transform.parent.parent.parent;
    }


}
