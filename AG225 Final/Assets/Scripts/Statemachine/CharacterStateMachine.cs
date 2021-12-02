using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateMachine : MonoBehaviour
{
    private void Awake()
    {
        
    }

    public JumpState currentJumpState;

    public MoveState currentMoveState;

    #region State Storage

    #region Jump States
    public JumpState_Grounded Jump_Grounded => gameObject.GetComponent<JumpState_Grounded>() ?? gameObject.AddComponent<JumpState_Grounded>();
    public JumpState_Jumping Jump_Jumping => gameObject.GetComponent<JumpState_Jumping>() ?? gameObject.AddComponent<JumpState_Jumping>();
    public JumpState_Landing Jump_Landing => gameObject.GetComponent<JumpState_Landing>() ?? gameObject.AddComponent<JumpState_Landing>();
    public JumpState_Liftoff Jump_Liftoff => gameObject.GetComponent<JumpState_Liftoff>() ?? gameObject.AddComponent<JumpState_Liftoff>();
    public JumpState_Falling Jump_Falling => gameObject.GetComponent<JumpState_Falling>() ?? gameObject.AddComponent<JumpState_Falling>();
    #endregion


    #region Movement States

    #endregion

    #endregion

    private void Update()
    {
        currentJumpState.PerformState();
    }

    public void ChangeJumpState(JumpState jState)
    {
        if (currentJumpState)
        {
            currentJumpState.ExitState();
        }

        if (jState)
        {
            currentJumpState = jState;

            currentJumpState.EnterState();
        }       
    }
}
