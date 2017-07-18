using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ParametricCurve))]
public class ParametricCurveEditor : Editor
{

    private ParametricCurve parametricCurve;
    
    public void Awake()
    {
        parametricCurve = (ParametricCurve) target as ParametricCurve;
    }


    public void OnSceneGUI()
    {
        Handles.color = Color.green;

        bool changed = false;
        Vector3 old = this.parametricCurve.A;
        this.parametricCurve.A = Handles.FreeMoveHandle(this.parametricCurve.A, Quaternion.identity, 1f, Vector3.zero, Handles.SphereCap);
        if (old != this.parametricCurve.A)
            changed = true;

        old = this.parametricCurve.B;
        this.parametricCurve.B = Handles.FreeMoveHandle(this.parametricCurve.B, Quaternion.identity, 1f, Vector3.zero, Handles.SphereCap);
        if (old != this.parametricCurve.B)
            changed = true;

        old = this.parametricCurve.C;
        this.parametricCurve.C = Handles.FreeMoveHandle(this.parametricCurve.C, Quaternion.identity, 1f, Vector3.zero, Handles.SphereCap);
        if (old != this.parametricCurve.C)
            changed = true;

        old = this.parametricCurve.D;
        this.parametricCurve.D = Handles.FreeMoveHandle(this.parametricCurve.D, Quaternion.identity, 1f, Vector3.zero, Handles.SphereCap);
        if (old != this.parametricCurve.D)
            changed = true;


        if(changed)
            parametricCurve.CalculatePointsSlow();


    }


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

		if(GUILayout.Button("Calculate Points")){
			parametricCurve.CalculatePoints();
		}

    }
}

