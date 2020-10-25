using System;
using System.Collections.Generic;

[Serializable]
public class GeneSurroundingSensorData {
	public SurroundingSensorChannelSensorTypeEnum[] sensorTypeAtChannel = new SurroundingSensorChannelSensorTypeEnum[7]; // 0 is ditched, 1 = is channel at output A

	public GeneSurroundingSensorChannelCreatureCellFovCovData[] creatureCellFovCovDataAtChannel = new GeneSurroundingSensorChannelCreatureCellFovCovData[7];
	public GeneSurroundingSensorChannelTerrainRockFovCovData[] terrainRockFovCovDataAtChannel = new GeneSurroundingSensorChannelTerrainRockFovCovData[7];

	public float direction;
	public float fieldOfView;
	public float rangeFar;
	public float rangeNear;

}