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
}
