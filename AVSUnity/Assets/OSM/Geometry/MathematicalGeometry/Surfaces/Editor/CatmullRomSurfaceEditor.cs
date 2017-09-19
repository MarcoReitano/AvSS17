using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CatmullRomSurfaceBehaviour))]
public class CatmullRomSurfaceEditor : Editor
{

    private CatmullRomSurfaceBehaviour catmullRomSurface;

    public void Awake()
    {
        catmullRomSurface = target as CatmullRomSurfaceBehaviour;
    }


    public void OnSceneGUI()
    {
        Event currentEvent = Event.current;

        Handles.color = Color.yellow;

        bool changed = false;

        if (this.catmullRomSurface.ControlPoints != null)
        {
            for (int u = 0; u < catmullRomSurface.NumberOfControlPointsWidth; u++)
            {
                for (int w = 0; w < catmullRomSurface.NumberOfControlPointsHeight; w++)
                {
                    Vector3 oldPosition = this.catmullRomSurface.ControlPoints[u, w];
                    Vector3 newPosition = Handles.FreeMoveHandle(oldPosition, Quaternion.identity, handleSize, Vector3.zero, Handles.SphereCap);
                    if (newPosition != oldPosition)
                    {
                        this.catmullRomSurface.ControlPoints[u, w] = new Vector3(oldPosition.x, newPosition.y, oldPosition.z);
                        changed = true;
                    }
                }
            }
        }
        if (changed)
        {
            catmullRomSurface.RecalculateSurface();
            catmullRomSurface.GenerateMesh();
            EditorUtility.SetDirty(catmullRomSurface);
        }
    }

    [SerializeField]
    private float handleSize = 3f;

    public override void OnInspectorGUI()
    {
    
        if (GUILayout.Button("Initialize"))
        {
            catmullRomSurface.Initialize();
        }
        
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();


        float oldGridSize = catmullRomSurface.GridSize;
        float newGridSize = EditorGUILayout.Slider("gridSize", oldGridSize, 0.1f, 100f);
        if (newGridSize != oldGridSize)
        {
            catmullRomSurface.GridSize = newGridSize;
            catmullRomSurface.GenerateMesh();
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        int oldIntValue = catmullRomSurface.NumberOfSegmentsWidth;
        int newIntValue = EditorGUILayout.IntSlider("# Segements Width", oldIntValue, 1, 100);
        if (oldIntValue != newIntValue)
        {
            catmullRomSurface.NumberOfSegmentsWidth = newIntValue;
            catmullRomSurface.GenerateMesh();
        }

        oldIntValue = catmullRomSurface.NumberOfSegmentsHeight;
        newIntValue = EditorGUILayout.IntSlider("# Segements Height", oldIntValue, 1, 100);
        if (oldIntValue != newIntValue)
        {
            catmullRomSurface.NumberOfSegmentsHeight = newIntValue;
            catmullRomSurface.GenerateMesh();
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        oldIntValue = catmullRomSurface.NumberOfControlPointsHeight;
        newIntValue = EditorGUILayout.IntSlider("# Controlpoints Height", oldIntValue, 4, 100);
        if (oldIntValue != newIntValue)
        {
            catmullRomSurface.NumberOfControlPointsHeight = newIntValue;
            catmullRomSurface.Initialize();
            catmullRomSurface.GenerateMesh();
        }

        oldIntValue = catmullRomSurface.NumberOfControlPointsWidth;
        newIntValue = EditorGUILayout.IntSlider("# Controlpoints Width", oldIntValue, 4, 100);
        if (oldIntValue != newIntValue)
        {
            catmullRomSurface.NumberOfControlPointsWidth = newIntValue;
            catmullRomSurface.Initialize();
            catmullRomSurface.GenerateMesh();
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        float oldGizmoSize = handleSize;
        float newGizmoSize = EditorGUILayout.Slider("GizmoSize", oldGizmoSize, 0f, 10f);
        if (newGizmoSize != oldGizmoSize)
        {
            handleSize = newGizmoSize;
        }


        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        catmullRomSurface.drawGizmos = EditorGUILayout.Toggle("Draw Gizmo", catmullRomSurface.drawGizmos);
        catmullRomSurface.drawMeshGizmo = EditorGUILayout.Toggle("Draw Mesh Gizmo", catmullRomSurface.drawMeshGizmo);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        bool oldBoolValue = catmullRomSurface.generateMesh;
        bool newBoolValue = EditorGUILayout.Toggle("Generate Mesh", catmullRomSurface.generateMesh);
        if (newBoolValue != oldBoolValue)
        {
            catmullRomSurface.generateMesh = newBoolValue;
            catmullRomSurface.GenerateMesh();
        }


        Material oldMaterial = catmullRomSurface.material;
        Material newMaterial = EditorGUILayout.ObjectField("Material", oldMaterial, typeof(Material), true) as Material;
        if (oldMaterial != newMaterial)
        {
            catmullRomSurface.material = newMaterial;
            catmullRomSurface.GenerateMesh();
        }

        Color oldColor = catmullRomSurface.color;
        Color newColor = EditorGUILayout.ColorField("Color", oldColor);
        if (newColor != oldColor)
        {
            catmullRomSurface.color = newColor;
            catmullRomSurface.GenerateMesh();
        }

        float oldFloatValue = catmullRomSurface.alpha;
        float newFloatValue = EditorGUILayout.Slider("Alpha", oldFloatValue, 0f, 1f);
        if (newFloatValue != oldFloatValue)
        {
            catmullRomSurface.alpha = newFloatValue;
            catmullRomSurface.GenerateMesh();
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (GUI.changed)
            EditorUtility.SetDirty(catmullRomSurface);

        DrawDefaultInspector();
    }




}

