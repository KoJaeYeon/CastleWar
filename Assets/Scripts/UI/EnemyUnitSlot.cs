using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUnitSlot : MonoBehaviour
{
    public int Id { get; set; }
    bool acitvated = false;

    public void OnAcitvateSprite()
    {
        if (acitvated) return;
        var image = GetComponent<Image>();
        if(image != null )
        {
            image.sprite = DatabaseManager.Instance.OnGetSpriteData(Id);
            acitvated = true;
        }
    }
}
