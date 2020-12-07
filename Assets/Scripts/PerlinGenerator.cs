using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinGenerator : MonoBehaviour
{
    [SerializeField]
    private int width = 256;
    [SerializeField]
    private int height = 256;
    [Range(0f, 100f), SerializeField]
    private float scale = 20f;

    private Renderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        meshRenderer.material.mainTexture = GenerateTexture();
    }

    private Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);

        //Perlin Noise
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = CalculateColor(x, y); 
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();

        return texture;
    }

    public Texture2D GetNoise()
    {
        return (Texture2D)meshRenderer.material.mainTexture;
    }

    private Color CalculateColor(int x, int y)
    {
        float xCoord = (float)x / width * scale;
        float yCoord = (float)y / width * scale;
        float sample = Mathf.PerlinNoise(xCoord, yCoord);

        return new Color(sample, sample, sample);
    }
}
