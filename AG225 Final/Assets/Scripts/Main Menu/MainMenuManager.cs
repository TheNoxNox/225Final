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
    public static UILogicDelegate LoadingUI_Update;

    #endregion


    #region Loading UI

    public GameObject loadingUI;

    public TMP_Text loadingText;

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
            Mathf.Clamp(loadScreenBG.color.a - (0.4f * Time.deltaTime),0f,1f)
            );
        loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b,
            Mathf.Clamp(loadingText.color.a - (0.4f * Time.deltaTime), 0f, 1f)
            );
        if (loadScreenBG.color.a == 0)
        {
            loadingUI.SetActive(false);
            LoadingUI_Update = null;
        }
    }

    #endregion

    #endregion


    #region Loaded UI

    #region Loaded UI methods

    #endregion

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
        }
        else
        {
            PlayerNetworkManager.ConnectedToMaster += LoadMainMenu;
        }
    }

    private void Update()
    {
        LoadingUI_Update?.Invoke();
    }
}
