using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState_Grounded : JumpState
{
    public override void EnterState()
    {
        MyCharacter.ResetJumps();
    }
    public override void PerformState()
    {
        if(MyCharacter.OwnRigidBody.velocity.y <= -0.05)
        {
            MyCharacter.GetPhotonView().RPC("SetJumpState", Photon.Pun.RpcTarget.AllBufferedViaServer, "falling");
        }
    }

    public override void Jump()
    {
        MyCharacter.DoJump();
        MyCharacter.GetPhotonView().RPC("SetJumpState", Photon.Pun.RpcTarget.AllBufferedViaServer, "liftoff");
    }
}
