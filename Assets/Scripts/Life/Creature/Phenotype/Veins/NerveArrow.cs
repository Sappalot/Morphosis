using UnityEngine;

public class NerveArrow : MonoBehaviour {
	public GameObject mainArrow;
	public GameObject headCircle;
	public GameObject tailCircle;


	[HideInInspector]
	public Nerve nerve;

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


	public void Setup(Nerve nerve) {
		this.nerve = nerve;
	}

	public void UpdateGraphics() {


		//Debug.Assert(nerve.nerveStatusEnum == NerveStatusEnum.Output_GenotypeExternal || nerve.nerveStatusEnum == NerveStatusEnum.Input_GenotypeExternal, "Strange nerve found: " + nerve.nerveStatusEnum + ", should be Output_GenotypeExternal or Output_GenotypeLocal");

		mainArrow.SetActive(true);

		if (!isHighlited) {
			mainArrow.GetComponent<LineRenderer>().startColor = ColorUtil.SetAlpha(ColorScheme.instance.signalOff, 0.5f);
			mainArrow.GetComponent<LineRenderer>().endColor = ColorUtil.SetAlpha(ColorScheme.instance.signalOff, 0.5f);
		} else {
			mainArrow.GetComponent<LineRenderer>().startColor = new Color(1f, 0f, 1f, 0.8f);
			mainArrow.GetComponent<LineRenderer>().endColor = new Color(1f, 0f, 1f, 0.8f);
		}

		if (nerve.nerveStatusEnum == NerveStatusEnum.Output_GenotypeExternal) {
			Debug.Assert(nerve.referenceCell != null, "We should allways have a reference cell in the nerve, when it is an external output, who else would be the one listening to nerves voice?");

			Vector3 headPosition = nerve.referenceCell.transform.position + (isHighlited ? 1f : 0f) * Vector3.back;
			mainArrow.GetComponent<LineRenderer>().SetPosition(1, headPosition);  // head = front = start = 1 = narrow

			// Wwe don't use a nerve vector on output
			Vector3 tailPosition = nerve.hostCell.transform.position + (isHighlited ? 1f : 0f) * Vector3.back;
			mainArrow.GetComponent<LineRenderer>().SetPosition(0, tailPosition); // tail = back = end = 0 = slim

			// Circles
			headCircle.SetActive(false);

			tailCircle.SetActive(highlitedEnum == HighliteEnum.highlitedArrowAndCircles);
			if (highlitedEnum == HighliteEnum.highlitedArrowAndCircles) {
				tailCircle.transform.position = tailPosition;
			}
		} else if (nerve.nerveStatusEnum == NerveStatusEnum.Input_GenotypeExternal) {
			if (nerve.tailSignalUnitEnum == SignalUnitEnum.Void) {
				mainArrow.GetComponent<LineRenderer>().startColor = new Color(0f, 1f, 0f, 1f);
				mainArrow.GetComponent<LineRenderer>().endColor = new Color(0f, 1f, 0f, 1f);
			}

			Vector3 headPosition = nerve.hostCell.transform.position + (isHighlited ? 1f : 0f) * Vector3.back;
			mainArrow.GetComponent<LineRenderer>().SetPosition(1, headPosition);  // head = front = start = 1 = narrow

			Vector2i nerveVector = nerve.nerveVector;

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

			Vector3 tailPosition = headPosition + Quaternion.Euler(0, 0, (nerve.hostCell.creature.GetOriginHeading(PhenoGenoEnum.Genotype)) - 90f) * CellMap.ToModelSpacePosition(nerveVector) + (isHighlited ? 1f : 0f) * Vector3.back;
			mainArrow.GetComponent<LineRenderer>().SetPosition(0, tailPosition); // tail = back = end = 0 = slim

			// Circles
			headCircle.SetActive(highlitedEnum == HighliteEnum.highlitedArrowAndCircles);
			if (highlitedEnum == HighliteEnum.highlitedArrowAndCircles) {
				headCircle.transform.position = headPosition;
			}

			tailCircle.SetActive(false);
		}
	}


	public void OnRecycle() {
		mainArrow.gameObject.SetActive(false);
	}
}