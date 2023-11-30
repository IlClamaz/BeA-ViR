using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beavir.Businesslogic.Utilities
{
    public class ObjectAnimator : MonoBehaviour
    {
        public static ObjectAnimator Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this);
            else Instance = this;
        }

        public void PlayAnimation(Animator targetAnimator, string targetAnimationTrigger)
        {
            if (!targetAnimator.GetBool(targetAnimationTrigger)) targetAnimator.SetBool(targetAnimationTrigger, true);
        }
        public void PlayAnimation(List<Animator> targetAnimators, string targetAnimationTrigger)
        {
            foreach (var item in targetAnimators) if (!item.GetBool(targetAnimationTrigger)) item.SetBool(targetAnimationTrigger, true);
        }
        public void PauseResumeAnimation(Animator targetAnimator, string targetAnimationTrigger)
        {
            if (targetAnimator.GetCurrentAnimatorStateInfo(0).length >
               targetAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime)
            {
                if (targetAnimator.GetFloat(targetAnimationTrigger) == 1)
                    targetAnimator.SetFloat(targetAnimationTrigger, 0);
                else
                    targetAnimator.SetFloat(targetAnimationTrigger, 1);
            }
        }
        public void StopAnimation(Animator targetAnimator, string targetAnimationTrigger)
        {
            if (targetAnimator.GetBool(targetAnimationTrigger)) targetAnimator.SetBool(targetAnimationTrigger, false);
        }

        public void StopAnimation(List<Animator> targetAnimators, string targetAnimationTrigger)
        {
            foreach (var item in targetAnimators) if (item.GetBool(targetAnimationTrigger)) item.SetBool(targetAnimationTrigger, false);
        }
        public void ResetAnimator(Animator targetAnimator)
        {
            targetAnimator.Rebind();
        }

        public void ResetAnimator(List<Animator> targetAnimators)
        {
            foreach (var item in targetAnimators) item.Rebind();
        }
    }
}
