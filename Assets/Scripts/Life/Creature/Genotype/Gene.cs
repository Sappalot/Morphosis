using UnityEngine;

public class Gene {
	// Egg cell
	public float eggCellFertilizeThreshold = 0.4f; //part of max energy (* 100 to get  %)
	// Has to be stored in childs origin cell, so that inf can be kept when i'm gone
	public ChildDetatchModeEnum eggCellDetatchMode = ChildDetatchModeEnum.Size;
	public float eggCellDetatchSizeThreshold = 0.5f; // completeness count / max count 
	public float eggCellDetatchEnergyThreshold = 0.4f; //part of max energy(* 100 to get  %)
	public bool eggCellIdleWhenAttached = false;
	// ^ Egg cell ^

	// Jaw Cell
	public bool jawCellCannibalizeKin;
	public bool jawCellCannibalizeMother;
	public bool jawCellCannibalizeFather;
	public bool jawCellCannibalizeSiblings;
	public bool jawCellCannibalizeChildren;
	public bool jawCellIdleWhenAttached = false;
	// ^ Jaw Cell ^

	// Leaf Cell
	public bool leafCellIdleWhenAttached = false;
	// ^ Leaf Cell ^

	// Muscle Cell
	public bool muscleCellIdleWhenAttached = false;
	// ^ Muscle Cell ^

	// Shell
	public int shellCellArmorClass = 2;
	public int shellCellTransparancyClass = 2;
	// ^ Shell ^

	// Axon
	private bool m_axonIsEnabled;
	public bool axonIsEnabled {
		get {
			return m_axonIsEnabled || isOrigin;
		}
		set {
			m_axonIsEnabled = value;
		}
	}
	
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

		mut = Random.Range(0, gs.mutation.cellIdleWhenAttachedLeave + gs.mutation.cellIdleWhenAttachedChange * strength);
		if (mut < gs.mutation.cellIdleWhenAttachedChange * strength) {
			eggCellIdleWhenAttached = !eggCellIdleWhenAttached; //toggle
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

		mut = Random.Range(0, gs.mutation.cellIdleWhenAttachedLeave + gs.mutation.cellIdleWhenAttachedChange * strength);
		if (mut < gs.mutation.cellIdleWhenAttachedChange * strength) {
			jawCellIdleWhenAttached = !jawCellIdleWhenAttached; //toggle
		}
		// ^ Jaw ^

		// Leaf
		mut = Random.Range(0, gs.mutation.cellIdleWhenAttachedLeave + gs.mutation.cellIdleWhenAttachedChange * strength);
		if (mut < gs.mutation.cellIdleWhenAttachedChange * strength) {
			leafCellIdleWhenAttached = !leafCellIdleWhenAttached; //toggle
		}
		// ^ Leaf ^ 

		// Muscle
		mut = Random.Range(0, gs.mutation.cellIdleWhenAttachedLeave + gs.mutation.cellIdleWhenAttachedChange * strength);
		if (mut < gs.mutation.cellIdleWhenAttachedChange * strength) {
			muscleCellIdleWhenAttached = !muscleCellIdleWhenAttached; //toggle
		}
		// ^ Muscle ^

		// Muscle
		mut = Random.Range(0, gs.mutation.cellIdleWhenAttachedLeave + gs.mutation.cellIdleWhenAttachedChange * strength);
		if (mut < gs.mutation.cellIdleWhenAttachedChange * strength) {
			muscleCellIdleWhenAttached = !muscleCellIdleWhenAttached; //toggle
		}
		// ^ Muscle ^

		// Shell
		mut = Random.Range(0, gs.mutation.shellCellArmorClassLeave + gs.mutation.shellCellArmorClassChange * strength);
		if (mut < gs.mutation.shellCellArmorClassChange * strength) {
			shellCellArmorClass = Mathf.Clamp(shellCellArmorClass - 1 + Mathf.FloorToInt( Random.Range(0, 3)), 0, ShellCell.armourClassCount - 1);
		}
		mut = Random.Range(0, gs.mutation.shellCellTransparancyClassLeave + gs.mutation.shellCellTransparancyClassChange * strength);
		if (mut < gs.mutation.shellCellTransparancyClassChange * strength) {
			shellCellTransparancyClass = Mathf.Clamp(shellCellTransparancyClass - 1 + Mathf.FloorToInt(Random.Range(0, 3)), 0, ShellCell.transparencyClassCount - 1);
		}
		// ^ Shell ^


		// Axon
		mut = Random.Range(0, gs.mutation.axonEnabledLeave + gs.mutation.axonEnabledChange * strength);
		if (mut < gs.mutation.axonEnabledChange * strength) {
			axonIsEnabled = !axonIsEnabled; //toggle
		}

		spread = 45f;
		mut = Random.Range(0, gs.mutation.axonFromOriginOffsetLeave + gs.mutation.axonFromOriginOffsetChange * strength);
		if (mut < gs.mutation.axonFromOriginOffsetChange * strength) {
			axonFromOriginOffset = Mathf.Clamp(axonFromOriginOffset - spread + Random.Range(0f, spread) + Random.Range(0f, spread), 0f, 360f);
		}

		mut = Random.Range(0, gs.mutation.axonIsFromOriginPlus180Leave + gs.mutation.axonIsFromOriginPlus180Change * strength);
		if (mut < gs.mutation.axonIsFromOriginPlus180Change * strength) {
			axonIsFromOriginPlus180 = !axonIsFromOriginPlus180; //toggle
		}

		spread = 45f;
		mut = Random.Range(0, gs.mutation.axonFromMeOffsetLeave + gs.mutation.axonFromMeOffsetChange * strength);
		if (mut < gs.mutation.axonFromMeOffsetChange * strength) {
			axonFromMeOffset = Mathf.Clamp(axonFromMeOffset - spread + Random.Range(0f, spread) + Random.Range(0f, spread), 0f, 360f);
		}

		spread = 0.25f;
		mut = Random.Range(0, gs.mutation.axonRelaxContractLeave + gs.mutation.axonRelaxContractChange * strength);
		if (mut < gs.mutation.axonRelaxContractChange * strength) {
			axonRelaxContract = Mathf.Clamp(axonRelaxContract - spread + Random.Range(0f, spread) + Random.Range(0f, spread), -1f, 1f);
		}

		mut = Random.Range(0, gs.mutation.axonIsReverseLeave + gs.mutation.axonIsReverseChange * strength);
		if (mut < gs.mutation.axonIsReverseChange * strength) {
			axonIsReverse = !axonIsReverse; //toggle
		}


		// ^ Axon ^ 

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
		geneData.eggCellDetatchMode = eggCellDetatchMode;
		geneData.eggCellDetatchSizeThreshold = eggCellDetatchSizeThreshold;
		geneData.eggCellDetatchEnergyThreshold = eggCellDetatchEnergyThreshold;
		geneData.eggCellIdleWhenAttached = eggCellIdleWhenAttached;

		// Jaw
		geneData.jawCellCannibalizeKin =      jawCellCannibalizeKin;
		geneData.jawCellCannibalizeMother =   jawCellCannibalizeMother;
		geneData.jawCellCannibalizeFather =   jawCellCannibalizeFather;
		geneData.jawCellCannibalizeSiblings = jawCellCannibalizeSiblings;
		geneData.jawCellCannibalizeChildren = jawCellCannibalizeChildren;
		geneData.jawCellIdleWhenAttached =    jawCellIdleWhenAttached;

		// Leaf
		geneData.leafCellIdleWhenAttached = leafCellIdleWhenAttached;

		// Muscle
		geneData.muscleCellIdleWhenAttached = muscleCellIdleWhenAttached;

		// Shell
		geneData.shellCellArmourClass = shellCellArmorClass;
		geneData.shellCellTransparancyClass = shellCellTransparancyClass;

		// Axon
		geneData.axonIsEnabled = axonIsEnabled;
		geneData.axonFromOriginOffset = axonFromOriginOffset;
		geneData.axonIsFromOriginPlus180 = axonIsFromOriginPlus180;
		geneData.axonFromMeOffset = axonFromMeOffset;
		geneData.axonRelaxContract = axonRelaxContract;
		geneData.axonIsReverse = axonIsReverse;

		geneData.arrangementData[0] = arrangements[0].UpdateData();
		geneData.arrangementData[1] = arrangements[1].UpdateData();
		geneData.arrangementData[2] = arrangements[2].UpdateData();

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
		eggCellIdleWhenAttached = geneData.eggCellIdleWhenAttached;

		// Jaw
		jawCellCannibalizeKin =      geneData.jawCellCannibalizeKin;
		jawCellCannibalizeMother =   geneData.jawCellCannibalizeMother;
		jawCellCannibalizeFather =   geneData.jawCellCannibalizeFather;
		jawCellCannibalizeSiblings = geneData.jawCellCannibalizeSiblings;
		jawCellCannibalizeChildren = geneData.jawCellCannibalizeChildren;
		jawCellIdleWhenAttached =    geneData.jawCellIdleWhenAttached;

		// Leaf
		leafCellIdleWhenAttached = geneData.leafCellIdleWhenAttached;

		// Muscle
		muscleCellIdleWhenAttached = geneData.muscleCellIdleWhenAttached;

		// Shell
		shellCellArmorClass = geneData.shellCellArmourClass;
		shellCellTransparancyClass = geneData.shellCellTransparancyClass;

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