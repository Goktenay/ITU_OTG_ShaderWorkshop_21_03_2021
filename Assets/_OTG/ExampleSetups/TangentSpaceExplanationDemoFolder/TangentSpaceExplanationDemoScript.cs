using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TangentSpaceExplanationDemoScript : MonoBehaviour
{
    

    [SerializeField] private Transform _localToWorldPivotTransform;
    [SerializeField] private Transform _tangentNormalVecTransform;
    [SerializeField] private Material _quadColorMat;

    [Space(20)] [Header("Settings")]
    [SerializeField] private bool _showVertexNorm;
    [SerializeField] private bool _showVertexTangent;
    [SerializeField] private bool _showVertexBitangent;
    [Space(10)]
    [SerializeField] private bool _showNormalMapVec;

    [HideInInspector][SerializeField] private Vector3 _previousColorVec;



    private void OnDrawGizmos()
    {
        Vector3 camPos = Camera.current.transform.position;

        Vector3 tangentNormalVecNorm = _tangentNormalVecTransform.localPosition.normalized;
        Vector3 zUpTangentNormalVecNorm = new Vector3(tangentNormalVecNorm.x, tangentNormalVecNorm.z, tangentNormalVecNorm.y);

        Color tangentNormalCol = new Color(zUpTangentNormalVecNorm.x * 0.5f + 0.5f, zUpTangentNormalVecNorm.y * 0.5f + 0.5f, zUpTangentNormalVecNorm.z,1);
        

        SetNormalColor();
        DrawZeroDirs();
        DrawNormalColorVec();
            

        
        
        void DrawNormalColorVec()
        {
            Gizmos.color = tangentNormalCol;
            Vector3 vecPos = _tangentNormalVecTransform.localToWorldMatrix *tangentNormalVecNorm ;
          
            if(_showNormalMapVec)
                DrawGizmoVector(_tangentNormalVecTransform.parent.position, vecPos, 1 * 0.5f, tangentNormalCol, camPos);
        }
        
        
        void SetNormalColor()
        {
            Vector3 colToVec = new Vector3(tangentNormalCol.r, tangentNormalCol.g, tangentNormalCol.b);

            if ((colToVec - _previousColorVec).magnitude > 0.1)
            {
                _previousColorVec = colToVec;
                _quadColorMat.SetColor("_Color", tangentNormalCol);
            }
        }
        
        
        void DrawZeroDirs()
        {
            if(_showVertexNorm)
                DrawGizmoVectorWithMatrix(Vector3.zero, Vector3.forward, 1 * 0.5f, Color.blue, camPos);
            
            if(_showVertexBitangent)
                DrawGizmoVectorWithMatrix(Vector3.zero, Vector3.up, 1 * 0.5f, Color.green, camPos);
            
            if(_showVertexTangent)
            DrawGizmoVectorWithMatrix(Vector3.zero, Vector3.right, 1 * 0.5f, Color.red, camPos);
        }
    }

    void DrawGizmoVector(Vector3 pos, Vector3 dir, float size, Color col, Vector3 camPos)
    {
        Vector3 posToCamDir = (camPos - pos + dir * size).normalized;
        
        Gizmos.color = col;
        Gizmos.DrawRay(pos, dir * size);
        Vector3 crossVal =Vector3.Cross(dir, posToCamDir);
        
        Vector3 arrowHand01 = Vector3.Normalize(Vector3.Lerp(-dir, crossVal, 0.3f));
        Vector3 arrowHand02 = Vector3.Normalize(Vector3.Lerp(-dir, -crossVal, 0.3f));
        
        Gizmos.DrawRay(pos + dir * size, size * 0.1f * arrowHand01);
        Gizmos.DrawRay(pos + dir * size, size * 0.1f * arrowHand02);
    }

    void DrawGizmoVectorWithMatrix(Vector3 pos, Vector3 dir, float size, Color col, Vector3 camPos)
    {
        pos = _localToWorldPivotTransform.localToWorldMatrix * new Vector4(pos.x, pos.y, pos.z, 1);
        dir = _localToWorldPivotTransform.localToWorldMatrix * new Vector4(dir.x, dir.y, dir.z, 0);
        
        Vector3 posToCamDir = (camPos - pos + dir * size).normalized;
        
        Gizmos.color = col;
        Gizmos.DrawRay(pos, dir * size);
        Vector3 crossVal =Vector3.Cross(dir, posToCamDir);
        
        Vector3 arrowHand01 = Vector3.Normalize(Vector3.Lerp(-dir, crossVal, 0.3f));
        Vector3 arrowHand02 = Vector3.Normalize(Vector3.Lerp(-dir, -crossVal, 0.3f));
        
        Gizmos.DrawRay(pos + dir * size, size * 0.1f * arrowHand01);
        Gizmos.DrawRay(pos + dir * size, size * 0.1f * arrowHand02);
    }
}
