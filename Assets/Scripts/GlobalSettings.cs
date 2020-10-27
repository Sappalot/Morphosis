using System;
using UnityEngine;

public class GlobalSettings : MonoSingleton<GlobalSettings> {

	public GUISkin popupSkin;

	//To avoid warning "Unable to find style 'MiniToolbarPopup' in skin 'LightSkin' Layout"
	private void OnGUI() {
		GUI.skin = popupSkin;
	}

	[Serializable]
	public class Debug {
		public bool debugLogViaEditor = true;
		public bool debugLogViaBuild = true;
		public bool debugLogMenuUpdate = false;
	}
	public Debug debug;

	[Serializable]
	public class Mutation {
		public float masterMutationStrength = 1f;

		public float cellTypeChange = 10f;

		// ...Egg...
		// TODO: child direction
		// ^ Egg ^

		// ...Jaw...
		public float jawCellCannibalizeKinChange = 10f;
		public float jawCellCannibalizeMotherChange = 10f;
		public float jawCellCannibalizeFatherChange = 10f;
		public float jawCellCannibalizeSiblingsChange = 10f;
		public float jawCellCannibalizeChildrenChange = 10f;
		// ^ Jaw ^

		// ...Shell...
		public float shellCellArmorClassChange = 10f;
		public float shellCellTransparancyClassChange = 10f;
		// ^ Shell ^

		public float axonEnabledToggle = 10f;

		public float axonFromOriginOffsetChange = 50f;
		public float axonFromOriginOffsetChangeMaxAmount = 45f;

		public float axonIsFromOriginPlus180Toggle = 10f;

		public float axonFromMeOffsetChange = 10f;
		public float axonFromMeOffsetChangeMaxAmount = 45f;

		public float axonRelaxContractChange = 10f;
		public float axonRelaxContractChangeMaxAmount = 2.5f;

		public float axonIsReverseToggle = 10f;

		public float originEmbryoMaxSizeCompletenessChange = 50f;
		public float originEmbryoMaxSizeCompletenessChangeMaxAmount = 0.2f; // % of full size 

		public float originGrowPriorityCellPersistenceChange = 10f;
		public float originPersistToGrowBlockedPriorityCellPatienseChangeMaxAmount = 30f; //s

		public float originPulseTickPeriodChange = 10f;
		public float originPulseTickPeriodChangeMaxAmount = 40f;

		public float buildPriorityBiasChange = 50f;
		public float buildPriorityBiasChangeMaxAmount = 4f;

		// nerve
		public float nerveSlotChange = 40f;
		public float nerveUnitAndSlotChange = 10f;

		// logic box (works the same regardles if inside work panel or not)
		public float logicBoxInputValveToggle = 10f; // chance per valve
		public float logicBoxGateExtendFlank = 1f; // extend flank one step left/right. At mutation: for each logicBox -> for each gate -> for each flank -> for each direction... chances are this big that it will be extended 
		public float logicBoxGateRemoveAdd = 5f; // remove presant gate. add a new one size 2 where there is room
		public float logicBoxGateToggleLogicOperation = 5f;

		// sensor (works the same regardles if inside work panel or not)
		public float energySensorThresholdChange = 50f;
		public float energySensorThresholdChangeMaxAmount = 30f; // +/-
		public float energySensorAreaRadiusChange = 50f;
		public float energySensorAreaRadiusChangeMaxAmount = 3f;

		public float effectSensorMeassureChange = 10f;
		public float effectSensorThresholdChange = 50f;
		public float effectSensorThresholdChangeMaxAmount = 2f; // +/-
		public float effectSensorThresholdMax = 20f;// +/-
		public float effectSensorAreaRadiusChange = 50f;
		public float effectSensorAreaRadiusChangeMaxAmount = 3f;

		public float sizeSensorSizeThresholdChange = 10f;
		public float sizeSensorSizeThresholdChangeMaxAmount = 0.2f; // % of body size
		public float sizeSensorCantGrowMorePatienseChange = 50f;
		public float originGrowPriorityCellPersistenceMaxAmount = 30; // s

		// arrangement
		public float isEnabledToggle = 10f;
		public float referenceChange = 10f;
		public float flipTypeSameOppositeToggle = 10f;
		public float flipTypeBlackWhiteToArrowToggle = 10f;
		public float isflipPairsEnabledToggle = 10f;
		public float typeChange = 10f;

		public float referenceCountDecrease1 = 30f;
		public float referenceCountDecrease2 = 20f;
		public float referenceCountDecrease3 = 10f;
		public float referenceCountIncrease1 = 30f;
		public float referenceCountIncrease2 = 20f;
		public float referenceCountIncrease3 = 10f;

		public float arrowIndexDecrease1 = 30f;
		public float arrowIndexDecrease2 = 20f;
		public float arrowIndexDecrease3 = 10f;
		public float arrowIndexIncrease1 = 30f;
		public float arrowIndexIncrease2 = 20f;
		public float arrowIndexIncrease3 = 10f;

		public float gapDecrease1 = 30f;
		public float gapDecrease2 = 20f;
		public float gapDecrease3 = 10f;
		public float gapIncrease1 = 30f;
		public float gapIncrease2 = 20f;
		public float gapIncrease3 = 10f;

		public float referenceSideToggle = 10f;

		public AnimationCurve randomDistributionCurve;
		// leaves a value in between [-1 ... 1] most likely close to 0
		public float RandomDistributedValue() {
			return randomDistributionCurve.Evaluate(UnityEngine.Random.Range(0f, 1f));
		}

	}
	public Mutation mutation;

	//Metabolism
	[Serializable]
	public class Phenotype {

		[Serializable]
		public class EggCell {
			[Tooltip("The cost of running this cell [W]")]
			public float effectProductionDown = 0.15f;
		}
		public EggCell eggCell;

		[Serializable]
		public class FungalCell {
			[Tooltip("The cost of running this cell [W]")]
			public float effectProductionDown = 0.15f;
		}
		[Tooltip("Up for grabs")]
		public FungalCell fungalCell;

		[Serializable]
		public class JawCell {
			[Tooltip("The cost of running this cell [W]")]
			public float effectProductionDown = 0.1f;

			[Tooltip("The jaws' eating effect [W] depending on my speed [m/s] towards pray cell. Pray cell is losing the same amount, which will show up under External effect")]
			public AnimationCurve effectProductionUpAtSpeed;

			[Tooltip("How big part of effectProductionUp (the eating effect) that is gaining the cell [W/W] (the rest is wasted into thin air)")]
			public float effectProductionUpKeepFactor = 0.85f;
		}
		public JawCell jawCell;

		[Serializable]
		public class LeafCell {
			[Tooltip("The cost of running this cell [W]")]
			public float effectProductionDown = 5f;

			[Tooltip("Ths leafs' fotosyntesis effect which is gaining the cell [W], proportional to the exposure of it.'")]
			public float effectProductionUpMax = 12f;

			public AnimationCurve exposureFactorAtBodySize;

			[Tooltip("Leaf exposure will be multiplied by a factor depending on number of cells in creatures' body. Few cells => low factor, so that algae can't clog up simulation")]
			public AnimationCurve exposureFactorAtPopulation;

			public AnimationCurve exposureFactorAtSpeed;

			public float exposurePenalty = 0.05f;

			[Tooltip("A sun beam hitting cell from this far [m] can contribute to its exposure. Or... Cell is searching this far [m] for open space.")]
			public float sunRayMaxRange = 35.0f;

			[Tooltip("Was 8. When ray is traveling away from cell it loses this much potential effect (effectProductionUpMax) for each meter it is penetrating own cells, measured in [W/m]. Other cells connected via cluster counts as own cells. This value affects the exposure, which in turn affects the productionUpEffect")]
			public float sunRayEffectLossPerDistanceThroughOwnCell = 8f;

			[Tooltip("Was 18. When ray is traveling away from cell it loses this much potential effect (effectProductionUpMax) for each meter it is penetrating other cells, measured in [W/m]. Other cells are all cells that are not connected to this one via cluser. This value affects the exposure, which in turn affects the productionUpEffect")]
			public float sunRayEffectLossPerDistanceThroughOtherCell = 18f;
		}
		public LeafCell leafCell;

		[Serializable]
		public class MuscleCell {
			[Tooltip("The cost of running this cell [W] This cost is there regardles if we are contracting or not")]
			public float effectProductionDown = 0.05f;

			[Tooltip("How much energy [J] we pay for each contraction. From this value an effect cost is calculated which is applied  together with the effectProductionDown when we contract the cell")]
			public float energyProductionDownPerContraction = 0.05f;
		}
		public MuscleCell muscleCell;

		[Serializable]
		public class RootCell {
			[Tooltip("The cost of running this cell [W]")]
			public float effectProductionDown = 0.1f;
		}
		public RootCell rootCell;

		[Serializable]
		public class ShellCell {
			[Tooltip("The cost of running this cell [W]")]
			public float effectProductionDown = 0.1f;
			public float armour = 10f;
			public float armourAffectFactorOnNeighbour = 0.5f;
			public float transparency = 0f;
		}
		public ShellCell shellCell;

		//Shell Cell
		//public AnimationCurve shellCellEffectCostAtArmor;
		//public AnimationCurve shellCellEffectCostMultiplierAtTransparancy; //cheeper the more transparent (when armor class constant) beacuse blocking leaf more
		//public AnimationCurve shellCellStrengthAtArmor;

		[Serializable]
		public class VeinCell {
			[Tooltip("The cost of running this cell [W]")]
			public float effectProductionDown = 0.1f;

		}
		public VeinCell veinCell;

		// Fin
		[Range(0f, 1f)]
		public float finForceLinearFactor = 0.3f;

		[Range(0f, 5f)]
		public float finForceSquareFactor = 0.45f;

		[Range(0f, 100f)]
		public float finForceMax = 1f;

		[Range(0f, 1f)]
		public float nonFinForceFactor = 0.5f;

		//Origin
		public float originPulseFrequenzyMin = 0.05f; //TODO change to period
		public float originPulseFrequenzyMax = 2f; //TODO change to period

		// Build Priority Bias
		public float buildPriorityBiasMin = -10f;
		public float buildPriorityBiasMax = 10f;

		//Veins
		public float veinFluxEffectWeak = 0.05f; //W
		public float veinFluxEffectMedium = 0.25f; //W
		public float veinFluxEffectStrong = 0.5f; //W

		//General
		public int creatureHexagonMaxRadius = 16; // used to limit blueprint. R = 16 ==> we can have can have 16 cells north of origin at most origin = 0, 1 = neighbour, .... cell 16 = perifery 
		public int creatureMaxCellCount = 40;
		public float cellMaxEnergy = 100f; // J
		public float cellDefaultEnergy = 0.33f;
		public bool reclaimCutBranchEnergy = true; //when a branch is detatched, its energy will be reclaimed and distributed among cells in creature

		//General -> build
		public float cellBuildCost = 10f; //Energy Cost to build cell, J (Newly built cell will have this energy at start of its life)
		public float cellNewlyBuiltKeepFactor = 1f; //How much of the energy spent on cell that will fill up new cell built
		public float cellBuildNeededRadius = 0.35f; //m | a new cell can be built only if there is an empty spot with this radius or more, at the build location (walls are excluded but should be included)
		public float cellBuildMaxDistance = 1.5f; //m | All neighbours contributing to a new build must be closer than this distance, or new cell can't be built
		public float cellRebuildCooldown = 40f; //s, before a cell which was killed/deleted can be rebuilt

		//General -> Detatch
		public AnimationCurve detatchmentKickAtCellCount;
		public float detatchSlideDurationTicks = 10; // s
		public float detatchSlideDurationTicksRandomDiff = 2; // s to make child locomotion come out of sync with mother

		// General -> Friction (aka drag)
		public float frictionUnderNormal = 0f;
		public float frictionUnderNormal1Cell = 0.3f; // used to cancel rotation on twin
		public float frictionUnderNormal2Cells = 0.1f; // used to slow down single cell

		public float frictionUnderNormalLeaf = 0.5f;
		public float frictionUnderNormal1CellLeaf = 0.8f; // used to cancel rotation on twin
		public float frictionUnderNormal2CellsLeaf = 0.6f; // used to slow down single cell

		public float frictionUnderSlidingFactor = 0f;


		public float springFrequenzy = 5;
		public float springFrequenzyMuscleCell = 20;

		// General -> Teleport
		public float telepokeImpulseStrength = 1f; // N / teleport tick | impulse applied every teleport tick

		//Sterile
		public ulong maxAge = 3600; //s

		//Springs
		public float springBreakingForce = 100f; // N
		public float springBreakingForceMuscle = 100f; // N
	}
	public Phenotype phenotype;

	[Serializable]
	public class Quality {
		// Life
		public int eggCellTickPeriod = 50;
		public int fungalCellTickPeriod = 50;
		public int jawCellTickPeriod = 50;
		public int leafCellTickPeriod = 20;
		public int muscleCellTickPeriod = 5; // also used for veins
		public int rootCellTickPeriod = 50;
		public int shellCellTickPeriod = 50;
		public int veinCellTickPeriod = 50;

		public AnimationCurve growTickPeriodAtSize; //has been stable at 80 (regardless of size) for many years,  Detatch attempt has same period as grow

		public int signalTickPeriod = 20;

		public int killOldCreaturesTickPeriod = 6000;

		// Terrain
		public int portalTeleportTickPeriod = 40;
		public int escapistCleanupTickPeriod = 1200;
		public int pidTickPeriod = 20;

		//Panels
		public int phenotypePanelTickPeriod = 10;
	}
	public Quality quality;

	[Serializable]
	public class Pooling {
		public bool creature = true;
		public bool cell = true;
		public bool geneCell = true;
		public bool vein = true;
		public bool edge = true;
		public bool effects = true;
		public bool nerveArrow = true;
	}

	public Pooling pooling;

	//Visual
	//public float orthoMinStrongFX = 10f;
	//public float orthoMaxHorizonFx = 30f;
	public float detailedGraphicsOrthoLimit = 50f;
	//public float loudAudioOrthoLimit = 50f;
	public AnimationCurve loudAudioVolumeAtOrtho;
	public AnimationCurve quietAudioVolumeAtOrtho;

}