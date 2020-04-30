﻿using UnityEngine;

[ExecuteInEditMode]
public class MirrorSystem : MonoBehaviour {

    const int MAX_REFLECTORS = 16;

    public Transform ellipsoids;

    int         reflectors = 1;
    Matrix4x4[] ellipseWorldToLocal = new Matrix4x4[MAX_REFLECTORS];
    Matrix4x4[] ellipseLocalToWorld = new Matrix4x4[MAX_REFLECTORS];
    float    [] minorAxes           = new float    [MAX_REFLECTORS];
    float    [] majorAxes           = new float    [MAX_REFLECTORS];
    float    [] isInside            = new float    [MAX_REFLECTORS];
    Vector4  [] boundsMin           = new Vector4  [MAX_REFLECTORS];
    Vector4  [] boundsMax           = new Vector4  [MAX_REFLECTORS];

    Material  reflectorShader;

    void Start() {
        // Depth is necessary to intersect with objects...
        FindObjectOfType<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }

    void Update() {
        if(reflectorShader == null) reflectorShader = GetComponent<Renderer>().sharedMaterial;

#if UNITY_EDITOR
        UnityEditor.Tools.hidden = false;
#endif

        reflectors = 0;
        foreach(Transform ell in ellipsoids) {
            if (!ell.gameObject.activeInHierarchy) continue;

            Ellipsoid ellipsoid = ell.GetComponent<Ellipsoid>();
            ellipseLocalToWorld[reflectors] = ellipsoid.localToWorld;
            ellipseWorldToLocal[reflectors] = ellipsoid.worldToLocal;
            minorAxes[reflectors]           = ellipsoid.minorAxis;
            majorAxes[reflectors]           = ellipsoid.majorAxis;
            isInside [reflectors]           = ellipsoid.isInside ? 1f : -1f; // -1 is exterior reflection, 1 is interior reflection
            boundsMin[reflectors]           = ellipsoid.intersectionBounds.min;
            boundsMax[reflectors]           = ellipsoid.intersectionBounds.max;

            reflectors++;
        }

        reflectorShader.SetFloat      ("_Reflectors",     reflectors);
        reflectorShader.SetMatrixArray("_worldToSpheres", ellipseWorldToLocal);
        reflectorShader.SetMatrixArray("_sphereToWorlds", ellipseLocalToWorld);
        reflectorShader.SetFloatArray ("_MajorAxes",      majorAxes);
        reflectorShader.SetFloatArray ("_MinorAxes",      minorAxes);
        reflectorShader.SetFloatArray ("_IsInsides",      isInside);
        reflectorShader.SetVectorArray("_BoundsMin",      boundsMin);
        reflectorShader.SetVectorArray("_BoundsMax",      boundsMax);
    }
}
