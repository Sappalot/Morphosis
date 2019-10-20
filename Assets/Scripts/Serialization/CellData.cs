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
	public LogicBoxData eggCellFertilizeLogicBoxData;
	public EnergySensorData eggCellFertilizeEnergySensorData;

	// Leaf
	public float leafCellLowPassExposure;

	// Constant
	public ConstantSensorData constantSensorData;

	// Dendrites
	public LogicBoxData dendritesLogicBoxData;

	// Sensors
	public EnergySensorData energySensorData;

	// Origin
	public int originPulseTick;
	public LogicBoxData originDetatchLogicBoxData;
	public SizeSensorData originSizeSensorData;
}