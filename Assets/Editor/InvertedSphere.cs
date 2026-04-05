using UnityEngine;
using UnityEditor;

public class InvertedSphere : EditorWindow
{
    private string sizeString = "1.0";

    [MenuItem("GameObject/Create Other/Inverted Sphere...")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<InvertedSphere>("Inverted Sphere");
    }

    public void OnGUI()
    {
        GUILayout.Label("Enter sphere size:", EditorStyles.boldLabel);
        sizeString = GUILayout.TextField(sizeString);

        if (GUILayout.Button("Create Inverted Sphere"))
        {
            if (float.TryParse(sizeString, out float size))
            {
                CreateInvertedSphere(size);
            }
            else
            {
                Debug.LogError("Please enter a valid number for the size.");
            }
        }
    }

    private void CreateInvertedSphere(float size)
    {
        // 1. Create a temporary primitive to grab the mesh data
        GameObject tempGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Mesh sourceMesh = tempGo.GetComponent<MeshFilter>().sharedMesh;
        Material sourceMaterial = tempGo.GetComponent<MeshRenderer>().sharedMaterial;

        // 2. Setup the new object
        GameObject goNew = new GameObject("Inverted Sphere");
        MeshFilter mfNew = goNew.AddComponent<MeshFilter>();
        MeshRenderer mrNew = goNew.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        mesh.name = "InvertedSphereMesh";

        // 3. Process Vertices (Scaling)
        Vector3[] vertices = sourceMesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] *= size;
        }

        // 4. Process Normals (Flip them inside out)
        Vector3[] normals = sourceMesh.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i];
        }

        // 5. Process Triangles (Reverse winding order)
        int[] triangles = sourceMesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Swap the first and third vertex of every triangle
            int temp = triangles[i];
            triangles[i] = triangles[i + 2];
            triangles[i + 2] = temp;
        }

        // 6. Assign data to new mesh
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;
        mesh.uv = sourceMesh.uv;

        mfNew.sharedMesh = mesh;
        mrNew.sharedMaterial = sourceMaterial;

        // Clean up
        DestroyImmediate(tempGo);

        // Focus the new object in the editor
        Selection.activeGameObject = goNew;
    }
}