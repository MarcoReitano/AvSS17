using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// BuildingTester.
/// </summary>
//[ExecuteInEditMode]
public class BuildingTester : MonoBehaviour
{
	//[SerializeField][HideInInspector]
    //private ModularMesh mesh = new ModularMesh((ModularMesh)null);
	[SerializeField][HideInInspector]
    private Polygon polygon = new Polygon();
	[SerializeField][HideInInspector]
    private List<Polygon> rotatedPolygons = new List<Polygon>();
	[SerializeField][HideInInspector]
    private List<QuadStrip> strips = new List<QuadStrip>();
	[SerializeField][HideInInspector]
    private PolygonSurface ceiling = new PolygonSurface();

    [SerializeField]
    [HideInInspector]
    private SmoothingGroup[] wallSmoothingGroups;
    
	[SerializeField][HideInInspector]
    private GameObject[] vertexObjects;

    public int times = 10;
    public float angle = 1f;
    public float height = 5f;
    public float amount = 1f;
    public buildingType thisOrThat = buildingType.version1;
	
    public void Reset()
    {
        polygon = new Polygon();
        vertexObjects = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            vertexObjects[i] = transform.GetChild(i).gameObject;
        }
    }
		
    public void Generate()
    {
        Debug.Log("Generating");
        polygon.Clear();
        rotatedPolygons.Clear();
        strips.Clear();
        ModularMesh mesh = new ModularMesh((ModularMesh) null);


        for (int i = 0; i < vertexObjects.Length; i++)
        {
            polygon.Add(new Vertex(vertexObjects[i].transform.position));
        }
        polygon.MakeClockwise();
        rotatedPolygons.Add(polygon);

        if (thisOrThat == buildingType.version1)
        {
            for (int i = 1; i <= times; i++)
            {
                if (i % 10 == 0)
                    rotatedPolygons.Add(rotatedPolygons[rotatedPolygons.Count - 1].Rotate(angle * 10));
                else
                    rotatedPolygons.Add(rotatedPolygons[rotatedPolygons.Count - 1].Rotate(angle));

                rotatedPolygons.Add(rotatedPolygons[rotatedPolygons.Count - 1].Translate(Vector3.up * height));

                strips.Add(new QuadStrip(rotatedPolygons[rotatedPolygons.Count - 1], rotatedPolygons[rotatedPolygons.Count - 2], mesh, MaterialManager.GetMaterial("diffuseWhite")));
            }

            ceiling = rotatedPolygons[rotatedPolygons.Count - 1].Triangulate(mesh, MaterialManager.GetMaterial("diffuseWhite"));
        }
        if (thisOrThat == buildingType.version2)
        {
            for (int i = 1; i <= times; i++)
            {
                if (i % 10 == 0)
                    rotatedPolygons.Add(rotatedPolygons[rotatedPolygons.Count - 1].Rotate(angle * 10).Translate(height* Vector3.up));
                else
                    rotatedPolygons.Add(rotatedPolygons[rotatedPolygons.Count - 1].Rotate(angle).Translate(height * Vector3.up));

                strips.Add(new QuadStrip(rotatedPolygons[rotatedPolygons.Count - 1], rotatedPolygons[rotatedPolygons.Count - 2], mesh, MaterialManager.GetMaterial("diffuseWhite")));
            }

            ceiling = rotatedPolygons[rotatedPolygons.Count - 1].Triangulate(mesh, MaterialManager.GetMaterial("diffuseWhite"));
        }
        if (thisOrThat == buildingType.version3)
        {
            for (int i = 1; i <= times; i++)
            {
                rotatedPolygons.Add(rotatedPolygons[rotatedPolygons.Count - 1].Rotate(angle).Translate(height * Vector3.up));

                strips.Add(new QuadStrip(rotatedPolygons[rotatedPolygons.Count - 1], rotatedPolygons[rotatedPolygons.Count - 2], mesh, MaterialManager.GetMaterial("diffuseWhite")));

            }

            for (int i = 0; i < wallSmoothingGroups.Length; i++)
            {
                wallSmoothingGroups[i] = new SmoothingGroup();
            }

            for (int i = 0; i < strips.Count; i++)
            {
                for( int j = 0; j < strips[i].Quads.Count; j++)
                {
                    strips[j].AddSmoothingGroup(wallSmoothingGroups[i]);
                }
            }

            ceiling = rotatedPolygons[rotatedPolygons.Count - 1].Triangulate(mesh, MaterialManager.GetMaterial("diffuseWhite"));

            ceiling.AddSmoothingGroup(new SmoothingGroup());
        }
        if (thisOrThat == buildingType.version4)
        {
            for (int i = 1; i <= times; i++)
            {
                rotatedPolygons.Add(rotatedPolygons[rotatedPolygons.Count - 1].Rotate(angle).Translate(height * Vector3.up).Inset(amount));

                strips.Add(new QuadStrip(rotatedPolygons[rotatedPolygons.Count - 1], rotatedPolygons[rotatedPolygons.Count - 2], mesh, MaterialManager.GetMaterial("diffuseWhite")));
            }

            ceiling = rotatedPolygons[rotatedPolygons.Count - 1].Triangulate(mesh, MaterialManager.GetMaterial("diffuseWhite"));
        }
        if (thisOrThat == buildingType.version5)
        {
            rotatedPolygons.Add(rotatedPolygons[0].Translate(height * Vector3.up));
            strips.Add(new QuadStrip(rotatedPolygons[rotatedPolygons.Count - 1], rotatedPolygons[0], mesh, MaterialManager.GetMaterial("diffuseWhite")));

            ceiling = rotatedPolygons[rotatedPolygons.Count - 1].Triangulate(mesh, MaterialManager.GetMaterial("diffuseWhite"));
        }


        mesh.FillMesh(transform, false);

        rotatedPolygons.Clear();
        strips.Clear();
        ceiling = null;
        Debug.Log("Generating done");
    }

    public void OnDrawGizmosSelecteds()
    {
        polygon.OnDrawGizmos();
        if (rotatedPolygons != null)
        {
            for(int i = 0; i < rotatedPolygons.Count; i++)
                rotatedPolygons[i].OnDrawGizmos();
        }
    }

    public enum buildingType
    {
        version1,
        version2,
        version3,
        version4,
        version5, 
        version6
    }
}
