using Beavir.Businesslogic.Controllers;
using System.Collections;
using UnityEngine;

namespace Beavir.Businesslogic.Utilities
{
    public class FadingAnimation : MonoBehaviour
    {
        private float fadingDuration;
        private enum Event { fadeInter, fadeIntra, both, fadeCamera }
        [SerializeField] private Event selectedEvent = Event.fadeInter;

        private const string standardShader = "Standard";
        private const string standardShaderColor = "_Color";
        private const string leavesShader = "Nature/Tree Creator Leaves";
        private const string glassShader = "Effects/FrostedGlass";

        private void OnEnable()
        {
            fadingDuration = BeavirAppManager.Instance.TransportManager.FadingDuration;
            if (selectedEvent == Event.fadeIntra || selectedEvent == Event.both)
                BeavirAppManager.Instance.TransportManager.FadeIntraEvent += FadeOut;
            if (selectedEvent == Event.fadeInter || selectedEvent == Event.both)
                BeavirAppManager.Instance.TransportManager.FadeInterEvent += FadeOut;
            if (selectedEvent == Event.fadeCamera)
                BeavirAppManager.Instance.TransportManager.FadeCameraEvent += FadeOut;
            else
            {
                ChangeRenderingMode(gameObject, true);
                FadeIn();
            }
        }

        private void OnDisable()
        {
            if (selectedEvent == Event.fadeIntra || selectedEvent == Event.both)
                BeavirAppManager.Instance.TransportManager.FadeIntraEvent -= FadeOut;
            if (selectedEvent == Event.fadeInter || selectedEvent == Event.both)
                BeavirAppManager.Instance.TransportManager.FadeInterEvent -= FadeOut;
            if (selectedEvent == Event.fadeCamera)
                BeavirAppManager.Instance.TransportManager.FadeCameraEvent -= FadeOut;
        }

        public void FadeOut()
        {
            StartCoroutine(FadingCoroutine(gameObject, 1f, 0f, fadingDuration));
        }
        public void FadeIn()
        {
            StartCoroutine(FadingCoroutine(gameObject, 0f, 1f, fadingDuration));
        }
        private IEnumerator FadingCoroutine(GameObject subject, float start_value, float value_end, float duration)
        {
            ChangeRenderingMode(subject, true);
            float elapsed = 0.0f;
            Color currentColor = new Color();
            Vector2 currentTexScale = new Vector2(0, 0);
            float currentGlassValue;
            while (elapsed < duration)
            {
                Material[] mats = subject.GetComponent<MeshRenderer>().materials;
                foreach (var mat in mats)
                {
                    float currentAlpha = Mathf.MoveTowards(start_value, value_end, elapsed / duration);
                    if (mat.shader.name == standardShader)
                    {
                        currentColor.r = mat.color.r;
                        currentColor.g = mat.color.g;
                        currentColor.b = mat.color.b;
                        currentColor.a = currentAlpha;
                        mat.SetColor(standardShaderColor, currentColor);
                    }
                    else if (mat.shader.name == leavesShader)
                    {
                        currentTexScale.x = currentTexScale.y = currentAlpha;
                        mat.mainTextureScale = currentTexScale;
                    }
                    else if (mat.shader.name == glassShader)
                    {
                        currentGlassValue = currentAlpha;
                        mat.SetFloat("_FrostIntensity", currentAlpha);
                    }
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            Material[] materials = subject.GetComponent<MeshRenderer>().materials;
            foreach (var mat in materials)
            {
                if (mat.shader.name == standardShader)
                {
                    currentColor.r = mat.color.r;
                    currentColor.g = mat.color.g;
                    currentColor.b = mat.color.b;
                    currentColor.a = value_end;
                    mat.SetColor(standardShaderColor, currentColor);
                }
                else if (mat.shader.name == leavesShader)
                {
                    currentTexScale.x = currentTexScale.y = value_end;
                    mat.mainTextureScale = currentTexScale;
                }
                else if (mat.shader.name == glassShader)
                {
                    currentGlassValue = value_end;
                    mat.SetFloat("_FrostIntensity", value_end);
                }
            }

            if (value_end == 1) ChangeRenderingMode(subject, false);
        }


        private void ChangeRenderingMode(GameObject subject, bool fade)
        {
            Material[] mats = subject.GetComponent<MeshRenderer>().materials;
            foreach (var mat in mats)
            {
                if (fade) ToFadeMode(mat);
                else ToOpaqueMode(mat);
            }
        }

        private void ToOpaqueMode(Material material)
        {
            if (material.shader.name == standardShader)
            {
                //Debug.Log("ToOpaqueMode");
                material.SetFloat("_Mode", 0);
                material.SetOverrideTag("RenderType", "");
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
            }

        }

        private void ToFadeMode(Material material)
        {
            switch (material.shader.name)
            {
                case standardShader:
                    if (material.GetFloat("_Mode") == 0)
                    {
                        material.SetFloat("_Mode", 2);
                        material.SetOverrideTag("RenderType", "Fade");
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_ZWrite", 0);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.EnableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    }
                    Color currentCol = material.color;
                    material.color = new Color(currentCol.r, currentCol.g, currentCol.b, 0);
                    break;
                case leavesShader:
                    material.mainTextureScale = new Vector2(0, 0);
                    break;
                case glassShader:
                    material.SetFloat("_FrostIntensity", 0.0f);
                    break;
            }
        }
    }
}
