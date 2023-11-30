using Beavir.Businesslogic.Utilities;
using Beavir.Businesslogic.View;
using System.Collections.Generic;
using UnityEngine;

namespace Beavir.Businesslogic.ViewModels
{
    /// <summary>
    /// Handles the enablement of the Timeline model and periods and their animation. 
    /// </summary>
    public class Timeline : MonoBehaviour
    {
        [SerializeField] private List<Animator> timelineAnimators;
        [SerializeField] private GameObject timelineModel, timelinePeriods;
        [SerializeField] private ColliderProxy enablerBig, enablerSmall;
        // Start is called before the first frame update
        void Start()
        {
            enablerBig.OnTriggerEnter_Action += EnterTimelineBig;
            enablerBig.OnTriggerExit_Action += ExitTimelineBig;

            enablerSmall.OnTriggerEnter_Action += EnterTimelineSmall;
            enablerSmall.OnTriggerExit_Action += ExitTimelineSmall;
        }

        private void EnterTimelineBig(Collider c)
        {
            enablerBig.gameObject.GetComponent<E2EObj>().OnTriggerEnter(c);
            ObjectEnabler.Instance.ActivateGameObject(new List<GameObject> { timelineModel, timelinePeriods }, 0);
        }

        private void ExitTimelineBig(Collider c)
        {
            enablerBig.gameObject.GetComponent<E2EObj>().OnTriggerEnter(c);
            ObjectEnabler.Instance.DeactivateGameObject(new List<GameObject> { timelineModel, timelinePeriods }, 0);
        }

        private void ExitTimelineSmall(Collider c)
        {
            ObjectEnabler.Instance.ActivateGameObject(new List<GameObject> { timelineModel, timelinePeriods }, 0);
        }

        private void EnterTimelineSmall(Collider c)
        {
            ObjectEnabler.Instance.DeactivateGameObject(new List<GameObject> { timelineModel, timelinePeriods }, 0);
        }

        public void Reset()
        {
            ObjectEnabler.Instance.DeactivateGameObject(new List<GameObject> { timelineModel, timelinePeriods }, 0.1f);
        }
    }
}
