using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GeographicCoordinates))]
public class GeographicCoordinatesEditor : Editor
{

    private GeographicCoordinates geoCoords;

	public void Awake()
	{
        geoCoords = target as GeographicCoordinates;
	}


	public void OnSceneGUI()
	{

        //this.geoCoords.Coords3D = this.geoCoords.transform.position;

        Vector3 oldPosition = this.geoCoords.transform.position;

        Vector3 newPosition = Handles.FreeMoveHandle(this.geoCoords.transform.position, Quaternion.identity, 1f, Vector3.zero, Handles.SphereCap);
        if (newPosition != oldPosition)
        {
            //Debug.Log("Changed");
            this.geoCoords.transform.position = newPosition;
            this.geoCoords.Coords3D = this.geoCoords.transform.position;
        }

        //if (newPosition != oldPosition)
        //{
        //    Debug.Log(oldPosition + " - " + newPosition + "  = " + (oldPosition != newPosition));
        //    //this.geoCoords.transform.position = newPosition;
        //    //this.geoCoords.Coords3D = this.geoCoords.transform.position;
        //    //Debug.Log(geoCoords.Longitude + " " + geoCoords.Latitude + "  ,  " + geoCoords.transform.position);
        //}
        ////Debug.Log(geoCoords.Longitude + " " + geoCoords.Latitude + "  ,  " + geoCoords.transform.position);


        if (GUI.changed)
            EditorUtility.SetDirty(geoCoords);
	}
    

       



	public override void OnInspectorGUI()
	{
        DrawDefaultInspector();

        if (GUILayout.Button("FromLon/Lat"))
        {
            geoCoords.transform.position = geoCoords.Coords3D;
        }

        if (GUILayout.Button("From Transform"))
        {
            geoCoords.Coords3D = geoCoords.transform.position;
        }

         
        //if (GUILayout.Button("MapCenter"))
        //{
        //    GeographicCoordinates.MapCenter = geoCoords.Longitude;
        //}

        if (GUILayout.Button("Open Google StreetView"))
        {
            geoCoords.OpenStreetView();
        }

        //geoCoords.showCoords = GUILayout.Toggle(geoCoords.showCoords, "Show Coords");

		if (GUI.changed)
            EditorUtility.SetDirty(geoCoords);
	}

}

