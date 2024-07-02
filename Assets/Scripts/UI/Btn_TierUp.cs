using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Btn_TierUp : MonoBehaviour
{
    Button button;
    [SerializeField] GameObject manaMask;
    [SerializeField] GameObject tierUpMask;
    [SerializeField] TextMeshProUGUI LeftTime;
    Image Img_tierUpMask;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick_TierUp);
        GameManager.Instance.RegisterTierChangeCallback(TierChange);

        Img_tierUpMask = tierUpMask.GetComponent<Image>();
    }

    private void Update()
    {
        if(tierUpMask.activeSelf)
        {
            Img_tierUpMask.fillAmount = 1 - GameManager.Instance.TierUpLeftTime;
            LeftTime.text = Mathf.Ceil(60 -(GameManager.Instance.TierUpLeftTime * 60f)).ToString();
        }
    }

    void OnClick_TierUp()
    {
        TouchManager.Instance.ChangeSelectalble(null);
        if (GameManager.Instance.RequestTierUpCheck())
        {
            button.interactable = false;
            manaMask.SetActive(false);
            tierUpMask.SetActive(true);
            int needMana = GameManager.Instance.getTierUpNeedMana();
            GameManager.Instance.RequestManaUse(-1 * needMana);
            CastleManager.Instance.AllyCastleTierUp();
        }
    }

    void TierChange(int tier)
    {
        button.interactable = true;
        manaMask.SetActive(true);
        tierUpMask.SetActive(false);
    }
}
