using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCanvas : MonoBehaviour
{
    public void OnClick_MatchGame()
    {
        TcpSender.Instance.OnClick_SceneLoad();
    }
}
