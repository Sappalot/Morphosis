using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudSignalArrowHandler : MonoBehaviour {
	public CellAndGenePanel cellAndGenePanel;
	public HudSignalArrowPool hudSignalArrowPool;

	private List<HudSignalArrow> arrowList = new List<HudSignalArrow>();

	[HideInInspector]
	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;
	protected bool ignoreSliderMoved = false;

	protected PhenoGenoEnum GetMode() {
		return mode;
	}

	public virtual void Initialize(PhenoGenoEnum mode) {
		this.mode = mode;
	}

	private static bool isDirtyConnections = false;
	public static void MakeDirtyConnections() {
		isDirtyConnections = true;
	}

	private void Update() {

		// Update connections
		if (isDirtyConnections) {
			List<GeneLogicBoxInput> geneLogicBoxInputList = cellAndGenePanel.GetAllGeneLogicBoxInputs();

			foreach (HudSignalArrow arrow in arrowList) {
				hudSignalArrowPool.Recycle(arrow);
			}
			arrowList.Clear();

			foreach (GeneLogicBoxInput geneLogicBoxInput in geneLogicBoxInputList) {
				if (geneLogicBoxInput.nerve.inputUnit != SignalUnitEnum.Void && geneLogicBoxInput.valveMode == SignalValveModeEnum.Pass) {
					HudSignalArrow arrow = hudSignalArrowPool.Borrow();
					arrow.gameObject.SetActive(true);
					
					arrow.inputUnit = geneLogicBoxInput.nerve.inputUnit;
					arrow.inputUnitSlot = geneLogicBoxInput.nerve.inputUnitSlot;
					arrow.outputUnit = geneLogicBoxInput.nerve.outputUnit;
					arrow.outputUnitSlot = geneLogicBoxInput.nerve.outputUnitSlot;

					Vector2 head = cellAndGenePanel.TotalPanelOffset(geneLogicBoxInput.nerve.outputUnit, geneLogicBoxInput.nerve.outputUnitSlot);
					Vector2 tail = cellAndGenePanel.TotalPanelOffset(geneLogicBoxInput.nerve.inputUnit, geneLogicBoxInput.nerve.inputUnitSlot);
					arrow.GetComponent<RectTransform>().localPosition = (head + tail) / 2f;
					arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(Vector2.Distance(head, tail), 10f);
					arrow.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(head.y - tail.y, head.x - tail.x));
					arrowList.Add(arrow);
				}
			}
			isDirtyConnections = false;
		}

		// Update signal

		foreach (HudSignalArrow arrow in arrowList) {
			Color color = Color.black;
			if (mode == PhenoGenoEnum.Phenotype && selectedCell != null) {
				color = selectedCell.GetOutputFromUnit(arrow.inputUnit, arrow.inputUnitSlot) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
			} else {
				color = ColorScheme.instance.signalOff;
			}
			arrow.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.5f);
		}

	}

	public Cell selectedCell {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return CellPanel.instance.selectedCell;
			} else {
				return null; // there could be many cells selected for the same gene
			}
		}
	}

}
