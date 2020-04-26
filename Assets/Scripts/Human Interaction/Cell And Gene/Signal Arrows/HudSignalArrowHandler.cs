using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudSignalArrowHandler : MonoBehaviour {
	
	public HudSignalArrowPool hudSignalArrowPool;

	private List<HudSignalArrow> arrowList = new List<HudSignalArrow>();

	[HideInInspector]
	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;
	private CellAndGenePanel cellAndGenePanel;

	protected bool ignoreSliderMoved = false;

	protected PhenoGenoEnum GetMode() {
		return mode;
	}

	public virtual void Initialize(PhenoGenoEnum mode, CellAndGenePanel cellAndGenePanel) {
		this.mode = mode;
		this.cellAndGenePanel = cellAndGenePanel;
	}

	private bool isDirtyConnections = true;
	public void MakeDirtyConnections() {
		isDirtyConnections = true;
		MakeDirtySignal();
	}

	private bool isDirtySignal = false;
	public void MakeDirtySignal() {
		isDirtySignal = true;
	}

	private void Update() {
		// Update connections
		if (isDirtyConnections) {
			if ((mode == PhenoGenoEnum.Phenotype && selectedCell == null) || (mode == PhenoGenoEnum.Genotype && selectedGene == null)) {
				// no menu
				isDirtyConnections = false;
				return;
			}

			List<IGeneInput> geneInputList = cellAndGenePanel.GetAllGeneInputs();

			foreach (HudSignalArrow arrow in arrowList) {
				hudSignalArrowPool.Recycle(arrow);
			}
			arrowList.Clear();

			foreach (IGeneInput geneInput in geneInputList) {
				if (geneInput.nerve.inputUnit != SignalUnitEnum.Void) {
					HudSignalArrow arrow = hudSignalArrowPool.Borrow();
					arrow.gameObject.SetActive(true);
					
					arrow.inputUnit = geneInput.nerve.inputUnit;
					arrow.inputUnitSlot = geneInput.nerve.inputUnitSlot;
					arrow.outputUnit = geneInput.nerve.outputUnit;
					arrow.outputUnitSlot = geneInput.nerve.outputUnitSlot;

					Vector2 head = cellAndGenePanel.TotalPanelOffset(geneInput.nerve.outputUnit, geneInput.nerve.outputUnitSlot);

					Vector2 tail;
					if (geneInput.nerve.nerveVector == null || geneInput.nerve.nerveVector == Vector2i.zero) { // Local 
						tail = cellAndGenePanel.TotalPanelOffset(geneInput.nerve.inputUnit, geneInput.nerve.inputUnitSlot);
					} else { // External
						tail = head + Vector2.down * 20f;
					}

					arrow.GetComponent<RectTransform>().localPosition = (head + tail) / 2f;
					arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(Vector2.Distance(head, tail), 10f);
					arrow.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(head.y - tail.y, head.x - tail.x));
					arrowList.Add(arrow);
				}
			}
			isDirtyConnections = false;
		}

		// Update signal TODO: update only when dirty, that is post signal update in creature
		if (isDirtySignal) {
			foreach (HudSignalArrow arrow in arrowList) {
				Color color = Color.black;
				if (mode == PhenoGenoEnum.Phenotype && selectedCell != null) {
					color = selectedCell.GetOutputFromUnit(arrow.inputUnit, arrow.inputUnitSlot) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
				} else {
					color = ColorScheme.instance.signalOff;
				}
				arrow.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.5f);
			}
			isDirtySignal = false;
		}
	}

	public Cell selectedCell {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return cellAndGenePanel.cell;
			} else {
				return null; // there could be many cells selected for the same gene
			}
		}
	}

	public Gene selectedGene {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return cellAndGenePanel.cell != null ? cellAndGenePanel.gene : null;
			} else {
				return cellAndGenePanel.gene;
			}
		}
	}
}
