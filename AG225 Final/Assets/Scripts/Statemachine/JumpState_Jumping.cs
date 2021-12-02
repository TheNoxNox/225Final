using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState_Jumping : JumpState
{
    public override void PerformState()
    {
        if (MyCharacter.IsTouchingGround)
        {
            MyCharacter.GetPhotonView().RPC("SetJumpState", Photon.Pun.RpcTarget.AllBufferedViaServer, "grounded");
        }
        else if (MyCharacter.OwnRigidBody.velocity.y <= -0.05)
        {
            MyCharacter.GetPhotonView().RPC("SetJumpState", Photon.Pun.RpcTarget.AllBufferedViaServer, "falling");
        }
    }
    public override void Jump()
    {
        MyCharacter.DoJump();
        MyCharacter.GetPhotonView().RPC("SetJumpState", Photon.Pun.RpcTarget.AllBufferedViaServer, "jumping");
    }
}
