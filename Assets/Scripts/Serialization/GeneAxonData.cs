using System;

[Serializable]
public class GeneAxonData {
	public bool axonIsEnabled;
	public float axonFromOriginOffset;
	public bool axonIsFromOriginPlus180;
	public float axonFromMeOffset;
	public float axonRelaxContract;
	public bool axonIsReverse;

	public int pulseProgram3;
	public int pulseProgram2;
	public int pulseProgram1;
	public int pulseProgram0;

	public GeneLogicBoxInputData axonInputLeft = new GeneLogicBoxInputData();
	public GeneLogicBoxInputData axonInputRight = new GeneLogicBoxInputData();
}
