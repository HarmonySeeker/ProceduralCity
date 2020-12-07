using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistributionPoint : MonoBehaviour
{
    public LineRenderer lineRenderer;
    
    public Vector3 GradientVector;

    private void Awake()
    {
        GradientVector = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    public void DrawPoints(float drawRange)
    {
        Vector3 lineEnd = transform.position + GradientVector * drawRange;
        Vector3[] drawPoints =
        {
            transform.position,
            lineEnd
        };

        lineRenderer.SetPositions(drawPoints);
    }

    public void ShowVector()
    {
        lineRenderer.enabled = true;
    }

    public void HideVector()
    {
        lineRenderer.enabled = false;
    }

    public void GenerateNew()
    {
        GradientVector = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }
}
