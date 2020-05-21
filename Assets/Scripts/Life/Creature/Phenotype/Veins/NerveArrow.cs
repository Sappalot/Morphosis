using System.Collections.Generic;
using UnityEngine;

public class NerveArrow : MonoBehaviour {
	public GameObject mainArrow;

	public Cell geneCell; // could be cell or GeneCell
	public Nerve nerve;

	public bool isHighlited;

	public void Setup(Cell geneCell, Nerve nerve) {
		this.geneCell = geneCell;
		this.nerve = nerve;
	}

	public void UpdateGraphics() {

		mainArrow.SetActive(true);
		//draw main
		mainArrow.GetComponent<LineRenderer>().SetPosition(1, geneCell.transform.position);  // head = front = start = 1 = narrow

		Vector2i nerveVector = nerve.nerveVector;


		// flip vector horizontally only if cell flip side is (white|black) 
		if (geneCell.flipSide == FlipSideEnum.WhiteBlack) {
			nerveVector = CellMap.HexagonalFlip(nerveVector);
		}

		// rotate
		int rootDirection = geneCell.bindCardinalIndex;
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

		mainArrow.GetComponent<LineRenderer>().SetPosition(0, geneCell.transform.position + Quaternion.Euler(0, 0, (geneCell.creature.GetOriginHeading(PhenoGenoEnum.Genotype)) - 90f) * CellMap.ToModelSpacePosition(nerveVector) + (isHighlited ? 1f : 0f) * Vector3.back); // tail = back = end = 0 = slim

		if (nerve.tailSignalUnitEnum == SignalUnitEnum.Void) {
			mainArrow.GetComponent<LineRenderer>().startColor = new Color(0f, 1f, 0f, 0.8f);
			mainArrow.GetComponent<LineRenderer>().endColor = new Color(0f, 1f, 0f, 0.8f);
		} else {
			mainArrow.GetComponent<LineRenderer>().startColor = new Color(ColorScheme.instance.signalOff.r, ColorScheme.instance.signalOff.g, ColorScheme.instance.signalOff.b, 0.5f);
			mainArrow.GetComponent<LineRenderer>().endColor = new Color(ColorScheme.instance.signalOff.r, ColorScheme.instance.signalOff.g, ColorScheme.instance.signalOff.b, 0.5f);
		}

		if (isHighlited) {
			mainArrow.GetComponent<LineRenderer>().startColor = new Color(1f, 0f, 1f, 0.8f);
			mainArrow.GetComponent<LineRenderer>().endColor = new Color(1f, 0f, 1f, 0.8f);
		}

	}

	public void OnRecycle() {
		geneCell = null;

		mainArrow.gameObject.SetActive(false);
	}
}