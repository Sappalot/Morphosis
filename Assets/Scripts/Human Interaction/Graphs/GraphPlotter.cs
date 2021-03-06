﻿using UnityEngine;

public class GraphPlotter : MonoSingleton<GraphPlotter> {
	public TimeRuler timeRuler;
	public Flags flags;
	public GraphMeasuringTool measuringTool;

	public float zoomStepSpeed = 0.1f;

	public ResizeViewport viewport;
	public LineRenderer frameLine;

	public GraphGroup[] graphGroups;

	public float topMargin;    //set from inspector
	public float bottomMargin; //set from inspector
	public float rightMargin;  //set from inspector

	[HideInInspector]
	public History history;

	private Vector2i res;
	private Rect graphArea = new Rect();

	private float scale = 10f; //pixels / second

	public void ZoomStepIn() {
		scale *= 1 + zoomStepSpeed;
		isDirty = true;
		Update();
	}

	public void ZoomStepOut() {
		scale *= 1 / (1 + zoomStepSpeed);
		isDirty = true;
		Update();
	}

	public override void Init() {
		gameObject.SetActive(false);
	}

	void Start() {
		res = new Vector2i();
	}

	private bool isDirty;

	public void MakeDirty() {  
		isDirty = true; 
	}

	public bool IsMouseInside() {
		return gameObject.activeSelf && Input.mousePosition.y > viewport.windowSize.rect.height - (viewport.height + viewport.topMargin) && Input.mousePosition.y < viewport.windowSize.rect.height - viewport.topMargin;
	}

	private bool isMeasuringToolUsed;

	private void Update() {
		if (res.x != (int)viewport.graphPlotterArea.width || res.y != (int)viewport.graphPlotterArea.height) {
			graphArea.xMin = viewport.graphPlotterArea.center.x - viewport.graphPlotterArea.width  / 2f;
			graphArea.xMax = viewport.graphPlotterArea.center.x + viewport.graphPlotterArea.width  / 2f - rightMargin;
			graphArea.yMin = viewport.graphPlotterArea.center.y - viewport.graphPlotterArea.height / 2f + bottomMargin;
			graphArea.yMax = viewport.graphPlotterArea.center.y + viewport.graphPlotterArea.height / 2f - topMargin;
			UpdateGraphics();

			// graphs
			foreach (GraphGroup g in graphGroups) {
				g.UpdateCanvases(graphArea);
			}

			// ruler
			timeRuler.UpdateCanvas(graphArea);
			// flags
			flags.UpdateCanvas(graphArea);
			//measuring tool
			measuringTool.UpdateCanvas(graphArea);

			res = new Vector2i((int)viewport.graphPlotterArea.width, (int)viewport.graphPlotterArea.height);
		}

		
	
		ulong secondsAgo = 0; //far in the future (that is rendered right of view)
		int measureStepsAgo = 0;

		if (Input.GetMouseButton(0) && IsMouseInside() && Input.mousePosition.x < graphArea.width) {
			isMeasuringToolUsed = true;
			short level = GetLevel(scale);

			float pixelsLeftOfNow = (graphArea.width - Input.mousePosition.x);
			secondsAgo = (ulong)(pixelsLeftOfNow / scale); // scale = pixels / sec
			ulong secondsAgoClick = (secondsAgo / (ulong)Mathf.Pow(2f, level)) * (ulong)Mathf.Pow(2f, level);




			float levelScale = scale * Mathf.Pow(2f, level);
			measureStepsAgo = (int)(pixelsLeftOfNow / levelScale);

			measuringTool.gameObject.SetActive(true);
			measuringTool.UpdateGraphics(graphArea, scale, secondsAgoClick);
		} else if (isMeasuringToolUsed) {
			isDirty = true;
			isMeasuringToolUsed = false;
			measuringTool.gameObject.SetActive(false);
		}

		if ((isDirty || isMeasuringToolUsed) && history != null) {
			short level = GetLevel(scale);
			foreach (GraphGroup g in graphGroups) {
				g.UpdateIsActive();
				g.DrawGraphs(graphArea, scale, level, history, isMeasuringToolUsed ? measureStepsAgo : 0);
			}

			timeRuler.UpdateGraphics(graphArea, scale);
			flags.UpdateGraphics(graphArea, scale, level, history);

			isDirty = false;
		}
	}

	private void UpdateGraphics() {
		frameLine.SetPosition(0,    new Vector3(graphArea.xMin, graphArea.yMax, -1));
		frameLine.SetPosition(1,    new Vector3(graphArea.xMax, graphArea.yMax, -1));
		frameLine.SetPosition(2, new Vector3(graphArea.xMax, graphArea.yMin, -1));
		frameLine.SetPosition(3, new Vector3(graphArea.xMin, graphArea.yMin, -1));
	}

	private short GetLevel(float scale) {
		       if (scale < 0.0048828125f) {
			return 11;
		} else if (scale < 0.009765625f) {
			return 10;
		} else if (scale < 0.01953125f) {
			return 9;
		} else if (scale < 0.0390625f) {
			return 8;
		} else if (scale < 0.078125f) {
			return 7;
		} else if (scale < 0.15625f) {
			return 6;
		} else if (scale < 0.3125f) {
			return 5;
		} else if (scale < 0.625f) {
			return 4;
		} else if (scale < 1.25f) {
			return 3;
		} else if (scale < 2.5f) {
			return 2;
		} else if (scale < 5f) {
			return 1;
		}
		return 0;
	}
}
