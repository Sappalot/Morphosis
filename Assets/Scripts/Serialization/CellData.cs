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
	public int buildIndex;
	public FlipSideEnum flipSide;
	public float radius = 0.5f;
	public Vector2 velocity;
	public float lastTime; //Last time muscle cell was updated
	public float energy;

	// Egg
	public ChildDetatchModeEnum eggCellDetatchMode;
	public float eggCellDetatchSizeThreshold;
	public float eggCellDetatchEnergyThreshold;

	// Leaf
	public float leafCellLowPassExposure;

	// Origin
	public ChildDetatchModeEnum originDetatchMode;
	public float originDetatchSizeThreshold;
	public float originDetatchEnergyThreshold;
	public int originPulseTick;
}