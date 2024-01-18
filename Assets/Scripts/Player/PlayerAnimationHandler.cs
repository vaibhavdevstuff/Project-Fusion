using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    private Animator anim;


    #region Animation Hash IDs
    public int AnimIDMoveX { get; private set; }
    public int AnimIDMoveZ { get; private set; }
    public int AnimIDJump { get; private set; }
    public int AnimIDGrounded { get; private set; }
    public int AnimIDVerticalAim { get; private set; }

    #endregion


    public Animator Animator {  get { return anim; } }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        SetAnimationIDs();
    }

    private void SetAnimationIDs()
    {
        AnimIDMoveX = Animator.StringToHash("MoveX");
        AnimIDMoveZ = Animator.StringToHash("MoveZ");
        AnimIDJump = Animator.StringToHash("Jump");
        AnimIDGrounded = Animator.StringToHash("Grounded");
        AnimIDVerticalAim = Animator.StringToHash("VerticalAim");
   
    }

    #region Animation Set

    public void SetFloat(int HashID, float Value)
    {
        anim.SetFloat(HashID, Value);
    }
    
    public void SetBool(int HashID, bool Value)
    {
        anim.SetBool(HashID, Value);
    }

    #endregion

    #region Animation Get

    public float GetFloat(int HashID)
    {
        return anim.GetFloat(HashID);
    }
    public bool GetBool(int HashID)
    {
        return anim.GetBool(HashID);
    }

    #endregion








}
