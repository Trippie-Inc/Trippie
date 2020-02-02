using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManagerCustom : NetworkManager
{
    public string playername;
    public GameObject AudioManager;

    void Start()
    {
        AudioManager = GameObject.Find("AudioManager");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AudioManager.GetComponent<AudioManager>().PlaySound("Kill");
        }

        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            SetupMenuSceneButtons();
        }
        
        if (SceneManager.GetActiveScene().name == "Main Map")
        {
            SetupOtherSceneButtons();
        }
    }

    public void StartupHost()
    {
        AudioManager.GetComponent<AudioManager>().PlaySound("Click");
        NetworkServer.Reset();
        SetPort();
        NetworkManager.singleton.StartHost();
        playername = GameObject.Find("inputFieldPlayerName").transform.Find("Text").GetComponent<Text>().text;
    }
    public void JoinGame()
    {
        AudioManager.GetComponent<AudioManager>().PlaySound("Click");
        SetIPAddres();
        SetPort();
        NetworkManager.singleton.StartClient();
        playername = GameObject.Find("inputFieldPlayerName").transform.Find("Text").GetComponent<Text>().text;
    }

    void SetIPAddres()
    {
        string ipAddress = GameObject.Find("inputFieldIPAddress").transform.Find("Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    void SetPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }

    void SetupMenuSceneButtons()
    {
        GameObject.Find("ButtonStartHost").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ButtonStartHost").GetComponent<Button>().onClick.AddListener(StartupHost);

        GameObject.Find("ButtonJoinGame").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ButtonJoinGame").GetComponent<Button>().onClick.AddListener(JoinGame);

        GameObject.Find("ExitGame").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ExitGame").GetComponent<Button>().onClick.AddListener(ExitToDeskTop);
    }

    void SetupOtherSceneButtons()
    {
        GameObject.Find("ButtonDisconnect").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("ButtonDisconnect").GetComponent<Button>().onClick.AddListener(Disconnect);
    }

    public void Disconnect()
    {
        AudioManager.GetComponent<AudioManager>().PlaySound("Click");
        NetworkManager.singleton.StopHost();
    }

    public void ExitToDeskTop()
    {
        AudioManager.GetComponent<AudioManager>().PlaySound("Click");
        Application.Quit();
    }
}
