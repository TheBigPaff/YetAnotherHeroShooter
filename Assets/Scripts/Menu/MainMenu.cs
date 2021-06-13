using MLAPI;
using MLAPI.Transports.UNET;
using MLAPI.Transports.PhotonRealtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    //[SerializeField]
    //private GameObject startMenu;
    //[SerializeField]
    //private GameObject registerMenu;
    //[SerializeField]
    //private GameObject logInMenu;
    //[SerializeField]
    //private GameObject mainMenu;

    [SerializeField]
    private TMP_InputField joinRoomName;
    [SerializeField]
    private TMP_InputField hostRoomName;

    [SerializeField]
    private string m_LobbySceneName = "Lobby";

    public void StartLocalGame()
    {
        //// Update the current HostNameInput with whatever we have set in the NetworkConfig as default
        //var unetTransport = (UNetTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        //if (unetTransport) m_HostIpInput.text = unetTransport.ConnectAddress;
        //LobbyControl.isHosting = true; //This is a work around to handle proper instantiation of a scene for the first time.(See LobbyControl.cs)
        //SceneTransitionHandler.sceneTransitionHandler.SwitchScene(m_LobbySceneName);


        var photonTransport = (PhotonRealtimeTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        if (photonTransport) photonTransport.RoomName = hostRoomName.text;
        LobbyControl.isHosting = true; //This is a work around to handle proper instantiation of a scene for the first time.(See LobbyControl.cs)
        SceneTransitionHandler.sceneTransitionHandler.SwitchScene(m_LobbySceneName);
    }

    public void JoinLocalGame()
    {
        //if (m_HostIpInput.text != "Hostname")
        //{
        //    var unetTransport = (UNetTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        //    if (unetTransport) unetTransport.ConnectAddress = m_HostIpInput.text;
        //    LobbyControl.isHosting = false; //This is a work around to handle proper instantiation of a scene for the first time.  (See LobbyControl.cs)
        //    SceneTransitionHandler.sceneTransitionHandler.SwitchScene(m_LobbySceneName);
        //}


        var photonTransport = (PhotonRealtimeTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        if (photonTransport) photonTransport.RoomName = joinRoomName.text;
        LobbyControl.isHosting = false; //This is a work around to handle proper instantiation of a scene for the first time.  (See LobbyControl.cs)
        SceneTransitionHandler.sceneTransitionHandler.SwitchScene(m_LobbySceneName);
    }

    public void JoinSoloGame()
    {
        // dont bother with scene transition handler because it's all client-sided, no need to sync I guess
        SceneManager.LoadScene("SoloHordeGame");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}