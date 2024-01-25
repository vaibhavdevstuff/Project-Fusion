using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : NetworkBehaviour
{
    private Animator anim;
    private NetworkMecanimAnimator netAnim;


    #region Animation Hash IDs
    public int AnimIDMoveX { get; private set; }
    public int AnimIDMoveZ { get; private set; }
    public int AnimIDJump { get; private set; }
    public int AnimIDGrounded { get; private set; }
    public int AnimIDVerticalAim { get; private set; }
    public int AnimIDFiring { get; private set; }
    public int AnimIDReload { get; private set; }
    public int AnimIDDeath { get; private set; }
    public int AnimIDReset { get; private set; }

    #endregion

    #region Layer ID
    public int LayerIDBase { get; private set; }
    public int LayerIDVerticalAim { get; private set; }
    public int LayerIDFireReload { get; private set; }



    #endregion


    public Animator Animator {  get { return anim; } }
    public NetworkMecanimAnimator NetworkMecanimAnimator {  get { return netAnim; } }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        netAnim = GetComponent<NetworkMecanimAnimator>();
    }

    private void Start()
    {
        SetAnimationIDs();
        SetStateIDs();
    }

    private void SetStateIDs()
    {
        LayerIDBase = anim.GetLayerIndex("Base Layer");
        LayerIDVerticalAim = anim.GetLayerIndex("Vertical Aim");
        LayerIDFireReload = anim.GetLayerIndex("Fire & Reload");
    }

    private void SetAnimationIDs()
    {
        AnimIDMoveX = Animator.StringToHash("MoveX");
        AnimIDMoveZ = Animator.StringToHash("MoveZ");
        AnimIDJump = Animator.StringToHash("Jump");
        AnimIDGrounded = Animator.StringToHash("Grounded");
        AnimIDVerticalAim = Animator.StringToHash("VerticalAim");
        AnimIDFiring = Animator.StringToHash("Firing");
        AnimIDReload = Animator.StringToHash("Reload");
        AnimIDDeath = Animator.StringToHash("Death");
        AnimIDReset = Animator.StringToHash("Reset");
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

    public void SetNetTrigger(int HashID)
    {
        if (Runner.IsServer)
            RPC_Trigger(HashID);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_Trigger(int HashID)
    {
        anim.SetTrigger(HashID);
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

    public void SetLayerWeight(int layerIndex, float weight)
    {
        anim.SetLayerWeight(layerIndex, weight);
    }

    public void PlayDeathAnimation()
    {
        SetLayerWeight(LayerIDVerticalAim, 0f);
        SetLayerWeight(LayerIDFireReload, 0f);

        SetNetTrigger(AnimIDDeath);
    }

    public void ResetAnimation()
    {
        SetLayerWeight(LayerIDVerticalAim, 1f);
        SetLayerWeight(LayerIDFireReload, 1f);
        
        SetNetTrigger(AnimIDReset);
    }

    


}
