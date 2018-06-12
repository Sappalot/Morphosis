//Layers of record
using UnityEngine;

public class RecordStrip {
	const int size = 400; // long enough to show one strip in graph plotter area, ~300 should be enough, so we are having a bit of a margin 

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
		RecordEnum.cellCountTotal,
		RecordEnum.cellCountJaw,
		RecordEnum.cellCountLeaf };

	public void Init() {
		Clear();
	}

	public void Clear() {
		for (int i = 0; i < records.Length; i++) {
			records[i] = new Record();
			foreach (RecordEnum t in types) {
				records[i].Add(t, 0f);
			}
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
			if (other.Contains(t)) {
				records[cursor].Add(t, other.Get(t));
			}
		}

		lowpassCounter++;

		if (lowpassCounter == 2) {
			Record lowpassRecord = new Record();

			foreach (RecordEnum t in types) {
				Record todayRecord =     records[cursor];
				Record yesterdayRecord = records[GetWrappedCursor(cursor - 1)];

				if (todayRecord.Contains(t) && yesterdayRecord.Contains(t)) {
					lowpassRecord.Add(t, (todayRecord.Get(t) + yesterdayRecord.Get(t)) / 2f);
				}
			}
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
}
