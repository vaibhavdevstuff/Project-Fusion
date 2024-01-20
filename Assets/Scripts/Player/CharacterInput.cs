using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Input
{
    public class CharacterInput : MonoBehaviour
    {
        public static CharacterInput Instance;

        public Vector2 moveDirection;
        public Vector2 lookDirection;
        public bool fire;
        public bool aim;
        public bool jump;
        public bool reload;

        public Vector3 forwardViewVector;

        private Camera cam;

        private void Start()
        {
            Instance = this;
            cam = Camera.main;
        }

        private void Update()
        {
            forwardViewVector = cam.transform.forward;
        }

        public void OnMove(InputValue value)
        {
            moveDirection = value.Get<Vector2>();
        }

        public void OnMouseX(InputValue value)
        {
            lookDirection.x = value.Get<float>();
        }
        
        public void OnMouseY(InputValue value)
        {
            lookDirection.y = value.Get<float>();
        }

        public void OnFire(InputValue value)
        {
            fire = value.isPressed;
        }
        
        public void OnAim(InputValue value)
        {
            aim = value.isPressed;
        }
        
        public void OnJump(InputValue value)
        {
            jump = value.isPressed;
        }
        
        public void OnReload(InputValue value)
        {
            reload = value.isPressed;
        }

        public NetworkInputData GetNetworkInput()
        {
            NetworkInputData networkInput = new NetworkInputData();

            networkInput.ForwardViewVector = forwardViewVector;
            networkInput.MoveDirection = moveDirection;
            networkInput.LookDirection = lookDirection;
            networkInput.Fire = fire;
            networkInput.Aim = aim;
            networkInput.Jump = jump;
            networkInput.Reload = reload;

            
            lookDirection = default;
            jump = false;
            reload = false;


            return networkInput;
        }















        }
}

[System.Serializable]
public struct NetworkInputData : INetworkInput
{
    public Vector2 ForwardViewVector;
    public Vector2 MoveDirection;
    public Vector2 LookDirection;
    public NetworkBool Fire;
    public NetworkBool Aim;
    public NetworkBool Jump;
    public NetworkBool Reload;


}