using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    #region Self information
    public static MainMenuManager Instance;
    #endregion


    #region UI Logic Delegates

    public delegate void UILogicDelegate();
    public UILogicDelegate LoadingUI_Update;
    public UILogicDelegate LoadedUI_Update;

    #endregion


    #region Loading UI

    [Header("Loading Screen References")]
    [Tooltip("The root gameobject for the Loading UI structure.")]
    public GameObject loadingUI;
    [Tooltip("The text that displays while connecting to master.")]
    public TMP_Text loadingText;
    [Tooltip("The opaque background for the loading screen.")]
    public Image loadScreenBG;

    #region Loading UI Methods
    
    public void LoadMainMenu()
    {
        loadingText.text = "Connected to Master Client. Starting game . . .";
        LoadingUI_Update += FadeOutLoadscreen;
    }

    public void FadeOutLoadscreen()
    {
        loadScreenBG.color = new Color(loadScreenBG.color.r, loadScreenBG.color.g, loadScreenBG.color.b, 
            Mathf.Clamp(loadScreenBG.color.a - (0.35f * Time.deltaTime),0f,1f)
            );
        loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b,
            Mathf.Clamp(loadingText.color.a - (0.35f * Time.deltaTime), 0f, 1f)
            );
        if (loadScreenBG.color.a <= 0.05f)
        {
            loadingUI.SetActive(false);
            LoadingUI_Update = null;
            LoadedUI_Update += DoMainMenuIntro;
        }
    }

    #endregion

    #endregion


    #region Loaded UI

    [Header("Main UI References")]
    public GameObject loadedUI;
    public GameObject titleImage;
    public Button createLobbyButton;
    public Button joinLobbyButton;
    public Button exitGameButton;
    public Transform loadedCenter;
    public TMP_Text menuInfo;

    [Header("Username Input References")]
    public TMP_InputField usernameInput;

    [Header("Loaded UI Intro Variables")]
    [Tooltip("The speed in degrees per second that the rotating elements of the UI will rotate at.")]
    public float loadedUI_ElementRotationSpeed = 75f;
    [Tooltip("The speed in canvas units per second that the translating elements of the UI will move at.")]
    public float loadedUI_ElementTranslationSpeed = 350f;

    #region Loaded UI methods

    public void PrepareMainMenuForIntro()
    {
        createLobbyButton.interactable = false;
        joinLobbyButton.interactable = false;
        exitGameButton.interactable = false;

        usernameInput.gameObject.SetActive(false);
        usernameInput.enabled = false;

        titleImage.transform.rotation = Quaternion.Euler(0, 90, 0);
        createLobbyButton.transform.position = new Vector3(createLobbyButton.transform.position.x + 1350,
            createLobbyButton.transform.position.y, 0);
        joinLobbyButton.transform.position = new Vector3(joinLobbyButton.transform.position.x - 1350,
            joinLobbyButton.transform.position.y, 0);
        exitGameButton.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    #region Main Menu Intro Delegate Methods

    public void DoMainMenuIntro()
    {
        titleImage.transform.Rotate(new Vector3(0, -loadedUI_ElementRotationSpeed * Time.deltaTime, 0));
        if(titleImage.transform.rotation.eulerAngles.y <= 2f)
        {
            titleImage.transform.rotation = Quaternion.identity;
            LoadedUI_Update = MoveHostButton;
        }
    }

    public void MoveHostButton()
    {
        createLobbyButton.transform.position = new Vector3(
            Mathf.Clamp(createLobbyButton.transform.position.x - (loadedUI_ElementTranslationSpeed * Time.deltaTime)
            ,loadedCenter.position.x, loadedCenter.position.x + 1500f),
            createLobbyButton.transform.position.y,
            0);
        if(createLobbyButton.transform.position.x == loadedCenter.position.x)
        {
            LoadedUI_Update = MoveJoinButton;
        }
    }

    public void MoveJoinButton()
    {
        joinLobbyButton.transform.position = new Vector3(
            Mathf.Clamp(joinLobbyButton.transform.position.x + (loadedUI_ElementTranslationSpeed * Time.deltaTime), 
            loadedCenter.position.x - 1500f, loadedCenter.position.x),
            joinLobbyButton.transform.position.y,
            0);
        if(joinLobbyButton.transform.position.x == loadedCenter.position.x)
        {
            LoadedUI_Update = RotateExitButton;
        }
    }

    public void RotateExitButton()
    {
        exitGameButton.transform.Rotate(new Vector3(-loadedUI_ElementRotationSpeed * Time.deltaTime, 0, 0));
        if (exitGameButton.transform.eulerAngles.x <= 2f)
        {
            exitGameButton.transform.rotation = Quaternion.identity;
            LoadedUI_Update = null;
            FinishMainMenuIntro();
        }
    }

    public void FinishMainMenuIntro()
    {
        createLobbyButton.interactable = true;
        joinLobbyButton.interactable = true;
        exitGameButton.interactable = true;

        usernameInput.gameObject.SetActive(true);
        usernameInput.enabled = true;
    }

    #endregion

    #endregion

    #endregion


    #region Game Beginning Methods

    public void HostLobby()
    {
        if (HasValidUsername())
        {
            PlayerNetworkManager.Instance.CreateRoom();
        }
    }

    public void JoinLobby()
    {
        if (HasValidUsername())
        {
            PlayerNetworkManager.Instance.JoinRandomRoom();
        }
    }

    public bool HasValidUsername()
    {
        if (string.IsNullOrEmpty(usernameInput.text))
        {
            menuInfo.text = "You must enter a username!";
            return false;
        }

        PlayerNetworkManager.Instance.SetUsername(usernameInput.text);
        return true;
    }

    public void NoFoundRooms()
    {
        menuInfo.text = "No lobbies currently exist. Try hosting your own!";
    }

    #endregion


    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        if (PlayerNetworkManager.Instance.IsConnected)
        {
            loadingUI.SetActive(false);
            LoadingUI_Update = null;
            LoadedUI_Update = null;
        }
        else
        {
            PlayerNetworkManager.ConnectedToMaster += LoadMainMenu;

            PrepareMainMenuForIntro();
        }

        PlayerNetworkManager.RoomJoinFailed += NoFoundRooms;
    }

    private void Update()
    {
        LoadingUI_Update?.Invoke();
        LoadedUI_Update?.Invoke();
    }
}
