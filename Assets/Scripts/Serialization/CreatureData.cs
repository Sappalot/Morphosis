using System;
using UnityEngine;

[Serializable]
public class CreatureData {
    public string id = "no id";
    public string nickname = "no name";

    public GenotypeData genotypeData = new GenotypeData();
    public PhenotypeData phenotypeData = new PhenotypeData();

}