using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BilinearSurface))]
public class BilinearSurfaceEditor : Editor
{

    private BilinearSurface bilinearSurface;

	public void Awake()
	{
        bilinearSurface = target as BilinearSurface;
	}


	public void OnSceneGUI()
	{
		Event currentEvent = Event.current;

		Handles.color = Color.yellow;
		Vector3 oldPosition = this.bilinearSurface.P00;
		Vector3 newPosition = Handles.FreeMoveHandle(this.bilinearSurface.P00, Quaternion.identity, 0.2f, Vector3.zero, Handles.SphereCap);
		if(newPosition != oldPosition)
			this.bilinearSurface.P00 = newPosition;

		oldPosition = this.bilinearSurface.P01;
		newPosition = Handles.FreeMoveHandle(this.bilinearSurface.P01, Quaternion.identity, 0.2f, Vector3.zero, Handles.SphereCap);
		if(newPosition != oldPosition)
			this.bilinearSurface.P01 = newPosition;
		
		
		oldPosition = this.bilinearSurface.P10;
		newPosition = Handles.FreeMoveHandle(this.bilinearSurface.P10, Quaternion.identity, 0.2f, Vector3.zero, Handles.SphereCap);
		if(newPosition != oldPosition)
			this.bilinearSurface.P10 = newPosition;
		
		
		oldPosition = this.bilinearSurface.P11;
		newPosition = Handles.FreeMoveHandle(this.bilinearSurface.P11, Quaternion.identity, 0.2f, Vector3.zero, Handles.SphereCap);
		if(newPosition != oldPosition)
			this.bilinearSurface.P11 = newPosition;
       


        EditorUtility.SetDirty(bilinearSurface);
	}


    public bool square = true;

	public override void OnInspectorGUI()
	{
//		DrawDefaultInspector();

//		if(GUILayout.Button("")) {
//
//		}
				
		
		Vector3 oldValue;
		Vector3 newValue;
		
		oldValue = bilinearSurface.P00;
		newValue = EditorGUILayout.Vector3Field("P00", bilinearSurface.P00);
		if(oldValue != newValue)
			bilinearSurface.P00 = newValue;
		
		oldValue = bilinearSurface.P01;
		newValue = EditorGUILayout.Vector3Field("P01", bilinearSurface.P01);
		if(oldValue != newValue)
			bilinearSurface.P01 = newValue;
		
		oldValue = bilinearSurface.P10;
		newValue = EditorGUILayout.Vector3Field("P10", bilinearSurface.P10);
		if(oldValue != newValue)
			bilinearSurface.P10 = newValue;
		
		oldValue = bilinearSurface.P11;
		newValue = EditorGUILayout.Vector3Field("P11", bilinearSurface.P11);
		if(oldValue != newValue)
			bilinearSurface.P11 = newValue;
		
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		
		int oldIntValue = bilinearSurface.NumberOfSegmentsU;
		int newIntValue = EditorGUILayout.IntSlider("U", oldIntValue,1, 100);
        if (oldIntValue != newIntValue)
        {
            bilinearSurface.NumberOfSegmentsU = newIntValue;
            if(square)
                bilinearSurface.NumberOfSegmentsW = newIntValue;
        }

        oldIntValue = bilinearSurface.NumberOfSegmentsW;
        newIntValue = EditorGUILayout.IntSlider("W", oldIntValue, 1, 100);
        if (oldIntValue != newIntValue)
        {
            bilinearSurface.NumberOfSegmentsW = newIntValue;
            if (square)
                bilinearSurface.NumberOfSegmentsU = newIntValue;
        }
         
        bool oldSquareValue = square; 
        bool newSquareValue = EditorGUILayout.Toggle("U = W", square);
        if (oldSquareValue != newSquareValue)
        {
            square = newSquareValue;
            bilinearSurface.NumberOfSegmentsU = bilinearSurface.NumberOfSegmentsW;
            bilinearSurface.GenerateMesh();
        }

	
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		
		bilinearSurface.drawGizmos = EditorGUILayout.Toggle("Draw Gizmo", bilinearSurface.drawGizmos);
		bilinearSurface.drawMeshGizmo = EditorGUILayout.Toggle("Draw Mesh Gizmo", bilinearSurface.drawMeshGizmo);
		
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		
		bool oldBoolValue = bilinearSurface.generateMesh;
		bool newBoolValue = EditorGUILayout.Toggle("Generate Mesh", bilinearSurface.generateMesh);
		if(newBoolValue != oldBoolValue){
			bilinearSurface.generateMesh = newBoolValue;
			bilinearSurface.GenerateMesh();
		}

        Material oldMaterial = bilinearSurface.material;
        Material newMaterial = EditorGUILayout.ObjectField("Material", oldMaterial, typeof(Material), true) as Material;
        if (oldMaterial != newMaterial)
        {
            bilinearSurface.material = newMaterial;
            bilinearSurface.GenerateMesh();
        }

        Color oldColor = bilinearSurface.color;
        Color newColor = EditorGUILayout.ColorField("Color", oldColor);
        if (newColor != oldColor)
        {
            bilinearSurface.color = newColor;
            bilinearSurface.GenerateMesh();
        }

        float oldFloatValue = bilinearSurface.alpha;
        float newFloatValue = EditorGUILayout.Slider("Alpha", oldFloatValue, 0f, 1f);
        if (newFloatValue != oldFloatValue)
        {
            bilinearSurface.alpha = newFloatValue;
            bilinearSurface.GenerateMesh();
        }

		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		
		if (GUI.changed)
            EditorUtility.SetDirty(bilinearSurface);
	}
	
	
	
	
}

