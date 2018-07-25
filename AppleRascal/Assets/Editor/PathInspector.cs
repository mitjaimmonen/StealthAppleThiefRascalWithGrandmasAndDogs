using WaypointSystem;
using UnityEngine;
using UE = UnityEditor;


[UE.CustomEditor(typeof(Path))]
public class PathInspector : UE.Editor
{
    private Path _target;

    protected void OnEnable()
    {
        _target = target as Path;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Add Waypoint"))
        {
            int waypointCount = _target.transform.childCount;
            string waypointName = string.Format("Waypoint{0}", (waypointCount + 1).ToString("D3"));
            GameObject waypoint = new GameObject(waypointName);
            waypoint.AddComponent<Waypoint>();
            waypoint.transform.SetParent(_target.transform);
        }
    }

}
