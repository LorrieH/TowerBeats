using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexGrid))]
public class HexGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Still draw default inspector
        DrawDefaultInspector();

        // Get a reference to the HexGrid
        HexGrid hexGrid = (HexGrid)target;

        // Add spacing
        for (int i = 0; i < 4; i++)
            EditorGUILayout.Space();

        // Add a Button to create the Grid
        if (GUILayout.Button("Create Grid"))
            hexGrid.CreateGrid();

        // Add a Button to remove the Grid
        if (GUILayout.Button("Remove Grid"))
            hexGrid.DestroyGrid(true);
    }
}
