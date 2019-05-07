using System;

[Serializable]
public class WorldData {
	public string worldName;
	public int metaCreatureCount; // meta data to be able to setup progress bar without need to read through LifeData
	public float fixedTime;
	public ulong worldTicks;
	public int runnersKilledCount;

	public LifeData lifeData;
	public HistoryData historyData; //we should be able to load files without data
	public TerrainData terrainData;
}