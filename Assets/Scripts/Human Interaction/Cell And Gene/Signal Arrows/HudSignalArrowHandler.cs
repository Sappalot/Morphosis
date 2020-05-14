using System.Collections.Generic;
using System.Linq;
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

			foreach (HudSignalArrow arrow in arrowList) {
				hudSignalArrowPool.Recycle(arrow);
			}
			arrowList.Clear();

			Dictionary<Cell,List<Nerve>> cellNervesDictionary = new Dictionary<Cell, List<Nerve>>();
			
			if (mode == PhenoGenoEnum.Genotype) {
				List<Cell> geneCells = CreatureSelectionPanel.instance.soloSelected.genotype.GetGeneCellsWithGene(selectedGene);

				// merge gene cells
				foreach (Cell geneCell in geneCells) {
					cellNervesDictionary.Add(geneCell, geneCell.GetAllNervesGenotype(false));
				}

			} else {
				// phenotype
				// TODO: make phenotype work as well

				isDirtyConnections = false;
				return;
			}

		

			if (cellNervesDictionary == null || cellNervesDictionary.Values.Count == 0) {
				// nothing to draw.... done here... bye
				isDirtyConnections = false;
				return;
			}

			// inputs
			foreach (Nerve nerve in cellNervesDictionary.Values.ToArray()[0]) {
				//When it comes to input, they all look the same so we just pich the first one, which we are sure to have

				
				if (nerve.nerveStatusEnum == NerveStatusEnum.Input_GenotypeLocal ||
					nerve.nerveStatusEnum == NerveStatusEnum.Input_GenotypeExternal ||
					nerve.nerveStatusEnum == NerveStatusEnum.Input_GenotypeExternalVoid) {

					HudSignalArrow arrow = GetArrowLikeNerve(nerve);

					Vector2 head = cellAndGenePanel.TotalPanelOffset(nerve.headSignalUnitEnum, nerve.headSignalUnitSlotEnum);
					Vector2 tail;
					if (nerve.nerveStatusEnum == NerveStatusEnum.Input_GenotypeLocal) {
						// Local input
						tail = cellAndGenePanel.TotalPanelOffset(nerve.tailSignalUnitEnum, nerve.tailSignalUnitSlotEnum);
					} else {
						// External input (short arrow, pointing up, with head at input )
						tail = head + Vector2.down * 20f;
					}

					SetArrowTransforms(arrow, head, tail);
					arrowList.Add(arrow);
				}
			}

			// outputs
			// merge list so that we have max one external nerve with tail at output
			List<Nerve> mergedList = new List<Nerve>(); // only output_external
			foreach (KeyValuePair<Cell, List<Nerve>> pair in cellNervesDictionary.ToList()) {
				foreach (Nerve nerve in pair.Value) {
					if (nerve.nerveStatusEnum == NerveStatusEnum.Output_GenotypeExternal) {
						if (mergedList.Find(n => n.tailSignalUnitEnum == nerve.tailSignalUnitEnum && n.tailSignalUnitSlotEnum == nerve.tailSignalUnitSlotEnum) == null) {
							// max one of each
							mergedList.Add(nerve);
						}
					}
				}
			}

			foreach (Nerve nerve in mergedList) {
				if (nerve.nerveStatusEnum == NerveStatusEnum.Output_GenotypeExternal) {
					// no point to draw local arrow inputs as they are allready drawn (with the tail here from another input)

					HudSignalArrow arrow = GetArrowLikeNerve(nerve);

					Vector2 tail = cellAndGenePanel.TotalPanelOffset(nerve.tailSignalUnitEnum, nerve.tailSignalUnitSlotEnum);
					// external output
					Vector2 head = tail + Vector2.up * 20f;

					SetArrowTransforms(arrow, head, tail);
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
					color = selectedCell.GetOutputFromUnit(arrow.tailUnit, arrow.tailUnitSlot) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
				} else {
					color = ColorScheme.instance.signalOff;
				}
				arrow.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.5f);
			}
			isDirtySignal = false;
		}
	}

	private HudSignalArrow GetArrowLikeNerve( Nerve nerve) {
		HudSignalArrow arrow = hudSignalArrowPool.Borrow();
		arrow.gameObject.SetActive(true);

		arrow.headUnit = nerve.headSignalUnitEnum;
		arrow.headUnitSlot = nerve.headSignalUnitSlotEnum;
		arrow.tailUnit = nerve.tailSignalUnitEnum;
		arrow.tailUnitSlot = nerve.tailSignalUnitSlotEnum;

		return arrow;
	}

	private void SetArrowTransforms(HudSignalArrow arrow, Vector2 head, Vector2 tail) {
		arrow.GetComponent<RectTransform>().localPosition = (head + tail) / 2f;
		arrow.GetComponent<RectTransform>().sizeDelta = new Vector2(Vector2.Distance(head, tail), 10f);
		arrow.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(head.y - tail.y, head.x - tail.x));
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
