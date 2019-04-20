using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoSingleton<GlobalSettings> {

	//[Serializable]
	//public class Mutation {
	//	public float mutationStrength = 1f;

	//}

	[Serializable]
	public class Mutation {
		public float masterMutationStrength = 1f;

		public float cellTypeLeave = 100f;
		public float cellTypeRandom = 1f;

		public float eggCellFertilizeThresholdLeave = 100f;
		public float eggCellFertilizeThresholdRandom = 10f;

		public float eggCellDetatchModeLeave = 1000f;
		public float eggCellDetatchModeChange = 10f;

		public float eggCellDetatchSizeThresholdLeave = 1000f;
		public float eggCellDetatchSizeThresholdRandom = 10f;

		public float eggCellDetatchEnergyThresholdLeave = 1000f;
		public float eggCellDetatchEnergyThresholdRandom = 10f;

		public float jawCellCannibalizeKinLeave = 1000f;
		public float jawCellCannibalizeKinChange = 10f;

		public float jawCellCannibalizeMotherLeave = 1000f;
		public float jawCellCannibalizeMotherChange = 10f;

		public float jawCellCannibalizeFatherLeave = 1000f;
		public float jawCellCannibalizeFatherChange = 10f;

		public float jawCellCannibalizeSiblingsLeave = 1000f;
		public float jawCellCannibalizeSiblingsChange = 10f;

		public float jawCellCannibalizeChildrenLeave = 1000f;
		public float jawCellCannibalizeChildrenChange = 10f;

		public float shellCellArmorClassLeave = 1000f;
		public float shellCellArmorClassChange = 10f;

		public float shellCellTransparancyClassLeave = 1000f;
		public float shellCellTransparancyClassChange = 10f;

		// General cell
		public float cellIdleWhenAttachedLeave = 1000f;
		public float cellIdleWhenAttachedChange = 10f;
		// ^ General Cell ^

		public float axonEnabledLeave = 1000f;
		public float axonEnabledChange = 10f;

		public float axonFromOriginOffsetLeave = 1000f;
		public float axonFromOriginOffsetChange = 50f;

		public float axonIsFromOriginPlus180Leave = 1000f;
		public float axonIsFromOriginPlus180Change = 10f;

		public float axonFromMeOffsetLeave = 1000f;
		public float axonFromMeOffsetChange = 10f;

		public float axonRelaxContractLeave = 1000f;
		public float axonRelaxContractChange = 10f;

		public float axonIsReverseLeave = 1000f;
		public float axonIsReverseChange = 10f;

		public float OriginPulseFrequenzyLeave = 1000f;
		public float OriginPulseFrequenzyRandom = 10f;

		public float isEnabledLeave = 100f;
		public float isEnabledToggle = 1f;

		public float referenceLeave = 100f;
		public float referenceRandom = 1f;

		public float flipTypeSameOppositeLeave = 100f;
		public float flipTypeSameOppositeToggle = 1f;

		public float flipTypeBlackWhiteToArrowLeave = 100f;
		public float flipTypeBlackWhiteToArrowToggle = 1f;

		public float isflipPairsEnabledLeave = 100f;
		public float isflipPairsEnabledToggle = 1f;

		public float typeLeave = 100f;
		public float typeChange = 1f;

		public float referenceCountLeave = 100f;
		public float referenceCountDecrease1 = 3f;
		public float referenceCountDecrease2 = 2f;
		public float referenceCountDecrease3 = 1f;
		public float referenceCountIncrease1 = 3f;
		public float referenceCountIncrease2 = 2f;
		public float referenceCountIncrease3 = 1f;

		public float arrowIndexLeave = 100f;
		public float arrowIndexDecrease1 = 3f;
		public float arrowIndexDecrease2 = 2f;
		public float arrowIndexDecrease3 = 1f;
		public float arrowIndexIncrease1 = 3f;
		public float arrowIndexIncrease2 = 2f;
		public float arrowIndexIncrease3 = 1f;

		public float gapLeave = 100f;
		public float gapDecrease1 = 3f;
		public float gapDecrease2 = 2f;
		public float gapDecrease3 = 1f;
		public float gapIncrease1 = 3f;
		public float gapIncrease2 = 2f;
		public float gapIncrease3 = 1f;

		public float referenceSideLeave = 100f;
		public float referenceSideToggle = 1f;
	}
	public Mutation mutation;

	//Metabolism
	[Serializable]
	public class Phenotype {
		//Egg Cell
		public float eggCellEffectCost = 0.2f; //W

		public float eggCellFertilizeThresholdMin = 0.3f; //cell energy fullness J/J
		public float eggCellFertilizeThresholdMax = 0.99f; //cell energy fullness J/J

		public float eggCellDetatchSizeThresholdMin = 0.01f;   //creature completeness count/count
		public float eggCellDetatchSizeThresholdMax = 1f; //creature completeness count/count

		public float eggCellDetatchEnergyThresholdMin = 0f;   //cell energy fullness J/J
		public float eggCellDetatchEnergyThresholdMax = 1.1f; //cell energy fullness J/J

		//Fungal Cell
		public float fungalCellEffectCost = 0f; // W
		public float fungalCellStrengthFactor = 0.25f; // 1=> as easy as other cells, 0.5 => weak, 20 ==> strong

		//Jaw Cell
		public float jawCellEffectCost = 0.2f; //W
		public AnimationCurve jawCellEatEffectAtSpeed; //W stolen from pray depending on ram speed
		public float jawCellEatEffect = 50f; //W raw eat effect, damaging pray. Only jawCellEatEffect * jawCellEatEffect will gain predator
		public float jawCellEatEarnFactor = 0.5f; // how big part of the total jawCellEatEffect that is gaining jaw cell J/J
		
		public float jawCellMutualEatKindness = 0.2f; // How much we gain from eating others creature jaw (compared to normal cells, which are 1)
													 // The other creature are loosing more than i gain, and vice versa ==> Both are losing, energy is being lost (when fighting) 
													 // A factor of 1 means 2 jaw cells are not affecting each other, zero sum

		//Leaf Cell
		public float leafCellEffectCost = 1.0f; //W
		public float leafCellSunMaxEffect = 4.0f; //W
		public AnimationCurve leafCellSunEffectFactorAtBodySize; //leafCellEffectCost and leafCellSunMaxEffect will be multiplied by this value
		public float leafCellSunMaxRange = 25.0f; //m
		public AnimationCurve leafCellSunLossFactorOwnCell;// Effect lost (W / m) own body penetrated : at phenotype size
		public AnimationCurve leafCellSunLossFactorOtherCell; // Effect lost (W/ m) others body penetrated : at phenotype size
		public float leafCellDefaultExposure = 0.33f; 

		//Muscle Cell
		public float muscleCellEffectCostPerHz = 0.4f; //W
												  //           muscleCellEffect                     0.0 W

		//Root cell
		public float rootCellEffectCost = 0.5f; //W
		public float rootCellEarthMaxEffect = 2.0f; //W

		//Shell Cell
		public float shellCellEffectCostDeprecated = 0.1f; //W 
		public AnimationCurve shellCellEffectCostAtArmor;
		public AnimationCurve shellCellEffectCostMultiplierAtTransparancy; //cheeper the more transparent (when armor class constant) beacuse blocking leaf more
		public AnimationCurve shellCellStrengthAtArmor;
		public AnimationCurve shellCellArmorAtNormalizedArmorClass; // sets how the values (strength & cost) are distributed over the buttons (along x-axis)
		

		//           shellCellEffect =                    0.0 W
		public float shellCellStrengthFactorDeprecated = 20f; // 1=> as easy as other cells, 0.5 => weak, 20 ==> strong

		//Vein Cell
		public float veinCellEffectCost = 0.1f; //W
												//           veinCellEffect                       0.0 W

		//Origin
		public float originPulseFrequenzyMin = 0.05f;
		public float originPulseFrequenzyMax = 2f;

		//Veins
		public float veinFluxEffectWeak = 0.05f; //W
		public float veinFluxEffectMedium = 0.25f; //W
		public float veinFluxEffectStrong = 0.5f; //W

		//General
		public float cellIdleEffectCost = 0.05f;
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
		public float detatchSlideDurationTicksRandomDiff = 2; // s
		public int detatchAfterCompletePersistance = 5; // How many times will we retry to find a spot to grow next cell in before we give up and realize that it is time to detatch (1 ==> give up (and detatch) after failing one time)

		// General -> Drag
		public float normalDrag = 0.15f;
		public float normalShellDrag = 0.15f;
		public float normalLeafDrag = 0.15f;
		public float slideDrag = 0.05f;

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
		public int muscleCellTickPeriod =   5;
		public int rootCellTickPeriod =    50;
		public int shellCellTickPeriod =   50;
		public int veinCellTickPeriod =    50;

		public int veinTickPeriod = 5;
		public int growTickPeriod = 30; // Detatch attempt has same period as grow

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
	public float orthoMinStrongFX = 10f;
	public float orthoMaxHorizonFx = 30f;
	public float orthoMaxHorizonDetailedCell = 50f;

	// DEBUG
	public bool printoutAtDirtyMarkedUpdate = true;
}