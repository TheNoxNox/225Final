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
}
