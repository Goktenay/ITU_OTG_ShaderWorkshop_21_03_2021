using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DotProductDemoScript : MonoBehaviour
{

    [SerializeField] private TextMeshPro _angleTmp;
    [SerializeField] private TextMeshPro _dotProduct;
    [SerializeField] private Transform _dynamicVecPivot;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        DrawCircle(1, Color.white);
        Vector3 camPos = Camera.current.transform.position;
        
        Vector3 staticVec = new Vector3(1,0,0);
        DrawGizmoVector(Vector3.zero, staticVec, 1, Color.green, (staticVec - camPos).normalized);

        Vector3 dynamicVecNorm = _dynamicVecPivot.position.normalized;
        DrawGizmoVector(Vector3.zero, dynamicVecNorm, 1, Color.red, (dynamicVecNorm - camPos).normalized);


        float dotVal = Vector3.Dot(dynamicVecNorm, staticVec);
        
        _dotProduct.text = "Dot Product = " + ((float)(Mathf.RoundToInt(dotVal * 100))) / 100;
        _angleTmp.text =  "Angle Between = " +  ((float)(Mathf.RoundToInt(Mathf.Acos(dotVal) * Mathf.Rad2Deg * 1))) / 1 ;
        
        void DrawCircle(float radius, Color col)
        {
            Vector3 firstLinePos = new Vector3(1, 0, 0) * radius;
            Vector3 lastLinePos = firstLinePos;
            Gizmos.color = col;
            int iterationCount = 64;
            for (int i = 0; i < iterationCount; i++)
            {
                float rad = (Mathf.PI * i) / (iterationCount * 0.5f);
                Vector3 offsetVec = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * radius;
                Vector3 currentLinePos = offsetVec;
                Gizmos.DrawLine(lastLinePos, currentLinePos);
                lastLinePos = currentLinePos;

                if (i == iterationCount - 1)
                {
                    Gizmos.DrawLine(lastLinePos, firstLinePos);
                }
            }
        }

    }

    void DrawGizmoVector(Vector3 pos, Vector3 dir, float size, Color col, Vector3 up)
    {
        Gizmos.color = col;
        Gizmos.DrawRay(pos, dir * size);
        Vector3 crossVal =Vector3.Cross(dir, up);
        
        Vector3 arrowHand01 = Vector3.Normalize(Vector3.Lerp(-dir, crossVal, 0.3f));
        Vector3 arrowHand02 = Vector3.Normalize(Vector3.Lerp(-dir, -crossVal, 0.3f));
        
        Gizmos.DrawRay(pos + dir * size, size * 0.1f * arrowHand01);
        Gizmos.DrawRay(pos + dir * size, size * 0.1f * arrowHand02);
    }
    
}
