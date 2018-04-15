public class Mother {
	public string id;

	public Mother() {}

	public Mother(string id) {
		this.id = id;
	}


	//Load / Save
	private MotherData motherData = new MotherData();

	// Save
	public MotherData UpdateData() {
		motherData.id = id;
		return motherData;
	}

	// Load
	public void ApplyData(MotherData motherData) {
		id = motherData.id;
	}

	// ^ Load / Save ^

}