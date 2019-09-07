using System.Collections.Generic;

public class GeneLogicBoxInput : GeneLogicBoxComponent {

	public enum Mode {
		Pass,
		Block,
	}

	public Mode mode = Mode.Pass;

	//Todo: have univeral reference to input which is possible to save
	// from this universal input, we should be able to retreive reference to input

	public GeneLogicBoxInput(int row, int column) {
		this.row = row;
		leftFlank = GetFlankLeftOfColumn(column);
		rightFlank = GetFlankRightOfColumn(column);
	}


	//// Save
	//private GeneLogicBoxGateData geneLogicBoxGateData = new GeneLogicBoxGateData();
	//public GeneLogicBoxGateData UpdateData() {
	//	geneLogicBoxGateData.operatorType = operatorType;
	//	geneLogicBoxGateData.leftFlank = leftFlank;
	//	geneLogicBoxGateData.rightFlank = rightFlank;
	//	geneLogicBoxGateData.isUsed = isUsed;
	//	return geneLogicBoxGateData;
	//}

	//// Load
	//public void ApplyData(GeneLogicBoxGateData geneLogicBoxGateData) {
	//	if (!isLocked) {
	//		operatorType = geneLogicBoxGateData.operatorType;
	//		leftFlank = geneLogicBoxGateData.leftFlank;
	//		rightFlank = geneLogicBoxGateData.rightFlank;
	//		isUsed = geneLogicBoxGateData.isUsed;
	//	}
	//}
}
