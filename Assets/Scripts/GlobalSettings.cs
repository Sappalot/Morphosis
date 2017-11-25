public class GlobalSettings : MonoSingleton<GlobalSettings> {

	// MUTATION
	public float mutationStrength = 1f;

	public float cellTypeLeave = 100f;
	public float cellTypeRandom = 1f;

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

	//Veins
	public float weakVeinFluxEffect = 0.05f; //W
	public float mediumVeinFluxEffect = 0.25f; //W
	public float strongVeinFluxEffect = 0.5f; //W

	// Rebuild
	public float cellRebuildCooldown = 30f; //s

	//effects
	public bool playVisualEffects = true;

	//Physics
	public float metabolismPeriod = 5f;

	//Egg Cell
	public float eggCellEffectCost =                  0.2f; //W
	//           eggCellEffect                        0.0 W
	public float eggCellFertilizeThresholdEnergy = 40.0f; //J
	public float eggCellOriginDetatchThresholdEnergy = 45.0f; //J

	//Fungal Cell
	//           fungalCellEffectCost =               0.0 W
	public float fungalCellEffect =                   0.1f;

	//Jaw Cell
	public float jawCellEffectCost =                  0.2f; //W
	public float jawCellEatEffect =                  10.0f; //W
	public float jawCellEatJawCellFactor =            0.2f; // How much we gain from eating others creature jaw (compared to normal cells, which are 1)
															// The other creature are loosing more than i gain, and vice versa ==> Both are losing, energy is being lost (when fighting) 
															// A factor of 1 means 2 jaw cells are not affecting each other, zero sum

	//Leaf Cell
	public float leafCellEffectCost =                 1.0f; //W
	public float leafCellSunMaxEffect =               4.0f; //W
	public float leafCellSunMaxRange =               50.0f; //m
	
	//Muscle Cell
	public float muscleCellEffectCost =               0.4f; //W
	//           muscleCellEffect                     0.0 W

	//Root cell
	public float rootCellEffectCost =                 0.5f; //W
	public float rootCellEarthMaxEffect =             2.0f; //W

	//Shell Cell
	public float shellCellEffectCost =                0.1f; //W
	//           shellCellEffect =                    0.0 W
	public float jawCellEatShellSellFactor =          0.02f; // 1=> as easy as other cells, 0 => inpossible to eat

	//VeinCell
	public float veinCellEffectCost =                 0.1f; //W
	//           veinCellEffect                       0.0 W

	// DEBUG
	public bool printoutAtDirtyMarkedUpdate = true;
}