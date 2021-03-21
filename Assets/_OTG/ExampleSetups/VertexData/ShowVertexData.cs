using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class ShowVertexData : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private Material _material;
    
    [Header("Settings")] 
    [SerializeField] private bool _showVertexPos;
    [Space(10)] 
    [SerializeField] private bool _showNormals;
    [SerializeField] private float _normalLengthMult = 0.1f;
    [Space(10)]
    [SerializeField] private bool _showTangents;
    [SerializeField] private float _tangentLengthMult = 0.1f;

    [OnValueChanged("OnExtrudeAmountChanged")]
    [Space(10)] [SerializeField] [Range(0f,1f)] private float _extrudeAmount = 0;
    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnExtrudeAmountChanged(float oldVal, float newVal)
    {
        if (_material)
        {
            _material.SetFloat("_ExtrudeAmount", newVal);
        }
    }
    

    private void OnDrawGizmos()
    {
        if (_meshFilter && _meshFilter.sharedMesh)
        {
            Mesh mesh = _meshFilter.sharedMesh;

            Matrix4x4 objToWorld = transform.localToWorldMatrix;

            int vertCount = mesh.vertices.Length;
            
            // Normals
            Vector3[] posValsLocal = mesh.vertices;
            Vector3[] posValsWorld = new Vector3[vertCount];
            for (int i = 0; i < vertCount; i++)
            {
                Vector4 posVal = posValsLocal[i];
                posVal.w = 1;
                posVal = objToWorld * posVal;
                posValsWorld[i] = posVal;
            }
            
            
            // Normals
            Vector3[] norms = mesh.normals;
            for (int i = 0; i < vertCount; i++)
            {
                Vector4 normV4 = norms[i];
                normV4.w = 0;
                norms[i] = objToWorld * normV4;
            }
            
            
            /// Tangents
            Vector4[] tangents = mesh.tangents;

            for (int i = 0; i < vertCount; i++)
            {
                Vector4 tangentV4 = tangents[i];
                tangentV4.w = 0;
                Vector4 tangentWorld =  objToWorld * tangentV4;
                tangentWorld.w = tangents[i].w;
                tangents[i] = tangentWorld;
            }
            
            
            
            if(_showNormals)
                ShowNormals();
            
            if(_showTangents)
                ShowTangents();

            if (_showVertexPos)
            {
                ShowVertexPositions();
            }
            
            
      
            
            void ShowNormals()
            {
                Vector3 camPos = Camera.current.transform.position;
                Gizmos.color = Color.blue;
                for (int i = 0; i < vertCount; i++)
                {
                    Vector3 pos = posValsWorld[i] + _extrudeAmount * norms[i].normalized;
                    Vector3 camToPos = (pos + _normalLengthMult * norms[i] - camPos).normalized;
                   // Gizmos.DrawRay(pos , norms[i] * _normalLengthMult);
                    DrawGizmoVector(pos, norms[i], _normalLengthMult, camToPos);
                }
                
            }

            void ShowTangents()
            {
                Gizmos.color = Color.red;
                Vector3 camPos = Camera.current.transform.position;
                for (int i = 0; i < vertCount; i++)
                {
                    Vector3 pos = posValsWorld[i] + _extrudeAmount * norms[i].normalized;
                    Vector3 tangentVec = tangents[i] * tangents[i].w;
                    tangentVec.Normalize();
                    Vector3 camToPos = (tangentVec * _tangentLengthMult + pos - camPos).normalized;

                    DrawGizmoVector(pos, tangentVec, _tangentLengthMult, camToPos);
                  //  Gizmos.DrawRay(pos, _tangentLengthMult * tangents[i].w * tangents[i]);
                }
            }


            void ShowVertexPositions()
            {
                Gizmos.color = new Color(1f, 0.47f, 0f);
                Vector3 camPos = Camera.current.transform.position;
                for (int i = 0; i < vertCount; i++)
                {
                    Vector3 pos = posValsWorld[i] + _extrudeAmount * norms[i].normalized;
                    Vector3 posToCam = (camPos - pos).normalized;
                    
                    DrawGizmoVector(Vector3.zero, pos.normalized, pos.magnitude, posToCam);
                   // Gizmos.DrawRay(pos, _tangentLengthMult * tangents[i].w * tangents[i]);
                }
            }
            
        }
    }
    
    
    void DrawGizmoVector(Vector3 pos, Vector3 dir, float size, Vector3 up)
    {
        Gizmos.DrawRay(pos, dir * size);
        Vector3 crossVal =Vector3.Cross(dir, up);
        
        Vector3 arrowHand01 = Vector3.Normalize(Vector3.Lerp(-dir, crossVal, 0.3f));
        Vector3 arrowHand02 = Vector3.Normalize(Vector3.Lerp(-dir, -crossVal, 0.3f));
        
        Gizmos.DrawRay(pos + dir * size,   0.1f * arrowHand01 *size);
        Gizmos.DrawRay(pos + dir * size,   0.1f * arrowHand02 * size);
    }
}
