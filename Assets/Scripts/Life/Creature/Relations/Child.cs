public class Child {

	public Child() {}

	public Child(string id, bool isConnectedToMother, Vector2i originMapPosition, int originBindCardinalIndex) {
		this.id = id;
		this.isConnectedToMother = isConnectedToMother;
		this.originMapPosition = originMapPosition;
		this.originBindCardinalIndex = originBindCardinalIndex;
	}

	public string id; // a creature carying a child with an id that can not be found in life ==> child concidered dead to mother
	public bool isConnectedToMother;
	public Vector2i originMapPosition; //As seen from mothers frame of reference
	public int originBindCardinalIndex; //As seen from mothers frame of reference

	//Load / Save
	private ChildData childData = new ChildData();

	// Save
	public ChildData UpdateData() {
		childData.id =                      id;
		childData.isConnectedToMother =     isConnectedToMother;
		childData.originMapPosition =       originMapPosition; //As seen from mothers frame of reference
		childData.originBindCardinalIndex = originBindCardinalIndex;
		return childData;
	}

	// Load
	public void ApplyData(ChildData childData) {
		id =                      childData.id;
		isConnectedToMother =     childData.isConnectedToMother;
		originMapPosition =       childData.originMapPosition; //As seen from mothers frame of reference
		originBindCardinalIndex = childData.originBindCardinalIndex;
	}

	// ^ Load / Save ^
}