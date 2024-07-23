using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _Timer;
    [SerializeField] EnemyUnitSlot[] enemyUnitSlot;
    void Update()
    {
        float startTime = GameManager.Instance.GameStartTime;
        float elapsedTime = Time.time - startTime;

        float leftTime = 360 - elapsedTime;

        int minute = (int)leftTime / 60;
        int second = (int)leftTime % 60;

        _Timer.text = $"{minute}:{second.ToString("00")}";
    }

    public void OnAddUnitId(int index, int unitId)
    {
        index %= 8;
        enemyUnitSlot[index].Id =unitId;
    }

    public void OnActivateSprite(int index)
    {
        index %= 8;
        enemyUnitSlot[index].OnAcitvateSprite();
    }
}
