using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;
using System.Linq;
using UnityEngine.Events;
using Mirror;
using Beavir.Businesslogic.Models;
using Beavir.Businesslogic.Utilities;

namespace Beavir.Businesslogic.Controllers
{
    /// <summary>
    /// Controller that manages travels inter/intra Environments
    /// </summary>
    public class TransportManager : NetworkBehaviour
    {
        [SerializeField] private float fadingDuration;
        [SerializeField] private GameObject startingPoint;
        [SerializeField] private Environment startingEnvironment;
        [SerializeField] private GameObject flyingJapan;
        [SerializeField] private GameEvent reset;

        private List<GameObject> locHistory;
        private Environment curEnv;
        private GameObject curObj;
        private bool cooldown = false;

        public Environment CurEnv { get => curEnv; }
        public GameObject CurObj { get => curObj; set => curObj = value; }
        public List<GameObject> LocHistory { get => locHistory; }
        public float FadingDuration { get => fadingDuration; }
        public GameObject StartingPoint { get => startingPoint; }
        public Environment StartingEnvironment { get => startingEnvironment; set => startingEnvironment = value; }
        public event UnityAction FadeInterEvent = delegate { };
        public event UnityAction FadeIntraEvent = delegate { };
        public event UnityAction FadeCameraEvent = delegate { };

        private void Start()
        {
            curEnv = startingEnvironment;
        }

        private void OnEnable()
        {
            BeavirAppManager.Instance.UserManager.InputReader.goBackHomeEvent += OnGoBackHome;
            BeavirAppManager.Instance.UserManager.InputReader.sightSeeingEvent += OnSightSeeing;
        }

        private void OnDisable()
        {
            BeavirAppManager.Instance.UserManager.InputReader.goBackHomeEvent -= OnGoBackHome;
            BeavirAppManager.Instance.UserManager.InputReader.sightSeeingEvent -= OnSightSeeing;
        }

        public void UpdateLocHistory(GameObject gameObject)
        {
            if (locHistory == null) locHistory = new List<GameObject>();
            if (!locHistory.Contains(gameObject)) locHistory.Add(gameObject);
        }

        public void SetJapan(bool isMoving)
        {
            if (flyingJapan.TryGetComponent(out MoveAroundHemi fj)) fj.IsMoving = isMoving;
        }

        public void SendFadeInter()
        {
            FadeInterEvent.Invoke();
            BeavirAppManager.Instance.UserManager.TemporarilyDisableAllInput(fadingDuration);
        }

        public void SendFadeIntra()
        {
            FadeIntraEvent.Invoke();
            BeavirAppManager.Instance.UserManager.TemporarilyDisableAllInput(fadingDuration);
        }

        public void SendFadeCamera()
        {
            FadeCameraEvent.Invoke();
            BeavirAppManager.Instance.UserManager.TemporarilyDisableAllInput(fadingDuration);
        }

        public void SendReset()
        {
            reset.Raise();
        }

        /// <summary>
        /// Method that handles the action "GoBackHome" after input request. ClientRPC is needed since clients don't receive user inputs (from host)
        /// </summary>
        [ClientRpc]
        private void OnGoBackHome()
        {
            if (curEnv.IsMainHall) Env2Home(startingEnvironment); //se in main hall
            else if (curEnv.Father != null && curEnv.Father.IsMainHall) SubEnv2Home(startingEnvironment); //se in timeline period
            else TravelTo(startingEnvironment); //se in un environment
        }

        /// <summary>
        /// Method that disables the current Environment and activates <param name="destinationEnvironment"></param>
        /// after fading the player camera e teleporting the player to the starting point.
        /// </summary>
        public void Env2Home(Environment destinationEnvironment)
        {
            SendFadeCamera();
            SendReset();
            ObjectEnabler.Instance.DeactivateGameObject(curEnv.gameObject, 0);
            ChangeEnvironment(destinationEnvironment);
            Teleport(startingPoint.transform.position, -startingPoint.transform.forward);
        }

        /// <summary>
        /// Method that disables the current SubEnvironment and activates <param name="destinationEnvironment"></param>
        /// after the reactivation of the objects disabled in the father Environment 
        /// and fading the player camera e teleporting the player to the starting point.
        /// </summary>
        public void SubEnv2Home(Environment destinationEnvironment)
        {
            SendFadeCamera();
            SendReset();
            ObjectEnabler.Instance.DeactivateGameObject(curEnv.Father.gameObject, 0);
            ObjectEnabler.Instance.DeactivateGameObject(curEnv.gameObject, 0);
            var DisabledInSub = curEnv.DisabledInSub;
            foreach (var item in DisabledInSub) ObjectEnabler.Instance.ActivateGameObject(item, 0);
            ChangeEnvironment(destinationEnvironment);
            Teleport(startingPoint.transform.position, -startingPoint.transform.forward);
        }

        /// <summary>
        /// Method that disables the current Environment and activates <param name="destinationEnvironment"></param>
        /// after fading all the objects.
        /// </summary>
        public void Home2Env(Environment destinationEnvironment)
        {
            SendFadeInter();
            SendReset();
            ObjectEnabler.Instance.DeactivateGameObject(curEnv.gameObject, fadingDuration);
            ChangeEnvironment(destinationEnvironment);
        }

        /// <summary>
        /// Method that disables the current SubEnvironment and activates <param name="destinationEnvironment"></param>
        /// after fading some objects in the Environment and reactivating the objects disabled in the Father. 
        /// In case that the SubEnvironment Father is not the destination Environment, Home2Env is called.
        /// </summary>
        public void SubEnv2Env(Environment destinationEnvironment)
        {
            SendFadeIntra();
            ObjectEnabler.Instance.DeactivateGameObject(curEnv.gameObject, fadingDuration);
            var DisabledInSub = curEnv.DisabledInSub;
            foreach (var item in DisabledInSub)
                ObjectEnabler.Instance.ActivateGameObject(item, 0);
            if (curEnv.Father != destinationEnvironment)
            {
                curEnv = curEnv.Father;
                Home2Env(destinationEnvironment);
            }
            else ChangeEnvironment(destinationEnvironment);
        }

        /// <summary>
        /// Method that disables the current Environment and activates <param name="destinationEnvironment"></param>
        /// after fading and disabling some objects in the Environment. 
        /// </summary>
        public void Env2SubEnv(Environment destinationEnvironment)
        {
            SendFadeIntra();
            var DisabledInSub = destinationEnvironment.DisabledInSub;
            foreach (var item in DisabledInSub)
                ObjectEnabler.Instance.DeactivateGameObject(item, fadingDuration);
            ChangeEnvironment(destinationEnvironment);
        }

        /// <summary>
        /// Method that disables the current SubEnvironment and activates <param name="destinationEnvironment"></param>
        /// after fading some objects in the Environment and reactivating the objects disabled in the Father but not disabled in destination Environment. 
        /// </summary>
        public void SubEnv2SubEnv(Environment destinationEnvironment)
        {
            SendFadeIntra();
            ObjectEnabler.Instance.DeactivateGameObject(curEnv.gameObject, fadingDuration);
            List<GameObject> disabledInSub = curEnv.DisabledInSub;
            List<GameObject> disabledInSubDest = destinationEnvironment.DisabledInSub;
            List<GameObject> intersected = disabledInSub.Intersect(disabledInSubDest).ToList();
            bool toBeEnabled;
            for (int i = 0; i < disabledInSub.Count; i++)
            {
                toBeEnabled = true;
                for (int j = 0; j < intersected.Count && toBeEnabled; j++)
                    if (intersected[j].name == disabledInSub[i].name) toBeEnabled = false;
                if (toBeEnabled) ObjectEnabler.Instance.ActivateGameObject(disabledInSub[i], 0);
            }

            foreach (var item in disabledInSubDest)
                ObjectEnabler.Instance.DeactivateGameObject(item, fadingDuration);
            ChangeEnvironment(destinationEnvironment);
        }
        /// <summary>
        /// Method that updates the current Environment with <param name="destinationEnvironment"></param> and activates it.
        /// It also changes the mobility of Flying Japan, changes the Sky color, activate Title UI and manages Audio.
        /// </summary>
        private void ChangeEnvironment(Environment destinationEnvironment)
        {
            curEnv = destinationEnvironment;
            ObjectEnabler.Instance.ActivateGameObject(curEnv.gameObject, 0);
            UpdateLocHistory(curEnv.gameObject);
            SetJapan(curEnv.IsJapanMoving);
            curEnv.UpdateSky();
            curEnv.ManageAudio(BeavirAppManager.Instance.SoundManager.AudioIsOn);
            BeavirAppManager.Instance.UIManager.ShowTitleUI();
        }

        public void TravelTo(Environment destinationEnvironment)
        {
            if (destinationEnvironment != curEnv)
            {
                if (destinationEnvironment.IsMainHall)
                {
                    if (curEnv.Father == null) Env2Home(destinationEnvironment);
                    else if (curEnv.Father == destinationEnvironment) SubEnv2Env(destinationEnvironment); //per i subenv della mainhall
                    else SubEnv2Home(destinationEnvironment);
                }
                else
                {
                    if (destinationEnvironment.Father == null)
                    {
                        if (curEnv.Father == null) Home2Env(destinationEnvironment);
                        else SubEnv2Env(destinationEnvironment);
                    }
                    else
                    {
                        if (curEnv.Father == null) Env2SubEnv(destinationEnvironment);
                        else if (curEnv.Father == destinationEnvironment.Father) SubEnv2SubEnv(destinationEnvironment);
                    }
                }
            }
        }

        /// <summary>
        /// Method that manages the Guided Tour, teleporting the player according to the sight points in the current Environment.
        /// If the current Environment has no sight Points but has a father, it uses the father's ones. 
        /// </summary>
        /// <param name="valuePressed"></param> is the key pressed by the user and leads to next or prev item.
        [ClientRpc]
        private void OnSightSeeing(float valuePressed) // -1 is Prev, 1 is Next
        {
            var sightPoints = curEnv.SightPoints;
            var curSightPoint = curEnv.CurSightPoint;
            if (curEnv.Father != null && sightPoints.Count == 0)
            {
                sightPoints = curEnv.Father.SightPoints;
                curSightPoint = curEnv.Father.CurSightPoint;
            }

            if (sightPoints != null && sightPoints.Count > 0 && !cooldown)
            {
                SendFadeCamera();
                SendReset();

                cooldown = true;
                if (valuePressed < 0) //Travel to PREV
                {
                    if (curSightPoint == 0) curSightPoint = sightPoints.Count;
                    curSightPoint--;
                }
                else //Travel to NEXT
                {
                    curSightPoint++;
                    if (curSightPoint == sightPoints.Count) curSightPoint = 0;
                }
                Teleport(sightPoints[curSightPoint].transform.position, -sightPoints[curSightPoint].transform.forward);
                Invoke(nameof(CooldownReset), fadingDuration + 0.1f);

                if (curEnv.Father != null && curEnv.SightPoints.Count == 0)
                    curEnv.Father.CurSightPoint = curSightPoint;
                else curEnv.CurSightPoint = curSightPoint;
            }
        }

        private void CooldownReset()
        {
            cooldown = false;
        }
        public void Teleport(Vector3 destination, Vector3 lookDirection)
        {
            BeavirAppManager.Instance.UserManager.UpdatePlayerPosition(destination);
            BeavirAppManager.Instance.UserManager.UpdatePlayerRotation(lookDirection);
            SendFadeCamera();
        }
    }
}

