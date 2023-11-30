using System.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Beavir.Businesslogic.Utilities
{
    public class ObjectEnabler : MonoBehaviour
    {
        public static ObjectEnabler Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(this);
            else Instance = this;
        }
        public void ActivateGameObject(GameObject targetGameObject, float delay)
        {
            if (delay > 0)
            {
                StartCoroutine(ExecuteAfterTime(delay, () =>
                {
                    if (targetGameObject && !targetGameObject.activeSelf) targetGameObject.SetActive(true);
                }));
            }
            else
            {
                if (targetGameObject && !targetGameObject.activeSelf) targetGameObject.SetActive(true);
            }
        }

        public void ActivateGameObject(List<GameObject> targetGameObjects, float delay)
        {
            if (delay > 0)
            {
                StartCoroutine(ExecuteAfterTime(delay, () =>
                {
                    foreach (var item in targetGameObjects) if (item && !item.activeSelf) item.SetActive(true);
                }));
            }
            else
            {
                foreach (var item in targetGameObjects) if (item && !item.activeSelf) item.SetActive(true);
            }
        }

        public void DeactivateGameObject(GameObject targetGameObject, float delay)
        {
            if (delay > 0)
            {
                StartCoroutine(ExecuteAfterTime(delay, () =>
                {
                    if (targetGameObject && targetGameObject.activeSelf) targetGameObject.SetActive(false);
                }));
            }
            else
            {
                if (targetGameObject && targetGameObject.activeSelf) targetGameObject.SetActive(false);
            }

        }

        public void DeactivateGameObject(List<GameObject> targetGameObjects, float delay)
        {
            if (delay > 0)
            {
                StartCoroutine(ExecuteAfterTime(delay, () =>
                {
                    foreach (var item in targetGameObjects) if (item && item.activeSelf) item.SetActive(false);
                }));
            }
            else
            {
                foreach (var item in targetGameObjects) if (item && item.activeSelf) item.SetActive(false);
            }

        }

        public void ToggleGameObject(GameObject targetGameObject, float delay)
        {
            if (delay > 0)
            {
                StartCoroutine(ExecuteAfterTime(delay, () =>
                {
                    if (targetGameObject) targetGameObject.SetActive(!targetGameObject.activeSelf);
                }));
            }
            else
            {
                if (targetGameObject) targetGameObject.SetActive(!targetGameObject.activeSelf);
            }

        }

        public void ToggleGameObject(List<GameObject> targetGameObjects, float delay)
        {
            if (delay > 0)
            {
                StartCoroutine(ExecuteAfterTime(delay, () =>
                {
                    foreach (var item in targetGameObjects) if (item) item.SetActive(!item.activeSelf);
                }));
            }
            else
            {
                foreach (var item in targetGameObjects) if (item) item.SetActive(!item.activeSelf);
            }

        }

        private IEnumerator ExecuteAfterTime(float time, Action task)
        {
            yield return new WaitForSeconds(time);
            task();
        }
    }
}
