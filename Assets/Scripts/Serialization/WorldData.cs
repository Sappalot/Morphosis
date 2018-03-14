using System;

[Serializable]
public class WorldData {
	public string worldName;
	public float fixedTime;
	public ulong worldTicks;
	public int runnersKilledCount;

	public LifeData lifeData;
	
	//terrain data...
}