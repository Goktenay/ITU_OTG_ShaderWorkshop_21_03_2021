using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionDemoScript : MonoBehaviour
{

    [SerializeField] private Camera _camera;
    [SerializeField] private Material _projectionMat;
    [SerializeField] private Vector3 _projectionBoxOffset;
    [SerializeField] private float _boxSize;
    
    private Renderer[] _renderers;
    private Mesh[] _meshes;

    private int _objectCount;
    
    void Start()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        var meshFilters = GetComponentsInChildren<MeshFilter>();
        _objectCount = meshFilters.Length;
        _meshes = new Mesh[_objectCount];

        
        for (int i = 0; i < _objectCount; i++)
        {
            _meshes[i] = meshFilters[i].mesh;
            _meshes[i].bounds = new Bounds(Vector3.zero, new Vector3(1,1,1) * 999);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalMatrix("_CamViewProjection", _camera.projectionMatrix  * _camera.worldToCameraMatrix );
        Shader.SetGlobalMatrix("_CamLocalToWorld", _camera.transform.localToWorldMatrix  );
    
        for (int i = 0; i < _objectCount; i++)
        {
            Renderer r = _renderers[i];
            Mesh m = _meshes[i];
            Graphics.DrawMesh(m, r.localToWorldMatrix, _projectionMat, 0 );
        }
    }

    private void OnDisable()
    {
        if (_meshes != null)
        {
            for (int i = 0; i < _objectCount; i++)
            {
                Mesh.Destroy(_meshes[i]);
            }

            _meshes = null;
        }
    }

    private void OnDrawGizmos()
    {
        ShowBox();
        
        void ShowBox()
        {
            if (_camera)
            {
                float size = _boxSize;
                Vector3 downBackLeft = new Vector3(-size, -size,-size * 0) + _projectionBoxOffset; 
                Vector3 downBackRight = new Vector3(size, -size,-size * 0) + _projectionBoxOffset; 
                Vector3 downForwardLeft = new Vector3(-size, -size,size * 0 )+ _projectionBoxOffset; 
                Vector3 downForwardRight = new Vector3(size, -size,size * 0) + _projectionBoxOffset; 
                
                Vector3 upBackLeft = new Vector3(-size, size,-size * 0) + _projectionBoxOffset; 
                Vector3 upBackRight = new Vector3(size, size,-size * 0) + _projectionBoxOffset; 
                Vector3 upForwardLeft = new Vector3(-size, size,size * 0)+ _projectionBoxOffset; 
                Vector3 upForwardRight = new Vector3(size, size,size * 0)+ _projectionBoxOffset;

                downBackLeft = _camera.transform.localToWorldMatrix * (new Vector4(downBackLeft.x, downBackLeft.y, downBackLeft.z, 1));
                downBackRight = _camera.transform.localToWorldMatrix * (new Vector4(downBackRight.x, downBackRight.y, downBackRight.z, 1));
                downForwardLeft = _camera.transform.localToWorldMatrix * (new Vector4(downForwardLeft.x, downForwardLeft.y, downForwardLeft.z, 1));
                downForwardRight = _camera.transform.localToWorldMatrix * (new Vector4(downForwardRight.x, downForwardRight.y, downForwardRight.z, 1));
                
                upBackLeft = _camera.transform.localToWorldMatrix * (new Vector4(upBackLeft.x, upBackLeft.y, upBackLeft.z, 1));
                upBackRight = _camera.transform.localToWorldMatrix * (new Vector4(upBackRight.x, upBackRight.y, upBackRight.z, 1));
                upForwardLeft = _camera.transform.localToWorldMatrix * (new Vector4(upForwardLeft.x, upForwardLeft.y, upForwardLeft.z, 1));
                upForwardRight = _camera.transform.localToWorldMatrix * (new Vector4(upForwardRight.x, upForwardRight.y, upForwardRight.z, 1));


                Gizmos.DrawLine(downBackLeft, downBackRight);
              //  Gizmos.DrawLine(downBackRight, downForwardRight);
               // Gizmos.DrawLine(downForwardRight, downForwardLeft);
              //  Gizmos.DrawLine(downForwardLeft, downBackLeft);
              
                Gizmos.DrawLine(upBackLeft, upBackRight);
               // Gizmos.DrawLine(upBackRight, upForwardRight);
               // Gizmos.DrawLine(upForwardRight, upForwardLeft);
               // Gizmos.DrawLine(upForwardLeft, upBackLeft);
                
                Gizmos.DrawLine(downBackLeft, upBackLeft);
                Gizmos.DrawLine(downBackRight, upBackRight);
               // Gizmos.DrawLine(downForwardLeft, upForwardLeft);
               // Gizmos.DrawLine(downForwardRight, upForwardRight);
            }
        }
        
    }
}
