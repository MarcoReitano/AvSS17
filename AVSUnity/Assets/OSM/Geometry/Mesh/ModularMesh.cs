using UnityEngine;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

/// <summary>
/// ModularMesh.
/// Dient dazu einen MeshBaum aufzubauen.
/// </summary>
public class ModularMesh
{
    public ModularMesh()
    { }
    public ModularMesh(string name)
    {
        this.name = name;
    }
    public ModularMesh(ModularMesh parentMesh)
    {
        this.parentMesh = parentMesh;
        if (parentMesh != null)
            parentMesh.AddChildMesh(this);
    }
    public ModularMesh(ModularMesh parentMesh, string name)
    {
        this.parentMesh = parentMesh;
        if (parentMesh != null)
            parentMesh.AddChildMesh(this);
        this.name = name;
    }

    public ModularMesh ParentMesh
    {
        get { return parentMesh; }
        set
        {
            if (value != parentMesh)
            {
                if (parentMesh != null)
                    parentMesh.RemoveChildMesh(this);
                parentMesh = value;
                if (parentMesh != null)
                    parentMesh.AddChildMesh(this);
            }
        }
    }
    public ReadOnlyCollection<ModularMesh> ChildMeshes
    {
        get { return childMeshes.AsReadOnly(); }
    }
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public ReadOnlyCollection<Vector3> Normals
    {
        get { return normals.AsReadOnly(); }
    }
    public ReadOnlyCollection<Vector3> Tangents
    {
        get { return tangents.AsReadOnly(); }
    }
    public ReadOnlyCollection<Vector2> UV
    {
        get { return uV.AsReadOnly(); }
    }
    public ReadOnlyCollection<Vector2> UV1
    {
        get { return uV1.AsReadOnly(); }
    }
    public ReadOnlyCollection<Vector2> UV2
    {
        get { return uV2.AsReadOnly(); }
    }

    //public GameObject RelatedMeshGameObject
    //{
    //    get { return relatedMeshGameObject; }
    //    set
    //    {
    //        relatedMeshGameObject = value;
    //        if (relatedMeshGameObject)
    //        {
    //            mF = relatedMeshGameObject.GetOrAddComponent<MeshFilter>();
    //            mC = relatedMeshGameObject.GetOrAddComponent<MeshCollider>();
    //            mR = relatedMeshGameObject.GetOrAddComponent<MeshRenderer>();
    //        }
    //    }
    //}
    //public Mesh RelatedUnityMesh
    //{
    //    get { return relatedUnityMesh; }
    //    set { relatedUnityMesh = value; }
    //}

    private ModularMesh parentMesh;
    private List<ModularMesh> childMeshes = new List<ModularMesh>();
    private string name;

    private Dictionary<Material, List<Triangle>> materialTriangles = new Dictionary<Material, List<Triangle>>();
    private List<Vector3> normals = new List<Vector3>();
    private List<Vector3> tangents = new List<Vector3>();
    private List<Vector2> uV = new List<Vector2>();
    private List<Vector2> uV1 = new List<Vector2>();
    private List<Vector2> uV2 = new List<Vector2>();
    private GameObject relatedMeshGameObject = null;
    private Mesh relatedUnityMesh;
    private MeshFilter mF;
    private MeshCollider mC;
    private MeshRenderer mR;
    private List<SmoothingGroup> smoothingGroups = new List<SmoothingGroup>();

    public void AddChildMesh(ModularMesh modularMesh)
    {
        if(!childMeshes.Contains(modularMesh))
            childMeshes.Add(modularMesh);
    }
    public void RemoveChildMesh(ModularMesh modularMesh)
    {
        childMeshes.Remove(modularMesh);
    }

    public void AddTriangle(Triangle t, Material m)
    {
        List<Triangle> materialTriangleList;
        if (materialTriangles.TryGetValue(m, out materialTriangleList))
            materialTriangleList.Add(t);
        else
        {
            materialTriangleList = new List<Triangle>();
            materialTriangleList.Add(t);
            materialTriangles.Add(m, materialTriangleList);
        }
    }
    public void AddTriangles(IEnumerable<Triangle> t, Material m)
    {
        List<Triangle> materialTriangleList;
        if (materialTriangles.TryGetValue(m, out materialTriangleList))
            materialTriangleList.AddRange(t);
        else
        {
            materialTriangleList = new List<Triangle>();
            materialTriangleList.AddRange(t);
            materialTriangles.Add(m, materialTriangleList);
        }

    }
    public bool RemoveTriangle(Triangle t)
    {
        bool hasRemoved = false;
        foreach (KeyValuePair<Material, List<Triangle>> kV in materialTriangles)
        {
            if (kV.Value.Remove(t))
                hasRemoved = true;
        }
        return hasRemoved;
    }
    public bool RemoveTriangle(Triangle t, Material m)
    {
        List<Triangle> triangleList;
        if (materialTriangles.TryGetValue(m, out triangleList))
            return triangleList.Remove(t);
        return false;
    }

    public void Clear()
    {
        for (int i = 0; i < childMeshes.Count; i++)
        {
            childMeshes[i].Clear();
        }
        materialTriangles.Clear();
        normals.Clear();
        tangents.Clear();
        uV.Clear();
        uV1.Clear();
        uV2.Clear();
        smoothingGroups.Clear();
    }

    public Dictionary<Material, List<Triangle>> CollectTriangles()
    {
        Dictionary<Material, List<Triangle>> result = new Dictionary<Material, List<Triangle>>(materialTriangles);
        Dictionary<Material, List<Triangle>> childResult = new Dictionary<Material, List<Triangle>>();

        for (int i = 0; i < childMeshes.Count; i++)
        {
            childResult = childMeshes[i].CollectTriangles();

            foreach (KeyValuePair<Material, List<Triangle>> item in childResult)
            {
                if (result.ContainsKey(item.Key))
                    result[item.Key].AddRange(item.Value);
                else
                    result.Add(item.Key, item.Value);
            }
        }
        return result;
    }

    Dictionary<Material, List<Triangle>> tmpMaterialTriangles;
    Dictionary<int, List<int>> triangleIndices = new Dictionary<int, List<int>>();
    Dictionary<MeshMember, int> test = new Dictionary<MeshMember, int>();
    Material[] materials;

    //Fills Mesh With OWN AND CHILDS vertices and triangles
    public void FillMesh(Transform transform, bool destroyChildsFirst)
    {
        if(destroyChildsFirst)
            destroyAllSubmeshContainer(transform);
        mR = transform.gameObject.GetOrAddComponent<MeshRenderer>();
        mC = transform.gameObject.GetOrAddComponent<MeshCollider>();
        mF = transform.gameObject.GetOrAddComponent<MeshFilter>();

        Stopwatch sw = new Stopwatch();
        sw.Start();

        if (relatedUnityMesh != null)
            relatedUnityMesh.Clear();
        else
            relatedUnityMesh = new Mesh();
        relatedUnityMesh.name = "GeneratedModularMesh";

        tmpMaterialTriangles = CollectTriangles();
        triangleIndices.Clear();
        test.Clear();
        materials = tmpMaterialTriangles.Keys.ToArray();
       
        foreach (KeyValuePair<Material, List<Triangle>> item in tmpMaterialTriangles)
        {
            triangleIndices.Add(triangleIndices.Count, new List<int>());


            for (int i = 0; i < item.Value.Count; i++) //Loop over each Triangle in list
            {
                Vertex[] triangleVertices = item.Value[i].Vertices;
                for(int j = 0; j < 3; j++)
                {
                    if (item.Value[i].SmoothingGroups.Count == 0)
                    {
                        MeshMember member = new MeshMember(triangleVertices[j], item.Value[i], item.Key, null);

                        if (!test.ContainsKey(member))
                        {
                            test.Add(member, test.Count);
                        }
                        triangleIndices[triangleIndices.Count - 1].Add(test[member]);
                    }
                    else
                    {
                        for (int k = 0; k < item.Value[i].SmoothingGroups.Count; k++)
                        {
                            MeshMember member = new MeshMember(triangleVertices[j], item.Value[i], item.Key, item.Value[i].SmoothingGroups[k]);

                            if (!test.ContainsKey(member))
                            {
                                test.Add(member, test.Count);
                            }
                            triangleIndices[triangleIndices.Count - 1].Add(test[member]);
                        }
                    }
                }
            }
        }


        Vector3[] vertexPositions = new Vector3[test.Count];
        Vector3[] vertexNormals = new Vector3[test.Count];
        Vector4[] vertexTangents = new Vector4[test.Count];
        Vector2[] vertexUVs = new Vector2[test.Count];


        foreach (KeyValuePair<MeshMember, int> kV in test)
        {
            vertexPositions[kV.Value] = kV.Key.Vertex.Position - transform.position; // DONE


            if (kV.Key.SmoothingGroup == null)
                vertexNormals[kV.Value] = kV.Key.Triangle.Normal;
            else
                vertexNormals[kV.Value] = kV.Key.SmoothingGroup.Normal(kV.Key.Vertex);

            vertexUVs[kV.Value] = new Vector2(kV.Key.Vertex.X, kV.Key.Vertex.Y); // TODO
        }

        mR.materials = materials;
        relatedUnityMesh.subMeshCount = materials.Length;
        relatedUnityMesh.vertices = vertexPositions;
        relatedUnityMesh.normals = vertexNormals;
        relatedUnityMesh.tangents = vertexTangents;
        relatedUnityMesh.uv = vertexUVs;

        foreach(KeyValuePair<int, List<int>> kV in triangleIndices)
        {
            relatedUnityMesh.SetTriangles(kV.Value.ToArray(), kV.Key);
        }

        relatedUnityMesh.RecalculateBounds();
        mF.mesh = relatedUnityMesh;
        mC.sharedMesh = relatedUnityMesh;
    }
    public void FillMeshDivideMaterials(Transform transform, bool excludingChilds, bool destroyChildsFirst)
    {
        if(destroyChildsFirst)
            destroyAllSubmeshContainer(transform);
        if (!mC)
            mC = transform.gameObject.GetComponent<MeshCollider>();
        if (!mF)
            mF = transform.gameObject.GetComponent<MeshFilter>();
        if (!mR)
            mR = transform.gameObject.GetComponent<MeshRenderer>();

        if(mC.sharedMesh != null)
            mC.sharedMesh.Clear();
        mC.sharedMesh = null;
        if(mF.sharedMesh != null)
            mF.sharedMesh.Clear();

        mR.materials = new Material[0];

        if (excludingChilds)
            tmpMaterialTriangles = materialTriangles;
        else
            tmpMaterialTriangles = CollectTriangles();

        foreach(KeyValuePair<Material, List<Triangle>> kV in tmpMaterialTriangles)
        {
            // IS THIS CORRECT?! WE MIGHT LOSE A TRIANGLE...SO BETTER CHECK IT AGAIN
            int i = 0;
            while (i < kV.Value.Count)
            {
                Transform submeshTransform = createSubmeshContainerGameobject(transform, this.name + "SubMesh" + kV.Key.name + i);
                ModularMesh submeshModularMesh = new ModularMesh(this, this.name + "SubMesh" + kV.Key.name + i);
                if (i + 21666 < kV.Value.Count)
                    submeshModularMesh.AddTriangles(kV.Value.GetRange(i, 21666), kV.Key);
                else
                    submeshModularMesh.AddTriangles(kV.Value.GetRange(i, kV.Value.Count - i), kV.Key);
                submeshModularMesh.FillMesh(submeshTransform, false);

                i += 21666;
            }
            
        }
 
    }
    public void FillMeshDivideMaterialsKeepMeshStructure(Transform transform, bool destroyChildsFirst)
    {
        if (destroyChildsFirst)
            destroyAllSubmeshContainer(transform);
        //UnityEngine.Debug.Log("FillMeshKeepStructure of " + this.name);
        //UnityEngine.Debug.Log("ChildMeshCount: " + childMeshes.Count);
        for (int i = 0; i < childMeshes.Count; i++)
        {
            //UnityEngine.Debug.Log(this.name + " parent of " + childMeshes[i].name);
            Transform childTransform = createSubmeshContainerGameobject(transform, childMeshes[i].name);
            childMeshes[i].FillMeshDivideMaterialsKeepMeshStructure(childTransform, false);
        }

        FillMeshDivideMaterials(transform, true, false);        
    }

    private Transform createSubmeshContainerGameobject(Transform transform, string subMeshName)
    {
        GameObject submeshContainer = new GameObject(subMeshName, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        submeshContainer.transform.position = transform.position;
        submeshContainer.transform.rotation = transform.rotation;
        submeshContainer.transform.localScale = transform.localScale;
        submeshContainer.transform.parent = transform;
        return submeshContainer.transform;
    }
    //private Transform createMaterialSubmeshContainerGameobject(Transform transform)
    //{
    //    GameObject submeshContainer = new GameObject(this.name + "MaterialSubMesh", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
    //    submeshContainer.transform.position = transform.position;
    //    submeshContainer.transform.rotation = transform.rotation;
    //    submeshContainer.transform.localScale = transform.localScale;
    //    submeshContainer.transform.parent = transform;
    //    return submeshContainer.transform;
    //}

    private void destroyAllSubmeshContainer(Transform transform)
    {
        for (int i = transform.childCount-1; i >= 0 ; i--)
        {
            //if (transform.GetChild(i).name == "SubmeshContainer") //Destroy all at the moment
            {
#if UNITY_EDITOR
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
#else
                GameObject.Destroy(transform.gameObject);
#endif
            }
        }
    }

    private struct MeshMember
    {
        public MeshMember(Vertex vertex, Triangle triangle, Material material, SmoothingGroup sG)
        {
            this.Vertex = vertex;
            this.Triangle = triangle;
            this.Material = material;
            this.SmoothingGroup = sG;
        }

        public readonly Vertex Vertex;
        public readonly Triangle Triangle;
        public readonly Material Material;
        public readonly SmoothingGroup SmoothingGroup;


        public override bool Equals(object obj)
        {
            if (!(obj is MeshMember))
                return false;

            MeshMember mM = (MeshMember)obj;

            if (this.SmoothingGroup == null)
            {
                if (this.Vertex == mM.Vertex && this.Triangle == mM.Triangle && this.Material == mM.Material)
                    return true;
                return false;
            }
            else
            {
                if (this.Vertex == mM.Vertex && this.Material == mM.Material && this.SmoothingGroup == mM.SmoothingGroup)
                    return true;
                return false; 
            }
        }
        public bool Equals(MeshMember mM)
        {
            if (this.SmoothingGroup == null)
            {
                if (this.Vertex == mM.Vertex && this.Triangle == mM.Triangle && this.Material == mM.Material)
                    return true;
                return false;
            }
            else
            {
                if (this.Vertex == mM.Vertex && this.Material == mM.Material && this.SmoothingGroup == mM.SmoothingGroup)
                    return true;
                return false;
            }
        }

        public override int GetHashCode()
        {

            ////// Überarbeiten wenn Vertex und Triangle ids bekommen?! Schneller?!

            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + Vertex.GetHashCode();
                if (SmoothingGroup == null)
                    hash = hash * 23 + Triangle.GetHashCode();
                hash = hash * 23 + Material.GetHashCode();
                if (SmoothingGroup != null)
                    hash = hash * 23 + SmoothingGroup.GetHashCode();
                return hash;
            }
        }
    }
}
