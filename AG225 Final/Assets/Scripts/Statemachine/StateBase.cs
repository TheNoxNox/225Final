using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class StateBase : MonoBehaviour
{
    public CharacterStateMachine MyStateMachine => gameObject.GetComponent<CharacterStateMachine>();

    public Character MyCharacter => MyStateMachine.MyCharacter;

    public virtual void EnterState()
    {

    }

    public virtual void PerformState()
    {

    }

    public virtual void ExitState()
    {

    }
}
