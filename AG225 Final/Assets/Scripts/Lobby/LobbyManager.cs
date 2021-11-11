using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviour
{
    public TMP_Text usernameTemp;

    public TMP_Text userListTemp;

    private void Awake()
    {
        usernameTemp.text = "Your username is " + PlayerNetworkManager.Instance.Username;

        gameObject.GetPhotonView().RPC("ShowPlayerJoin", RpcTarget.AllBuffered, PlayerNetworkManager.Instance.Username);
    }

    [PunRPC]
    public void ShowPlayerJoin(string uName)
    {
        userListTemp.text += "\n" + uName;
    }
}
