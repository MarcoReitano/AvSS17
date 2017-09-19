using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BicubicPatch))]
public class BicubicPatchEditor : Editor
{

    private BicubicPatch bicubicPatch;

	public void Awake()
	{
        bicubicPatch = target as BicubicPatch;
	}


	public void OnSceneGUI()
	{
		Event currentEvent = Event.current;

		Handles.color = Color.yellow;

        bool changed = false;
        for (int u = 0; u < 4; u++)
        {
            for (int w = 0; w < 4; w++)
            {
                Vector3 oldPosition = this.bicubicPatch.points[u, w];
                Vector3 newPosition = Handles.FreeMoveHandle(oldPosition, Quaternion.identity, 0.2f, Vector3.zero, Handles.SphereCap);
                if (newPosition != oldPosition)
                {
                    this.bicubicPatch.points[u, w] = newPosition;
                    changed = true;
                }
            }
        }
        if (changed)
            bicubicPatch.RecalculateSurface();

        EditorUtility.SetDirty(bicubicPatch);
	}




	public override void OnInspectorGUI()
	{
//		DrawDefaultInspector();

//		if(GUILayout.Button("")) {
//
//		}
				

        if (GUILayout.Button("Initialize"))
        {
            bicubicPatch.Initialize();
        }


		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		
		int oldIntValue = bicubicPatch.NumberOfSegmentsU;
		int newIntValue = EditorGUILayout.IntSlider("U", oldIntValue,1, 100);
		if(oldIntValue != newIntValue)
			bicubicPatch.NumberOfSegmentsU = newIntValue;

        oldIntValue = bicubicPatch.NumberOfSegmentsW;
        newIntValue = EditorGUILayout.IntSlider("W", oldIntValue, 1, 100);
        if (oldIntValue != newIntValue)
            bicubicPatch.NumberOfSegmentsW = newIntValue;
	
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		
		bicubicPatch.drawGizmos = EditorGUILayout.Toggle("Draw Gizmo", bicubicPatch.drawGizmos);
		bicubicPatch.drawMeshGizmo = EditorGUILayout.Toggle("Draw Mesh Gizmo", bicubicPatch.drawMeshGizmo);
		
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		
		bool oldBoolValue = bicubicPatch.generateMesh;
		bool newBoolValue = EditorGUILayout.Toggle("Generate Mesh", bicubicPatch.generateMesh);
		if(newBoolValue != oldBoolValue){
			bicubicPatch.generateMesh = newBoolValue;
			bicubicPatch.GenerateMesh();
		}


        Material oldMaterial = bicubicPatch.material;
        Material newMaterial = EditorGUILayout.ObjectField("Material", oldMaterial, typeof(Material), true) as Material;
        if (oldMaterial != newMaterial)
        {
            bicubicPatch.material = newMaterial;
            bicubicPatch.GenerateMesh();
        }

        Color oldColor = bicubicPatch.color;
        Color newColor = EditorGUILayout.ColorField("Color", oldColor);
        if (newColor != oldColor)
        {
            bicubicPatch.color = newColor;
            bicubicPatch.GenerateMesh();
        }

        float oldFloatValue = bicubicPatch.alpha;
        float newFloatValue = EditorGUILayout.Slider("Alpha", oldFloatValue, 0f, 1f);
        if (newFloatValue != oldFloatValue)
        {
            bicubicPatch.alpha = newFloatValue;
            bicubicPatch.GenerateMesh();
        }

		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		
		if (GUI.changed)
            EditorUtility.SetDirty(bicubicPatch);
	}
	
	
	
	
}

