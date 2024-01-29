using Cinemachine;
using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private GameObject cinemachineCameraTarget;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    [Space]
    [SerializeField] private float NormalFOV = 40f;
    [SerializeField] private float AimFOV = 25f;
    [SerializeField] private float AimChangeSpeed = 10f;

    //[SerializeField] private float TopClamp = 70.0f;
    //[SerializeField] private float BottomClamp = -30.0f;

    //[Space]
    //[SerializeField] private float MouseSensitivityX = 2.0f;
    //[SerializeField] private float MouseSensitivityY = 2.0f;
    //[SerializeField] private bool InvertX = false;
    //[SerializeField] private bool InvertY = true;

    //[Space]
    //[SerializeField] private bool LockCameraPosition = false;

    // cinemachine
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;

    private const float _threshold = 0.01f;

    private SimpleKCC kcc;
    private NetworkInputData networkInput;
    private CharacterHealth health;
    
    public GameObject CinemachineCameraTarget { get { return cinemachineCameraTarget; } }
    public CinemachineVirtualCamera CinemachineVirtualCamera { get { return cinemachineVirtualCamera; } }


    private void Awake()
    {
        kcc = GetComponent<SimpleKCC>();
        health = GetComponent<CharacterHealth>();
    }

    private void Start()
    {
        cinemachineVirtualCamera.Follow = cinemachineCameraTarget.transform;
        cinemachineVirtualCamera.transform.parent = null;

        cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInput))
        {
            this.networkInput = networkInput;
        }

        if (!health.IsAlive) return;

        CameraAim();
        CameraRotation();

    }

    private void LateUpdate()
    {
        //CameraRotation();
    }

    private void CameraAim()
    {
        float _fov = cinemachineVirtualCamera.m_Lens.FieldOfView;

        if (networkInput.Aim)
        {
            _fov = Mathf.Lerp(_fov, AimFOV, AimChangeSpeed * Runner.DeltaTime);
        }
        else
        {
            _fov = Mathf.Lerp(_fov, NormalFOV, AimChangeSpeed * Runner.DeltaTime);
        }

        cinemachineVirtualCamera.m_Lens.FieldOfView = _fov;
    }

    private void CameraRotation()
    {
        Vector2 pitchRotation = kcc.GetLookRotation(true, false);
        cinemachineCameraTarget.transform.localRotation = Quaternion.Euler(pitchRotation);
    }









}



