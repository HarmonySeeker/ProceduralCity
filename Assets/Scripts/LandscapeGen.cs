using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class LandscapeGen : MonoBehaviour
{
    [Range(1, 10)]
    public int tileWidth;
    public float pointOffset;
    
    public int PointsNum;
    public GameObject Plane;
    public GameObject Point;
    public List<List<DistributionPoint>> pointsMat;

    private float constraintX;
    private float constraintY;
    private float constraintZ;
    private float constraintCubeX;
    private float constraintCubeZ;
    public LineRenderer lineRenderer;

    private void Start()
    {
        tileWidth = tileWidth == 0 ? 1 : tileWidth;
        pointsMat = new List<List<DistributionPoint>>();

        GenerateGrid();
        DrawVectors();
        GenerateNoise();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ClearVariables();
            GenerateGrid();
            DrawVectors();
            GenerateNoise();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            ShowVectors();
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            HideVectors();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            tileWidth = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            tileWidth = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            tileWidth = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            tileWidth = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            tileWidth = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            tileWidth = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            tileWidth = 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            tileWidth = 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            tileWidth = 9;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            tileWidth = 10;
        }
    }

    private void CalculateContstrains()
    {
        Renderer PlaneRenderer = Plane.GetComponent<Renderer>();
        Renderer PointRenderer = Point.GetComponent<Renderer>();

        constraintX = PlaneRenderer.bounds.size.x / 2;
        constraintY = PlaneRenderer.bounds.size.y / 2;
        constraintZ = PlaneRenderer.bounds.size.z / 2;

        constraintCubeX = PointRenderer.bounds.size.x / 2;
        constraintCubeZ = PointRenderer.bounds.size.z / 2;
    }

    private void GenerateGrid()
    {
        Vector3 position;
        List<DistributionPoint> pointRow;

        CalculateContstrains();
        for (float i = -constraintX; i < constraintX; i += tileWidth)
        {
            pointRow = new List<DistributionPoint>();
            for (float j = -constraintZ; j < constraintZ; j += tileWidth)
            {
                position = new Vector3(i, 0, j);
                GameObject tempPoint = Instantiate(Point, position, Quaternion.identity, Plane.transform);

                pointRow.Add(tempPoint.GetComponent<DistributionPoint>());
            }

            pointsMat.Add(pointRow);
            PointsNum += pointRow.Count;
        }
    }

    private void GenerateNoise()
    {
        Vector3 position;
        DistributionPoint node1;
        DistributionPoint node2;
        DistributionPoint node3;
        DistributionPoint node4;

        Debug.Log(pointsMat[0][0].transform.position);

        for (int lineIdx = 0; lineIdx < pointsMat.Count - 1; lineIdx += 1)
        {
            for (int nodeIdx = 0; nodeIdx < pointsMat[0].Count - 1; nodeIdx += 1)
            {
                node1 = pointsMat[lineIdx][nodeIdx];
                node2 = pointsMat[lineIdx][nodeIdx + 1];
                node3 = pointsMat[lineIdx + 1][nodeIdx];
                node4 = pointsMat[lineIdx + 1][nodeIdx + 1];

                position = node1.transform.position;
                
                for (float i = node1.transform.position.z; i < node2.transform.position.z; i += constraintCubeZ * 20)
                {
                    for (float j = node1.transform.position.x; j < node3.transform.position.x; j += constraintCubeX * 20)
                    {
                        Vector3 nodevec1 = position - node1.transform.position;
                        Vector3 nodevec2 = position - node2.transform.position;
                        Vector3 nodevec3 = position - node3.transform.position;
                        Vector3 nodevec4 = position - node4.transform.position;

                        float dotProduct1 = Vector3.Dot(node1.GradientVector, nodevec1);
                        float dotProduct2 = Vector3.Dot(node2.GradientVector, nodevec2);
                        float dotProduct3 = Vector3.Dot(node3.GradientVector, nodevec3);
                        float dotProduct4 = Vector3.Dot(node4.GradientVector, nodevec4);

                        float pointX = QunticCurve(node1.transform.position.x - position.x);
                        float pointZ = QunticCurve(node1.transform.position.z - position.z);

                        float y1 = LinearInterpolation(dotProduct1, dotProduct2, position.x);
                        float y2 = LinearInterpolation(dotProduct3, dotProduct4, position.z);
                        float yfinal = LinearInterpolation(y1, y2, pointZ);

                        //Vector3 currentPos = position;

                        //currentPos.y = yfinal;

                        var kek = Instantiate(Point, position, Quaternion.identity, transform);
                        var lel = kek.GetComponent<Renderer>();
                        lel.material.color = new UnityEngine.Color(yfinal, yfinal, yfinal);

                        position.z = i;
                        position.x = j;

                        //Debug.Log($"{dotProduct1} {dotProduct2} {dotProduct3} {dotProduct4}");
                    }
                }
            }
        }
    }
    private float QunticCurve(float t)
    {
        return Mathf.Pow(t, 3) * (t * (t * 6 - 15) + 10);
    }

    private float LinearInterpolation(float a, float b, float t)
    {
        return a + t * (b - a);
    }

    private void ClearVariables()
    {
        foreach(List<DistributionPoint> pointsRow in pointsMat)
        {
            foreach(DistributionPoint point in pointsRow)
            {
                Destroy(point.gameObject);
            }

            pointsRow.Clear();
        }

        pointsMat.Clear();
    }

    private void DrawVectors()
    {
        foreach (List<DistributionPoint> pointsRow in pointsMat)
        {
            foreach (DistributionPoint point in pointsRow)
            {
                point.DrawPoints(5f);
            }
        }
    }

    private void HideVectors()
    {
        foreach (List<DistributionPoint> pointsRow in pointsMat)
        {
            foreach (DistributionPoint point in pointsRow)
            {
                point.HideVector();
            }
        }
    }

    private void ShowVectors()
    {
        foreach (List<DistributionPoint> pointsRow in pointsMat)
        {
            foreach (DistributionPoint point in pointsRow)
            {
                point.ShowVector();
            }
        }
    }
}
