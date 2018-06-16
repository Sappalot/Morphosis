using System.Collections.Generic;

public class Record {
	float fps;
	float pps;
	float cellCountTotal;
	float cellCountJaw;
	float cellCountLeaf;

	public string tag = null;
	public bool showLine = false;

	//private Dictionary<RecordEnum, float> entries = new Dictionary<RecordEnum, float>();

	//public bool Contains(RecordEnum type) {
	//	return entries.ContainsKey(type);
	//}

	public void Clear() {
		fps =            0f;
		pps =            0f;
		cellCountTotal = 0f;
		cellCountJaw =   0f;
		cellCountLeaf =  0f;
		tag = "";
	}

	public bool HasTag() {
		return tag != null && tag != "";
	}

	public void SetTagText(string text, bool drawLine) {
		tag = text;
		showLine = drawLine;
	}

	//public void ExtendTagText(string text, bool drawLine) {
	//	if (HasTag()) {
	//		tag = tag + " " + text;
	//		tagLine |= drawLine;
	//	} else {
	//		SetTagText(text, drawLine);
	//	}
	//}

	public float Get(RecordEnum type) {
		if (type == RecordEnum.fps) {
			return fps;
		}
		if (type == RecordEnum.pps) {
			return pps;
		}
		if (type == RecordEnum.cellCountTotal) {
			return cellCountTotal;
		}
		if (type == RecordEnum.cellCountJaw) {
			return cellCountJaw;
		}
		if (type == RecordEnum.cellCountLeaf) {
			return cellCountLeaf;
		}
		return 0f;
	}

	public void Add(RecordEnum type, float value) {
		if (type == RecordEnum.fps) {
			fps = value;
		}
		if (type == RecordEnum.pps) {
			pps = value;
		}
		if (type == RecordEnum.cellCountTotal) {
			cellCountTotal = value;
		}
		if (type == RecordEnum.cellCountJaw) {
			cellCountJaw = value;
		}
		if (type == RecordEnum.cellCountLeaf) {
			cellCountLeaf = value;
		}
	}

	//public string testString;

	private RecordData recordData = new RecordData();

	// Save
	public RecordData UpdateData() {
		recordData.fps = fps;
		recordData.pps = pps;
		recordData.cellCountTotal = cellCountTotal;
		recordData.cellCountJaw = cellCountJaw;
		recordData.cellCountLeaf = cellCountLeaf;

		recordData.tag = tag;
		recordData.showLine = showLine;
		//recordData.entries = new Dictionary<RecordEnum, float>();
		//foreach (KeyValuePair<RecordEnum, float> entry in entries) {
		//	recordData.entries.Add(entry.Key, entry.Value);
		//}
		//recordData.testString = "yo!";
		return recordData;
	}

	// Load
	public void ApplyData(RecordData recordData) {
		fps =            recordData.fps;
		pps =            recordData.pps;
		cellCountTotal = recordData.cellCountTotal;
		cellCountJaw =   recordData.cellCountJaw;
		cellCountLeaf =  recordData.cellCountLeaf;

		tag = recordData.tag;
		showLine = recordData.showLine;
		//foreach (KeyValuePair<RecordEnum, float> entry in recordData.entries) {
		//	entries.Add(entry.Key, entry.Value);
		//}
		//testString = recordData.testString;
	}
}
