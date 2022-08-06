using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Base;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEditor.Animations;
using UnityEngine;

public class AnimationPuller : MonoBehaviour {
    public Animator Animator;

    public string[] _AnimationNames;

    private Coroutine ShowcaseAnimaitons;

    private void Start() {
        PlayAnimations();
    }

    [Button]
    public void GetAnimations() {
        _AnimationNames = Animator.GetClipNames();
    }

    [Button]
    public void CreateEnums() {
        Animator.CreateAnimationEnums("Enum_Anim");
    }

    public void PlayAnimations() {
        if (ShowcaseAnimaitons != null) { 
            StopCoroutine(ShowcaseAnimaitons);
        }

        ShowcaseAnimaitons = StartCoroutine(IE_ShowcaseAnimations());
    }

    private IEnumerator IE_ShowcaseAnimations() {
        foreach (string animationName in _AnimationNames) {
            Animator.PlayClip((int)Enum_AnimControllerStandart_1.Jump_1, .3f, 0);
            
            yield return new WaitForSeconds(.2f);
            yield return new WaitForSeconds(Animator.GetCurrentAnimatorStateInfo(0).length);
        }
    }
}