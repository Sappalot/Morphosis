using System;
using UnityEngine;

[Serializable]
public class SoulData {
	public string id = "no id";

	//??
	public string motherId = string.Empty; //means has no mother
	public bool isMotherConnected;

	public string[] childrenId;
	public bool[] isChildrenConnected;
	public Vector2i[] childrenRootMapPosition; //In mothers frame of reference
	public int[] rootBindCardinalIndex; //In mothers frame of reference
}