using System.Collections;
using UnityEngine;
using Mirror;
using Beavir.Persistance.Database;
using Beavir.Businesslogic.Utilities;

namespace Beavir.Businesslogic.Controllers
{
    /// <summary>
    /// This class manages the various controllers, owning an instance of them. 
    /// It enables the various controllers and makes sure that the database loads the data correctly. 
    /// If there is no Internet connection, it activates a screen informing of no network. 
    /// Finally, it enables the player and activates the starting environment.
    /// </summary>
    public class BeavirAppManager : NetworkBehaviour
    {
        private TransportManager transportManager;
        private UIManager uIManager;
        private SoundManager soundManager;
        private UserManager userManager;
        private DatabaseManager databaseManager;

        public static BeavirAppManager Instance { get; private set; }
        public TransportManager TransportManager { get => transportManager; }
        public UIManager UIManager { get => uIManager; }
        public SoundManager SoundManager { get => soundManager; }
        public UserManager UserManager { get => userManager; }
        public DatabaseManager DatabaseManager { get => databaseManager; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this);
            else Instance = this;

            transportManager = GetComponent<TransportManager>();
            uIManager = GetComponent<UIManager>();
            soundManager = GetComponent<SoundManager>();
            userManager = GetComponent<UserManager>();
            databaseManager = GetComponent<DatabaseManager>();

            databaseManager.enabled = true;
            transportManager.enabled = true;
            userManager.enabled = true;
            uIManager.enabled = true;
            soundManager.enabled = true;
        }
        
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(LoadingScene());
        }
        /// <summary>
        /// Check if the Database Manager has correctly filled the DB Reports with all images.
        /// </summary>
        private IEnumerator LoadingScene()
        {
            while (databaseManager.ProcessedImages < 27 && isServer)
            {
                if (databaseManager.ProcessedImages == -1)
                {
                    ObjectEnabler.Instance.ActivateGameObject(uIManager.NoInternetUI, 0);
                    break;
                }
                uIManager.ProgressBar.fillAmount = Mathf.Clamp01(databaseManager.ProcessedImages / .9f);
                yield return null;
            }

            ObjectEnabler.Instance.ActivateGameObject(transportManager.CurEnv.gameObject, 0);
            transportManager.UpdateLocHistory(transportManager.CurEnv.gameObject);

            ObjectEnabler.Instance.DeactivateGameObject(uIManager.ProgressBar.transform.parent.transform.parent.gameObject, 0);
            ObjectEnabler.Instance.ActivateGameObject(userManager.Player.gameObject, 0);
            transportManager.SendFadeCamera();
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
