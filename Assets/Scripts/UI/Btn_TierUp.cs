using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Btn_TierUp : MonoBehaviour
{
    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick_TierUp);
        GameManager.Instance.RegisterTierChangeCallback(TierChange);
    }

    void OnClick_TierUp()
    {
        if (GameManager.Instance.RequestTierUpCheck())
        {
            button.interactable = false;
        }
    }

    void TierChange(int tier)
    {
        button.interactable = true;
    }
}
