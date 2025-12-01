using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _spawn;

    private void Start()
    {
        PhotonNetwork.Instantiate(_player.name, _spawn.position, Quaternion.identity);
    }
}
