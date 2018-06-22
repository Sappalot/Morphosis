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

		public float eggCellCanFertilizeWhenAttachedLeave = 1000f;
		public float eggCellCanFertilizeWhenAttachedChange = 10f;

		public float eggCellDetatchModeLeave = 1000f;
		public float eggCellDetatchModeChange = 5f;

		public float eggCellDetatchSizeThresholdLeave = 1000f;
		public float eggCellDetatchSizeThresholdRandom = 10f;

		public float eggCellDetatchEnergyThresholdLeave = 1000f;
		public float eggCellDetatchEnergyThresholdRandom = 10f;

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
											   //           eggCellEffect                        0.0 W
		public float eggCellFertilizeThresholdMin = 30f; //J

		//Fungal Cell
		public float fungalCellEffectCost = 0f; // W
		public float fungalCellEffect = 0.1f; // W

		//Jaw Cell
		public float jawCellEffectCost = 0.2f; //W
		public AnimationCurve jawCellEatEffectAtSpeed; //W stolen from pray depending on ram speed
		public int jawCellEatLinger = 5; // jawCellEatLinger * jawCellTickPeriod is the time after jaw cell has left pray, that it will still chew on it
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

		//Muscle Cell
		public float muscleCellEffectCost = 0.4f; //W
												  //           muscleCellEffect                     0.0 W

		//Root cell
		public float rootCellEffectCost = 0.5f; //W
		public float rootCellEarthMaxEffect = 2.0f; //W

		//Shell Cell
		public float shellCellEffectCost = 0.1f; //W
												 //           shellCellEffect =                    0.0 W
		public float shellCellWeaknessFactor = 0.02f; // 1=> as easy as other cells, 0 => inpossible to eat

		//Vein Cell
		public float veinCellEffectCost = 0.1f; //W
												//           veinCellEffect                       0.0 W

		//Veins
		public float veinFluxEffectWeak = 0.05f; //W
		public float veinFluxEffectMedium = 0.25f; //W
		public float veinFluxEffectStrong = 0.5f; //W

		//General
		public float cellBuildCost = 10f; //Energy Cost to build cell, J (Newly built cell will have this energy at start of its life)
		public float cellBuildNeededRadius = 0.35f; //m
		public float cellBuildMaxDistance = 1.5f; //m // All neighbours contributing to a new build must be closer than this distance, or new cell can't be built

													// Rebuild
		public float cellRebuildCooldown = 40f; //s, before a cell which was killed/deleted can be rebuilt

		//Detatch
		public float detatchmentKick = 0.05f; //N
		public float detatchmentKickSquare = 0.05f; //N
		public float detatchSlideDuration = 10; // s
		public int detatchAfterCompletePersistance = 5; // How many times will we retry to find a spot to grow next cell in before we give up and realize that it is time to detatch (1 ==> give up (and detatch) after failing one time)

		//Teleport
		public float telepokeImpulseStrength = 1f;

		//Sterile
		public float maxAgeAsChildless = 3600; //s
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

		public int cellEnergyTickPeriod = 5;
		public int veinTickPeriod = 5;
		public int growTickPeriod = 30; // Detatch attempt has same period as grow

		public int killSterileCreaturesTickPeriod = 300;

		// Terrain
		public int portalTeleportPeriod = 10;
		public int escapistCleanupPeriod = 10;

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