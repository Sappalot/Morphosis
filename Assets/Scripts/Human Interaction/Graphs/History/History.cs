//Layers of record
using UnityEngine;

public class History {

	//Same size, double the length & half the res for each level
	private RecordStrip level0 = new RecordStrip();
	private RecordStrip level1 = new RecordStrip();
	private RecordStrip level2 = new RecordStrip();
	private RecordStrip level3 = new RecordStrip();
	private RecordStrip level4 = new RecordStrip();
	private RecordStrip level5 = new RecordStrip();
	private RecordStrip level6 = new RecordStrip();
	private RecordStrip level7 = new RecordStrip();
	private RecordStrip level8 = new RecordStrip();
	private RecordStrip level9 = new RecordStrip();
	private RecordStrip level10 = new RecordStrip();
	private RecordStrip level11 = new RecordStrip();
	private RecordStrip level12 = new RecordStrip();
	private RecordStrip level13 = new RecordStrip();
	private RecordStrip level14 = new RecordStrip(); //lowest quality. every record in strip holds 4.55h worth of time,  should last for 75.85 days (if record strip is 400 long)

	private RecordStrip[] strips;

	public void Init() {
		strips = new RecordStrip[] {
			level0,
			level1,
			level2,
			level3,
			level4,
			level5,
			level6,
			level7,
			level8,
			level9,
			level10,
			level11,
			level12,
			level13,
			level14 };

		for (int l = 0; l < strips.Length; l++) {
			strips[l].Init();
		}
	}

	public void Clear() {
		for (int l = 0; l < strips.Length; l++) {
			strips[l].Clear();
		}
	}

	public void AddRecord(Record other) {
		Record lowpass0 = level0.AddRecord(other); // allways added to level 0
		if (lowpass0 != null) {
			Record lowpass1 = level1.AddRecord(lowpass0);
			if (lowpass1 != null) {
				Record lowpass2 = level2.AddRecord(lowpass1);
				if (lowpass2 != null) {
					Record lowpass3 = level3.AddRecord(lowpass2);
					if (lowpass3 != null) {
						Record lowpass4 = level4.AddRecord(lowpass3);
						if (lowpass4 != null) {
							Record lowpass5 = level5.AddRecord(lowpass4);
							if (lowpass5 != null) {
								Record lowpass6 = level6.AddRecord(lowpass5);
								if (lowpass6 != null) {
									Record lowpass7 = level7.AddRecord(lowpass6);
									if (lowpass7 != null) {
										Record lowpass8 = level8.AddRecord(lowpass7);
										if (lowpass8 != null) {
											Record lowpass9 = level9.AddRecord(lowpass8);
											if (lowpass9 != null) {
												Record lowpass10 = level10.AddRecord(lowpass9);
												if (lowpass10 != null) {
													Record lowpass11 = level11.AddRecord(lowpass10);
													if (lowpass11 != null) {
														Record lowpass12 = level12.AddRecord(lowpass11);
														if (lowpass12 != null) {
															Record lowpass13 = level13.AddRecord(lowpass12);
															if (lowpass13 != null) {
																level14.AddRecord(lowpass13);
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public Record GetRecord(int level, int timeAgo) { // time in seconds * (2^level), 0 mans last record for all levels, 1 means 4 seconds for level2
		return strips[level].GetRecord(timeAgo);
	}

	public int GetLowpassCounter(int level) {
		return strips[level].lowpassCounter;
	}

	// Load Save

	private HistoryData historyData = new HistoryData();

	// Save
	public HistoryData UpdateData() {
		historyData.level0 =  level0.UpdateData();
		historyData.level1 =  level1.UpdateData();
		historyData.level2 =  level2.UpdateData();
		historyData.level3 =  level3.UpdateData();
		historyData.level4 =  level4.UpdateData();
		historyData.level5 =  level5.UpdateData();
		historyData.level6 =  level6.UpdateData();
		historyData.level7 =  level7.UpdateData();
		historyData.level8 =  level8.UpdateData();
		historyData.level9 =  level9.UpdateData();
		historyData.level10 = level10.UpdateData();
		historyData.level11 = level11.UpdateData();
		historyData.level12 = level12.UpdateData();
		historyData.level13 = level13.UpdateData();
		historyData.level14 = level14.UpdateData();

		return historyData;
	}

	// Load
	public void ApplyData(HistoryData historyData) {
		Clear();

		level0.ApplyData(historyData.level0);
		level1.ApplyData(historyData.level1);
		level2.ApplyData(historyData.level2);
		level3.ApplyData(historyData.level3);
		level4.ApplyData(historyData.level4);
		level5.ApplyData(historyData.level5);
		level6.ApplyData(historyData.level6);
		level7.ApplyData(historyData.level7);
		level8.ApplyData(historyData.level8);
		level9.ApplyData(historyData.level9);
		level10.ApplyData(historyData.level10);
		level11.ApplyData(historyData.level11);
		level12.ApplyData(historyData.level12);
		level13.ApplyData(historyData.level13);
		level14.ApplyData(historyData.level14);
	}

	// ^ Load Save ^
}
