using System;

[Serializable]
public class RecordStripData {
	public RecordData[] records;
	public int cursor;
	public int lowpassCounter;
}