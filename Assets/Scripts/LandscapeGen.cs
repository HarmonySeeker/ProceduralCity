using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class LandscapeGen : MonoBehaviour
{
    [Range(1, 10)]
    public int tileWidth;
    public int PointsNum;
    
    [Range(1f, 100f)]
    public float scale;
    
    public GameObject Plane;
    public GameObject AnglePoint;
    
    public List<GameObject> Buildings;

    private float constraintX;
    private float constraintZ;
    private float constraintCubeX;
    private float constraintCubeZ;

    private List<float> constraintsMeshes;
    private List<List<DistributionPoint>> pointsMat;
    private List<GameObject> spawnedBuildings;
    private Texture2D noisePattern;

    private void Start()
    {
        tileWidth = tileWidth == 0 ? 1 : tileWidth;
        pointsMat = new List<List<DistributionPoint>>();
        constraintsMeshes = new List<float>();
        spawnedBuildings = new List<GameObject>();

        GenerateGrid();
        GenerateNoisePattern();
        GenerateNoise();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RedrawGrid();
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

    private void RedrawGrid()
    {
        ClearVariables();
        GenerateNewGrid();
        DrawVectors();
        HideVectors();
        GenerateNoise();
    }

    private void CalculateContstrains()
    {
        Renderer PlaneRenderer = Plane.GetComponent<Renderer>();
        Renderer PointRenderer = Buildings[0].GetComponent<Renderer>();

        constraintX = PlaneRenderer.bounds.size.x / 2;
        constraintZ = PlaneRenderer.bounds.size.z / 2;

        constraintCubeX = PointRenderer.bounds.size.x / 2;
        constraintCubeZ = PointRenderer.bounds.size.z / 2;

        foreach (GameObject building in Buildings)
        {
            Renderer buildingRenderer = building.GetComponent<Renderer>();
            constraintsMeshes.Add(buildingRenderer.bounds.size.y / 2);
        }
    }

    private void GenerateGrid()
    {
        Vector3 position;
        List<DistributionPoint> pointRow;

        PointsNum = 0;

        CalculateContstrains();
        for (float i = -constraintX; i < constraintX; i += tileWidth)
        {
            pointRow = new List<DistributionPoint>();
            for (float j = -constraintZ; j < constraintZ; j += tileWidth)
            {
                position = new Vector3(i, 0, j);
                GameObject tempPoint = Instantiate(AnglePoint, position, Quaternion.identity, Plane.transform);

                pointRow.Add(tempPoint.GetComponent<DistributionPoint>());
            }

            pointsMat.Add(pointRow);
        }

        PointsNum = pointsMat.Count * pointsMat[0].Count;
    }

    private void GenerateNewGrid()
    {
        foreach (List<DistributionPoint> pointsRow in pointsMat)
        {
            foreach (DistributionPoint point in pointsRow)
            {
                point.GenerateNew();
            }
        }
    }
    private void GenerateNoisePattern()
    {
        int width = (int)constraintX * 2;
        int height = (int)constraintZ * 2;
        Texture2D texture = new Texture2D(width, height);

        //Perlin Noise

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                UnityEngine.Color color = CalculateColor(x, y, width, height, scale);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();

        noisePattern = texture;
    }

    private UnityEngine.Color CalculateColor(int x, int y, int width, int height, float scale)
    {
        float xCoord = (float)x / width * scale;
        float yCoord = (float)y / height * scale;
        float sample = Mathf.PerlinNoise(xCoord, yCoord);

        return new UnityEngine.Color(sample, sample, sample);
    }

    private void GenerateMesh()
    {
        Vector3 position;
        DistributionPoint node1;
        DistributionPoint node2;
        DistributionPoint node3;
        DistributionPoint node4;

        //Debug.Log(pointsMat[0][0].transform.position);

        for (int lineIdx = 0; lineIdx < pointsMat.Count - 1; lineIdx += 1)
        {
            for (int nodeIdx = 0; nodeIdx < pointsMat[0].Count - 1; nodeIdx += 1)
            {
                node1 = pointsMat[lineIdx][nodeIdx];
                node2 = pointsMat[lineIdx][nodeIdx + 1];
                node3 = pointsMat[lineIdx + 1][nodeIdx];
                node4 = pointsMat[lineIdx + 1][nodeIdx + 1];

                position = node1.transform.position;

                for (float i = node1.transform.position.z; i <= node2.transform.position.z; i += constraintCubeZ * 20)
                {
                    for (float j = node1.transform.position.x; j <= node3.transform.position.x; j += constraintCubeX * 20)
                    {
                        Vector3 nodevec1 = position - node1.transform.position;
                        Vector3 nodevec2 = position - node2.transform.position;
                        Vector3 nodevec3 = position - node3.transform.position;
                        Vector3 nodevec4 = position - node4.transform.position;

                        float dotProduct1 = Vector3.Dot(node1.GradientVector, nodevec1);
                        float dotProduct2 = Vector3.Dot(node2.GradientVector, nodevec2);
                        float dotProduct3 = Vector3.Dot(node3.GradientVector, nodevec3);
                        float dotProduct4 = Vector3.Dot(node4.GradientVector, nodevec4);

                        float pointX = QunticCurve(position.x - (int)position.x);
                        float pointZ = QunticCurve(position.z - (int)position.z);

                        float y1 = LinearInterpolation(dotProduct1, dotProduct2, pointX);
                        float y2 = LinearInterpolation(dotProduct3, dotProduct4, pointX);
                        float yfinal = LinearInterpolation(y1, y2, pointZ);

                        Vector3 currentPos = position;

                        /*
                        if (yfinal < 0)
                        {
                            currentPos.y += constraintsMeshes[Buildings.Count - 1] * 10;
                            spawnedBuildings.Add(Instantiate(Buildings[Buildings.Count - 1], currentPos, Quaternion.identity, transform));
                        }
                        else
                        {
                        }
                        */

                        Debug.Log(yfinal);

                        int buildingNum = (int)(Mathf.Abs(yfinal) % Buildings.Count);

                        currentPos.y += constraintsMeshes[buildingNum] * 10;
                        spawnedBuildings.Add(Instantiate(Buildings[buildingNum], currentPos, Quaternion.identity, transform));

                        position.z = i;
                        position.x = j;

                        //Debug.Log($"{dotProduct1} {dotProduct2} {dotProduct3} {dotProduct4}");
                    }
                }
            }
        }
    }

    private void GenerateNoise()
    {
        Vector3 position;
        DistributionPoint node1;
        DistributionPoint node2;
        DistributionPoint node3;
        DistributionPoint node4;

        //Debug.Log(pointsMat[0][0].transform.position);

        for (int lineIdx = 0; lineIdx < pointsMat.Count - 1; lineIdx += 1)
        {
            for (int nodeIdx = 0; nodeIdx < pointsMat[0].Count - 1; nodeIdx += 1)
            {
                node1 = pointsMat[lineIdx][nodeIdx];
                node2 = pointsMat[lineIdx][nodeIdx + 1];
                node3 = pointsMat[lineIdx + 1][nodeIdx];
                node4 = pointsMat[lineIdx + 1][nodeIdx + 1];

                position = node1.transform.position;
                
                for (float i = node1.transform.position.z; i <= node2.transform.position.z; i += constraintCubeZ * 20)
                {
                    for (float j = node1.transform.position.x; j <= node3.transform.position.x; j += constraintCubeX * 20)
                    {
                        Vector3 nodevec1 = position - node1.transform.position;
                        Vector3 nodevec2 = position - node2.transform.position;
                        Vector3 nodevec3 = position - node3.transform.position;
                        Vector3 nodevec4 = position - node4.transform.position;

                        float dotProduct1 = Vector3.Dot(node1.GradientVector, nodevec1);
                        float dotProduct2 = Vector3.Dot(node2.GradientVector, nodevec2);
                        float dotProduct3 = Vector3.Dot(node3.GradientVector, nodevec3);
                        float dotProduct4 = Vector3.Dot(node4.GradientVector, nodevec4);

                        float pointX = QunticCurve(position.x - (int)position.x);
                        float pointZ = QunticCurve(position.z - (int)position.z);

                        float y1 = LinearInterpolation(dotProduct1, dotProduct2, pointX);
                        float y2 = LinearInterpolation(dotProduct3, dotProduct4, pointX);
                        float yfinal = LinearInterpolation(y1, y2, pointZ);

                        Vector3 currentPos = position;

                        /*
                        if (yfinal < 0)
                        {
                            currentPos.y += constraintsMeshes[Buildings.Count - 1] * 10;
                            spawnedBuildings.Add(Instantiate(Buildings[Buildings.Count - 1], currentPos, Quaternion.identity, transform));
                        }
                        else
                        {
                        }
                        */

                        Debug.Log(yfinal);

                        int buildingNum = (int)(Mathf.Abs(yfinal) % Buildings.Count);

                        currentPos.y += constraintsMeshes[buildingNum] * 10;
                        spawnedBuildings.Add(Instantiate(Buildings[buildingNum], currentPos, Quaternion.identity, transform));

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
        foreach (GameObject building in spawnedBuildings)
        {
            Destroy(building);
        }

        spawnedBuildings.Clear();
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
