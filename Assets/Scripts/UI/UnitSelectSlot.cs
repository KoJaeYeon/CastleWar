using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectSlot : MonoBehaviour
{
    [SerializeField]
    int _id;

    public int Id
    {
        get { return _id; }
        private set { _id = value; }
    }

    private void Awake()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnClicked_Slot);
    }

    private void OnEnable()
    {
        Image image = GetComponent<Image>();
        if (image.sprite == null)
        {
            image.sprite = DatabaseManager.Instance.OnGetSpriteData(_id);
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
