using UnityEngine;

public class Gene {
	// Egg cell
	public float eggCellFertilizeThreshold = 40f; // J
	public bool eggCellCanFertilizeWhenAttached = true;

	public ChildDetatchModeEnum eggCellDetatchMode = ChildDetatchModeEnum.Size;
	public float eggCellDetatchSizeThreshold = 5; //J 
	public float eggCellDetatchEnergyThreshold = 45; //J 

	//Jaw Cell
	//bool: eat mother, eat child, eat sibling 


	public CellTypeEnum m_type = CellTypeEnum.Leaf;
	public CellTypeEnum type {
		get {
			if (index == 0 && m_type == CellTypeEnum.Egg) {
				return CellTypeEnum.Vein;
			}
			return m_type;
		}
		set {
			if (index == 0 && value == CellTypeEnum.Egg) {
				m_type = CellTypeEnum.Vein;
			} else {
				m_type = value;
			}
		}
	}
	public int index;

	public readonly Arrangement[] arrangements = new Arrangement[3];

	public Gene(int index) {
		this.index = index;

		arrangements[0] = new Arrangement();
		arrangements[1] = new Arrangement();
		arrangements[2] = new Arrangement();
	}

	public void SetReferenceGeneFromReferenceGeneIndex(Gene[] genes) {
		arrangements[0].SetReferenceGeneFromReferenceGeneIndex(genes);
		arrangements[1].SetReferenceGeneFromReferenceGeneIndex(genes);
		arrangements[2].SetReferenceGeneFromReferenceGeneIndex(genes);
	}

	public void Mutate(float strength) {
		GlobalSettings gs = GlobalSettings.instance;
		float mut = Random.Range(0, gs.mutation.cellTypeLeave + gs.mutation.cellTypeRandom * strength);
		if (mut < gs.mutation.cellTypeRandom * strength) {
			type = (CellTypeEnum)Random.Range(0, 8);
		}

		//Egg
		float spread = 6f;
		mut = Random.Range(0, gs.mutation.eggCellFertilizeThresholdLeave + gs.mutation.eggCellFertilizeThresholdRandom * strength);
		if (mut < gs.mutation.eggCellFertilizeThresholdRandom * strength) {
			eggCellFertilizeThreshold = Mathf.Clamp(eggCellFertilizeThreshold - spread + Random.Range(0f, spread) + Random.Range(0f, spread), 0, 99f); // J 
		}
	
		mut = Random.Range(0, gs.mutation.eggCellCanFertilizeWhenAttachedLeave + gs.mutation.eggCellCanFertilizeWhenAttachedChange * strength);
		if (mut < gs.mutation.eggCellCanFertilizeWhenAttachedChange * strength) {
			eggCellCanFertilizeWhenAttached = !eggCellCanFertilizeWhenAttached; //toggle
		}

		spread = 2f;
		mut = Random.Range(0, gs.mutation.eggCellDetatchSizeThresholdLeave + gs.mutation.eggCellDetatchSizeThresholdRandom * strength);
		if (mut < gs.mutation.eggCellDetatchSizeThresholdRandom * strength) {
			eggCellDetatchSizeThreshold = Mathf.Clamp(eggCellDetatchSizeThreshold - spread + Random.Range(0f, spread) + Random.Range(0f, spread), 1f, 30f); // J
		}

		spread = 6f;
		mut = Random.Range(0, gs.mutation.eggCellDetatchEnergyThresholdLeave + gs.mutation.eggCellDetatchEnergyThresholdRandom * strength);
		if (mut < gs.mutation.eggCellDetatchEnergyThresholdRandom * strength) {
			eggCellDetatchEnergyThreshold = Mathf.Clamp(eggCellDetatchEnergyThreshold - spread + Random.Range(0f, spread) + Random.Range(0f, spread), 0f, 110); // J
		}

		//arrangements
		arrangements[0].Mutate(strength);
		arrangements[1].Mutate(strength);
		arrangements[2].Mutate(strength);
	}

	public void Scramble() {
		type = (CellTypeEnum)Random.Range(0, 8);
		arrangements[0].Scramble();
		arrangements[1].Scramble();
		arrangements[2].Scramble();
	}

	public GeneReference GetFlippableReference(int referenceCardinalIndex, FlipSideEnum flipSide) {
		GeneReference first = null;
		for (int index = 0; index < arrangements.Length; index++) {
			GeneReference found = arrangements[index].GetFlippableReference(referenceCardinalIndex, flipSide);
			if (found != null && arrangements[index].isEnabled) {
				first = found;
				break;
			}
		}
		return first;
	}

	public void SetDefault(Gene[] genome) {
		arrangements[0].referenceGene = genome[1];
		arrangements[1].referenceGene = genome[1];
		arrangements[2].referenceGene = genome[1];

		arrangements[0].isEnabled = false;
		arrangements[1].isEnabled = false;
		arrangements[2].isEnabled = false;
	}

	public Gene GetClone() {
		Gene clone = new Gene(index);
		clone.ApplyData(UpdateData());
		return clone;
	}

	// Save
	private GeneData geneData = new GeneData();
	public GeneData UpdateData() {
		geneData.type = type;
		geneData.index = index;

		// Egg
		geneData.eggCellFertilizeThreshold = eggCellFertilizeThreshold;
		geneData.eggCellCanFertilizeWhenAttached = eggCellCanFertilizeWhenAttached;
		geneData.eggCellDetatchMode = eggCellDetatchMode;
		geneData.eggCellDetatchSizeThreshold = eggCellDetatchSizeThreshold;
		geneData.eggCellDetatchEnergyThreshold = eggCellDetatchEnergyThreshold;

		geneData.arrangementData[0] = arrangements[0].UpdateData();
		geneData.arrangementData[1] = arrangements[1].UpdateData();
		geneData.arrangementData[2] = arrangements[2].UpdateData();

		return geneData;
	}

	// Load
	public void ApplyData(GeneData geneData) {
		type = geneData.type;
		index = geneData.index;

		// Egg
		eggCellFertilizeThreshold = geneData.eggCellFertilizeThreshold;
		eggCellCanFertilizeWhenAttached = geneData.eggCellCanFertilizeWhenAttached;
		eggCellDetatchMode = geneData.eggCellDetatchMode;
		eggCellDetatchSizeThreshold = geneData.eggCellDetatchSizeThreshold;
		eggCellDetatchEnergyThreshold = geneData.eggCellDetatchEnergyThreshold;

		arrangements[0].ApplyData(geneData.arrangementData[0]);
		arrangements[1].ApplyData(geneData.arrangementData[1]);
		arrangements[2].ApplyData(geneData.arrangementData[2]);
	}
}

