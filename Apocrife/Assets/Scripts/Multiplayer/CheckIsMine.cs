using UnityEngine;
using Photon.Pun;
using Muryotaisu;

public class CheckIsMine : MonoBehaviour
{
    [SerializeField] private MuryotaisuController _move;
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private AudioListener _audioListener;
    [SerializeField] private Camera _camera;

    void Start()
    {
        if (!_photonView.IsMine)
        {
            _camera.enabled = false;
            _move.enabled = false;
            _audioListener.enabled = false;
            _move.enabled = false;
        }
    }
}