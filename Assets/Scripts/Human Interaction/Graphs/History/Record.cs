using System.Collections.Generic;

public class Record {
	private Dictionary<RecordEnum, float> values = new Dictionary<RecordEnum, float>();

	public bool Contains(RecordEnum type) {
		return values.ContainsKey(type);
	}

	public void Clear() {
		values.Clear();
	}

	public float Get(RecordEnum type) {
		return values[type];
	}

	public void Add(RecordEnum type, float value) {
		values[type] =  value;
	}
}
