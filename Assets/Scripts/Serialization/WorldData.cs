using System;

[Serializable]
public class WorldData {
	public string worldName;
	public float fixedTime;
	public ulong worldTicks;
	public int runnersKilledCount;

	public LifeData lifeData;
	public HistoryData historyData; //we should be able to load files without data
	//terrain data...
}