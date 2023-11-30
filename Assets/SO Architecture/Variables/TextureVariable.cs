using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjectArchitecture
{
    [System.Serializable]
    public class TextureEvent : UnityEvent<Texture> { }

    [CreateAssetMenu(
        fileName = "TextureVariable.asset",
        menuName = SOArchitecture_Utility.VARIABLE_SUBMENU + "texture",
        order = SOArchitecture_Utility.ASSET_MENU_ORDER_COLLECTIONS + 120)]
    public sealed class TextureVariable : BaseVariable<Texture, TextureEvent>
    {
    }
}
