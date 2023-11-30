using Beavir.Businesslogic.Controllers;
using Beavir.Businesslogic.Utilities;
using Mirror;
using UnityEngine;

namespace Beavir.Businesslogic.ViewModels
{
    /// <summary>
    /// Handles the enablement of objects related to medium content objects (DBReport and Pedestal) and their animation. 
    /// </summary>
    public class ItemAggregatorMedium : ItemInteractor
    {
        [SerializeField] private ColliderProxy rotationTrigger, zoomTrigger;
        public GameObject dbReport, pedestal;
        private Animator itemAnimator;
        // Start is called before the first frame update
        public void Start()
        {
            itemAnimator = item.GetComponent<Animator>();

            rotationTrigger.OnTriggerEnter_Action += EnterRotationTrigger;
            rotationTrigger.OnTriggerExit_Action += ExitRotationTrigger;

            zoomTrigger.OnTriggerEnter_Action += (Collider c) => ObjectAnimator.Instance.PlayAnimation(itemAnimator, "ZoomTrigger");
            zoomTrigger.OnTriggerExit_Action += (Collider c) => ObjectAnimator.Instance.StopAnimation(itemAnimator, "ZoomTrigger");
        }

        private void OnEnable()
        {
            BeavirAppManager.Instance.UserManager.InputReader.itemPauseResumeEvent += PauseResumeRotation;
        }

        private void OnDisable()
        {
            BeavirAppManager.Instance.UserManager.InputReader.itemPauseResumeEvent -= PauseResumeRotation;
        }

        public void Reset()
        {
            OnTriggerExit(null);
            ObjectEnabler.Instance.DeactivateGameObject(dbReport, 0);
            if (pedestal != null) ObjectEnabler.Instance.ActivateGameObject(pedestal, 0);
            ObjectAnimator.Instance.ResetAnimator(itemAnimator);
        }

        private void EnterRotationTrigger(Collider collision)
        {
            OnTriggerEnter(null);
            ObjectEnabler.Instance.ActivateGameObject(dbReport, 0);
            if (pedestal != null) ObjectEnabler.Instance.DeactivateGameObject(pedestal, 0);
            ObjectAnimator.Instance.PlayAnimation(itemAnimator, "RotatingTrigger");
        }

        private void ExitRotationTrigger(Collider collision)
        {
            OnTriggerExit(null);
            ObjectEnabler.Instance.DeactivateGameObject(dbReport, 0);
            if (pedestal != null) ObjectEnabler.Instance.ActivateGameObject(pedestal, 0);
            ObjectAnimator.Instance.StopAnimation(itemAnimator, "RotatingTrigger");
        }

        [ClientRpc]
        private void PauseResumeRotation()
        {
            ObjectAnimator.Instance.PauseResumeAnimation(itemAnimator, "AnimationSpeed");
        }
    }
}
