using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState_Jumping : JumpState
{
    public override void Jump()
    {
        MyCharacter.DoJump();
        MyCharacter.GetPhotonView().RPC("SetJumpState", Photon.Pun.RpcTarget.AllBufferedViaServer, "jumping");
    }
}
