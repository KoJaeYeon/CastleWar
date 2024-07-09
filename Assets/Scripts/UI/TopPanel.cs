using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TopPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _Timer;
    void Update()
    {
        float startTime = GameManager.Instance.GameStartTime;
        float elapsedTime = Time.time - startTime;

        float leftTime = 360 - elapsedTime;

        int minute = (int)leftTime/ 6;
        int second = (int)leftTime % 60;

        _Timer.text = $"{minute}:{second}";
    }
}
