using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalVsPointVectorDemoScript : MonoBehaviour
{
    [SerializeField] private Transform _pivotTransform;

    [Header("Settings")] 
    [SerializeField] private bool _zeroLocationVectorsBool;
    [Space(10)]
    [SerializeField] private bool _positionVecBool;
    [Space(10)]
    [SerializeField] private bool _akNormalVecBool;
    [SerializeField] private bool _zeroNormalVecBool;
    
    
    private void OnDrawGizmos()
    {
        Camera cam = Camera.current;
        Vector3 camToPosDir = (_pivotTransform.position - cam.transform.position).normalized;

        if (_zeroLocationVectorsBool)
        {
            DrawGizmoVector(Vector3.zero, Vector3.up, 1,Color.green, (Vector3.up-cam.transform.position).normalized ); 
            DrawGizmoVector(Vector3.zero, Vector3.forward, 1,Color.blue, (Vector3.forward-cam.transform.position).normalized ); 
            DrawGizmoVector(Vector3.zero, Vector3.right, 1,Color.red, (Vector3.right-cam.transform.position).normalized );
        }


        if(_positionVecBool)
            DrawGizmoVector(Vector3.zero, _pivotTransform.position.normalized, _pivotTransform.position.magnitude, new Color(1f, 0.59f, 0f), camToPosDir);
        
       // Vector3 camToNormalVecDir = (_pivotTransform.position - cam.transform.position + _pivotTransform.forward).normalized;
        
        if(_akNormalVecBool)
            DrawGizmoVector(_pivotTransform.position, _pivotTransform.forward, 1, new Color(0.03f, 1f, 0.97f), Vector3.up);
        
        if(_zeroNormalVecBool)
            DrawGizmoVector(Vector3.zero, _pivotTransform.forward, 1, new Color(0.03f, 1f, 0.97f), Vector3.up);
        
    }
    
    void DrawGizmoVector(Vector3 pos, Vector3 dir, float size, Color col, Vector3 up)
    {
        Gizmos.color = col;
        Gizmos.DrawRay(pos, dir * size);
        Vector3 crossVal =Vector3.Cross(dir, up);
        
        Vector3 arrowHand01 = Vector3.Normalize(Vector3.Lerp(-dir, crossVal, 0.3f));
        Vector3 arrowHand02 = Vector3.Normalize(Vector3.Lerp(-dir, -crossVal, 0.3f));
        
        Gizmos.DrawRay(pos + dir * size,   0.1f * arrowHand01);
        Gizmos.DrawRay(pos + dir * size,   0.1f * arrowHand02);
    }
}
