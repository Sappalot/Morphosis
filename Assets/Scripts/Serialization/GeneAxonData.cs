using System;

[Serializable]
public class GeneAxonData {
	public bool axonIsEnabled;
	public float axonFromOriginOffset;
	public bool axonIsFromOriginPlus180;
	public float axonFromMeOffset;
	public float axonRelaxContract;
	public bool axonIsReverse;

	public int pulseProgram11;
	public int pulseProgram10;
	public int pulseProgram01;
	public int pulseProgram00;

	public GeneLogicBoxInputData axonInputLeft = new GeneLogicBoxInputData();
	public GeneLogicBoxInputData axonInputRight = new GeneLogicBoxInputData();
}
