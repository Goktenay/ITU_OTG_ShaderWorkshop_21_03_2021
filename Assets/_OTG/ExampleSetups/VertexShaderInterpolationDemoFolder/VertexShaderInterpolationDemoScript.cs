using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexShaderInterpolationDemoScript : MonoBehaviour
{
    [NaughtyAttributes.OnValueChanged("CalculateBarycentricCoordinates")] 
    [SerializeField] private float _rasterSize = 0.1f;

    [Space(30)]
    [SerializeField] private bool _showVertexPosLines;
    [Space(10)]
    [SerializeField] private bool _showVertexPosVectors;
    [SerializeField] private bool _showFragmentPosVectors;
    [Space(10)]
    
    [SerializeField] private bool _showFragmentRasters;
    [Space(30)]
    [NaughtyAttributes.OnValueChanged("CalculateBarycentricCoordinates")] 
    [SerializeField] private Vector2 _vertexPos0;
    [NaughtyAttributes.OnValueChanged("CalculateBarycentricCoordinates")] 
    [SerializeField] private Vector2 _vertexPos1;
    [NaughtyAttributes.OnValueChanged("CalculateBarycentricCoordinates")] 
    [SerializeField] private Vector2 _vertexPos2;

    [Space(30)]
    [SerializeField] private bool _showVertexColors;
    [SerializeField] private bool _showFragmentColors;
    [Space(10)]
    [SerializeField] private Color _col0;
    [SerializeField] private Color _col1;
    [SerializeField] private Color _col2;

    [Space(30)]
    [SerializeField] private bool _showVertexNorms;
    [SerializeField] private bool _showFragmentNorms;
    [Space(10)]
    [SerializeField] private Vector3 _norm0;
    [SerializeField] private Vector3 _norm1;
    [SerializeField] private Vector3 _norm2;
    [Space(10)] 
    [SerializeField] private Color _normCol;
    [SerializeField] private bool _normalizeFragmentNormals;
    
    private List<Vector3> _baryCentricCoordinates;


    void CalculateBarycentricCoordinates(Vector2 oldVal, Vector2 newVal)
    {
      if(_baryCentricCoordinates == null)
          _baryCentricCoordinates = new List<Vector3>(1000);
      else
          _baryCentricCoordinates.Clear();


      float minX = Mathf.Min(_vertexPos0.x, Mathf.Min(_vertexPos1.x, _vertexPos2.x));
      float minZ = Mathf.Min(_vertexPos0.y, Mathf.Min(_vertexPos1.y, _vertexPos2.y));

      float maxX = Mathf.Max(_vertexPos0.x, Mathf.Max(_vertexPos1.x, _vertexPos2.x));
      float maxZ = Mathf.Max(_vertexPos0.y, Mathf.Max(_vertexPos1.y, _vertexPos2.y));
      
      Vector2 point = Vector2.zero;
      for (float i = Mathf.Round(minX -2); i < maxX + 1; i += _rasterSize)
      {
          for (float j = Mathf.Round(minZ -2); j < maxZ+1; j += _rasterSize)
          {
              point = new Vector2(i,j);
              
             Vector3 bar = Barycentric(point,_vertexPos0, _vertexPos1, _vertexPos2);

             if (bar.x < 1 && bar.y < 1 && bar.z < 1 && bar.x >= 0 && bar.y >= 0 && bar.z >= 0)
             {
                // Debug.DrawRay(new Vector3(i,0,j), Vector3.up, Color.white, 1 );
                 _baryCentricCoordinates.Add(bar);
             }
             else
             {
               // Debug.DrawRay(new Vector3(i,0,j), Vector3.up, Color.red, 1 );
             }
             
             
          }
      }
      
    }


    private void OnDrawGizmos()
    {
        if(_baryCentricCoordinates == null)
            return;
        
        
        Vector3 pos0 = new Vector3(_vertexPos0.x, 5, _vertexPos0.y);
        Vector3 pos1 = new Vector3(_vertexPos1.x, 5, _vertexPos1.y);
        Vector3 pos2 = new Vector3(_vertexPos2.x, 5, _vertexPos2.y);
        
        ShowVertexPositionVectors();
        ShowVertexes();
        ShowColors();
        ShowNormals();
        
        
        void ShowVertexPositionVectors()
        {
            Vector3 camPos = Camera.current.transform.position;
            float arrowHandSize = 0.1f;
            
            if (_showVertexPosVectors)
            {
                Gizmos.color = new Color(1f, 0.6f, 0f);
                DrawGizmoVector(Vector3.zero, pos0.normalized / arrowHandSize,pos0.magnitude * arrowHandSize, (camPos-pos0).normalized );
                DrawGizmoVector(Vector3.zero, pos1.normalized / arrowHandSize,pos1.magnitude * arrowHandSize, (camPos-pos1).normalized );
                DrawGizmoVector(Vector3.zero, pos2.normalized / arrowHandSize,pos2.magnitude * arrowHandSize, (camPos-pos2).normalized );
            }

            if (_showFragmentPosVectors)
            {
                Gizmos.color = new Color(1f, 0.6f, 0f);
                foreach (var bar in _baryCentricCoordinates)
                {
                    Vector3 interpolatedVal = pos0 * bar.x + pos1 * bar.z + pos2 * bar.y;
                    
                    DrawGizmoVector(Vector3.zero, interpolatedVal.normalized / arrowHandSize,interpolatedVal.magnitude * arrowHandSize, (camPos-interpolatedVal).normalized );
                }
            }
        }

        
        
        void ShowVertexes()
        {
            if (_showVertexPosLines)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(pos0, pos1 );
                Gizmos.DrawLine(pos1, pos2 );
                Gizmos.DrawLine(pos2, pos0 );
            }

            if (_showFragmentRasters)
            {
                Gizmos.color = Color.gray;
                Vector3 rasterSizeVec = _rasterSize * new Vector3(1,0.1f,1);
                foreach (var bar in _baryCentricCoordinates)
                {
                    Vector3 interpolatedVal = pos0 * bar.x + pos1 * bar.z + pos2 * bar.y;
                    Gizmos.DrawCube(interpolatedVal, rasterSizeVec * 0.95f);
                }
            }
        }

        void ShowColors()
        {
            Color col0Val = _col0;
            col0Val.a = 1;
            
            Color col1Val = _col1;
            col1Val.a = 1;
            
            Color col2Val = _col2;
            col2Val.a = 1;

            if (_showFragmentColors)
            {
                Vector3 col0Vec = new Vector3(col0Val.r, col0Val.g, col0Val.b);
                Vector3 col1Vec = new Vector3(col1Val.r, col1Val.g, col1Val.b);
                Vector3 col2Vec = new Vector3(col2Val.r, col2Val.g, col2Val.b);
                
                
                Color col = new Color(1,1,1,1);
                Vector3 rasterSizeVec = _rasterSize * new Vector3(1,0.1f,1);
                foreach (var bar in _baryCentricCoordinates)
                {
                    Vector3 interpolatedPos = pos0 * bar.x + pos1 * bar.z + pos2 * bar.y;
                    Vector3 interpolatedColVec = col0Vec * bar.x + col1Vec * bar.z + col2Vec * bar.y;
                    col.r = interpolatedColVec.x;
                    col.g = interpolatedColVec.y;
                    col.b = interpolatedColVec.z;

                    Gizmos.color = col;
                    Gizmos.DrawCube(interpolatedPos + Vector3.up * 0.025f, 0.85f * rasterSizeVec);
                }
            }
            
            
            if (_showVertexColors)
            {
                Gizmos.color = col0Val;
                Gizmos.DrawSphere(pos0 + Vector3.up * 0.05f, 0.075f );
                Gizmos.color = col1Val;
                Gizmos.DrawSphere(pos1  + Vector3.up * 0.05f, 0.075f );
                Gizmos.color = col2Val;
                Gizmos.DrawSphere(pos2  + Vector3.up * 0.05f, 0.075f);
            }
        }

        void ShowNormals()
        {
            _norm0.Normalize();
            _norm1.Normalize();
            _norm2.Normalize();

            Vector3 camToPos0 = (pos0 - Camera.current.transform.position).normalized;
            Vector3 camToPos1 = (pos1 - Camera.current.transform.position).normalized;
            Vector3 camToPos2 = (pos2 - Camera.current.transform.position).normalized;

            _normCol.a = 1;
            Gizmos.color = _normCol;
            
            float normVecSize = 1;
            
            if (_showVertexNorms)
            {
                DrawGizmoVector(pos0, _norm0,normVecSize , camToPos0);
                DrawGizmoVector(pos1, _norm1,normVecSize , camToPos1);
                DrawGizmoVector(pos2, _norm2 ,normVecSize, camToPos2);
            }

            if (_showFragmentNorms)
            {
                foreach (var bar in _baryCentricCoordinates)
                {
                    Vector3 interpolatedPos = pos0 * bar.x + pos1 * bar.z + pos2 * bar.y;
                    Vector3 interpolatedNorm = _norm0 * bar.x + _norm1 * bar.z + _norm2 * bar.y;
                    Vector3 interpolatedDirVec = (interpolatedPos + interpolatedNorm - Camera.current.transform.position).normalized;
                   

                    if (_normalizeFragmentNormals)
                    {
                        interpolatedNorm.Normalize();
                    }
                    
                    DrawGizmoVector(interpolatedPos, interpolatedNorm, normVecSize, interpolatedDirVec);
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
        
        Gizmos.DrawRay(pos + dir * size, size * 0.1f * arrowHand01);
        Gizmos.DrawRay(pos + dir * size, size * 0.1f * arrowHand02);
    }
    
    Vector3 Barycentric(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        Vector2 v0 = b - a;
        Vector2 v1 = c - a;
        Vector2 v2 = p - a;
        
        float d00 = Vector2.Dot(v0, v0);
        float d01 = Vector2.Dot(v0, v1);
        float d11 = Vector2.Dot(v1, v1);
        float d20 = Vector2.Dot(v2, v0);
        float d21 = Vector2.Dot(v2, v1);
        float denom = d00 * d11 - d01 * d01;
        
        float v = (d11 * d20 - d01 * d21) / denom;
        float w = (d00 * d21 - d01 * d20) / denom;
        float u = 1.0f - v - w;


        return new Vector3(u, v, w);
    }
    
}



