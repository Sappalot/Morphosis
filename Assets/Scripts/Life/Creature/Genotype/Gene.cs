using System;
using System.Collections.Generic;
using UnityEngine;

public class Gene {
	// Egg cell ...
	public GeneLogicBox eggCellFertilizeLogic;
	public GeneEnergySensor eggCellFertilizeEnergySensor;
	public GeneAttachmentSensor eggCellAttachmentSensor;
	// ^ Egg cell ^

	// Jaw Cell ...
	private bool m_jawCellCannibalizeKin;
	public bool jawCellCannibalizeKin { 
		get {
			return m_jawCellCannibalizeKin;
		}
		set {
			m_jawCellCannibalizeKin = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}

	private bool m_jawCellCannibalizeMother;
	public bool jawCellCannibalizeMother {
		get {
			return m_jawCellCannibalizeMother;
		}
		set {
			m_jawCellCannibalizeMother = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}

	private bool m_jawCellCannibalizeFather;
	public bool jawCellCannibalizeFather {
		get {
			return m_jawCellCannibalizeFather;
		}
		set {
			m_jawCellCannibalizeFather = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}

	private bool m_jawCellCannibalizeSiblings;
	public bool jawCellCannibalizeSiblings {
		get {
			return m_jawCellCannibalizeSiblings;
		}
		set {
			m_jawCellCannibalizeSiblings = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}
	private bool m_jawCellCannibalizeChildren;
	public bool jawCellCannibalizeChildren {
		get {
			return m_jawCellCannibalizeChildren;
		}
		set {
			m_jawCellCannibalizeChildren = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}
	// ^ Jaw Cell ^

	// Leaf Cell
	// ^ Leaf Cell ^



	// Muscle Cell
	// ^ Muscle Cell ^

	// Shell
	//public int shellCellArmorClass;
	//public int shellCellTransparancyClass;
	// ^ Shell ^

	public float transparancy {
		get {
			if (type == CellTypeEnum.Shell) {
				return GlobalSettings.instance.phenotype.shellCell.transparency;
			} else {
				return 0f;
			}
		}
	}

	public GeneConstantSensor constantSenesor;

	public GeneAxon axon;

	// Dendrites....
	public GeneLogicBox dendritesLogicBox;
	// ^ Dendrites ^

	// Sensors...
	public GeneEnergySensor energySensor;
	public GeneEffectSensor effectSensor;
	// ^ sensors ^

	// Origin...
	public GeneLogicBox originDetatchLogicBox;
	public GeneSizeSensor originSizeSensor;
	public int m_originPulseTickPeriod;
	public int originPulseTickPeriod {
		get {
			return m_originPulseTickPeriod;
		}
		set {
			m_originPulseTickPeriod = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}

	public float m_originEmbryoMaxSizeCompleteness;
	public float originEmbryoMaxSizeCompleteness {
		get {
			return m_originEmbryoMaxSizeCompleteness;
		}
		set {
			m_originEmbryoMaxSizeCompleteness = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}

	public int m_originGrowPriorityCellPersistance;
	public int originGrowPriorityCellPersistance {
		get {
			return m_originGrowPriorityCellPersistance;
		}
		set {
			m_originGrowPriorityCellPersistance = value;
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}

	// ^ origin ^

	public float originPulsePeriod {
		get {
			return originPulseTickPeriod * Time.fixedDeltaTime;
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
			genotypeDirtyfy.MakeGeneCellPatternDirty();
		}
	}
	public int index;
	public bool isOrigin {
		get {
			return index == 0;
		}
	}

	public readonly Arrangement[] arrangements = new Arrangement[3];

	public GeneSignalUnit GetGeneSignalUnit(SignalUnitEnum signalUnitType) {
		switch (signalUnitType) {
			case SignalUnitEnum.Void:
				return null;
			case SignalUnitEnum.WorkHibernate: //unused
				return null;
			case SignalUnitEnum.WorkLogicBoxA:
				if (type == CellTypeEnum.Egg) {
					return eggCellFertilizeLogic;
				}
				return null;
			case SignalUnitEnum.WorkLogicBoxB:
				return null;
			case SignalUnitEnum.WorkSensorA:
				if (type == CellTypeEnum.Egg) {
					return eggCellFertilizeEnergySensor;
				}
				return null;
			case SignalUnitEnum.WorkSensorB:
				if (type == CellTypeEnum.Egg) {
					return eggCellAttachmentSensor;
				}
				return null;
			case SignalUnitEnum.WorkSensorC:
				break;
			case SignalUnitEnum.WorkSensorD:
				return null;
			case SignalUnitEnum.ConstantSensor: // aka emitter
				return constantSenesor;
			case SignalUnitEnum.Axon:
				return axon;
			case SignalUnitEnum.FilterChange:
				return null;
			case SignalUnitEnum.FilterTrend:
				return null;
			case SignalUnitEnum.DendritesLogicBox:
				return dendritesLogicBox;
			case SignalUnitEnum.EnergySensor:
				return energySensor;
			case SignalUnitEnum.EffectSensor:
				return effectSensor;
			case SignalUnitEnum.OriginDetatchLogicBox:
				if (isOrigin) {
					return originDetatchLogicBox;
				}
				return null;
			case SignalUnitEnum.OriginSizeSensor:
				return originSizeSensor;
		}
		return null;
	}

	public void PreUpdateInterGeneCell() {
		foreach (SignalUnitEnum type in Enum.GetValues(typeof(SignalUnitEnum))) {
			GeneSignalUnit unit = GetGeneSignalUnit(type);
			if (unit != null) {
				unit.isRooted = false;
			}
		}
	}

	// Assume geneCellMap has been built now
	public void UpdateInterGeneCell(Genotype genotype) {
		foreach (Cell geneCell in genotype.GetGeneCellsWithGene(this)) {
			// need to check all geneCells containing gene same gene but differs in location and heading

			MarkThisAndChildrenAsUsedHelper(genotype, geneCell, SignalUnitEnum.WorkLogicBoxA);
			MarkThisAndChildrenAsUsedHelper(genotype, geneCell, SignalUnitEnum.WorkLogicBoxB);

			GeneSignalUnit unit = GetGeneSignalUnit(SignalUnitEnum.Axon);
			if (unit != null && (unit as GeneAxon).isEnabled) { // axone is root if it is sending pulse to muscles
				unit.MarkThisAndChildrenAsUsed(genotype, geneCell, SignalUnitEnum.WorkLogicBoxB);
			}

			MarkThisAndChildrenAsUsedHelper(genotype, geneCell, SignalUnitEnum.OriginDetatchLogicBox);
		}
	}

	private void MarkThisAndChildrenAsUsedHelper(Genotype genotype, Cell geneCell, SignalUnitEnum signalUnit) {
		GeneSignalUnit unit = GetGeneSignalUnit(signalUnit);
		if (unit != null) {
			unit.MarkThisAndChildrenAsUsed(genotype, geneCell, signalUnit);
		}
	}

	public List<GeneNerve> GetExternalGeneNerves() {
		List<GeneNerve> allGeneNerves = new List<GeneNerve>();
		AddToGeneNerveList(SignalUnitEnum.WorkLogicBoxA, allGeneNerves);
		AddToGeneNerveList(SignalUnitEnum.WorkLogicBoxB, allGeneNerves);
		AddToGeneNerveList(SignalUnitEnum.Axon, allGeneNerves);
		AddToGeneNerveList(SignalUnitEnum.DendritesLogicBox, allGeneNerves);
		AddToGeneNerveList(SignalUnitEnum.OriginDetatchLogicBox, allGeneNerves);
		return allGeneNerves;
	}

	private void AddToGeneNerveList(SignalUnitEnum unitEnum, List<GeneNerve> geneNerves) {
		GeneSignalUnit unit = GetGeneSignalUnit(unitEnum);
		if (unit != null) {
			List<GeneNerve> foundGeneNerves = unit.GetExternalGeneNerves();
			if (foundGeneNerves != null) {
				geneNerves.AddRange(foundGeneNerves);
			}
		}
	}

	private IGenotypeDirtyfy genotypeDirtyfy;

	public Gene(int index, IGenotypeDirtyfy genotypeDirtyfy) {
		this.index = index;
		this.genotypeDirtyfy = genotypeDirtyfy;

		// egg...
		eggCellFertilizeLogic = new GeneLogicBox(SignalUnitEnum.WorkLogicBoxA, this.genotypeDirtyfy);
		eggCellFertilizeEnergySensor = new GeneEnergySensor(SignalUnitEnum.WorkSensorA, this.genotypeDirtyfy);
		eggCellAttachmentSensor = new GeneAttachmentSensor(SignalUnitEnum.WorkSensorB);
		// ^egg^

		constantSenesor = new GeneConstantSensor(SignalUnitEnum.ConstantSensor);

		axon = new GeneAxon(this.genotypeDirtyfy);
		
		dendritesLogicBox = new GeneLogicBox(SignalUnitEnum.DendritesLogicBox, this.genotypeDirtyfy);
		energySensor = new GeneEnergySensor(SignalUnitEnum.EnergySensor, this.genotypeDirtyfy);
		effectSensor = new GeneEffectSensor(SignalUnitEnum.EffectSensor, this.genotypeDirtyfy);

		originDetatchLogicBox = new GeneLogicBox(SignalUnitEnum.OriginDetatchLogicBox, this.genotypeDirtyfy);
		originSizeSensor = new GeneSizeSensor(SignalUnitEnum.OriginSizeSensor, this.genotypeDirtyfy);

		arrangements[0] = new Arrangement(index, this.genotypeDirtyfy);
		arrangements[1] = new Arrangement(index, this.genotypeDirtyfy);
		arrangements[2] = new Arrangement(index, this.genotypeDirtyfy);

		Defaultify();
	}

	public void Defaultify() {
		// Cell neighbours
		arrangements[0].Defaultify(index);
		arrangements[1].Defaultify(index);
		arrangements[2].Defaultify(index);

		// Metabolism
		type = CellTypeEnum.Leaf;

		// Jaw Cell
		jawCellCannibalizeKin = false;
		jawCellCannibalizeMother = false;
		jawCellCannibalizeFather = false;
		jawCellCannibalizeSiblings = false;
		jawCellCannibalizeChildren = false;
		// ^ Jaw Cell ^

		// Shell
		//shellCellArmorClass = 2;
		//shellCellTransparancyClass = 2;
		// ^ Shell ^

		// Build order
		buildPriorityBias = 0;
		// ^ Build Order ^

		// Signal
		// ...egg...
		// Force gateLayer0 to And, lock it so that it cant be changed by apply (= load)
		eggCellFertilizeLogic.Defaultify();
		eggCellFertilizeLogic.TryCreateGate(0, LogicOperatorEnum.And, 0, GeneLogicBox.rightmostFlank, true);
		eggCellFertilizeLogic.ConnectInputTo(0, SignalUnitEnum.WorkSensorA, SignalUnitSlotEnum.outputLateA); // connect to on board energy sensor
		eggCellFertilizeLogic.ConnectInputTo(1, SignalUnitEnum.WorkSensorB, SignalUnitSlotEnum.outputLateB); // connect to on board attachemnt sensor (free from mother)
		eggCellFertilizeLogic.SetInputToPass(0); // energy
		eggCellFertilizeLogic.SetInputToPass(1); // attachment
		eggCellFertilizeLogic.SetInputLockness(0, LocknessEnum.Locked); // energy
		eggCellFertilizeLogic.SetInputLockness(1, LocknessEnum.SemiLocked); // attachment
		eggCellFertilizeLogic.SetCellToLocked(1, 0); // above energy
		eggCellFertilizeLogic.SetCellToLocked(2, 0); // above energy
		eggCellFertilizeLogic.SetCellToLocked(1, 1); // above attachment
		eggCellFertilizeLogic.SetCellToLocked(2, 1); // above attachment
		eggCellFertilizeLogic.UpdateConnections();
		eggCellFertilizeEnergySensor.Defaultify();
		eggCellFertilizeEnergySensor.thresholdMin = 20f;
		eggCellFertilizeEnergySensor.threshold = 25f;
		// attachmentSensor: no need, no settings
		// ^ egg ^

		// ... axon ....
		axon.Defaultify();
		axon.isOrigin = isOrigin; // origin is allways enabled
		// ^ axon ^

		// ...dendrites...
		dendritesLogicBox.Defaultify();
		dendritesLogicBox.TryCreateGate(0, LogicOperatorEnum.Or, 0, GeneLogicBox.rightmostFlank, false);
		dendritesLogicBox.UpdateConnections();
		// ^ dendrites ^

		energySensor.Defaultify();
		effectSensor.Defaultify();

		// ...origing...
		originDetatchLogicBox.Defaultify();
		originDetatchLogicBox.TryCreateGate(0, LogicOperatorEnum.And, 0, GeneLogicBox.rightmostFlank, false);
		originDetatchLogicBox.TryCreateGate(2, LogicOperatorEnum.Or, 4, GeneLogicBox.rightmostFlank, true);
		originDetatchLogicBox.ConnectInputTo(4, SignalUnitEnum.OriginSizeSensor, SignalUnitSlotEnum.outputLateE);
		originDetatchLogicBox.ConnectInputTo(5, SignalUnitEnum.OriginSizeSensor, SignalUnitSlotEnum.outputLateF);
		originDetatchLogicBox.SetInputToPass(4); // blocked
		originDetatchLogicBox.SetInputToPass(5); // max size
		originDetatchLogicBox.SetInputLockness(4, LocknessEnum.SemiLocked); // blocked
		originDetatchLogicBox.SetInputLockness(5, LocknessEnum.SemiLocked); // max size
		originDetatchLogicBox.UpdateConnections();
		
		originSizeSensor.Defaultify();
	
		originPulseTickPeriod = 80;
		originEmbryoMaxSizeCompleteness = 0.5f;
		originGrowPriorityCellPersistance = 20; //secounds
	}

	public void Randomize() {
		type = (CellTypeEnum)UnityEngine.Random.Range(0, 8);
		arrangements[0].Randomize();
		arrangements[1].Randomize();
		arrangements[2].Randomize();

		eggCellFertilizeLogic.Randomize();
		eggCellFertilizeEnergySensor.Randomize();
		
		axon.Randomize();
		axon.isOrigin = isOrigin;

		dendritesLogicBox.Randomize();
		originDetatchLogicBox.Randomize();
	}

	public void Mutate(float strength) {
		GlobalSettings gs = GlobalSettings.instance;
		float mut = UnityEngine.Random.Range(0, 1000f + gs.mutation.cellTypeChange * strength);
		if (mut < gs.mutation.cellTypeChange * strength) {
			type = (CellTypeEnum)UnityEngine.Random.Range(0, 8);
			//ScrambleMetabolism(); // not really a good idea to allways do this, todo make it occur occationaly
		}

		// Egg...
		eggCellFertilizeLogic.Mutate(strength, isOrigin);
		eggCellFertilizeEnergySensor.Mutate(strength);
		// ^ Egg ^

		// Jaw...
		mut = UnityEngine.Random.Range(0, 1000f + gs.mutation.jawCellCannibalizeKinChange * strength);
		if (mut < gs.mutation.jawCellCannibalizeKinChange * strength) {
			jawCellCannibalizeKin = !jawCellCannibalizeKin;
		}

		mut = UnityEngine.Random.Range(0, 1000f + gs.mutation.jawCellCannibalizeMotherChange * strength);
		if (mut < gs.mutation.jawCellCannibalizeMotherChange * strength) {
			jawCellCannibalizeMother = !jawCellCannibalizeMother;
		}

		mut = UnityEngine.Random.Range(0, 1000f + gs.mutation.jawCellCannibalizeFatherChange * strength);
		if (mut < gs.mutation.jawCellCannibalizeFatherChange * strength) {
			jawCellCannibalizeFather = !jawCellCannibalizeFather;
		}

		mut = UnityEngine.Random.Range(0, 1000f + gs.mutation.jawCellCannibalizeSiblingsChange * strength);
		if (mut < gs.mutation.jawCellCannibalizeSiblingsChange * strength) {
			jawCellCannibalizeSiblings = !jawCellCannibalizeSiblings;
		}

		mut = UnityEngine.Random.Range(0, 1000f + gs.mutation.jawCellCannibalizeChildrenChange * strength);
		if (mut < gs.mutation.jawCellCannibalizeChildrenChange * strength) {
			jawCellCannibalizeChildren = !jawCellCannibalizeChildren;
		}
		// ^ Jaw ^

		// Shell...
		//mut = UnityEngine.Random.Range(0, 1000f + gs.mutation.shellCellArmorClassChange * strength);
		//if (mut < gs.mutation.shellCellArmorClassChange * strength) {
		//	shellCellArmorClass = Mathf.Clamp(shellCellArmorClass - 2 + UnityEngine.Random.Range(0, 5), 0, ShellCell.armourClassCount - 1);
		//}
		//mut = UnityEngine.Random.Range(0, 1000f + gs.mutation.shellCellTransparancyClassChange * strength);
		//if (mut < gs.mutation.shellCellTransparancyClassChange * strength) {
		//	shellCellTransparancyClass = Mathf.Clamp(shellCellTransparancyClass - 2 + UnityEngine.Random.Range(0, 5), 0, ShellCell.transparencyClassCount - 1);
		//}
		// ^ Shell ^

		// Axone
		axon.Mutate(strength, isOrigin);

		// Dendrites
		dendritesLogicBox.Mutate(strength, isOrigin);

		// Sensors
		energySensor.Mutate(strength);
		effectSensor.Mutate(strength);

		// Origin..
		originDetatchLogicBox.Mutate(strength, isOrigin);
		originSizeSensor.Mutate(strength);

		mut = UnityEngine.Random.Range(0, 1000f + gs.mutation.originEmbryoMaxSizeCompletenessChange * strength);
		if (mut < gs.mutation.originEmbryoMaxSizeCompletenessChange * strength) {
			originEmbryoMaxSizeCompleteness = Mathf.Clamp(originEmbryoMaxSizeCompleteness + gs.mutation.originEmbryoMaxSizeCompletenessChangeMaxAmount * gs.mutation.RandomDistributedValue(), 0f, 1f);
		}

		mut = UnityEngine.Random.Range(0, gs.mutation.originGrowPriorityCellPersistenceChange * strength + 1000f);
		if (mut < gs.mutation.originGrowPriorityCellPersistenceChange * strength) {
			originGrowPriorityCellPersistance = (int)Mathf.Clamp(originGrowPriorityCellPersistance + gs.mutation.originGrowPriorityCellPersistenceMaxAmount * gs.mutation.RandomDistributedValue(), 0f, 120f);
		}

		mut = UnityEngine.Random.Range(0, 1000f + gs.mutation.originPulseTickPeriodChange * strength);
		if (mut < gs.mutation.originPulseTickPeriodChange * strength) {
			originPulseTickPeriod = (int)Mathf.Clamp(originPulseTickPeriod + gs.mutation.originPulseTickPeriodChangeMaxAmount * gs.mutation.RandomDistributedValue(), 1f / (Time.fixedDeltaTime * GlobalSettings.instance.phenotype.originPulseFrequenzyMax), 1f / (Time.fixedDeltaTime * GlobalSettings.instance.phenotype.originPulseFrequenzyMin));
		}
		// ^ Origin ^

		// Build priority
		mut = UnityEngine.Random.Range(0, 1000f + gs.mutation.buildPriorityBiasChange * strength);
		if (mut < gs.mutation.buildPriorityBiasChange * strength) {
			buildPriorityBias = Mathf.Clamp(buildPriorityBias + gs.mutation.buildPriorityBiasChangeMaxAmount * gs.mutation.RandomDistributedValue(), GlobalSettings.instance.phenotype.buildPriorityBiasMin, GlobalSettings.instance.phenotype.buildPriorityBiasMax);
			buildPriorityBias = Mathf.Round(buildPriorityBias * 10f) / 10f; // Round to closest tenth
		}
		// ^ Build Priority ^

		//arrangements
		arrangements[0].Mutate(strength);
		arrangements[1].Mutate(strength);
		arrangements[2].Mutate(strength);
	}

	public void SetReferenceGeneFromReferenceGeneIndex(Gene[] genes) {
		arrangements[0].SetReferenceGeneFromReferenceGeneIndex(genes);
		arrangements[1].SetReferenceGeneFromReferenceGeneIndex(genes);
		arrangements[2].SetReferenceGeneFromReferenceGeneIndex(genes);
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

	public Gene GetClone(IGenotypeDirtyfy genotypeDirtyfy) {
		Gene clone = new Gene(index, genotypeDirtyfy);
		clone.ApplyData(UpdateData());
		return clone;
	}

	// Save
	private GeneData geneData = new GeneData();
	public GeneData UpdateData() {
		geneData.type = type;
		geneData.index = index;

		// Egg
		geneData.eggFertilizeLogicBoxData = eggCellFertilizeLogic.UpdateData();
		geneData.eggFertilizeEnergySensorData = eggCellFertilizeEnergySensor.UpdateData();

		// Jaw
		geneData.jawCellCannibalizeKin =      jawCellCannibalizeKin;
		geneData.jawCellCannibalizeMother =   jawCellCannibalizeMother;
		geneData.jawCellCannibalizeFather =   jawCellCannibalizeFather;
		geneData.jawCellCannibalizeSiblings = jawCellCannibalizeSiblings;
		geneData.jawCellCannibalizeChildren = jawCellCannibalizeChildren;

		// Leaf

		// Muscle

		// Shell
		//geneData.shellCellArmourClass = shellCellArmorClass;
		//geneData.shellCellTransparancyClass = shellCellTransparancyClass;

		// Axon
		geneData.geneAxoneData = axon.UpdateData();

		// Dendrites
		geneData.dendritesLogicBoxData = dendritesLogicBox.UpdateData();

		// Sensors
		geneData.energySensorData = energySensor.UpdateData();
		geneData.effectSensorData = effectSensor.UpdateData();

		// Origin
		geneData.originPulsePeriodTicks =     originPulseTickPeriod;
		geneData.originDetatchLogicBoxData = originDetatchLogicBox.UpdateData();
		geneData.originSizeSensorData = originSizeSensor.UpdateData();
		geneData.embryoMaxSizeCompleteness = originEmbryoMaxSizeCompleteness;
		geneData.growPriorityCellPersistance = originGrowPriorityCellPersistance;

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
		eggCellFertilizeLogic.ApplyData(geneData.eggFertilizeLogicBoxData); // An operator for gate atl level 0 might be set here, though it is overridden in constructor
		eggCellFertilizeEnergySensor.ApplyData(geneData.eggFertilizeEnergySensorData);

		// Jaw
		jawCellCannibalizeKin =      geneData.jawCellCannibalizeKin;
		jawCellCannibalizeMother =   geneData.jawCellCannibalizeMother;
		jawCellCannibalizeFather =   geneData.jawCellCannibalizeFather;
		jawCellCannibalizeSiblings = geneData.jawCellCannibalizeSiblings;
		jawCellCannibalizeChildren = geneData.jawCellCannibalizeChildren;

		// Leaf

		// Muscle

		// Shell
		//shellCellArmorClass = geneData.shellCellArmourClass;
		//shellCellTransparancyClass = geneData.shellCellTransparancyClass;

		// Axon
		axon.ApplyData(geneData.geneAxoneData);

		// Dendrites
		dendritesLogicBox.ApplyData(geneData.dendritesLogicBoxData);

		// Sensors
		energySensor.ApplyData(geneData.energySensorData);
		effectSensor.ApplyData(geneData.effectSensorData);

		// Origin
		originPulseTickPeriod = geneData.originPulsePeriodTicks == 0 ? 80 : geneData.originPulsePeriodTicks;
		originDetatchLogicBox.ApplyData(geneData.originDetatchLogicBoxData);
		originSizeSensor.ApplyData(geneData.originSizeSensorData);
		originEmbryoMaxSizeCompleteness = geneData.embryoMaxSizeCompleteness;
		originGrowPriorityCellPersistance = geneData.growPriorityCellPersistance;

		// Build order
		buildPriorityBias = geneData.buildPriorityBias;

		// 3 Arrangements
		arrangements[0].ApplyData(geneData.arrangementData[0]);
		arrangements[1].ApplyData(geneData.arrangementData[1]);
		arrangements[2].ApplyData(geneData.arrangementData[2]);
	}
}