using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private bool shouldSendInput;
    private void Awake()
    {
        shouldSendInput = GameplayManager.Instance.myPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        shouldSendInput = GameplayManager.Instance.myPlayer;
    }

    public void XMovement(InputAction.CallbackContext ctx)
    {
        if (shouldSendInput)
        {
            GameplayManager.Instance.myPlayer.MoveCharacter(ctx.ReadValue<float>());
        }      
    }

    public void PlayerJump(InputAction.CallbackContext ctx)
    {
        if (shouldSendInput && ctx.performed)
        {
            GameplayManager.Instance.myPlayer.JumpCharacter();
        }
    }

    public void PlayerFlip(InputAction.CallbackContext ctx)
    {
        if(shouldSendInput && ctx.performed)
        {
            GameplayManager.Instance.myPlayer.FlipCharacter();
        }
    }

    public void PlayerAttack(InputAction.CallbackContext ctx)
    {
        if (shouldSendInput && ctx.performed)
        {
            GameplayManager.Instance.myPlayer.AttackCharacter(ctx.ReadValue<Vector2>());
        }
    }
}
