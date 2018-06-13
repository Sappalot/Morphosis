using System;
using System.Collections.Generic;

[Serializable]
public class RecordStripData {
	public RecordData[] records;
	public int cursor;
	public int lowpassCounter;
}