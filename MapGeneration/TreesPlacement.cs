using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreesPlacement : MonoBehaviour
{

    // https://www.youtube.com/watch?v=eyaxqo9JV4w&ab_channel=Flaroon
    // GPU instancing optimize draw calls for trees
    // Biggest concern is how that affects the navmesh?
    // if it is a problem then the draw calls and the navmesh colliders can be seperate entities

    
    public GameObject treePrefab;
    private float [,] noiseGrid;
    
    public int TreeScale = 1;
    public LayerMask treeLayer;
    public void GenerateTrees(ref (float, int) [,] map){

        noiseGrid = new float[MapData.worldSizeInUnits * TreeScale,MapData.worldSizeInUnits* TreeScale];


        // int i =0;
        for (int widthx = 0; widthx < MapData.worldSizeInUnits; widthx++)
        {
            for (int widthz = 0; widthz < MapData.worldSizeInUnits; widthz++)
            {
                noiseGrid[widthx,widthz] = calculateNoise(widthx, widthz);
                
                if (noiseGrid[widthx,widthz] < appearingThreshold && WorldGenerator.instance.gridMapInfo[widthx,widthz].Item2 == 0){
                    GameObject forrest_tree = Instantiate(treePrefab) as GameObject;

                    //forrest_tree.layer = treeLayer;
                    BoxCollider boxCollider = forrest_tree.GetComponent<BoxCollider>();
                    Vector3 original_size = boxCollider.size;
                    //randomize color
                    Renderer r = forrest_tree.GetComponent<Renderer>();
                    r.materials[0].color = new Color(r.materials[0].color.r*Random.Range(0.6f,3.5f),r.materials[0].color.g*Random.Range(0.6f,1.4f),r.materials[0].color.b*Random.Range(0.6f,3));

                    forrest_tree.transform.position = new Vector3(widthx +0.5f,map[(widthx),(widthz)].Item1, widthz + 0.5f);
                    forrest_tree.transform.localScale = new Vector3(0.33f,Random.Range(0.25f,0.65f),0.33f);


                    //boxCollider.size = forrest_tree.transform.InverseTransformVector(new Vector3(3,4,3));
                    forrest_tree.transform.Rotate(Random.Range(-5,5), Random.Range(-5,5), Random.Range(-5,5), Space.Self);
                    map[widthx,widthz] = (map[widthx,widthz].Item1,2);
                }
                //i++;
            }
        }
        //Texture2D texture = new Texture2D(dimensionx, dimensiony);

        // for (int x = 0; x < dimensionx; x++)
        // {
        //     for (int y = 0; y < dimensiony; y++)
        //     {
        //         // grid[x, y] = Mathf.PerlinNoise((float)x / dimensionx * scale, (float)y / dimensiony * scale);
        //         grid[x, y] = calculateNoise(x, y);
        //         //Color color;
        //         if (grid[x, y] < appearingThreshold)
        //         {
        //             //Vector3 treePosition = new Vector3(x * dimx - mapSize.x / 2, 0, y * dimy - mapSize.y / 2);
        //             Vector3 treePosition = new Vector3(x * dimx + mapFirstCorner.x, 0, y * dimy+mapFirstCorner.y);
        //             if (Physics.Raycast(new Vector3(treePosition.x, 500f, treePosition.z), Vector3.down, out hit, 500000f, layerMask) && hit.point.y < 0) {
        //                 continue;
        //             }
                    
        //             GameObject forrest_tree = Instantiate(treePrefab) as GameObject;
        //             Renderer r = forrest_tree.GetComponent<Renderer>();
        //             r.materials[0].color = new Color(r.materials[0].color.r*Random.Range(0.8f,1.2f),r.materials[0].color.g*Random.Range(0.8f,1.2f),r.materials[0].color.b*Random.Range(0.8f,1.2f));
        //             BoxCollider standardCollider = forrest_tree.GetComponent<BoxCollider>();
        //             // forrest_tree.transform.position = new Vector3(x * dimx - mapSize.x / 2, 0, y * dimy - mapSize.y / 2);
        //             forrest_tree.transform.position = treePosition;
        //             forrest_tree.transform.localScale = new Vector3(Random.Range(0.7f,1f),Random.Range(0.7f,1f),Random.Range(0.7f,1f));
        //             forrest_tree.transform.Rotate(Random.Range(-5,5), Random.Range(-5,5), Random.Range(-5,5), Space.Self);
        //            // forrest_tree.GetComponent<Collider>() = standardCollider;
        //             forrest_tree.layer = unitLayer;
        //             //treeList.Add(forrest_tree);
        //             treeGrid[x,y] = forrest_tree;
        //             // color = new Color(0, 0, 0);
        //             // texture.SetPixel(x, y, color);
        //         }
        //         else
        //         {
        //             //treeGrid[x,y] = 0;
        //             // color = new Color(1, 1, 1);
        //             // texture.SetPixel(x, y, color);
        //         }
        //     }
        // }
        // r.material.mainTexture = GenerateTexture(dimensionx, dimensiony);
    }

    public float appearingThreshold = 0.5f;
    public float lacunarity = 1;
    public float persistance = 1;
    public float scrollX = 0;
    public float scrollY = 0;
    public float octaves = 4;
    public float scale = 10f;
    private float calculateNoise(int x, int y){
        float rgb = 0;
        float amplitude = 1;
        float frequency = 1;
        for (int i = 0; i < octaves; i++)
        {
            float perlinPosX = (float)(x+scrollX) / MapData.worldSizeInUnits * scale * frequency;
            float perlinPosZ = (float)(y+scrollY) / MapData.worldSizeInUnits * scale * frequency;
            float perlin = Mathf.PerlinNoise(perlinPosX, perlinPosZ);
            rgb += perlin * amplitude;
            amplitude *= persistance;
            frequency *= lacunarity;

        }
        if (rgb < appearingThreshold)
        {
            return 0f;
        }
        return 1f;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
