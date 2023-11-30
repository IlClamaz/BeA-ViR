using Beavir.Businesslogic.Utilities;
using UnityEngine;

namespace Beavir.Businesslogic.ViewModels
{
    /// <summary>
    /// Handles the enablement of the archaeometric room and the objects related to ItemAggregatorMedium. 
    /// </summary>
    public class ItemAggregatorMediumAL : ItemAggregatorMedium
    {
        [SerializeField] private ColliderProxy archaeoLabTrigger, archaeoWalls;
        [SerializeField] private GameObject archaeoLab;

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            archaeoLabTrigger.OnTriggerEnter_Action += (Collider c) =>
            {
                ObjectEnabler.Instance.ActivateGameObject(archaeoLab, 0);
                ObjectEnabler.Instance.DeactivateGameObject(dbReport.transform.GetChild(0).gameObject, 0);
            };
            archaeoWalls.OnTriggerExit_Action += (Collider c) =>
            {
                ObjectEnabler.Instance.DeactivateGameObject(archaeoLab, 0);
                ObjectEnabler.Instance.ActivateGameObject(dbReport.transform.GetChild(0).gameObject, 0);
            };
        }

        // Update is called once per frame
        new void Reset()
        {
            base.Reset();
            ObjectEnabler.Instance.DeactivateGameObject(archaeoLab, 0);
            ObjectEnabler.Instance.ActivateGameObject(dbReport.transform.GetChild(0).gameObject, 0);
        }
    }
}
