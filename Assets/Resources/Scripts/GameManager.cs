using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;
using Meta.XR.ImmersiveDebugger;
using TMPro;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks
{

    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;
    public static GameManager Instance;
    public Transform spawnPoint;
    public TextMeshProUGUI PlayerCount;
    private GameObject localplayer;
    private String playerdevice;
    private int playerNumber;
    private List<int> phonePlayers = new List<int>();
    private int myPhoneNumber = 0;

    #region Photon Callbacks
    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    #endregion


    #region Public Methods
    void Start()
    {
        //Instance = this;
        /*
        if (PlayerManager.LocalPlayerInstance == null)
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
        }
        else
        {
            Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
        }*/
        if(PlayerCount != null) PlayerCount.text = "Players: " + PhotonNetwork.CurrentRoom.PlayerCount.ToString();

    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        if(playerdevice == null)
        {
            this.SetMyDeviceType();
        }
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
        this.UpdatePhonePlayers();
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            LoadArena();
        }
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            LoadArena();
        }
    }

    #endregion
  
    #region Private Methods

    public void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            return;
        }
        Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
        //PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("General Room");
    }

    #endregion
    
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Player has joined the room. Instantiating player model.");
        Debug.Log("Here");
        PhotonNetwork.AutomaticallySyncScene = true;
        this.SetMyDeviceType();

    }

    void UpdatePhonePlayers() //loop through all players and record all that are phone players
    {
        phonePlayers.Clear();
        int phoneNum = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (GetDeviceType(player) != "quest")
            {
                phonePlayers.Add(player.ActorNumber);
                phoneNum++;
                if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber) //records what phone you are (first phone to join, second phone to join, etc)
                {
                    this.myPhoneNumber = phoneNum;
                    Debug.Log("I am phone #:");
                    Debug.Log(this.myPhoneNumber);
                }
            }
        }

        Debug.Log($"Phone players: {string.Join(", ", phonePlayers)}");
    }

    void SetMyDeviceType()
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("deviceType"))
        {
            string myDeviceType = GetMyDeviceType();
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props["deviceType"] = myDeviceType;
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            this.playerdevice = myDeviceType;
        }
    }

    string GetDeviceType(Player player)
    {
        if (player.CustomProperties.TryGetValue("deviceType", out object deviceTypeObj))
        {
            return deviceTypeObj.ToString();
        }
        return "Unknown";
    }

    public int getMyPhoneNumber()
    {
        this.UpdatePhonePlayers();
        return this.myPhoneNumber;
    }

    public int getNumberOfPhones()
    {
        this.UpdatePhonePlayers();
        return this.phonePlayers.Count;
    }

    string GetMyDeviceType()
    {
        if (SystemInfo.deviceModel.ToLower().Contains("quest"))
            return "quest";
        else
         return "phone";

    }
}
