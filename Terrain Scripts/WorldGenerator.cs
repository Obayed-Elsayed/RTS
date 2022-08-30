using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AI;


public class WorldGenerator : MonoBehaviour {

    public int WorldSizeInChunks = 10;

    [Header("Noise Params")]
    [Range(0f,100f)]
    public float noise_map_scale = 45f;
    [Range(0f,10f)]
    public float xScroll =0.001f;
    [Range(0f,100f)]
    public  float zScroll =0.001f;



    public Material river_mat;
    public bool interpolation = true;
    public Dictionary<Vector3Int, cubes> chunks = new Dictionary<Vector3Int, cubes>();
    /*
    * if a point on grid is both sloped and has a tree, it will simply show as tree
    * 0: Flat land
    * 1: Sloped
    * 2: Tree
    * 3: Building
    * 4: River
    * 5
    */
    public (float,int) [,] gridMapInfo;
    TreesPlacement treePlacer;
    public Camera mainCam;

    private static WorldGenerator _instance;
    public static WorldGenerator instance {get {return _instance;}}
    void Awake(){
        if (_instance != null && _instance != this){
            Destroy(this.gameObject);
        }else{
            _instance = this;
        }
    }

    void Start() {

        treePlacer = GetComponent<TreesPlacement>();
        gridMapInfo = new (float,int)[MapData.ChunkWidth*WorldSizeInChunks,MapData.ChunkWidth*WorldSizeInChunks];
        MapData.UpdateWorldSize(WorldSizeInChunks);
        GeneratePlainsAndCliffs();
        PopulateInfoMapData();
        GenerateRiver();
        DebugdrawNoiseMap();
        treePlacer.GenerateTrees(ref gridMapInfo);
        bakeNavMesh();
    }
    void GenerateRiver(){


        float angle = Mathf.PI/4;
        int riverThickness =10;
        int riverLength =Mathf.CeilToInt(Mathf.Sqrt(2*MapData.worldSizeInUnits *MapData.worldSizeInUnits));
        int x1 =0;
        int y1 =0;
        for (int j = 0; j < riverThickness; j++) {
            for (int i = 0; i < riverLength; i++) {

                y1 = Mathf.RoundToInt(((i) * Mathf.Cos(angle) - (Mathf.Sin((i/2 - 0))  + j*0.4f) * Mathf.Sin(angle)));
                x1 = Mathf.RoundToInt(((i) * Mathf.Sin(angle) + (Mathf.Sin((i/2 - 0))  + j*0.4f) * Mathf.Cos(angle)));

                if(x1 < MapData.worldSizeInUnits && y1 < MapData.worldSizeInUnits && x1 >=0 && y1 >=0){
                    gridMapInfo[x1,y1].Item2 = 4;
                }

            }
        }
        List<cubes> water_chunks = new List<cubes>();
        foreach (var chunk in chunks.Values){
            Vector3 pos = chunk.chunk_gameObj.transform.position;
            bool added_chunk = false;
            for (int z = 0 + (int)pos.z - 1; z < MapData.ChunkWidth + (int)pos.z; z++){
            for (int x = 0 + (int)pos.x - 1; x < MapData.ChunkWidth + (int)pos.x; x++){
                    if(x>=0 && z >= 0 &&x+1 < MapData.worldSizeInUnits && z+1 < MapData.worldSizeInUnits && 
                        (gridMapInfo[x,z].Item2 == 4 ||gridMapInfo[x+1,z].Item2 == 4 ||
                            gridMapInfo[x,z+1].Item2 == 4 ||gridMapInfo[x+1,z+1].Item2 == 4) ){
                            Debug.Log("true -1");

                            if(!added_chunk){
                                added_chunk = true;
                                water_chunks.Add(chunk);
                            }
                            // get the corner value hight
                            RaycastHit hitinfo;
                            if(Physics.Raycast(new Vector3(x,5,z),new Vector3(0,-10,0),out hitinfo,6f, (1<<8))){
                                // chunk.getCornerMap()[x - (int)pos.x +1,(int)hitinfo.point.y,z -(int)pos.z + 1] = 1f;
                                // Debug.Log(hitinfo.point + Mathf.FloorToInt(hitinfo.point.y).ToString() + " " + chunk.getCornerMap()[x - (int)pos.x +1,Mathf.FloorToInt(hitinfo.point.y) +1,z -(int)pos.z + 1].ToString()
                                // + " " + chunk.getCornerMap()[x - (int)pos.x +1,Mathf.FloorToInt(hitinfo.point.y) -1,z -(int)pos.z +1 ].ToString());
                                chunk.getCornerMap()[x - (int)pos.x +1,Mathf.FloorToInt(hitinfo.point.y),z -(int)pos.z + 1] = 1f;
                                
                                // bad aid fix to wonky slopes
                                chunk.getCornerMap()[x - (int)pos.x +1,Mathf.FloorToInt(hitinfo.point.y) +1,z -(int)pos.z + 1] =1f;
                                if(chunk.getCornerMap()[x - (int)pos.x +1,Mathf.FloorToInt(hitinfo.point.y) -1,z -(int)pos.z + 1] >= -0.55){
                                    chunk.getCornerMap()[x - (int)pos.x +1,Mathf.FloorToInt(hitinfo.point.y) -1,z -(int)pos.z + 1] =1f;
                                }


                            }else{
                                Debug.Log("Bad ray cast");
                            }
                    }

                }
            }
        }

        Debug.Log("Water Chunks : " + water_chunks.Count.ToString());
        // final loop through water chunks
        foreach (var chunk in water_chunks){
            GameObject water_chunk = new GameObject();

            water_chunk.AddComponent<MeshFilter>();
            water_chunk.GetComponent<MeshFilter>().mesh =chunk.chunk_gameObj.GetComponent<MeshFilter>().mesh;
            water_chunk.AddComponent<MeshRenderer>();
            water_chunk.GetComponent<MeshRenderer>().material = river_mat;
            Vector3 position = chunk.chunk_gameObj.transform.position;
            water_chunk.transform.position = new Vector3(position.x, position.y -0.5f, position.z);
            water_chunk.transform.SetParent(chunk.chunk_gameObj.transform);
            chunk.createMeshData();
        }
    }
    void Update(){
        //regenrateMesh(new Vector3(noise_map_scale, xScroll, zScroll), interpolation);
        // PopulateInfoMapData();


    }

    public void bakeNavMesh(){
        //foreach(var chunk in chunks.Values){
            NavMeshBuilder.BuildNavMesh();
        //}
    }
    void regenrateMesh(Vector3 noise_data, bool interpolation){
        foreach(var chunk in chunks.Values){
            chunk.updateNoiseParams(noise_data,interpolation);
            chunk.handleCubeCornerInfo();
            chunk.createMeshData();
        }
    }
    void GeneratePlainsAndCliffs () {

        //.SetActive(true);

        for (int x = 0; x < WorldSizeInChunks; x++) {
            for (int z = 0; z < WorldSizeInChunks; z++) {

                Vector3Int chunkPos = new Vector3Int(x * MapData.ChunkWidth, 0, z * MapData.ChunkWidth);
                chunks.Add(chunkPos, new cubes(chunkPos, new Vector3(noise_map_scale, xScroll, zScroll), interpolation));
                chunks[chunkPos].chunk_gameObj.transform.SetParent(transform);

            }
        }


       // Debug.Log(string.Format("{0} x {0} world generated.", (WorldSizeInChunks * MapData.ChunkWidth)));

    }
    
    public Renderer testPlaneRenderer;

    //a debugger method
	public void DebugdrawNoiseMap() {
        //gridMapInfo = new int[MapData.ChunkWidth*WorldSizeInChunks,MapData.ChunkWidth*WorldSizeInChunks];
		int width = MapData.ChunkWidth*WorldSizeInChunks;
		int height = MapData.ChunkWidth*WorldSizeInChunks;

		Texture2D texture = new Texture2D (width, height);

		Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				colourMap [y * width + x] = Color.Lerp (Color.black, Color.white, gridMapInfo [x, y].Item2);
			}
		}
		texture.SetPixels (colourMap);
		texture.Apply ();

		testPlaneRenderer.sharedMaterial.mainTexture = texture;
		//testPlaneRenderer.transform.localScale = new Vector3 (width, 1, height);
	}
    void PopulateInfoMapData(){
        for(int i =0; i < MapData.ChunkWidth*WorldSizeInChunks; i++){
            for(int j =0; j < MapData.ChunkWidth*WorldSizeInChunks; j++){
                // hardcoded values could cause problems
                // Debug.Log(LayerMask.NameToLayer("Ground"));
                RaycastHit hit1;
                RaycastHit hit2;
                if(Physics.Raycast(new Vector3(i+0.3f,5,j+0.3f),new Vector3(0,-10,0),out hit1,6f, (1<<8)) ){
                    // assumes all hits are guranteed, possible unsafe
                    gridMapInfo[i,j] = (hit1.point.y,0);
                    if(hit1.normal == Vector3.up ){
                        if(Physics.Raycast(new Vector3(i+0.7f,5,j+0.7f), Vector3.down,out hit2,6f, (1<<8))){
                            if(hit2.normal != Vector3.up){
                                gridMapInfo[i,j] = (gridMapInfo[i,j].Item1,1);
                            }
                        }
                    }else{
                        gridMapInfo[i,j] = (gridMapInfo[i,j].Item1,1);
                    }
                }


                //Physics.Raycast(new Vector3(i+0.7f,5,j+0.7f), Vector3.down,out hit,6f, LayerMask.NameToLayer("Ground"));
                //Debug.DrawRay(new Vector3(i+0.3f,5,j+0.3f), new Vector3(0,-10,0),Color.red, 100);
            }
        }

        // Debug.Log(gridMapInfo[20,0]);
        // Debug.Log(gridMapInfo[20,3]);
        // Debug.Log(gridMapInfo[19,0]);
    }
    public cubes positionToChunk(Vector3 pos){
        return (chunks[new Vector3Int(Mathf.CeilToInt(pos.x/16f)*16-16, 0, Mathf.CeilToInt(pos.z/16f)*16-16)]);
        //Debug.Log(new Vector3Int(Mathf.CeilToInt(pos.x), Mathf.CeilToInt(pos.y), Mathf.CeilToInt(pos.z)));
    }

    // This is buggy for some reason
    public static Vector2Int worldPointToGrid(Vector3 worldPosition){
        // potential error because worldSizeInUnits may not have run yet, although unlikely
        int gridSize = MapData.worldSizeInUnits;
        Debug.Log(gridSize);
        // percetange of the object on the board
        float percentX = (worldPosition.x + gridSize/2)/gridSize;
        float percentY = (worldPosition.z + gridSize/2)/gridSize;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // percentage to actual position
        int x = Mathf.RoundToInt((gridSize-1) * percentX);
        int y = Mathf.RoundToInt((gridSize-1) * percentY);
        return new Vector2Int(x,y);
    }

    public static Vector2Int positionToWorldPoint(Vector3 worldPosition){
        //int gridSize = MapData.worldSizeInUnits;
        return new Vector2Int(Mathf.FloorToInt(worldPosition.x),Mathf.FloorToInt(worldPosition.z));
    }
}