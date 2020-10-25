using System;
using System.Collections.Generic;

[Serializable]
public class GeneSurroundingSensorData {
	public SurroundingSensorChannelSensorTypeEnum[] sensorTypeAtChannel = new SurroundingSensorChannelSensorTypeEnum[6]; // 0 is A

	public GeneSurroundingSensorChannelCreatureCellFovCovData[] creatureCellFovCovDataAtChannel = new GeneSurroundingSensorChannelCreatureCellFovCovData[6];
	public GeneSurroundingSensorChannelTerrainRockFovCovData[] terrainRockFovCovDataAtChannel = new GeneSurroundingSensorChannelTerrainRockFovCovData[6];

	public float directionLocal;
	public float fieldOfView;
	public float rangeFar;
	public float rangeNear;

}