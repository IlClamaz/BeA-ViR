using Beavir.Businesslogic.Utilities;
using UnityEngine;

namespace Beavir.Businesslogic.ViewModels
{
    /// <summary>
    /// Handles the enablement of objects related to large content objects, such as DBReports and the Tobiotsuka orange mound. 
    /// </summary>
    public class ItemAggregatorLarge : ItemInteractor
    {
        [SerializeField] private ColliderProxy enableTrigger;
        [SerializeField] private GameObject targetObj;
        private bool initialState;
        // Start is called before the first frame update
        void Start()
        {
            initialState = targetObj.activeSelf;
            enableTrigger.OnTriggerEnter_Action += (Collider c) =>
            {
                ObjectEnabler.Instance.ToggleGameObject(targetObj, 0);
                OnTriggerEnter(null);
            };

            enableTrigger.OnTriggerExit_Action += (Collider c) =>
            {
                ObjectEnabler.Instance.ToggleGameObject(targetObj, 0);
                OnTriggerExit(null);
            };
        }


        public void Reset()
        {
            if (initialState) ObjectEnabler.Instance.ActivateGameObject(targetObj, 0);
            else ObjectEnabler.Instance.DeactivateGameObject(targetObj, 0);
            OnTriggerExit(null);
        }
    }
}
