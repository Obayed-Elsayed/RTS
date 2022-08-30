using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreationUtils : MonoBehaviour
{
    public Material mountainMaterial;
    public void createMountain(ref List<Mountain> mountains){
        mountains.Add(new Mountain(new Vector3(0,0,0),Vector3.zero,(10,10),false));
    }
    public void createMountain(Vector3 position, Vector3 size, (int x, int y) meshSize, bool randomSize){
        Mountain mountain = new Mountain(position, size, meshSize, randomSize);
        mountain.UpdatemountainMesh(mountainMaterial);
        
    }
    public class Mountain
    {
        public Vector3 center;
        public Vector3 size;
        public (int x, int y) meshQuadsSize;
        public GameObject mountain;
        Mesh mountainMesh;
        Vector3[] mountainVertices;
        int[] mountainTriangles;

        public Mountain(Vector3 position, Vector3 size, (int x, int y) meshSize, bool randomSize)
        {
            this.center = position;
            this.size = size;
            this.meshQuadsSize = meshSize;
            if (randomSize)
            {
                this.size = new Vector3(Random.Range(7, 10), 0, Random.Range(7, 10));
            }
            mountain = new GameObject();
            mountain.transform.position = position;
            Vector3 cornerA = new Vector3(-size.x, 0 , -size.z);
            Vector3 cornerB = new Vector3(size.x, 0 , size.z);
            this.CreatemountainMesh(cornerA, cornerB, 0.1f);

        }
        private void CreatemountainMesh(Vector3 startPos, Vector3 endPos, float yPosition)
        {
            //List<Vector3> pond= new List<Vector3>();
            Vector3 scale = endPos - startPos;
            scale.x /= meshQuadsSize.x;
            scale.z /= meshQuadsSize.y;
            mountain.AddComponent<MeshFilter>();
            mountain.AddComponent<MeshRenderer>();
            mountainMesh = new Mesh();
            mountain.GetComponent<MeshFilter>().mesh = mountainMesh;
            mountainVertices = new Vector3[(meshQuadsSize.x + 1) * (meshQuadsSize.y + 1)];
            int i = 0;
            int c = 4;
            int a = 7;

            // int mountaintops = Random.Range(1,5);
            // c/=mountaintops;
            //Debug.Log("Mounation tops" +mountaintops.ToString());
            //Vector2 [] mountainPositions = new Vector2[mountaintops];
            // for (int j = 0; j < mountainPositions.Length; j++)
            // {
            //     mountainPositions[j] = new Vector2(Random.Range(meshQuadsSize.x/4, 3*meshQuadsSize.x/4),Random.Range(meshQuadsSize.y/4, 3*meshQuadsSize.y/4));
            // }
            for (int z = 0; z <= meshQuadsSize.y; z++)
            {
                // gaussian distribution + slap some noisseee
                for (int x = 0; x <= meshQuadsSize.x; x++)
                {

                    mountainVertices[i] = new Vector3(startPos.x + x * scale.x, yPosition, startPos.z + z * scale.z);
                    if(x==0 || z == 0 || x == meshQuadsSize.x || z == meshQuadsSize.y){
                        i++;
                        continue;
                    }
                    mountainVertices[i].y = a*(Mathf.Exp(-1 * (Mathf.Pow(x - meshQuadsSize.x/2, 2) + Mathf.Pow(z - meshQuadsSize.y/2, 2)) / (2 * c * c)));
                    //mountainVertices[i].y = 3*Mathf.Sin(x/3) + Mathf.Sin(z/3);
                    mountainVertices[i].y+= Mathf.PerlinNoise((float)(5*x)/meshQuadsSize.x, (float)(5*z)/meshQuadsSize.y)*3f;
                    //mountainVertices[i].y = Mathf.Clamp(mountainVertices[i].y,0.2f,10f);
                    i++;
                }
            }
            int vert = 0;
            int tris = 0;
            mountainTriangles = new int[6 * meshQuadsSize.x * meshQuadsSize.y];
            for (int z = 0; z < meshQuadsSize.y; z++)
            {
                for (int x = 0; x < meshQuadsSize.x; x++)
                {
                    mountainTriangles[tris + 0] = vert + 0;
                    mountainTriangles[tris + 1] = vert + meshQuadsSize.x + 1;
                    mountainTriangles[tris + 2] = vert + 1;
                    mountainTriangles[tris + 3] = vert + 1;
                    mountainTriangles[tris + 4] = vert + meshQuadsSize.x + 1;
                    mountainTriangles[tris + 5] = vert + meshQuadsSize.x + 2;
                    vert++;
                    tris += 6;
                }
                vert++;
            }

        }
        public void UpdatemountainMesh(Material mat)
        {
            mountainMesh.Clear();
            mountainMesh.vertices = mountainVertices;
            mountainMesh.triangles = mountainTriangles;
            mountainMesh.RecalculateNormals();
            mountain.GetComponent<Renderer>().material = mat;
        }


    }
}

/*
                    mountainVertices[i] = new Vector3(startPos.x + x * scale.x, yPosition, startPos.z + z * scale.z);
                    float dist = Vector3.Distance(mountainVertices[i]+this.center, this.center);
                    //Debug.Log(Mathf.PerlinNoise(dist, 0) *1/dist);
                    if(x==0 || z == 0 || x == meshQuadsSize.x || z == meshQuadsSize.y){
                        i++;
                        continue;
                    }
                    //mountainVertices[i].y+= (2/dist+1)*Mathf.PerlinNoise((float)(2*x)/meshQuadsSize.x, (float)(2*z)/meshQuadsSize.y);
                    if (x < meshQuadsSize.x / 2 &&  (-x + meshQuadsSize.x / 2)!=0)
                    {
                        mountainVertices[i].y += 5 / (-x + meshQuadsSize.x / 2);
                    }
                    else if((x - meshQuadsSize.x / 2)!=0)
                    {
                        mountainVertices[i].y += 5 / (x - meshQuadsSize.x / 2);
                    }else{
                        mountainVertices[i].y+= 5f+ Random.RandomRange(-0.5f,0.5f);
                    }
                    if(mountainVertices[i].y>5){
                        mountainVertices[i].y += -2+Random.RandomRange(-0.5f,0.9f);
                    }
                    mountainVertices[i].y+= Mathf.PerlinNoise((float)(5*x)/meshQuadsSize.x, (float)(5*z)/meshQuadsSize.y);
                    //mountainVertices[i].y = Mathf.Clamp(mountainVertices[i].y,0.1f,5f);

                    i++;
*/
