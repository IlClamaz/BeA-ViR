using Sirenix.OdinInspector;
using UnityEngine;

public class CaveWall : MonoBehaviour
{
    [SerializeField] MachineConfig machineConfig;

    [HideInEditorMode]
    [SerializeField] private bool shouldBeActive;

    [Header("Debug Options")]
    [SerializeField] private bool forceActive;

    public bool ForceActive
    {
        get => forceActive;
        set => forceActive = value;
    }

    public bool ShouldBeActive => CheckMachineName();

    private void Start()
    {
        shouldBeActive = CheckMachineName();
    }

    private bool CheckMachineName()
    {
        return (forceActive || machineConfig.Name.Equals(Util.GetMachineName()));
    }
}
