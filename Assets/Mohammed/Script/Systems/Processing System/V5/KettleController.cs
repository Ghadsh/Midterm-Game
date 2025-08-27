// KettleController.cs
using UnityEngine;

public class KettleController : MonoBehaviour
{
    [Header("Home Pose")]
    public Transform home;
    Vector3 _homePos; Quaternion _homeRot;

    void Awake()
    {
        if (home == null) home = transform;
        _homePos = home.localPosition; _homeRot = home.localRotation;
    }

    public void ResetToHome()
    {
        home.localPosition = _homePos;
        home.localRotation = _homeRot;
    }
}
