using System;
using System.Collections.Generic;

[Serializable]
public class SoulReferenceData {
	// me 
	public string id = "no id";
	
	// As seen from mother:
	public bool isChildConnected; // Should be connected
	public Vector2i childRootMapPosition; //As seen from mothers frame of reference
	public int childRootBindCardinalIndex; //As seen from mothers frame of reference
}