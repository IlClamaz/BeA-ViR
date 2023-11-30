using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StereoRig : MonoBehaviour
{
    private const float DefaultSeparation = 0.067f;

    [SerializeField] private Transform leftEyeOrigin;
    [SerializeField] private Transform rightEyeOrigin;

    [Header("Properties")]
    [SerializeField] float separation = DefaultSeparation;

    private float previousSeparation;

    void Start()
    {
        Debug.Assert(rightEyeOrigin != null, "rightCamera can't be null!");
        Debug.Assert(leftEyeOrigin != null, "leftCamera can't be null!");

        ApplyLayout();
    }

    void Update()
    {
        if (Application.isPlaying == false)
        {
            return;
        }
    }

    [Button]
    private void ApplyLayout()
    {
        leftEyeOrigin.transform.localPosition = new Vector3(-separation / 2, 0.0f, 0.0f);
        rightEyeOrigin.transform.localPosition = new Vector3(separation / 2, 0.0f, 0.0f);

        previousSeparation = separation;
    }

    [Button]
    private void RestoreDefault()
    {
        separation = 0.067f;
        ApplyLayout();
    }
}