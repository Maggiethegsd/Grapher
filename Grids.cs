using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace MaggieTools
{
	namespace Grapher
	{
		public static class Functions
		{
			public delegate float func(float x);

			public static func sin = Mathf.Sin;
			public static func cos = Mathf.Cos;
			public static func tan = Mathf.Tan;
			public static func cosec = Cosec;
			public static func sec = Sec;
			public static func cot = Cot;

			public static func abs = Mathf.Abs;
			public static func sqrt = Mathf.Sqrt;

			public static func floor = Mathf.Floor;
			public static func ceil = Mathf.Ceil;
			public static func signum = Mathf.Sign;

			private static float Cosec(float x) => 1 / Mathf.Sin(x);
			private static float Sec(float x) => 1 / Mathf.Cos(x);
			private static float Cot(float x) => 1 / Mathf.Tan(x);
		}

		public class Grids : MonoBehaviour
		{
			public static class Plotter
			{
				/* DEPRECATED
				public static void PlotFunctionGraph(Functions.func f, float resolution, float length)
				{
					float x = f == Functions.sqrt ? 0 : -length;

					while (x < length)
					{
						x += resolution;

						PlotPoint(x, f(x));
					}
				}
				*/

				public static void PlotFunctionGraph(Functions.func f)
				{
					float x = f == Functions.sqrt ? 0 : grid.lx;

					Point _p = PlotPoint(x, f(x));

					while (x < grid.ux)
					{
						x += 1 / grid.graphResolution;

						PlotPoint(x, f(x));
					}
				}

				public static Point [] PlotData(Vector2[] data)
				{
					int i = 0;

					Point[] pointsPlotted = new Point[data.Length]; 

					while (i < data.Length)
					{
						Point p = PlotPoint(data[i].x, data[i].y);
						pointsPlotted[i] = p;

						i++;
					}

					return pointsPlotted;
				}

				public static Point PlotPoint(float x, float y)
				{
					GameObject p = Instantiate(grid.point, new Vector2(x, y), Quaternion.identity);


					#region Color and Scale
					int fullLength = grid.gridSize;
					float pathCoveredX = Mathf.Abs(x - grid.lx) / fullLength;
					float pathCoveredY = Mathf.Abs(y - grid.ly) / fullLength;

					Color colorX = grid.pointColor.Evaluate(pathCoveredX);
					Color colorY = grid.pointColor.Evaluate(pathCoveredY);

					Color finalCol = Color.Lerp(colorX, colorY, .5f);

					p.GetComponent<SpriteRenderer>().color = finalCol;
					p.transform.localScale = Vector3.one * grid.pointSize;

					Point _p = p.GetComponent<Point>();
					_p.x = x;
					_p.y = y;

					return _p;
					#endregion
				}

				public static Point PlotPoint(float x, float y, float colorRangeX, float colorRangeY)
				{
					GameObject p = Instantiate(grid.point, new Vector2(x, y), Quaternion.identity);


					#region Color and Scale
					Color colorX = grid.pointColor.Evaluate(colorRangeX);
					Color colorY = grid.pointColor.Evaluate(colorRangeY);

					Color finalCol = Color.Lerp(colorX, colorY, .5f);

					p.GetComponent<SpriteRenderer>().color = finalCol;
					p.transform.localScale = Vector3.one * grid.pointSize;

					Point _p = p.GetComponent<Point>();
					_p.x = x;
					_p.y = y;

					return _p;
					#endregion
				}
			}

			[Header("Grid Preferences")]
			public bool drawGrid;

			[Space(10f)]

			[Range(1f, 100f)]
			public int gridSize;
			[Range(.01f, .3f)]
			public float axesLinesThickness;
			[SerializeField]
			private Color xAxisColor, yAxisColor;
			[SerializeField]
			private Vector2 axesScale;
			[SerializeField]
			private bool showOnlyAxes;

			[Header("Plotting Preferences")]
			[SerializeField]
			private GameObject point;
			[SerializeField, Range(.01f, .5f)]
			private float pointSize;
			[SerializeField]
			private Gradient pointColor;
			[Range(.01f, 100f)]
			public float graphResolution;

			[Header("Setups")]
			[SerializeField]
			private TextMeshProUGUI textCopy;

			[SerializeField]
			private RectTransform rectTransform;
			[SerializeField]
			private Sprite lineSprite;
			private GameObject[] points { get { return GameObject.FindGameObjectsWithTag("Point"); } }

			public static Grids grid;

			//true lower and upper coordinates
			[HideInInspector]
			public int lx { get { return -Mathf.FloorToInt(gridSize / 2f); } }
			[HideInInspector]
			public int ux { get { return Mathf.CeilToInt(gridSize / 2f); } }
			[HideInInspector]
			public int ly { get { return -Mathf.FloorToInt(gridSize / 2f); } }
			[HideInInspector]
			public int uy { get { return Mathf.CeilToInt(gridSize / 2f); } }


			void Start()
			{
				grid = this;

				UpdateSingleton();

				if (drawGrid)
					CreateAxis();
			}

			void OnValidate()
			{
				UpdatePoints();
			}

			/// <summary>
			/// Manage Singleton entity status in the game.
			/// </summary>
			private void UpdateSingleton()
			{
				if (grid != null)
				{
					Grids[] grids = FindObjectsByType<Grids>(FindObjectsSortMode.None);

					foreach (Grids g in grids)
					{
						if (g == this)
							continue;

						Destroy(g);
					}
				}

				grid = this;
			}

			///<summary>
			/// Update all points attributes
			///</summary>
			void UpdatePoints()
			{
				foreach (GameObject p in points)
				{
					UpdatePoint(p.GetComponent<Point>());
				}
			}

			///<summary>
			/// Update singular point attributes
			/// </summary>
			void UpdatePoint(Point p)
			{
				p.GetComponent<SpriteRenderer>().color = grid.pointColor.Evaluate(p.pathCovered);
				p.transform.localScale = Vector3.one * grid.pointSize;
			}

			/// <summary>
			/// Clear the entire plane of any points
			/// </summary>
			public void ClearGrid()
			{
				foreach (GameObject point in points)
					Destroy(point);
			}

			/// <summary>
			/// Create the x and y axes and grid
			/// </summary>
			public void CreateAxis()
			{
				//y axis
				for (int i = lx; i <= ux; i++)
				{
					Vector2 linePos = new Vector2(i, 0);
					float length = gridSize;

					if (showOnlyAxes && linePos == Vector2.zero)
						CreateLine(linePos, length, axesLinesThickness, yAxisColor, 90f);

					else if (!showOnlyAxes)
						CreateLine(linePos, length, axesLinesThickness, yAxisColor, 90f);

					int[] illegal = new int[] { lx, 0, ux };
					if (illegal.Contains(i))
						continue;

					Vector2 textPos = new Vector2(i + .2f, -.2f);
					CreateText((i*axesScale.y).ToString(), textPos, 0f);
				}

				//x axis
				for (int j = ly; j <= uy; j++)
				{
					Vector2 linePos = new Vector2(0, j);
					float length = gridSize;
					
					if (showOnlyAxes && linePos == Vector2.zero)
						CreateLine(linePos, length, axesLinesThickness, xAxisColor, 0f);

					else if (!showOnlyAxes)
						CreateLine(linePos, length, axesLinesThickness, xAxisColor, 0f);

					int[] illegal = new int[] { ly, 0, uy };
					if (illegal.Contains(j))
						continue;

					Vector2 textPos = new Vector2(-.2f, j + .2f);
					CreateText((j*axesScale.x).ToString(), textPos, 0f);
				}
			}

			/* -------- DEPRECATED - UNDER CONSTRUCTION -------- 
			private void ConnectPoints(Point A, Point B)
			{
				float x = Mathf.Abs(A.x - B.x);
				float y = Mathf.Abs(A.y - B.y);
				float mag = Mathf.Sqrt(Mathf.Pow(A.x - B.x, 2) + Mathf.Pow(A.y - B.y, 2));

				float tantheta = y / x;
				float deg = Mathf.Atan(tantheta) * Mathf.Rad2Deg;

				CreateLine(new Vector2(A.x, A.y), mag, .1f, Color.green, deg);
			}
			*/

			public Line CreateLine(Vector2 pos, float length, float thickness, Color color, float deg)
			{
				//get slope for given angle

				Vector3 scale = new Vector3(length, thickness, 0f);
				Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, deg));

				//draw a line of given thickness and length
				Transform line = new GameObject("_line").transform;
				SpriteRenderer sr = line.AddComponent<SpriteRenderer>();
				sr.sprite = lineSprite;
				sr.color = color;

				line.position = pos;
				line.localScale = scale;
				line.rotation = rotation;
				line.parent = transform;

				return new Line(deg, length, thickness, color);
			}

			public GameObject CreateText(string text, Vector2 position, float rot)
			{
				GameObject go = Instantiate(textCopy.gameObject, position, Quaternion.Euler(new Vector3(0f, 0f, rot)), rectTransform);
				go.name = text;

				TextMeshProUGUI t = go.GetComponent<TextMeshProUGUI>();

				t.text = text;

				return go;
			}
		}
	}

	public class Line
	{
		public float theta { get; private set; }
		public float length { get; private set; }
		public float thickness { get; private set; }
		public Color color { get; private set; }

		public Line(float _theta, float _length, float _thickness, Color _color)
		{
			theta = _theta;
			length = _length;
			thickness = _thickness;
			color = _color;
		}
	}
}
