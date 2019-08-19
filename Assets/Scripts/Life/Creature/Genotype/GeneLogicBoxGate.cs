public class GeneLogicBoxGate {

	public LogicOperatorEnum operatorType = LogicOperatorEnum.And;
	public int leftFlankPosition;
	public int rightFlankPosition;

	public GeneLogicBoxGate(GeneLogicBoxGateData geneLogicBoxGateData) {
		ApplyData(geneLogicBoxGateData);
	}



	// Save
	private GeneLogicBoxGateData geneLogicBoxGateData = new GeneLogicBoxGateData();
	public GeneLogicBoxGateData UpdateData() {
		geneLogicBoxGateData.operatorType = operatorType;

		return geneLogicBoxGateData;
	}

	// Load
	public void ApplyData(GeneLogicBoxGateData geneLogicBoxGateData) {
		operatorType = geneLogicBoxGateData.operatorType;
	}
}
