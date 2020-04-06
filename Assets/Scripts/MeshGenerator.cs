using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{

    public float xSize = 5;
    public float zSize = 5;

    public float ySize = 10;

    public int yGlobalModifier = 5;
    private float currentGlobalYModifier;
    private float oldYModifier = 0;
    private int[] yModifiers;
    private Vector2[] uvs;
    
    [SerializeField]
    private int vertCount;
    
    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;

    private float step;
    private float speed;
    
    // Start is called before the first frame update
    void Start()
    {    
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CalculateNewHeight();
        oldYModifier = currentGlobalYModifier;
        CreateShapes(currentGlobalYModifier);
        speed = Random.Range(0.5f, 1.25f);
    }


    private void OnDrawGizmosSelected()
    {
        var offset = 0f;
        if (Application.isPlaying) offset = 1f;
        Gizmos.matrix = this.transform.localToWorldMatrix;
        
        
        Gizmos.color = new Color(255,63,0,0.3f);
        var position = transform.position;
        
        Gizmos.DrawCube(
            position - new Vector3(0f,-ySize *0.5f, 0f),
            new Vector3(xSize+ offset,ySize,zSize + offset));
        
        Gizmos.color = new Color(255,127,0,0.3f);
        Gizmos.DrawCube(
            position - new Vector3(0f,(-ySize - currentGlobalYModifier) *0.5f, 0f),
            new Vector3(xSize+ offset,ySize + currentGlobalYModifier ,zSize + offset));
    }

    // Update is called once per frame
    void Update()
    {
        var currentYModifierStep = currentGlobalYModifier;


        if (step < 1f)
        {
            currentYModifierStep = Mathf.Lerp(oldYModifier, currentGlobalYModifier, step);
            step += Time.deltaTime * speed;
        }
        else
        {
            oldYModifier = currentGlobalYModifier;
            CalculateNewHeight();
            step = 0f;
        }
        
        CreateShapes(currentYModifierStep);
        UpdateMesh();

    }

    void CreateShapes(float yModifier)
    {
        var newYSize = ySize + yModifier;

        var halfXSize = xSize * 0.5f;
        var halfZSize = zSize * 0.5f;
        
        vertices = new List<Vector3>
        {
            // Bottom
            new Vector3(-halfXSize, 0f, -halfZSize),     // Front Left       (0)
            new Vector3(-halfXSize, 0f, halfZSize),         // Back Left        (1)
            new Vector3(halfXSize, 0f, -halfZSize),         // Front Right      (2)
            new Vector3(halfXSize, 0f, halfZSize),             // Back Right       (3)
            
            //Top
            new Vector3(-halfXSize, newYSize, -halfZSize),  // Front Left       (4)
            new Vector3(-halfXSize, newYSize, halfZSize),      // Back Left        (5)
            new Vector3(halfXSize, newYSize, -halfZSize),      // Front Right      (6)
            new Vector3(halfXSize, newYSize, halfZSize),          // Back Right       (7)
        };
        
        triangles = new List<int>
        {
            // Bottom Surface
            0, 1, 2,
            1, 3, 2,
            
            // Left Surface
            0, 1, 4,
            4, 5, 1,
            
            // Right Surface
            2, 3, 6,
            6, 7, 3,
            
            // Front Surface
            0, 2, 4,
            2, 6, 4,

            // Back Surface
            1, 3, 5,
            3, 7, 5,
            
            // Top Surface
            4, 5, 6,
            5, 7, 6
        };
        
        uvs = new Vector2[vertices.Count];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
    }
    
    
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.uv = uvs;
    }

    void CalculateNewHeight()
    {
        currentGlobalYModifier = Random.Range(0, yGlobalModifier);
    }
}
