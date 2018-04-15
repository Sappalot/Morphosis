using System;
using System.Collections.Generic;

[Serializable]
public class ChildData {
	public string id; // a creature carying a child with an id that can not be found in life ==> child concidered dead to mother
	public bool isConnectedToMother; // Should be connected
	public Vector2i originMapPosition; //As seen from mothers frame of reference
	public int originBindCardinalIndex; //As seen from mothers frame of reference
}