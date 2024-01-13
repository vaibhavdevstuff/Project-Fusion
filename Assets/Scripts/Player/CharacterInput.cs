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
        public bool jump;

        private void Start()
        {
            Instance = this;
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

        public void OnJump(InputValue value)
        {
            jump = value.isPressed;
        }

        public NetworkInputData GetNetworkInput()
        {
            NetworkInputData networkInput = new NetworkInputData();

            networkInput.MoveDirection = moveDirection;
            networkInput.LookDirection = lookDirection;
            networkInput.Jump = jump;

            
            lookDirection = default;
            jump = false;


            return networkInput;
        }















        }
}

[System.Serializable]
public struct NetworkInputData : INetworkInput
{
    public Vector2 MoveDirection;
    public Vector2 LookDirection;
    public NetworkBool Jump;


}