using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGeneration: MonoBehaviour 
{
    public LayerMask unitLayer;
    public int width = 256;
    public int height = 256;
    public float scale = 10f;
    public float appearingThreshold = 0.5f;
    public GameObject treePrefab;
    
    // private Renderer r;
    private float[,] grid;
    
    public void Start()
    {
        // r = GetComponent<Renderer>();
        GameObject tree = Instantiate(treePrefab) as GameObject;
        BoxCollider collider = tree.GetComponent<BoxCollider>();
        Destroy(tree);
        int dimx = (int)(collider.size.x * tree.transform.localScale.x);
        int dimy = (int)(collider.size.y * tree.transform.localScale.y);
        int dimensionx = (int)300 / dimx;
        int dimensiony = (int)300 / dimy;
        grid = new float[dimensionx, dimensiony];

        unitLayer = LayerMask.NameToLayer("Buildings and Resources");
        //Texture2D texture = new Texture2D(dimensionx, dimensiony);

        for (int x = 0; x < dimensionx; x++)
        {
            for (int y = 0; y < dimensiony; y++)
            {
                grid[x, y] = Mathf.PerlinNoise((float)x / dimensionx * scale, (float)y / dimensiony * scale);
                //Color color;
                if (grid[x, y] < appearingThreshold)
                {
                    GameObject forrest_tree = Instantiate(treePrefab) as GameObject;
                    forrest_tree.transform.position = new Vector3(x * dimx - 300 / 2, 0, y * dimy - 300 / 2);
                    forrest_tree.layer = unitLayer;
                    // color = new Color(0, 0, 0);
                    // texture.SetPixel(x, y, color);
                }
                else
                {

                    // color = new Color(1, 1, 1);
                    // texture.SetPixel(x, y, color);
                }
            }
        }
        // r.material.mainTexture = GenerateTexture(dimensionx, dimensiony);
    }
    void Update()
    {
        // Debug.Log(r.bounds);
        // for (int i = 0; i < 10; i++)
        // {
        //     GameObject tree = Instantiate(treePrefab) as GameObject;
        //     BoxCollider collider = tree.GetComponent<BoxCollider>();
        //     tree.transform.position = new Vector3(0, 0, collider.size.y * tree.transform.localScale.y * i);
        // }

       // r.material.mainTexture = GenerateTexture(width, height);
    }

    // Texture2D GenerateTexture(int w, int h)
    // {
    //     Texture2D texture = new Texture2D(w, h);
    //     for (int x = 0; x < width; x++)
    //     {
    //         for (int y = 0; y < height; y++)
    //         {
    //             Color color = calculateColor(x, y);
    //             texture.SetPixel(x, y, color);
    //         }
    //     }
    //     texture.Apply();
    //     return texture;
    // }
    // Color calculateColor(int x, int y)
    // {
    //     float rgb = Mathf.PerlinNoise((float)x / width * scale, (float)y / height * scale);
    //     if (rgb < appearingThreshold)
    //     {
    //         return new Color(0, 0, 0);
    //     }
    //     return new Color(1, 1, 1);
    // }
}
