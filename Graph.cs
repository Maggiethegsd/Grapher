using UnityEngine;
using MaggieTools.Grapher;
using UnityEditor;

[RequireComponent(typeof(Grids))]
public class Graph : MonoBehaviour
{
	[SerializeField]
	private string xData, yData;

	public Vector2 [] data;

	public static Graph graph;

	void Start()
	{
		if (graph == null)
			graph = this;
	}

	public void CreateGraph()
	{
		Grids.Plotter.PlotData(data);
	}
}

[CustomEditor(typeof(Graph))]
public class GraphEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		GUILayout.Space(10f);

		if (GUILayout.Button("Create Graph"))
		{
			Graph.graph.CreateGraph();
		}

	}
}
