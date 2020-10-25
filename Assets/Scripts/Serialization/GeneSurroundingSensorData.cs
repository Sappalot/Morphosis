using System;

[Serializable]
public class GeneSurroundingSensorData {
	public SurroundingSensorChannelSensorTypeEnum[] sensorAtChannel = new SurroundingSensorChannelSensorTypeEnum[7]; // 0 is ditched, 1 = is channel at output A

	public float direction;
	public float fieldOfView;
	public float rangeFar;
	public float rangeNear;
}