using MaggieTools.Grapher;
using UnityEngine;
using TMPro;

public class InputHandler : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField _input;

	public void CheckInput()
	{
		switch (_input.text.ToLower())
		{
			case "sinx":
				Grids.Plotter.PlotFunctionGraph(Functions.sin);
				break;

			case "cosx":
				Grids.Plotter.PlotFunctionGraph(Functions.cos);
				break;

			case "tanx":
				Grids.Plotter.PlotFunctionGraph(Functions.tan);
				break;

			case "secx":
				Grids.Plotter.PlotFunctionGraph(Functions.sec);
				break;

			case "cosecx":
				Grids.Plotter.PlotFunctionGraph(Functions.cosec);
				break;

			case "cotx":
				Grids.Plotter.PlotFunctionGraph(Functions.cot);
				break;	
			
			case "absx":
				Grids.Plotter.PlotFunctionGraph(Functions.abs);
				break;

			case "sqrtx":
				Grids.Plotter.PlotFunctionGraph(Functions.sqrt);
				break;

			case "floorx":
				Grids.Plotter.PlotFunctionGraph(Functions.floor);
				break;

			case "ceilx":
				Grids.Plotter.PlotFunctionGraph(Functions.ceil);
				break;

			case "signumx":
				Grids.Plotter.PlotFunctionGraph(Functions.signum);
				break;

			default:
				Grids.grid.ClearGrid();
				break;
		}
	}
}
