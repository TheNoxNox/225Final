using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MatchStats : MonoBehaviour
{
    [SerializeField]
    protected float GameTimeMax = 60f;

    private float currentGameTime = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameInstance.Instance.IsHost)
        {
            this.GetPhotonView().RPC("UpdateTime", RpcTarget.AllBufferedViaServer, currentGameTime + Time.deltaTime);
        }
    }

    [PunRPC]
    public void UpdateTime(float newTime)
    {
        currentGameTime = newTime;
    }
}
