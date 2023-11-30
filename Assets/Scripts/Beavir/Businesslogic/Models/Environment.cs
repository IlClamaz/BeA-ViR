using Beavir.Businesslogic.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Beavir.Businesslogic.Models
{
    /// <summary>
    /// An Environment is an object that hosts objects 
    /// </summary>
    public class Environment : MonoBehaviour
    {
        [Header("SubEnvironment Fields")]
        [SerializeField] private Environment father;
        [SerializeField] private List<GameObject> disabledInSub;

        [Header("Guided Tour Fields")]
        [SerializeField] private List<GameObject> sightPoints;
        private int curSightPoint = 0;

        [Header("Other Fields")]
        [SerializeField] private bool isMainHall = false;
        [SerializeField] private bool isJapanMoving = true;
        [SerializeField] private Material skyMaterial;
        [SerializeField] private List<GameObject> overlays;
        [SerializeField] private List<GameObject> titleUIs;
        [SerializeField] private GameObject soundtrack;


        public List<GameObject> SightPoints { get => sightPoints; }
        public int CurSightPoint { get => curSightPoint; set => curSightPoint = value; }
        public bool IsMainHall { get => isMainHall; }
        public bool IsJapanMoving { get => isJapanMoving; }
        public List<GameObject> DisabledInSub { get => disabledInSub; }
        public Environment Father { get => father; }


        public void ManageAudio(bool audioIsOn)
        {
            if (audioIsOn) ObjectEnabler.Instance.ActivateGameObject(soundtrack, 0);
            else ObjectEnabler.Instance.DeactivateGameObject(soundtrack, 0);
        }

        public void ShowOverlay(bool overlayIsOn)
        {
            if (overlayIsOn) ObjectEnabler.Instance.ActivateGameObject(overlays, 0);
            else ObjectEnabler.Instance.DeactivateGameObject(overlays, 0);
        }

        public void ShowTitleUI()
        {
            if (titleUIs != null)
            {
                ObjectEnabler.Instance.ActivateGameObject(titleUIs, 0);
                ObjectEnabler.Instance.DeactivateGameObject(titleUIs, 2);
            }
        }

        public void UpdateSky()
        {
            if (skyMaterial != null)
            {
                RenderSettings.skybox = skyMaterial;
                DynamicGI.UpdateEnvironment();
            }
        }
    }
}
