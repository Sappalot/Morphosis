using UnityEngine;

public class Gene {
	// Egg cell
	// Has to be stored in childs origin cell, so that inf can be kept when i'm gone
	public ChildDetatchModeEnum eggCellDetatchMode = ChildDetatchModeEnum.Size;
	public float eggCellDetatchSizeThreshold = 0.5f; // completeness count / max count 
	public float eggCellDetatchEnergyThreshold = 0.4f; //part of max energy(* 100 to get  %)
	public bool eggCellHibernateWhenAttachedToMother = false;
	public bool eggCellHibernateWhenAttachedToChild = false;
	public GeneLogicBox eggCellFertilizeLogic = new GeneLogicBox(SignalUnitEnum.WorkLogicBoxA);
	public GeneEnergySensor eggCellFertilizeEnergySensor = new GeneEnergySensor(SignalUnitEnum.WorkSensorA);
	// ^ Egg cell ^

	// Jaw Cell
	public bool jawCellCannibalizeKin;
	public bool jawCellCannibalizeMother;
	public bool jawCellCannibalizeFather;
	public bool jawCellCannibalizeSiblings;
	public bool jawCellCannibalizeChildren;
	public bool jawCellHibernateWhenAttachedToMother = false;
	public bool jawCellHibernateWhenAttachedToChild = false;
	// ^ Jaw Cell ^

	// Leaf Cell
	public bool leafCellHibernateWhenAttachedToMother = false;
	public bool leafCellHibernateWhenAttachedToChild = false;
	// ^ Leaf Cell ^

	// Muscle Cell
	public bool muscleCellHibernateWhenAttachedToMother = false;
	public bool muscleCellHibernateWhenAttachedToChild = false;
	// ^ Muscle Cell ^

	// Shell
	public int shellCellArmorClass = 2;
	public int shellCellTransparancyClass = 2;
	// ^ Shell ^

	public float armour {
		get {
			if (type == CellTypeEnum.Shell) {
				return ShellCell.GetStrength(shellCellArmorClass);
			} else {
				return 1f;
			}
		}
	}

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

	// Dendrites....
	public GeneLogicBox dendritesLogicBox = new GeneLogicBox(SignalUnitEnum.DendritesLogicBox);
	// ^ Dendrites ^

	// Energy Sensor...
	public GeneEnergySensor energySensor = new GeneEnergySensor(SignalUnitEnum.EnergySensor);
	// Energy Sensor

	// Origin...
	public int originPulsePeriodTicks = 80;
	public GeneLogicBox originDetatchLogicBox = new GeneLogicBox(SignalUnitEnum.OriginDetatchLogicBox);

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

	// Build order
	public float buildPriorityBias;
	// ^ Build Order ^

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

		// ...egg...
		// Force gateLayer0 to And, lock it so that it cant be changed by apply (= load)
		eggCellFertilizeLogic.TryCreateGate(0, LogicOperatorEnum.And, 0, GeneLogicBox.rightmostFlank, true);
		eggCellFertilizeLogic.ConnectAllInputInputTo(SignalUnitEnum.ConstantSensor, SignalUnitSlotEnum.A); // constant 0
		eggCellFertilizeLogic.SetAllInputToBlocked();
		eggCellFertilizeLogic.ConnectInputTo(0, SignalUnitEnum.WorkSensorA, SignalUnitSlotEnum.A); // connect to on board energy sensor
		eggCellFertilizeLogic.SetInputToPass(0);
		eggCellFertilizeLogic.SetInputToLocked(0);
		eggCellFertilizeLogic.SetCellToLocked(1, 0);
		eggCellFertilizeLogic.SetCellToLocked(2, 0);
		eggCellFertilizeLogic.UpdateConnections();
		// ^ egg ^

		// ...dendrites...
		dendritesLogicBox.TryCreateGate(0, LogicOperatorEnum.Or, 0, GeneLogicBox.rightmostFlank, false);
		dendritesLogicBox.ConnectAllInputInputTo(SignalUnitEnum.ConstantSensor, SignalUnitSlotEnum.A); // constant 0
		dendritesLogicBox.SetAllInputToBlocked();
		dendritesLogicBox.UpdateConnections();
		// ^ dendrites ^

		// ...origing...
		originDetatchLogicBox.TryCreateGate(0, LogicOperatorEnum.Or, 0, GeneLogicBox.rightmostFlank, false);
		originDetatchLogicBox.ConnectAllInputInputTo(SignalUnitEnum.ConstantSensor, SignalUnitSlotEnum.A); // constant 0
		originDetatchLogicBox.SetAllInputToBlocked();
		originDetatchLogicBox.UpdateConnections();
		// ^ origin ^
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
			ScrambleMetabolism();
		}

		// Egg
		float spread = 0.06f; // TODO move toGlobal settings
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
			eggCellHibernateWhenAttachedToMother = !eggCellHibernateWhenAttachedToMother; //toggle
		}
		mut = Random.Range(0, gs.mutation.cellIdleWhenAttachedLeave + gs.mutation.cellIdleWhenAttachedChange * strength);
		if (mut < gs.mutation.cellIdleWhenAttachedChange * strength) {
			eggCellHibernateWhenAttachedToChild = !eggCellHibernateWhenAttachedToChild; //toggle
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
			jawCellHibernateWhenAttachedToMother = !jawCellHibernateWhenAttachedToMother; //toggle
		}
		mut = Random.Range(0, gs.mutation.cellIdleWhenAttachedLeave + gs.mutation.cellIdleWhenAttachedChange * strength);
		if (mut < gs.mutation.cellIdleWhenAttachedChange * strength) {
			jawCellHibernateWhenAttachedToChild = !jawCellHibernateWhenAttachedToChild; //toggle
		}
		// ^ Jaw ^

		// Leaf
		mut = Random.Range(0, gs.mutation.cellIdleWhenAttachedLeave + gs.mutation.cellIdleWhenAttachedChange * strength);
		if (mut < gs.mutation.cellIdleWhenAttachedChange * strength) {
			leafCellHibernateWhenAttachedToMother = !leafCellHibernateWhenAttachedToMother; //toggle
		}
		mut = Random.Range(0, gs.mutation.cellIdleWhenAttachedLeave + gs.mutation.cellIdleWhenAttachedChange * strength);
		if (mut < gs.mutation.cellIdleWhenAttachedChange * strength) {
			leafCellHibernateWhenAttachedToChild = !leafCellHibernateWhenAttachedToChild; //toggle
		}
		// ^ Leaf ^ 

		// Muscle
		mut = Random.Range(0, gs.mutation.cellIdleWhenAttachedLeave + gs.mutation.cellIdleWhenAttachedChange * strength);
		if (mut < gs.mutation.cellIdleWhenAttachedChange * strength) {
			muscleCellHibernateWhenAttachedToMother = !muscleCellHibernateWhenAttachedToMother; //toggle
		}
		mut = Random.Range(0, gs.mutation.cellIdleWhenAttachedLeave + gs.mutation.cellIdleWhenAttachedChange * strength);
		if (mut < gs.mutation.cellIdleWhenAttachedChange * strength) {
			muscleCellHibernateWhenAttachedToChild = !muscleCellHibernateWhenAttachedToChild; //toggle
		}
		// ^ Muscle ^

		// Shell
		mut = Random.Range(0, gs.mutation.shellCellArmorClassLeave + gs.mutation.shellCellArmorClassChange * strength);
		if (mut < gs.mutation.shellCellArmorClassChange * strength) {
			shellCellArmorClass = Mathf.Clamp(shellCellArmorClass - 2 + Random.Range(0, 5), 0, ShellCell.armourClassCount - 1);
		}
		mut = Random.Range(0, gs.mutation.shellCellTransparancyClassLeave + gs.mutation.shellCellTransparancyClassChange * strength);
		if (mut < gs.mutation.shellCellTransparancyClassChange * strength) {
			shellCellTransparancyClass = Mathf.Clamp(shellCellTransparancyClass - 2 + Random.Range(0, 5), 0, ShellCell.transparencyClassCount - 1);
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

		// Origin..
		spread = 40;
		mut = Random.Range(0, gs.mutation.OriginPulseFrequenzyLeave + gs.mutation.OriginPulseFrequenzyRandom * strength);
		if (mut < gs.mutation.OriginPulseFrequenzyRandom * strength) {
			originPulsePeriodTicks = (int)Mathf.Clamp(originPulsePeriodTicks - spread + Random.Range(0, spread) + Random.Range(0, spread), 1f / (Time.fixedDeltaTime * GlobalSettings.instance.phenotype.originPulseFrequenzyMax), 1f / (Time.fixedDeltaTime * GlobalSettings.instance.phenotype.originPulseFrequenzyMin));
		}
		// ^ Origin ^

		// Build priority
		spread = 5;
		mut = Random.Range(0, gs.mutation.buildPriorityBiasLeave + gs.mutation.buildPriorityBiasRandom * strength);
		if (mut < gs.mutation.buildPriorityBiasRandom * strength) {
			buildPriorityBias = Mathf.Clamp(buildPriorityBias - spread + Random.Range(0, spread) + Random.Range(0, spread), GlobalSettings.instance.phenotype.buildPriorityBiasMin, GlobalSettings.instance.phenotype.buildPriorityBiasMax);
			buildPriorityBias = Mathf.Round(buildPriorityBias * 10f) / 10f; // Round to closest tenth
		}

		// ^ Build Priority ^

		//arrangements
		arrangements[0].Mutate(strength);
		arrangements[1].Mutate(strength);
		arrangements[2].Mutate(strength);
	}

	public void ScrambleArrangements() {
		type = (CellTypeEnum)Random.Range(0, 8);
		arrangements[0].Scramble();
		arrangements[1].Scramble();
		arrangements[2].Scramble();
	}

	private void ScrambleMetabolism() {
		if (type == CellTypeEnum.Shell) {
			if (Random.Range(0, 3) == 0) {
				shellCellArmorClass = Random.Range(0, ShellCell.armourClassCount);
				shellCellTransparancyClass = Random.Range(0, ShellCell.transparencyClassCount);
			}
		}
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
		geneData.eggCellDetatchMode = eggCellDetatchMode;
		geneData.eggCellDetatchSizeThreshold = eggCellDetatchSizeThreshold;
		geneData.eggCellDetatchEnergyThreshold = eggCellDetatchEnergyThreshold;
		geneData.eggCellHibernateWhenAttachedToMother = eggCellHibernateWhenAttachedToMother;
		geneData.eggCellHibernateWhenAttachedToChild = eggCellHibernateWhenAttachedToChild;
		geneData.eggFertilizeLogicBoxData = eggCellFertilizeLogic.UpdateData();
		geneData.eggFertilizeEnergySensorData = eggCellFertilizeEnergySensor.UpdateData();

		// Jaw
		geneData.jawCellCannibalizeKin =      jawCellCannibalizeKin;
		geneData.jawCellCannibalizeMother =   jawCellCannibalizeMother;
		geneData.jawCellCannibalizeFather =   jawCellCannibalizeFather;
		geneData.jawCellCannibalizeSiblings = jawCellCannibalizeSiblings;
		geneData.jawCellCannibalizeChildren = jawCellCannibalizeChildren;
		geneData.jawCellHibernateWhenAttachedToMother = jawCellHibernateWhenAttachedToMother;
		geneData.jawCellHibernateWhenAttachedToChild = jawCellHibernateWhenAttachedToChild;

		// Leaf
		geneData.leafCellHibernateWhenAttachedToMother = leafCellHibernateWhenAttachedToMother;
		geneData.leafCellHibernateWhenAttachedToChild = leafCellHibernateWhenAttachedToChild;

		// Muscle
		geneData.muscleCellHibernateWhenAttachedToMother = muscleCellHibernateWhenAttachedToMother;
		geneData.muscleCellHibernateWhenAttachedToChild = muscleCellHibernateWhenAttachedToChild;

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

		// Dendrites
		geneData.dendritesLogicBoxData = dendritesLogicBox.UpdateData();

		//Sensors
		geneData.energySensorData = energySensor.UpdateData();

		// Origin
		geneData.originPulsePeriodTicks =     originPulsePeriodTicks;
		geneData.originDetatchLogicBoxData = originDetatchLogicBox.UpdateData();

		//build order
		geneData.buildPriorityBias = buildPriorityBias;

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
		eggCellHibernateWhenAttachedToMother = geneData.eggCellHibernateWhenAttachedToMother;
		eggCellHibernateWhenAttachedToChild = geneData.eggCellHibernateWhenAttachedToChild;
		eggCellFertilizeLogic.ApplyData(geneData.eggFertilizeLogicBoxData); // An operator for gate atl level 0 might be set here, though it is overridden in constructor
		eggCellFertilizeEnergySensor.ApplyData(geneData.eggFertilizeEnergySensorData);

		// Jaw
		jawCellCannibalizeKin =      geneData.jawCellCannibalizeKin;
		jawCellCannibalizeMother =   geneData.jawCellCannibalizeMother;
		jawCellCannibalizeFather =   geneData.jawCellCannibalizeFather;
		jawCellCannibalizeSiblings = geneData.jawCellCannibalizeSiblings;
		jawCellCannibalizeChildren = geneData.jawCellCannibalizeChildren;
		jawCellHibernateWhenAttachedToMother = geneData.jawCellHibernateWhenAttachedToMother;
		jawCellHibernateWhenAttachedToChild = geneData.jawCellHibernateWhenAttachedToChild;

		// Leaf
		leafCellHibernateWhenAttachedToMother = geneData.leafCellHibernateWhenAttachedToMother;
		leafCellHibernateWhenAttachedToChild = geneData.leafCellHibernateWhenAttachedToChild;

		// Muscle
		muscleCellHibernateWhenAttachedToMother = geneData.muscleCellHibernateWhenAttachedToMother;
		muscleCellHibernateWhenAttachedToChild = geneData.muscleCellHibernateWhenAttachedToChild;

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

		// Dendrites
		dendritesLogicBox.ApplyData(geneData.dendritesLogicBoxData);

		// Sensors
		energySensor.ApplyData(geneData.energySensorData);

		// Origin
		originPulsePeriodTicks = geneData.originPulsePeriodTicks == 0 ? 80 : geneData.originPulsePeriodTicks;
		originDetatchLogicBox.ApplyData(geneData.originDetatchLogicBoxData);

		// Build order
		buildPriorityBias = geneData.buildPriorityBias;

		// 3 Arrangements
		arrangements[0].ApplyData(geneData.arrangementData[0]);
		arrangements[1].ApplyData(geneData.arrangementData[1]);
		arrangements[2].ApplyData(geneData.arrangementData[2]);
	}
}