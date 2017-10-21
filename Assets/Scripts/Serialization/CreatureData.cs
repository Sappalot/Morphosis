using System;
using UnityEngine;

[Serializable]
public class CreatureData {
	public string id = "no id";
	public string nickname = "no name";

	public GenotypeData genotypeData = new GenotypeData();
	public PhenotypeData phenotypeData = new PhenotypeData();

	public string motherId = string.Empty; //means has no mother
	public bool isMotherConnected;

	public string[] childrenId;
	public bool[] isChildrenConnected;
	public Vector2i[] childrenPlacentaMapPosition;
}