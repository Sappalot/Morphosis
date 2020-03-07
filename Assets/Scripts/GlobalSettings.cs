using System;
using UnityEngine;

public class GlobalSettings : MonoSingleton<GlobalSettings> {

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
		public float logicBoxInputValveToggle = 10f;

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
		//Egg Cell
		public float eggCellEffectCost = 0.2f; //W

		//Fungal Cell
		public float fungalCellEffectCost = 0f; // W
		public float fungalCellStrengthFactor = 0.25f; // 1=> as easy as other cells, 0.5 => weak, 20 ==> strong

		//Jaw Cell
		public float jawCellEffectCost = 0.2f; //W
		//public float jawCellEatEffect = 20f; //W raw eat effect, damaging pray. Only jawCellEatEffect * jawCellEatEffect will gain predator
		public AnimationCurve jawCellEatEffectAtSpeed; // What is my eating effect depending on my speed towards pray cell
		public float jawCellEatEarnFactor = 0.9f; // how big part of the total jawCellEatEffect that is gaining jaw cell J/J (rest is wasted)
		
		//public float jawCellMutualEatKindness = 0.2f; // How much we gain from eating others creature jaw (compared to normal cells, which are 1)
													 // The other creature are loosing more than i gain, and vice versa ==> Both are losing, energy is being lost (when fighting) 
													 // A factor of 1 means 2 jaw cells are not affecting each other, zero sum

		//Leaf Cell
		public float leafCellEffectCost = 1.0f; //W
		public float leafCellSunMaxEffect = 4.0f; //W
		public AnimationCurve leafCellSunEffectFactorAtBodySize; //leafCellEffectCost and leafCellSunMaxEffect will be multiplied by this value
		public AnimationCurve leafCellSunexposureFactorAtPopulation; //exposure will be multiplied by this value

		public float leafCellSunMaxRange = 25.0f; //m

		public float leafCellSunLossFactorOwnCell = 8f;// Effect lost (W / m) own body penetrated
		public float leafCellSunLossFactorOtherCell = 20f; // Effect lost (W/ m) others body penetrated

		public float leafCellDefaultExposure = 0.33f;

		//Muscle Cell
		public float muscleEffectCostRelaxing = 0.05f;
		public float muscleCellEnergyCostPerContraction = 2f; //J
												  //           muscleCellEffect                     0.0 W

		//Root cell
		public float rootCellEffectCost = 0.5f; //W
		public float rootCellEarthMaxEffect = 2.0f; //W

		//Shell Cell
		public AnimationCurve shellCellEffectCostAtArmor;
		public AnimationCurve shellCellEffectCostMultiplierAtTransparancy; //cheeper the more transparent (when armor class constant) beacuse blocking leaf more
		public AnimationCurve shellCellStrengthAtArmor;
		public AnimationCurve shellCellArmorAtNormalizedArmorClass; // sets how the values (strength & cost) are distributed over the buttons (along x-axis)

		//           shellCellEffect =                    0.0 W
		public float shellCellStrengthFactorDeprecated = 20f; // 1=> as easy as other cells, 0.5 => weak, 20 ==> strong

		//Vein Cell
		public float veinCellEffectCost = 0.1f; //W
												//           veinCellEffect                       0.0 W

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
		public float cellHibernateEffectCost = 0.05f;
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
		public int eggCellTickPeriod =     50;
		public int fungalCellTickPeriod =  50;
		public int jawCellTickPeriod =     50;
		public AnimationCurve leafCellTickPeriodAtSpeed; // Period length in ticks (0.1s) at certain speeds
		public int leafCellTickPeriod =    50;
		public int muscleCellTickPeriod =   5; // also used for veins
		public int rootCellTickPeriod =    50;
		public int shellCellTickPeriod =   50;
		public int veinCellTickPeriod =    50;

		public int growTickPeriod = 30; // Detatch attempt has same period as grow

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
		public bool cell =     true;
		public bool geneCell = true;
		public bool vein =     true;
		public bool edge =     true;
		public bool effects =  true;
	}

	public Pooling pooling;

	//Visual
	//public float orthoMinStrongFX = 10f;
	//public float orthoMaxHorizonFx = 30f;
	public float detailedGraphicsOrthoLimit = 50f;
	//public float loudAudioOrthoLimit = 50f;
	public AnimationCurve loudAudioVolumeAtOrtho;
	public AnimationCurve quietAudioVolumeAtOrtho;

	// DEBUG
	public bool printoutAtDirtyMarkedUpdate = true;
}