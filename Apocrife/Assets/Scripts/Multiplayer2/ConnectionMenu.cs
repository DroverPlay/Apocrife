using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ConnectionMenu : MonoBehaviourPunCallbacks
{
    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom("Room", roomOptions);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }


    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("game");
        Debug.Log("Подключение к комнате успешно!");
    }
}
