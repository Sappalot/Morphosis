﻿using System;
using UnityEngine;

[Serializable]
public class GenotypeData  {
	public GeneData[] geneData = new GeneData[Genotype.genomeLength];

	public Vector2 rootPosition;
	public float rootHeading;
}