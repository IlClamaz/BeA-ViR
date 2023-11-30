using Mirror;
using UnityEngine;

public class ClusterNode : NetworkBehaviour
{
    [Header("Cluster Actor Components")]
    public Camera cam;

    [SyncVar]
    int playerNo;

    [SyncVar(hook = nameof(OnEyeSeparationChanged))]
    float eyeSeparation;

    public override void OnStartServer()
    {
        base.OnStartServer();

        // Set SyncVar values
        playerNo = connectionToClient.connectionId;
        eyeSeparation = 0.067f;

        //Debug.Log($"[SERVER] OnStart! Setting EyeSeparation:{eyeSeparation}");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        //Debug.Log($"[CLIENT] OnStart! -> Apply eye separation value... {eyeSeparation.ToString("F3")}");
    }

    #region SyncVar Hooks
    private void OnEyeSeparationChanged(float oldEyeSeparation, float newEyeSeparation)
    {
        // Show the data in the UI
        //Debug.Log($"[CLIENT] On Eye separation change: {newEyeSeparation.ToString("F3")}");


        if (cam != null)
        {
            // Apply Eye Separation to local Camera
            cam.stereoSeparation = eyeSeparation;
        }
    }

    #endregion
}