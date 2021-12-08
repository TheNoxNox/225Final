using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

using Photon.Realtime;
using Photon.Pun;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    public GameObject GameplayPlayerPrefab;

    public PlayerInput playerInput;

    public GameplayPlayer myPlayer;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }

        if (GameplayPlayerPrefab)
        {
            GameInstance.Instance.myGameplayPlayer = PhotonNetwork.Instantiate("GameplayPlayer", Vector3.zero, Quaternion.identity).GetComponent<GameplayPlayer>();
            myPlayer = GameInstance.Instance.myGameplayPlayer;
        }
    }

    private void Update()
    {
        
    }

    public void FlipCamera(bool isFlipped)
    {
        if (isFlipped) { Camera.main.transform.rotation = Quaternion.Euler(0, 0, 180); }
        else { Camera.main.transform.rotation = Quaternion.Euler(0, 0, 0); }
    }
}
