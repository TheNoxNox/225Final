using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState_Liftoff : JumpState
{
    private float maxLiftoffDuration = 0.1f;
    private float liftoffDuration = 0f;

    public override void PerformState()
    {
        liftoffDuration += Time.deltaTime;
        if(liftoffDuration >= maxLiftoffDuration)
        {
            MyCharacter.GetPhotonView().RPC("SetJumpState", Photon.Pun.RpcTarget.AllBufferedViaServer, "jumping");
        }
    }
}
