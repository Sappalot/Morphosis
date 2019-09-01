public class GeneLogicBox {
	public GeneLogicBoxGate gateLayer0;
	//public List<GeneLogicBoxGate> gateLayer1 = new List<GeneLogicBoxGate>();
	//public List<GeneLogicBoxGate> gateLayer2 = new List<GeneLogicBoxGate>();

	public GeneLogicBox() {

	}

	public GeneLogicBox(GeneLogicBoxData geneLogicBoxData) {
		ApplyData(geneLogicBoxData);
	}

	// Save
	private GeneLogicBoxData geneLogicBoxData = new GeneLogicBoxData();
	public GeneLogicBoxData UpdateData() {
		//geneLogicBoxData.layer0LogicBoxGateData = gateLayer0.UpdateData();


		return geneLogicBoxData;
	}

	// Load
	public void ApplyData(GeneLogicBoxData geneLogicBoxData) {
		gateLayer0 = new GeneLogicBoxGate(geneLogicBoxData.layer0LogicBoxGateData);

	}
}
