using Cinemachine;
using Fusion.Addons.SimpleKCC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject cinemachineCameraTarget;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    [SerializeField] private float TopClamp = 70.0f;
    [SerializeField] private float BottomClamp = -30.0f;

    [Space]
    [SerializeField] private float MouseSensitivityX = 2.0f;
    [SerializeField] private float MouseSensitivityY = 2.0f;
    [SerializeField] private bool InvertX = false;
    [SerializeField] private bool InvertY = true;

    [Space]
    [SerializeField] private bool LockCameraPosition = false;

    // cinemachine
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;

    private const float _threshold = 0.01f;

    private SimpleKCC kcc;
    
    public GameObject CinemachineCameraTarget { get { return cinemachineCameraTarget; } }
    public CinemachineVirtualCamera CinemachineVirtualCamera { get { return cinemachineVirtualCamera; } }


    private void Start()
    {
        kcc = GetComponent<SimpleKCC>();
        cinemachineVirtualCamera.Follow = cinemachineCameraTarget.transform;
        cinemachineVirtualCamera.transform.parent = null;

        cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    private void LateUpdate()
    {
        Vector2 pitchRotation = kcc.GetLookRotation(true, false);
        cinemachineCameraTarget.transform.localRotation = Quaternion.Euler(pitchRotation);
        //CameraRotation();
    }

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        //if (input.LookDirection.sqrMagnitude >= _threshold && !LockCameraPosition)
        //{
        //    cinemachineTargetYaw += input.LookDirection.y * MouseSensitivityX * (InvertX ? -1 : 1);
        //    cinemachineTargetPitch += input.LookDirection.x * MouseSensitivityY * (InvertY ? -1 : 1);
        //}

        // clamp our rotations so our values are limited 360 degrees
        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        cinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch,
            cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }









}



