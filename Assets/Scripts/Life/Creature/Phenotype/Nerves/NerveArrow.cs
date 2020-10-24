using UnityEngine;

public class NerveArrow : MonoBehaviour {
	public GameObject mainArrow;
	public GameObject headCircle;
	public GameObject tailCircle;


	[HideInInspector]
	public Nerve nerve;

	private PhenoGenoEnum phenoGenoMode = PhenoGenoEnum.Genotype;

	public enum HighliteEnum {
		notHighlited,
		highlitedArrow,
		highlitedArrowAndCircles,
	}
	[HideInInspector]
	public HighliteEnum highlitedEnum = HighliteEnum.notHighlited;

	private bool isHighlited {
		get {
			return highlitedEnum == HighliteEnum.highlitedArrow || highlitedEnum == HighliteEnum.highlitedArrowAndCircles;
		}
	}


	public void Setup(PhenoGenoEnum phenoGenoMode, Nerve nerve) {
		this.nerve = nerve;
		this.phenoGenoMode = phenoGenoMode;
	}

	public void UpdateGraphics(bool isGrabbed) {


		//Debug.Assert(nerve.nerveStatusEnum == NerveStatusEnum.Output_GenotypeExternal || nerve.nerveStatusEnum == NerveStatusEnum.Input_GenotypeExternal, "Strange nerve found: " + nerve.nerveStatusEnum + ", should be Output_GenotypeExternal or Output_GenotypeLocal");

		mainArrow.SetActive(true);

		if (!isHighlited) {
			if (nerve.isRootable) {
				SetArrowColor(ColorUtil.SetAlpha(ColorScheme.instance.signalRootable, 0.5f));
			} else if (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Phenotype && nerve.IsOn) {
				SetArrowColor(ColorUtil.SetAlpha(ColorScheme.instance.signalOn, 0.5f));
			} else {
				SetArrowColor(ColorUtil.SetAlpha(ColorScheme.instance.signalOff, 0.5f));
			}
		} else {
			SetArrowColor(ColorUtil.SetAlpha(ColorScheme.instance.signalViewed, 0.8f));
		}

		if (nerve.nerveStatusEnum == NerveStatusEnum.OutputExternal) {
			//Debug.Assert(nerve.nonHostCell != null, "We should allways have a reference cell in the nerve, when it is an external output, who else would be the one listening to nerves voice?");

			Vector3 headPosition;
			Vector3 tailPosition;

			if (nerve.headCell != null) {
				headPosition = nerve.nonHostCell.transform.position + (isHighlited ? 1f : 0f) * Vector3.back;
				// We don't use a nerve vector on output
				tailPosition = nerve.hostCell.transform.position + (isHighlited ? 1f : 0f) * Vector3.back;
			} else {
				// We are missing a cellTo reach out head to and since we are missing it we can't reach out using nerve.vector either

				tailPosition = nerve.tailCell.transform.position + (isHighlited ? 1f : 0f) * Vector3.back;

				Vector2i nerveVector = nerve.toHeadVector;

				// flip vector horizontally only if cell flip side is (white|black) 
				if (nerve.headCellLost.flipSide == FlipSideEnum.WhiteBlack) {
					nerveVector = CellMap.HexagonalFlip(nerveVector);
				}

				// rotate
				int rootDirection = nerve.headCellLost.bindCardinalIndex;
				int turnToCreatureAngle = 0;
				if (rootDirection == 0) { // ne
					turnToCreatureAngle = 5; // +300
				} else if (rootDirection == 1) { // n
					turnToCreatureAngle = 0; // just fine
				} else if (rootDirection == 2) { // nw
					turnToCreatureAngle = 1; // +60
				} else if (rootDirection == 3) { // sw
					turnToCreatureAngle = 2; // +120
				} else if (rootDirection == 4) { // s
					turnToCreatureAngle = 3; // +180
				} else if (rootDirection == 5) { // se
					turnToCreatureAngle = 4; // +240
				}
				nerveVector = CellMap.HexagonalRotate(nerveVector, turnToCreatureAngle);

				// if grabbed we need  to rotate nerve by origin heading since the other headings are not updated
				// Don't know why i had to add this correction: - nerve.headCell.bindCardinalIndex * 60f
				headPosition = tailPosition + Quaternion.Euler(0, 0, (isGrabbed ? nerve.headCellLost.creature.GetOriginHeading(phenoGenoMode) - 90f : nerve.tailCell.heading - 30f - nerve.tailCell.bindCardinalIndex * 60f)) * CellMap.ToModelSpacePosition(nerveVector) + (isHighlited ? 1f : 0f) * Vector3.back;
			}

			mainArrow.GetComponent<LineRenderer>().SetPosition(1, headPosition);  // head = front = start = 1 = narrow
			mainArrow.GetComponent<LineRenderer>().SetPosition(0, tailPosition); // tail = back = end = 0 = slim

			// Circles
			headCircle.SetActive(false);

			tailCircle.SetActive(highlitedEnum == HighliteEnum.highlitedArrowAndCircles);
			if (highlitedEnum == HighliteEnum.highlitedArrowAndCircles) {
				tailCircle.transform.position = tailPosition;
				tailCircle.GetComponent<SpriteRenderer>().color = ColorScheme.instance.signalViewed;
			}
		} else if (nerve.nerveStatusEnum == NerveStatusEnum.InputExternal) {
			if (nerve.tailSignalUnitEnum == SignalUnitEnum.Void) {
				SetArrowColor(ColorUtil.SetAlpha(Color.green, 1f));
			}

			Vector3 headPosition = nerve.hostCell.transform.position + (isHighlited ? 1f : 0f) * Vector3.back;
			Vector3 tailPosition = Vector3.zero;

			if (nerve.tailCell == null) {
				// We are missing tailCell so we are reaching out tail into the void using toTailVector

				Vector2i nerveVector = nerve.toTailVector;

				// flip vector horizontally only if cell flip side is (white|black) 
				if (nerve.hostCell.flipSide == FlipSideEnum.WhiteBlack) {
					nerveVector = CellMap.HexagonalFlip(nerveVector);
				}

				// rotate
				int rootDirection = nerve.hostCell.bindCardinalIndex;
				int turnToCreatureAngle = 0;
				if (rootDirection == 0) { // ne
					turnToCreatureAngle = 5; // +300
				} else if (rootDirection == 1) { // n
					turnToCreatureAngle = 0; // just fine
				} else if (rootDirection == 2) { // nw
					turnToCreatureAngle = 1; // +60
				} else if (rootDirection == 3) { // sw
					turnToCreatureAngle = 2; // +120
				} else if (rootDirection == 4) { // s
					turnToCreatureAngle = 3; // +180
				} else if (rootDirection == 5) { // se
					turnToCreatureAngle = 4; // +240
				}
				nerveVector = CellMap.HexagonalRotate(nerveVector, turnToCreatureAngle);

				// if grabbed we need  to rotate nerve by origin heading since the other headings are not updated
				// Don't know why i had to add this correction: - nerve.headCell.bindCardinalIndex * 60f
				tailPosition = headPosition + Quaternion.Euler(0, 0, isGrabbed ? nerve.headCell.creature.GetOriginHeading(phenoGenoMode) - 90f : nerve.headCell.heading - 30f - nerve.headCell.bindCardinalIndex * 60f) * CellMap.ToModelSpacePosition(nerveVector) + (isHighlited ? 1f : 0f) * Vector3.back;
			} else /* tailCell != null */ {
				tailPosition = nerve.tailCell.transform.position + (isHighlited ? 1f : 0f) * Vector3.back;
			}

			mainArrow.GetComponent<LineRenderer>().SetPosition(1, headPosition);  // head = front = start = 1 = narrow			
			mainArrow.GetComponent<LineRenderer>().SetPosition(0, tailPosition); // tail = back = end = 0 = slim

			// Circles
			headCircle.SetActive(highlitedEnum == HighliteEnum.highlitedArrowAndCircles);
			if (highlitedEnum == HighliteEnum.highlitedArrowAndCircles) {
				headCircle.transform.position = headPosition;
				headCircle.GetComponent<SpriteRenderer>().color = ColorScheme.instance.signalViewed;
			}

			tailCircle.SetActive(false);
		}
	}

	private void SetArrowColor(Color color) {
		mainArrow.GetComponent<LineRenderer>().startColor = color;
		mainArrow.GetComponent<LineRenderer>().endColor = color;
	}

	public void OnRecycle() {
		mainArrow.gameObject.SetActive(false);
	}
}