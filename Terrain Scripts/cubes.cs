using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/*
TODO:
* more perlin noise sliders FIN
* no longer adding sclaing / resolution for now

* BUGS :(
    * at the very top or very bottom the mesh doesn't get fully enclosed - sort of fixed 
*/
public class cubes 
{
    public GameObject chunk_gameObj;
    MeshFilter filter;
    MeshCollider meshCollider;
    MeshRenderer meshRenderer;

    Vector3Int chunkWorldPosition;
    // Mesh Data
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();




    float[,,] cornerValueMap;
    //  [Space(12)]
    //[Header("Noise Params")]
   // [Range(0f,100f)]
    public float noise_map_scale;
    public float xScroll;
    public float zScroll;
    public bool interpolation;

    //
    int widthx { get { return MapData.ChunkWidth; } }
    int widthz { get { return MapData.ChunkWidth; } }
    int height { get { return MapData.ChunkHeight; } }
    float terrainSurface { get { return MapData.terrainSurface; } }
    public float[,,] getCornerMap(){
        return cornerValueMap;
    }
    public cubes(Vector3Int position, Vector3 noise_params, bool interpolation)
    {
        noise_map_scale = noise_params.x;
        xScroll = noise_params.y;
        zScroll = noise_params.z;
        this.interpolation = interpolation;
        chunk_gameObj = new GameObject();
        chunkWorldPosition = position;
        chunk_gameObj.transform.position = chunkWorldPosition;

        filter = chunk_gameObj.AddComponent<MeshFilter>();
        meshCollider = chunk_gameObj.AddComponent<MeshCollider>();
        meshRenderer = chunk_gameObj.AddComponent<MeshRenderer>();
        meshRenderer.material =  Resources.Load<Material>("Shaders/pls");;
        chunk_gameObj.transform.tag="Terrain";
        chunk_gameObj.layer = LayerMask.NameToLayer("Ground");

        cornerValueMap = new float[widthx + 1, height + 1, widthz + 1];
		handleCubeCornerInfo();
		createMeshData();
        GameObjectUtility.SetStaticEditorFlags(chunk_gameObj, StaticEditorFlags.NavigationStatic);
    }
    private float GetTerrainHeight (int x, int z) {
		//return float randomHeight = (float)(TerrainHeightRange-1) * Mathf.PerlinNoise((float)x / noise_map_scale * 1.5f + xScroll, (float)z / noise_map_scale * 1.5f + zScroll);
        return (float)(MapData.TerrainHeightRange -1) * Mathf.PerlinNoise((float)x / noise_map_scale * 1.5f + xScroll, (float)z / noise_map_scale * 1.5f + zScroll) + MapData.BaseTerrainHeight;

    }
    void marchCube(Vector3Int position)
    {
        float[] cube = new float[8];
        for (int i = 0; i < 8; i++) {

            cube[i] = SampleTerrain(position + MapData.CornerTable[i]);

        }

        int config_index = getCubeConfig(cube);
        if (config_index == 0 || config_index == 255)
        {
            return;
        }
        int edgeIndex = 0;
        //one config will have at most 5 triangles
        // each triangle is obv 3 verticies 
        for (int max_triangles = 0; max_triangles < 5; max_triangles++)
        {
            for (int triangle_vert = 0; triangle_vert < 3; triangle_vert++)
            {
                int indice = MapData.TriangleTable[config_index, edgeIndex];
                // -1 marks the end of the configuration
                if (indice == -1)
                {
                    return;
                }
                // 2 verticies form an edge
                Vector3 vertex1 = position + MapData.CornerTable[MapData.EdgeTable[indice, 0]];
                Vector3 vertex2 = position + MapData.CornerTable[MapData.EdgeTable[indice, 1]];
                Vector3 vertexPos;

                if (interpolation)
                {
                    float vert1Interpol = cube[MapData.EdgeTable[indice,0]];
                    float vert2Interpol = cube[MapData.EdgeTable[indice,1]];

                    float diff = vert2Interpol- vert1Interpol;
                    if(diff ==0){
                        diff = terrainSurface;
                    }else{
                        diff = (terrainSurface - vert1Interpol)/diff;
                    }

                    vertexPos = vertex1 + ((vertex2-vertex1)*diff);

                }
                else
                {
                    vertexPos = (vertex1 + vertex2) / 2f;
                }

                vertices.Add(vertexPos);
                triangles.Add(vertices.Count - 1);
                edgeIndex++;
            }
        }
    }


    public void placeTerrain(Vector3 position){
        //where 16 is chun.k width this isn't made to be scalable with chun.k size, as this scritp will change
        // this is mostly for fun
        Vector3 v3 = position - new Vector3(Mathf.FloorToInt(position.x/16)*16, 0, Mathf.FloorToInt(position.z/16)*16);
        // Debug.Log(v3);
        Vector3Int v3Int = new Vector3Int(Mathf.CeilToInt(v3.x), Mathf.CeilToInt(v3.y), Mathf.CeilToInt(v3.z));
        cornerValueMap[v3Int.x, v3Int.y, v3Int.z] = 0f;

        createMeshData();
    }

    public void removeTerrain(Vector3 pos){
       Vector3Int v3Int = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
        cornerValueMap[v3Int.x, v3Int.y, v3Int.z] = 1f;
        createMeshData();
    }

    float SampleTerrain (Vector3Int point) {

        return cornerValueMap[point.x, point.y, point.z];

    }


    public void handleCubeCornerInfo()
    {
        // when sampling corners and including the cube overlap the number of corners
        // is just number of cubes +1 -> each middle point between brackets is already sampled/shared except for the edges or [].[].[]
        for (int x = 0; x < widthx  + 1; x++){
            for (int y = 0; y < height + 1; y++){
            	for (int z = 0; z < widthz + 1; z++){
                    //float randomHeight = (float)(height-1) * Mathf.PerlinNoise((float)x / noise_map_scale * 1.5f + xScroll, (float)z / noise_map_scale * 1.5f + zScroll);
                    float randomHeight = GetTerrainHeight(x + chunkWorldPosition.x,z + chunkWorldPosition.z);
                    cornerValueMap[x, y, z] = (float) y - randomHeight;

                }
            }
        }
    }
    public void createMeshData()
    {
		clearMeshData();
        for (int i = 0; i < widthx  ; i++){
            for (int j = 0; j < height ; j++){
                for (int k = 0; k < widthz ; k++){
					marchCube(new Vector3Int(i,j,k));
                }
            }
        }
        buildMesh();
    }

    public void updateNoiseParams( Vector3 noise_params, bool interpolation){
        this.noise_map_scale = noise_params.x;
        this.xScroll = noise_params.y;
        this.zScroll = noise_params.z;
        this.interpolation = interpolation;
    }
    int getCubeConfig(float[] cube)
    {
        int conf_index = 0;
        for (int i = 0; i < 8; i++)
        {
            // simple method to determine which of the 255 configurations gets used
            // if all if statements in this loop end up true we get 11111111 which is 255 which means the cube is above air
            if (cube[i] >= terrainSurface)
            {
                conf_index |= 1 << i;
            }
        }
        return conf_index;
    }


    void clearMeshData()
    {
        vertices.Clear();
        triangles.Clear();
    }

    void buildMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        filter.mesh = mesh;
        // Calculating mesh UVs-- from unity documentation, still isn't very clear considering
        // some triangles can go on top of each other how does a 2d plane translate that???
        Vector2[] uvs = new Vector2[mesh.vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        mesh.uv = uvs;
        meshCollider.sharedMesh = mesh;
    }

}


