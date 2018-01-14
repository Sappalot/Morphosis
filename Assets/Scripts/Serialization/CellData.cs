using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CellData {
	public Vector3 position = Vector3.zero;
	public float heading; //Flip triangle is pointing in this direction at the moment
	public int bindCardinalIndex;
	public int geneIndex = 0;
	public Vector2i mapPosition;
	public int buildOrderIndex;
	public FlipSideEnum flipSide;
	public float radius = 0.5f;
	public Vector2 velocity;
	public float timeOffset; //To be removed
	public float lastTime; //Last time muscle cell was updated

	// Egg
	public float eggCellFertilizeThreshold;
	public ChildDetatchModeEnum eggCellDetatchMode;
	public float eggCellDetatchSizeThreshold;
	public float eggCellDetatchEnergyThreshold;

	// Origin
	public ChildDetatchModeEnum originDetatchMode;
	public float originDetatchSizeThreshold;
	public float originDetatchEnergyThreshold;

	public float energy;
}