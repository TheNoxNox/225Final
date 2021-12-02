using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState_Falling : JumpState
{
    public override void PerformState()
    {
        if (MyCharacter.IsTouchingGround)
        {
            MyCharacter.GetPhotonView().RPC("SetJumpState", Photon.Pun.RpcTarget.AllBufferedViaServer, "grounded");
        }
    }

    public override void Jump()
    {
        MyCharacter.DoJump();
        MyCharacter.GetPhotonView().RPC("SetJumpState", Photon.Pun.RpcTarget.AllBufferedViaServer, "jumping");
    }
}
