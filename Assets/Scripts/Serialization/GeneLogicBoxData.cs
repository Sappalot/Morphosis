using System;
using System.Collections.Generic;

[Serializable]
public class GeneLogicBoxData {
	public GeneLogicBoxGateData layer0LogicBoxGateData = new GeneLogicBoxGateData();
	public GeneLogicBoxGateData[] layer1LogicBoxGateData = new GeneLogicBoxGateData[GeneLogicBox.maxGatesPerLayer];
	public GeneLogicBoxGateData[] layer2LogicBoxGateData = new GeneLogicBoxGateData[GeneLogicBox.maxGatesPerLayer];
}
