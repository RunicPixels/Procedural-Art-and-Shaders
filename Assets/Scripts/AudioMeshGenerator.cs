using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class AudioMeshGenerator : MonoBehaviour
{
    public bool rockBottom = true;
    
    public AudioScale audioScale;
    
    public float xSize = 5;
    public float zSize = 5;

    public float ySize = 10;

    public float yGlobalModifier = 5f;
    private float currentGlobalYModifier;
    private int[] yModifiers;
    private Vector2[] uvs;
    
    [SerializeField]
    private int vertCount;
    
    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;

    private float step;
    private float speed;
    
    [Range(0,7)]
    public int range = 0;
    
    // Start is called before the first frame update
    void Start()
    {    
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CalculateNewHeight();
        CreateShapes(currentGlobalYModifier);
        speed = Random.Range(0.5f, 1.25f);
    }


    private void OnDrawGizmosSelected()
    {
        var offset = 0f;
        Gizmos.matrix = this.transform.localToWorldMatrix;
        
        if (Application.isPlaying) offset = 1f;
        
        Gizmos.color = new Color(255,63,0,0.3f);
        var position = Vector3.zero;
        
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
        CalculateNewHeight();

        CreateShapes(currentGlobalYModifier);
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
            
            // Rock Bottom
            new Vector3(0,-ySize-(yModifier * 0.3f), 0)                  // Bottom          (8)
        };
        if(!rockBottom) triangles = new List<int>
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
        
        if(rockBottom) triangles = new List<int>
        {
            // Bottom Left Surface
            0, 1, 8,

            // Bottom Right Surface
            2, 3, 8,

            // Bottom Front Surface
            0, 2, 8,

            // Bottom Back Surface
            1, 3, 8,

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
        mesh.uv = uvs;
        mesh.RecalculateNormals();
    }

    void CalculateNewHeight()
    {
        currentGlobalYModifier = AudioScale.BandBuffer[range] * yGlobalModifier;
    }
}
