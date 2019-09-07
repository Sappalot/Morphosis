using System;

[Serializable]
public class GeneLogicBoxGateData {
	public LogicOperatorEnum operatorType;
	public int leftFlank;
	public int rightFlank;
	public bool isUsed;
}
