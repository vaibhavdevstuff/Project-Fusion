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
    [SerializeField] private CinemachineVirtualCamera virtualDeathCamera;

    [Space]
    [SerializeField] private float NormalFOV = 40f;
    [SerializeField] private float AimFOV = 25f;
    [SerializeField] private float AimChangeSpeed = 10f;
    [SerializeField] private float DeathCameraSwitchSpeed = 1f;

    private SimpleKCC kcc;
    private NetworkInputData networkInput;
    private CharacterHealth health;
    private CinemachineBrain cmBrain;
    
    public GameObject CinemachineCameraTarget { get { return cinemachineCameraTarget; } }
    public CinemachineVirtualCamera CinemachineVirtualCamera { get { return cinemachineVirtualCamera; } }


    private void Awake()
    {
        kcc = GetComponent<SimpleKCC>();
        health = GetComponent<CharacterHealth>();
    }

    private void Start()
    {
        cmBrain = Camera.main.GetComponent<CinemachineBrain>();

        cinemachineVirtualCamera.Follow = cinemachineCameraTarget.transform;
        cinemachineVirtualCamera.transform.parent = null;

        virtualDeathCamera.Follow = transform; 
        virtualDeathCamera.LookAt = transform;
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

    public void DisableCameras()
    {
        cinemachineVirtualCamera.gameObject.SetActive(false);
        virtualDeathCamera.gameObject.SetActive(false);
    }

    public void SwitchToTPSCamera()
    {
        if(!cmBrain) cmBrain = Camera.main.GetComponent<CinemachineBrain>();

        if (cmBrain)
        {
            cmBrain.m_DefaultBlend.m_Time = 0;
            cinemachineVirtualCamera.enabled = true;
        }
    }

    public void SwitchToDeathCamera()
    {
        if (!cmBrain) cmBrain = Camera.main.GetComponent<CinemachineBrain>();

        if (cmBrain)
        {
            cmBrain.m_DefaultBlend.m_Time = DeathCameraSwitchSpeed;
            cinemachineVirtualCamera.enabled = false;
        }
    }






}



