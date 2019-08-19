using System;
using System.Collections.Generic;

[Serializable]
public class GeneLogicBoxData {
	public GeneLogicBoxGateData layer0LogicBoxGateData = new GeneLogicBoxGateData();
	public List<GeneLogicBoxGateData> layer1LogicBoxGateData = new List<GeneLogicBoxGateData>();
	public List<GeneLogicBoxGateData> layer2LogicBoxGateData = new List<GeneLogicBoxGateData>();
}
