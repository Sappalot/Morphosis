using System;

[Serializable]
public class GeneAxonData {
	public bool axonIsEnabled;

	public int pulseProgram3;
	public int pulseProgram2;
	public int pulseProgram1;
	public int pulseProgram0;

	public GeneLogicBoxInputData axonInputLeft = new GeneLogicBoxInputData();
	public GeneLogicBoxInputData axonInputRight = new GeneLogicBoxInputData();

	public GeneAxonPulseData pulseDataA = new GeneAxonPulseData();
	public GeneAxonPulseData pulseDataB = new GeneAxonPulseData();
	public GeneAxonPulseData pulseDataC = new GeneAxonPulseData();
	public GeneAxonPulseData pulseDataD = new GeneAxonPulseData();
}
