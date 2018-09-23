using UnityEngine;

public class Gene {
	// Egg cell
	public float eggCellFertilizeThreshold = 0.4f; //part of max energy (* 100 to get  %)
	public bool eggCellCanFertilizeWhenAttached = true;
	// Has to be stored in childs origin cell, so that inf can be kept when i'm gone
	public ChildDetatchModeEnum eggCellDetatchMode = ChildDetatchModeEnum.Size;
	public float eggCellDetatchSizeThreshold = 0.5f; // completeness count / max count 
	public float eggCellDetatchEnergyThreshold = 0.4f; //part of max energy(* 100 to get  %)
	// ^ Egg cell ^

	// Jaw Cell
	public bool jawCellCannibalizeKin;
	public bool jawCellCannibalizeMother;
	public bool jawCellCannibalizeFather;
	public bool jawCellCannibalizeSiblings;
	public bool jawCellCannibalizeChildren;
	// ^ Jaw Cell ^

	// Axon
	public bool axonIsEnabled;
	public float axonFromOriginOffset;
	public bool axonIsFromOriginPlus180;
	public float axonFromMeOffset;
	public float axonRelaxContract;
	public bool axonIsReverse;
	// ^ Axon ^

	// Origin
	public int originPulsePeriodTicks = 80;

	public float originPulsePeriod {
		get {
			return originPulsePeriodTicks * Time.fixedDeltaTime;
		}
	}

	public float originPulseFequenzy {
		get {
			return 1f / originPulsePeriod;
		}
	}
	// ^ Origin ^


	private CellTypeEnum m_type = CellTypeEnum.Leaf;
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
	public bool isOrigin {
		get {
			return index == 0;
		}
	}


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

		// Egg
		float spread = 0.06f; // TODO move toGlobal settings
		mut = Random.Range(0, gs.mutation.eggCellFertilizeThresholdLeave + gs.mutation.eggCellFertilizeThresholdRandom * strength);
		if (mut < gs.mutation.eggCellFertilizeThresholdRandom * strength) {
			eggCellFertilizeThreshold = Mathf.Clamp(eggCellFertilizeThreshold - spread + Random.Range(0f, spread) + Random.Range(0f, spread), gs.phenotype.eggCellFertilizeThresholdMin, gs.phenotype.eggCellFertilizeThresholdMax); // Cell energy fullness J/J
		}
	
		mut = Random.Range(0, gs.mutation.eggCellCanFertilizeWhenAttachedLeave + gs.mutation.eggCellCanFertilizeWhenAttachedChange * strength);
		if (mut < gs.mutation.eggCellCanFertilizeWhenAttachedChange * strength) {
			eggCellCanFertilizeWhenAttached = !eggCellCanFertilizeWhenAttached; //toggle
		}

		spread = 0.06f; // TODO move toGlobal settings
		mut = Random.Range(0, gs.mutation.eggCellDetatchSizeThresholdLeave + gs.mutation.eggCellDetatchSizeThresholdRandom * strength);
		if (mut < gs.mutation.eggCellDetatchSizeThresholdRandom * strength) {
			eggCellDetatchSizeThreshold = Mathf.Clamp(eggCellDetatchSizeThreshold - spread + Random.Range(0f, spread) + Random.Range(0f, spread), gs.phenotype.eggCellDetatchSizeThresholdMin, gs.phenotype.eggCellDetatchSizeThresholdMax); // count / max count
		}

		spread = 0.06f;
		mut = Random.Range(0, gs.mutation.eggCellDetatchEnergyThresholdLeave + gs.mutation.eggCellDetatchEnergyThresholdRandom * strength);
		if (mut < gs.mutation.eggCellDetatchEnergyThresholdRandom * strength) {
			eggCellDetatchEnergyThreshold = Mathf.Clamp(eggCellDetatchEnergyThreshold - spread + Random.Range(0f, spread) + Random.Range(0f, spread), gs.phenotype.eggCellDetatchEnergyThresholdMin, gs.phenotype.eggCellDetatchEnergyThresholdMax); // J
		}
		// ^ Egg ^

		// Jaw
		mut = Random.Range(0, gs.mutation.jawCellCannibalizeKinLeave + gs.mutation.jawCellCannibalizeKinChange * strength);
		if (mut < gs.mutation.jawCellCannibalizeKinChange * strength) {
			jawCellCannibalizeKin = !jawCellCannibalizeKin;
		}

		mut = Random.Range(0, gs.mutation.jawCellCannibalizeMotherLeave + gs.mutation.jawCellCannibalizeMotherChange * strength);
		if (mut < gs.mutation.jawCellCannibalizeMotherChange * strength) {
			jawCellCannibalizeMother = !jawCellCannibalizeMother;
		}

		mut = Random.Range(0, gs.mutation.jawCellCannibalizeFatherLeave + gs.mutation.jawCellCannibalizeFatherChange * strength);
		if (mut < gs.mutation.jawCellCannibalizeFatherChange * strength) {
			jawCellCannibalizeFather = !jawCellCannibalizeFather;
		}

		mut = Random.Range(0, gs.mutation.jawCellCannibalizeSiblingsLeave + gs.mutation.jawCellCannibalizeSiblingsChange * strength);
		if (mut < gs.mutation.jawCellCannibalizeSiblingsChange * strength) {
			jawCellCannibalizeSiblings = !jawCellCannibalizeSiblings;
		}

		mut = Random.Range(0, gs.mutation.jawCellCannibalizeChildrenLeave + gs.mutation.jawCellCannibalizeChildrenChange * strength);
		if (mut < gs.mutation.jawCellCannibalizeChildrenChange * strength) {
			jawCellCannibalizeChildren = !jawCellCannibalizeChildren;
		}
		// ^ Jaw ^

		// Origo
		spread = 40;
		mut = Random.Range(0, gs.mutation.OriginPulseFrequenzyLeave + gs.mutation.OriginPulseFrequenzyRandom * strength);
		if (mut < gs.mutation.OriginPulseFrequenzyRandom * strength) {
			originPulsePeriodTicks = (int)Mathf.Clamp(originPulsePeriodTicks - spread + Random.Range(0, spread) + Random.Range(0, spread), 1f / (Time.fixedDeltaTime * GlobalSettings.instance.phenotype.originPulseFrequenzyMax), 1f / (Time.fixedDeltaTime * GlobalSettings.instance.phenotype.originPulseFrequenzyMin));
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

		// Jaw
		geneData.jawCellCannibalizeKin =      jawCellCannibalizeKin;
		geneData.jawCellCannibalizeMother =   jawCellCannibalizeMother;
		geneData.jawCellCannibalizeFather =   jawCellCannibalizeFather;
		geneData.jawCellCannibalizeSiblings = jawCellCannibalizeSiblings;
		geneData.jawCellCannibalizeChildren = jawCellCannibalizeChildren;

		// Axon
		geneData.axonIsEnabled = axonIsEnabled;
		geneData.axonFromOriginOffset = axonFromOriginOffset;
		geneData.axonIsFromOriginPlus180 = axonIsFromOriginPlus180;
		geneData.axonFromMeOffset = axonFromMeOffset;
		geneData.axonRelaxContract = axonRelaxContract;
		geneData.axonIsReverse = axonIsReverse;


		// Origin
		geneData.originPulsePeriodTicks =     originPulsePeriodTicks;

		// 3 Arrangements
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
		//backward compatibility
		if (geneData.eggCellFertilizeThreshold > GlobalSettings.instance.phenotype.eggCellFertilizeThresholdMax) {// if more than 100% must be old (where we measured cell energy)
			eggCellFertilizeThreshold = geneData.eggCellFertilizeThreshold / 100f;
		} else {
			eggCellFertilizeThreshold = geneData.eggCellFertilizeThreshold;
		}
		eggCellCanFertilizeWhenAttached = geneData.eggCellCanFertilizeWhenAttached;
		eggCellDetatchMode = geneData.eggCellDetatchMode;

		if (geneData.eggCellDetatchSizeThreshold > GlobalSettings.instance.phenotype.eggCellDetatchSizeThresholdMax) {
			eggCellDetatchSizeThreshold = geneData.eggCellDetatchSizeThreshold / 30f;
		} else {
			eggCellDetatchSizeThreshold = geneData.eggCellDetatchSizeThreshold;
		}

		if (geneData.eggCellDetatchEnergyThreshold > GlobalSettings.instance.phenotype.eggCellDetatchEnergyThresholdMax) {
			eggCellDetatchEnergyThreshold = geneData.eggCellDetatchEnergyThreshold / 100f;
		} else {
			eggCellDetatchEnergyThreshold = geneData.eggCellDetatchEnergyThreshold;
		}

		// Jaw
		jawCellCannibalizeKin =      geneData.jawCellCannibalizeKin;
		jawCellCannibalizeMother =   geneData.jawCellCannibalizeMother;
		jawCellCannibalizeFather =   geneData.jawCellCannibalizeFather;
		jawCellCannibalizeSiblings = geneData.jawCellCannibalizeSiblings;
		jawCellCannibalizeChildren = geneData.jawCellCannibalizeChildren;

		// Axon
		axonIsEnabled =             geneData.axonIsEnabled;
		axonFromOriginOffset =      geneData.axonFromOriginOffset;
		axonIsFromOriginPlus180 =   geneData.axonIsFromOriginPlus180;
		axonFromMeOffset =          geneData.axonFromMeOffset;
		axonRelaxContract =         geneData.axonRelaxContract;
		axonIsReverse =             geneData.axonIsReverse;

		// Origin
		originPulsePeriodTicks = geneData.originPulsePeriodTicks == 0 ? 80 : geneData.originPulsePeriodTicks;

		// 3 Arrangements
		arrangements[0].ApplyData(geneData.arrangementData[0]);
		arrangements[1].ApplyData(geneData.arrangementData[1]);
		arrangements[2].ApplyData(geneData.arrangementData[2]);
	}
}