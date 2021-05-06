using MLAPI;
using MLAPI.Transports.UNET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField m_HostIpInput;

    [SerializeField]
    private string m_LobbySceneName = "Lobby";

    public void StartLocalGame()
    {
        // Update the current HostNameInput with whatever we have set in the NetworkConfig as default
        var unetTransport = (UNetTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        if (unetTransport) m_HostIpInput.text = unetTransport.ConnectAddress;
        LobbyControl.isHosting = true; //This is a work around to handle proper instantiation of a scene for the first time.(See LobbyControl.cs)
        SceneTransitionHandler.sceneTransitionHandler.SwitchScene(m_LobbySceneName);
    }

    public void JoinLocalGame()
    {
        if (m_HostIpInput.text != "Hostname")
        {
            var unetTransport = (UNetTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            if (unetTransport) unetTransport.ConnectAddress = m_HostIpInput.text;
            LobbyControl.isHosting = false; //This is a work around to handle proper instantiation of a scene for the first time.  (See LobbyControl.cs)
            SceneTransitionHandler.sceneTransitionHandler.SwitchScene(m_LobbySceneName);
        }
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
