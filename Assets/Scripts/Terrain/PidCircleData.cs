using System;

[Serializable]
public class PidCircleData {
	public bool isOn;
	public float radius;
	public short pidTicks;
	public float fpsGoal;
	public float fpsError;
	public float fpsErrorI;
	public float fpsErrorD;
}
