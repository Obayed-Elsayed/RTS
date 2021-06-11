using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTesting : MonoBehaviour
{
    public int rayCastMask;
    public int width = 256;
    public int height = 256;
    public float scale = 10f;
    public float appearingThreshold = 0.4f;


    public GameObject terrain;
    Renderer r;
    
    private float[,] grid;
    
    public void Start()
    {
        r = terrain.GetComponent<Renderer>();
        // grid = new float[width, height];
        // Texture2D texture = new Texture2D(width, height);

        // for (int x = 0; x < width; x++)
        // {
        //     for (int y = 0; y < height; y++)
        //     {

        //         Color color;
        //         if (grid[x, y] < appearingThreshold)
        //         {
        //             color = new Color(0, 0, 0);
        //             texture.SetPixel(x, y, color);
        //         }
        //         else
        //         {

        //             color = new Color(1, 1, 1);
        //             texture.SetPixel(x, y, color);
        //         }
        //     }
        // }
        r.material.mainTexture = GenerateTexture();
    }


    void Update()
    {
        if(Input.GetKey(KeyCode.Keypad8)){
        }
           r.material.mainTexture = GenerateTexture();
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = calculateColor(x, y);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        return texture;
    }
    public float lacunarity = 1;
    public float persistance = 1;
    public float scrollX = 0;
    public float scrollY = 0;
    public float octaves = 4;
    // Scrolling + frequency
    // add cleaning for straglers
    // A nice config: scale 8 ,persistance 1.1 octaves 4 lacunarit 1, appearnce at 1
    Color calculateColor(int x, int y)
    {
        float rgb = 0;
        float amplitude = 1;
        float frequency = 1;
        for (int i = 0; i < octaves; i++)
        {
            float perlinPosX = (float)(x+scrollX) / width * scale * frequency;
            float perlinPosY = (float)(y+scrollY) / height * scale * frequency;
            float perlin = Mathf.PerlinNoise(perlinPosX, perlinPosY);
            rgb += perlin * amplitude;
            amplitude *= persistance;
            frequency *= lacunarity;

        }
        // if (rgb < appearingThreshold)
        // {
        //     return new Color(0, 0, 0);
        // }
        // return new Color(1, 1, 1);
        return new Color(rgb, rgb, rgb);
    }
}
