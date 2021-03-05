using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDraw : MonoBehaviour
{
    void Start()
    {
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();        
        Mesh mesh = new Mesh();
        List<Vector3> verticies = new List<Vector3>();
        List<int> indicies = new List<int>();
            
        for (int i = 0; i <= 0; i++)
        {
            verticies.Add(new Vector3(i, 0, 0));
            verticies.Add(new Vector3(i, 0, 100));
            indicies.Add(8 * i + 0); //0,2,8
            indicies.Add(8 * i + 1); //1,3,9
                
            verticies.Add(new Vector3(i, 0, 0));
            verticies.Add(new Vector3(100, 0, i));
            indicies.Add(8 * i + 2); //0,2,8
            indicies.Add(8 * i + 3); //1,3,9
                
            verticies.Add(new Vector3(100, 0, i));
            verticies.Add(new Vector3(100, 0, 100));
            indicies.Add(8 * i + 4); //0,2,8
            indicies.Add(8 * i + 5); //1,3,9
                
            verticies.Add(new Vector3(100, 0, 100));
            verticies.Add(new Vector3(i, 0, 100));
            indicies.Add(8 * i + 6); //0,2,8
            indicies.Add(8 * i + 7); //1,3,9
        }

        mesh.vertices = verticies.ToArray(); 
        mesh.SetIndices(indicies.ToArray(), MeshTopology.Lines, 0);
        filter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = Color.white;
    }

}
