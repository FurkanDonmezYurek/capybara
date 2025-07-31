using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class CapybaraAnimation
{
    public Animator _animator;
    private static readonly int Blend = Animator.StringToHash("Blend");
    private static readonly int Sit = Animator.StringToHash("isSitting");
    private static readonly int Sleepy = Animator.StringToHash("isSleeping");
    private static readonly int Freeze = Animator.StringToHash("isFreezing");
    private static readonly int Jump = Animator.StringToHash("isJumping");

    public CapybaraAnimation(Animator animator)
    {
        _animator = animator;
    }
    public void SetSitAnim(bool status)
    {
        _animator.SetBool(Sit, status);
    }

    public void SetSleepyAnim(bool status)
    {
        _animator.SetBool(Sleepy, status);
    }
    public void SetJumpAnim(bool status)
    {
        _animator.SetBool(Sleepy, status);
    }
    public void SetFreezeAnim(bool status)
    {
        _animator.SetBool(Jump, status);
    }
    public void SetMovementAnimByMagnitude(float magnitude, bool instant = false)
    {
        if (!instant)
        {
            _animator.SetFloat(Blend, magnitude, .1f, Time.deltaTime);
        }
        else
        {
            _animator.SetFloat(Blend, magnitude);
        }
    }
}

