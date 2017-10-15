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


	// DEBUG
	public bool printoutAtDirtyMarkedUpdate = true;
}