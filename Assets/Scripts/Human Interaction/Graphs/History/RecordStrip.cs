//Layers of record
using UnityEngine;

public class RecordStrip {
	public static int size = 400; // long enough to show one strip in graph plotter area, ~300 should be enough, so we are having a bit of a margin 

	private Record[] records = new Record[size];
	private int cursor = 0; // cursor is standing on last written record

	public int lowpassCounter = 0;

	private int GetWrappedCursor(int position) {
		if (position < 0) {
			return records.Length + position;
		}
		return position % size;
	}

	private RecordEnum[] types = new RecordEnum[] {
		RecordEnum.fps,
		RecordEnum.pps,
		RecordEnum.cellCountTotal,
		RecordEnum.cellCountEgg,
		RecordEnum.cellCountFungal,
		RecordEnum.cellCountJaw,
		RecordEnum.cellCountLeaf,
		RecordEnum.cellCountMuscle,
		RecordEnum.cellCountRoot,
		RecordEnum.cellCountShell,
		RecordEnum.cellCountShellWood,
		RecordEnum.cellCountShellMetal,
		RecordEnum.cellCountShellGlass,
		RecordEnum.cellCountShellDiamond,
		RecordEnum.cellCountVein,
		RecordEnum.creatureCount,
		RecordEnum.creatureBirthsPerSecond,
		RecordEnum.creatureDeathsPerSecond};

	public void Init() {
		Clear();
	}

	public void Clear() {
		for (int i = 0; i < records.Length; i++) {
			records[i] = new Record();
			foreach (RecordEnum t in types) {
				records[i].Set(t, 0f);
			}
			records[i].tagText = null;
		}

		cursor = 0;
		lowpassCounter = 0;
	}

	public Record AddRecord(Record other) {
		cursor++;
		if (cursor >= records.Length) {
			cursor = 0;
		}
		records[cursor].Clear(); // clear oldes record to use it again

		foreach (RecordEnum t in types) {
			records[cursor].Set(t, other.Get(t));
		}
		records[cursor].tagText =     other.tagText;
		records[cursor].tagShowLine = other.tagShowLine;
		records[cursor].tagRed =      other.tagRed;
		records[cursor].tagGreen =    other.tagGreen;
		records[cursor].tagBlue =     other.tagBlue;


		lowpassCounter++;

		if (lowpassCounter == 2) {
			Record lowpassRecord = new Record();

			Record todayRecord = records[cursor];
			Record yesterdayRecord = records[GetWrappedCursor(cursor - 1)];
			
			//merge values
			foreach (RecordEnum t in types) {
				lowpassRecord.Set(t, (todayRecord.Get(t) + yesterdayRecord.Get(t)) / 2f);
			}

			//merge tags
			if (todayRecord.HasTag() || yesterdayRecord.HasTag()) {
				if (todayRecord.HasTag() && yesterdayRecord.HasTag()) {
					lowpassRecord.tagText =  (yesterdayRecord.tagText + todayRecord.tagText); //merge;
					lowpassRecord.tagRed =   (yesterdayRecord.tagRed +   todayRecord.tagRed)   / 2f;
					lowpassRecord.tagGreen = (yesterdayRecord.tagGreen + todayRecord.tagGreen) / 2f;
					lowpassRecord.tagBlue =  (yesterdayRecord.tagBlue +  todayRecord.tagBlue)  / 2f;
				} else if (yesterdayRecord.HasTag()) {
					lowpassRecord.tagText =  yesterdayRecord.tagText;
					lowpassRecord.tagRed =   yesterdayRecord.tagRed;
					lowpassRecord.tagGreen = yesterdayRecord.tagGreen;
					lowpassRecord.tagBlue =  yesterdayRecord.tagBlue;
				} else {
					lowpassRecord.tagText =  todayRecord.tagText;
					lowpassRecord.tagRed =   todayRecord.tagRed;
					lowpassRecord.tagGreen = todayRecord.tagGreen;
					lowpassRecord.tagBlue =  todayRecord.tagBlue;
				}
			}

			lowpassRecord.tagShowLine = todayRecord.tagShowLine | yesterdayRecord.tagShowLine;

			lowpassCounter = 0;
			return lowpassRecord;
		}
		return null;
	}

	public Record GetRecord(int timeAgo) { // time in seconds, 0 mans last record
		int pic = cursor - timeAgo;
		if (pic >= 0) {
			return records[pic];
		} else {
			pic = records.Length + pic;
			if (pic > cursor) {
				return records[pic];
			} else {
				return records[(cursor + 1) % size];
			}
		}
	}

	// Load / Save

	private RecordStripData recordStripData = new RecordStripData();

	// Save
	public RecordStripData UpdateData() {
		recordStripData.records = new RecordData[size];
		for (int i = 0; i < size; i++) {
			recordStripData.records[i] = records[i].UpdateData();
		}

		recordStripData.cursor = cursor;
		recordStripData.lowpassCounter = lowpassCounter;

		return recordStripData;
	}

	// Load
	public void ApplyData(RecordStripData recordStripData) {
		if (recordStripData == null) { // to be able to load files without history data
			return;
		}
		for (int i = 0; i < size; i++) {
			records[i].ApplyData(recordStripData.records[i]);
		}
		cursor = recordStripData.cursor;
		lowpassCounter = recordStripData.lowpassCounter;
	}
}
