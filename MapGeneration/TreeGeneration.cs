using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AI;

using Random=UnityEngine.Random;
public class TreeGeneration: MonoBehaviour 
{
    public LayerMask unitLayer;
    public int rayCastMask;
    public int width = 256;
    public int height = 256;
    public float scale = 10f;
    public float appearingThreshold = 0.5f;
    public GameObject treePrefab;
    public GameObject terrain;
    public GameObject waterTerrain;
    
    // private Renderer r;
    private int layerMask = (1 << 8);
    public LayerMask boxCastMask;
    private float[,] grid;
    private GameObject[,] treeGrid;
    private MeshCreationUtils meshCreationUtils;
    List<GameObject> treeList;
    
    public bool enable_cleanup = true;

    public Vector2 mapFirstCorner = new Vector2(-150,-150);
    public Vector2 mapFinalCorner = new Vector2(150,150);
    public void Start()
    {
        bounds = new ShowMeshBounds();
        meshCreationUtils= GetComponent<MeshCreationUtils>();
        
        mapSize = new Vector2(Math.Abs(mapFirstCorner.x-mapFinalCorner.x),Math.Abs(mapFirstCorner.y-mapFinalCorner.y));
        mesh = new Mesh();
        terrain.GetComponent<MeshFilter>().mesh = mesh;
        CreateMesh(new Vector3(mapFirstCorner.x, 0, mapFirstCorner.y), new Vector3(mapFinalCorner.x, 0, mapFinalCorner.y), 0);
        UpdateMesh();
        CreateWaterMesh(new Vector3(-50, 0, -50), new Vector3(50, 0, 50),-1);
        UpdateWaterMesh();
        GenerateTrees();
        //GenerateMountains();
        meshCreationUtils.createMountain(new Vector3(50,0,100), new Vector3 (15,0,15), (15,15),false);
        if(enable_cleanup)cleanUpTrees();
        bake();
    }

    // public void OnDrawGizmos(){
    //     Gizmos.DrawWireCube(transform.position, new Vector3(mapSize.x, 1, mapSize.y));
    // }

    /*
    Settings:-----------------
        scale, apperance, lacunarity ,persistance
        12,0.6,30,0.7

    TODO:--------------------
        ***Unit Tree Measurment*** blocks/ distance will be measured in trees

        Tree scaling, hitbox readjustment post scaling
        mountain scaling to fit units
        mountain scattering 
        -> chunks / larger maps generation (low prio)
        -> Occlusion (low prio)
    */
    int dimensionx, dimensiony, dimx, dimy;
    private Vector2 mapSize;
    private void GenerateTrees(){
                rayCastMask = (1<<8);
        // r = GetComponent<Renderer>();
        GameObject tree = Instantiate(treePrefab) as GameObject;
        BoxCollider collider = tree.GetComponent<BoxCollider>();
        Destroy(tree);
        dimx = (int)(collider.size.x * tree.transform.localScale.x);
        dimy = (int)(collider.size.z * tree.transform.localScale.z);
        dimensionx = (int)mapSize.x / dimx;
        dimensiony = (int)mapSize.y / dimy;
        grid = new float[dimensionx, dimensiony];
        treeGrid = new GameObject[dimensionx, dimensiony];

        unitLayer = LayerMask.NameToLayer("Buildings and Resources");
        treeList = new List<GameObject>();
        //Texture2D texture = new Texture2D(dimensionx, dimensiony);

        for (int x = 0; x < dimensionx; x++)
        {
            for (int y = 0; y < dimensiony; y++)
            {
                // grid[x, y] = Mathf.PerlinNoise((float)x / dimensionx * scale, (float)y / dimensiony * scale);
                grid[x, y] = calculateNoise(x, y);
                //Color color;
                if (grid[x, y] < appearingThreshold)
                {
                    //Vector3 treePosition = new Vector3(x * dimx - mapSize.x / 2, 0, y * dimy - mapSize.y / 2);
                    Vector3 treePosition = new Vector3(x * dimx + mapFirstCorner.x, 0, y * dimy+mapFirstCorner.y);
                    if (Physics.Raycast(new Vector3(treePosition.x, 500f, treePosition.z), Vector3.down, out hit, 500000f, layerMask) && hit.point.y < 0) {
                        continue;
                    }
                    
                    GameObject forrest_tree = Instantiate(treePrefab) as GameObject;
                    Renderer r = forrest_tree.GetComponent<Renderer>();
                    r.materials[0].color = new Color(r.materials[0].color.r*Random.Range(0.8f,1.2f),r.materials[0].color.g*Random.Range(0.8f,1.2f),r.materials[0].color.b*Random.Range(0.8f,1.2f));
                    BoxCollider standardCollider = forrest_tree.GetComponent<BoxCollider>();
                    // forrest_tree.transform.position = new Vector3(x * dimx - mapSize.x / 2, 0, y * dimy - mapSize.y / 2);
                    forrest_tree.transform.position = treePosition;
                    forrest_tree.transform.localScale = new Vector3(Random.Range(0.7f,1f),Random.Range(0.7f,1f),Random.Range(0.7f,1f));
                    forrest_tree.transform.Rotate(Random.Range(-5,5), Random.Range(-5,5), Random.Range(-5,5), Space.Self);
                   // forrest_tree.GetComponent<Collider>() = standardCollider;
                    forrest_tree.layer = unitLayer;
                    //treeList.Add(forrest_tree);
                    treeGrid[x,y] = forrest_tree;
                    // color = new Color(0, 0, 0);
                    // texture.SetPixel(x, y, color);
                }
                else
                {
                    //treeGrid[x,y] = 0;
                    // color = new Color(1, 1, 1);
                    // texture.SetPixel(x, y, color);
                }
            }
        }
        // r.material.mainTexture = GenerateTexture(dimensionx, dimensiony);
    }

    public void GenerateMountains(){

        for (int x = 0; x < dimensionx; x++)
        {
            for (int y = 0; y < dimensiony; y++)
            {
                // opposite condition of trees! -> no trees probably
                if (grid[x, y] > appearingThreshold){
                    Vector3 mountainPosition = new Vector3(x * dimx + mapFirstCorner.x, 0, y * dimy+mapFirstCorner.y);

                    (int x, int y) size= (Random.Range(5,8),Random.Range(5,8));
                    RaycastHit[] hits = Physics.BoxCastAll(mountainPosition, new Vector3 (size.x,2,size.y), Vector3.up, Quaternion.identity, 0, boxCastMask);
                    if(hits.Length>0){
                        continue;
                    }
                    meshCreationUtils.createMountain(mountainPosition, new Vector3 (size.x,0,size.y), (10,10),false);
                }
            }
        }
    }
    public int neighbour_range = 1;
    public int cleanUp = 3;
    public int neightbours_to_live = 2;
    private void cleanUpTrees(){
        for (int x = 0; x < dimensionx; x++)
        {
            for (int y = 0; y < dimensiony; y++)
            {
                if (treeGrid[x, y]!=null && treeGrid[x,y].CompareTag("Tree") && calculateNeightbours(x, y) < neightbours_to_live)
                {
                    Destroy(treeGrid[x,y]);
                    treeGrid[x,y] = null;
                }

            }
        }
    }

    private int calculateNeightbours(int posx, int posy)
    {
        int neightbours = 0;
        for (int x = -neighbour_range; x <= neighbour_range; x++)
        {
            for (int y = -neighbour_range; y <= neighbour_range; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }
                int checkX = posx + x;
                int checkY = posy + y;
                if (checkX >= 0 && checkX < dimensionx && checkY >= 0 && checkY < dimensiony)
                {
                    if (treeGrid[checkX, checkY]!= null && treeGrid[checkX, checkY].CompareTag("Tree"))
                    {
                        neightbours++;
                    }
                }
            }
        }
        return neightbours;
    }



    public int terrainX = 20;
    public int terrainZ = 20;
    public int waterTerrainX = 20;
    public int waterTerrainZ = 20;
    Mesh waterMesh;
    Vector3[] waterVertices;
    int[] waterTriangles;
    private void CreateWaterMesh(Vector3 startPos, Vector3 endPos, float yPosition) {
        //List<Vector3> pond= new List<Vector3>();
        Vector3 scale = endPos - startPos;
        scale.x /= waterTerrainX;
        scale.z /= waterTerrainZ;
        waterMesh = new Mesh();
        waterTerrain.GetComponent<MeshFilter>().mesh = waterMesh;
        waterVertices = new Vector3[(waterTerrainX + 1) * (waterTerrainZ + 1)];
        int i = 0;
        for( int z = 0; z <= waterTerrainZ ;z++){
            for( int x = 0; x <= waterTerrainX ;x++){
                waterVertices[i] = new Vector3(startPos.x + x * scale.x, yPosition, startPos.z + z * scale.z);
                i++;
            }
        }
        int vert = 0;
        int tris = 0;
        waterTriangles = new int[6 * waterTerrainX * waterTerrainZ];
        for(int z =0; z< waterTerrainZ; z++){
            for(int x =0; x< waterTerrainX; x++){
                waterTriangles[tris + 0] = vert + 0;
                waterTriangles[tris + 1] = vert + waterTerrainX +1;
                waterTriangles[tris + 2] = vert + 1;
                waterTriangles[tris + 3] = vert + 1;
                waterTriangles[tris + 4] = vert + waterTerrainX + 1;
                waterTriangles[tris + 5] = vert + waterTerrainX + 2;
                vert++;
                tris+=6;
            }
            vert++;
        }

    }
    private void UpdateWaterMesh(){
        waterMesh.Clear();
        waterMesh.vertices = waterVertices;
        waterMesh.triangles = waterTriangles;
        waterMesh.RecalculateNormals();
    }

        Mesh mesh;
        Vector3[] vertices;
        int[] triangles;
        private void CreateMesh(Vector3 startPos, Vector3 endPos, float yPosition) {
        //List<Vector3> pond= new List<Vector3>();
        Vector3 scale = endPos - startPos;
        scale.x /= terrainX;
        scale.z /= terrainZ;
        vertices = new Vector3[(terrainX + 1) * (terrainZ + 1)];
        int i = 0;
        for( int z = 0; z <= terrainZ ;z++){
            for( int x = 0; x <= terrainX ;x++){
                vertices[i] = new Vector3(startPos.x + x * scale.x, yPosition, startPos.z + z * scale.z);
                // this Water pond is relative to the zero position of the map
                float dist = Vector3.Distance(vertices[i], Vector3.zero);
                if(dist < 50){
                    vertices[i].y= Mathf.PerlinNoise(dist, 0) *-10;
                }
                i++;
            }
        }
        int vert = 0;
        int tris = 0;
        triangles = new int[6 * terrainX * terrainZ];
        for(int z =0; z< terrainZ; z++){
            for(int x =0; x< terrainX; x++){
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + terrainX +1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + terrainX + 1;
                triangles[tris + 5] = vert + terrainX + 2;
                vert++;
                tris+=6;
            }
            vert++;
        }

    }

    private void UpdateMesh(){
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        MeshCollider collider = terrain.GetComponent<MeshCollider>();
        collider.sharedMesh = mesh;
    }


    public float lacunarity = 1;
    public float persistance = 1;
    public float scrollX = 0;
    public float scrollY = 0;
    public float octaves = 4;
    private float calculateNoise(int x, int y){
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
        if (rgb < appearingThreshold)
        {
            return 0f;
        }
        return 1f;
    }

    private void bake(){
        NavMeshBuilder.BuildNavMesh();
    }
    private void destroyTrees(){
        foreach(GameObject tree in treeGrid){
            Destroy(tree);
        }
        Array.Clear(treeGrid,0 ,treeGrid.Length);
    } 

    private RaycastHit hit;
    ShowMeshBounds bounds; // for debugging box cast
    void Update()
    {

            if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("spawn tree");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 500000f, rayCastMask))
                {
                    GameObject forrest_tree = Instantiate(treePrefab) as GameObject;
                    forrest_tree.transform.position = hit.point;
                    forrest_tree.layer = unitLayer;
                    //bake();
                }
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                Debug.Log("Box Cast Test");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 500000f, rayCastMask))
                {
                    RaycastHit[] hits = Physics.BoxCastAll(hit.point, new Vector3 (5,2,5), Vector3.up, Quaternion.identity, 0, boxCastMask);
                        bounds.v3Center = hit.point;
                        bounds.v3Extents = new Vector3(5,2,5);
                    if(hits.Length>0){
                        Debug.Log("Hits");
                    }
                    //bake();
                }
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                destroyTrees();
                GenerateTrees();
                if(enable_cleanup){
                    for(int i =0; i < cleanUp; i++){
                        cleanUpTrees();
                    }
                }
            }

    }
}
