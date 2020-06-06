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

		List<Nerve> uniqueNerves = null;
		List<Nerve> nervesToHighlite = null;
		bool isNervesHighliteAllMode = false;
		if (isDirtyConnections || isDirtySignal) {
			if (mode == PhenoGenoEnum.Genotype) {
				Genotype genotype = CreatureSelectionPanel.instance.soloSelected.genotype;
				uniqueNerves = genotype.GetAllUniqueNervesGenotype(selectedGene);
				nervesToHighlite = GetNervesToHighliteGenotype(genotype, selectedGene);
				isNervesHighliteAllMode = IsNervesHighliteAllModeGenotype();
			} else /*Phenotype*/{
				uniqueNerves = selectedCell.GetAllNervesGenotypePhenotype();
				nervesToHighlite = GetNervesToHighlitePhenotype(selectedCell);
				isNervesHighliteAllMode = IsNervesHighliteAllModePhenotype();
			}
		}


		if (isDirtyConnections) {
			if (mode == PhenoGenoEnum.Genotype && CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.genotype.isInterGeneCellDirty) {
				Debug.Log("Ooooops, not ready to refresh yet");
				return; // Ooooops, not ready to refresh yet
			}

			if ((mode == PhenoGenoEnum.Phenotype && selectedCell == null) || (mode == PhenoGenoEnum.Genotype && selectedGene == null)) {
				// no menu
				isDirtyConnections = false;
				return;
			}

			//Debug.Log("Updating Hud Signal Arrow Handler");

			foreach (HudSignalArrow arrow in arrowList) {
				hudSignalArrowPool.Recycle(arrow);
			}
			arrowList.Clear();

			if (uniqueNerves == null || uniqueNerves.Count == 0) {
				// nothing to draw.... done here... bye
				isDirtyConnections = false;
				return;
			}

			// inputs
			foreach (Nerve nerve in uniqueNerves) {
				//When it comes to input, they all look the same so we just pick the first one, which we are sure to have

				if (nerve.nerveStatusEnum == NerveStatusEnum.OutputLocal ||
					nerve.nerveStatusEnum == NerveStatusEnum.OutputExternal) {

					// We draw internal output arrows (from their tail side), though they have been drawn (from their head side) allready
					// The reason for this is so that we can highlite them from the tail side as well

					HudSignalArrow arrow = GetArrowLikeNerve(nerve);

					Vector2 tail = cellAndGenePanel.TotalPanelOffset(nerve.tailSignalUnitEnum, nerve.tailSignalUnitSlotEnum);
					// external output
					Vector2 head;

					if (nerve.nerveStatusEnum == NerveStatusEnum.OutputLocal) {
						// Local input
						head = cellAndGenePanel.TotalPanelOffset(nerve.headSignalUnitEnum, nerve.headSignalUnitSlotEnum);
					} else {
						// External input (short arrow, pointing up, with head at input )
						head = tail + Vector2.up * 20f;
					}

					SetArrowTransforms(arrow, head, tail);
					arrow.GetComponent<Image>().color = ColorScheme.instance.signalOff;
					ShowTextAtArrowHead(arrow, false);
					ShowTextAtArrowTail(arrow, false);

					// Highlite
					if (nervesToHighlite != null) {
						if (nervesToHighlite.Find(n => Nerve.AreTwinNerves(n, nerve, true)) != null) {
							HighliteArrow(arrow);
							if (!isNervesHighliteAllMode) {
								ShowTextAtArrowHead(arrow, true);
								ShowTextAtArrowTail(arrow, false);
							} else {
								ShowTextAtArrowHead(arrow, false);
								ShowTextAtArrowTail(arrow, false);
							}
						}
					}

					arrowList.Add(arrow);
				} else if (nerve.nerveStatusEnum == NerveStatusEnum.InputLocal ||
					nerve.nerveStatusEnum == NerveStatusEnum.InputExternal) {

					HudSignalArrow arrow = GetArrowLikeNerve(nerve);

					Vector2 head = cellAndGenePanel.TotalPanelOffset(nerve.headSignalUnitEnum, nerve.headSignalUnitSlotEnum);
					Vector2 tail;

					arrow.GetComponent<Image>().color = ColorScheme.instance.signalOff;

					if (nerve.tailSignalUnitEnum == SignalUnitEnum.Void) {
						arrow.GetComponent<Image>().color = ColorScheme.instance.signalOff;
						tail = head + Vector2.down * 20f;

					} else if (nerve.nerveStatusEnum == NerveStatusEnum.InputLocal) {
						// Local input
						tail = cellAndGenePanel.TotalPanelOffset(nerve.tailSignalUnitEnum, nerve.tailSignalUnitSlotEnum);
					} else {
						// External input (short arrow, pointing up, with head at input )
						tail = head + Vector2.down * 20f;
					}

					SetArrowTransforms(arrow, head, tail);
					ShowTextAtArrowHead(arrow, false);
					ShowTextAtArrowTail(arrow, false);

					// Highlite
					if (nervesToHighlite != null) {
						if (nervesToHighlite.Find(n => Nerve.AreTwinNerves(n, nerve, true)) != null) {
							HighliteArrow(arrow);
							if (!isNervesHighliteAllMode) {
								ShowTextAtArrowHead(arrow, false);
								ShowTextAtArrowTail(arrow, true);
							} else {
								ShowTextAtArrowHead(arrow, false);
								ShowTextAtArrowTail(arrow, false);
							}
						}
					}

					arrowList.Add(arrow);
				}
			}
			isDirtyConnections = false;
		}

		// Update signal TODO: update only when dirty, that is post signal update in creature
		if (isDirtySignal && nervesToHighlite == null) {
			foreach (HudSignalArrow arrow in arrowList) {
				Color color = Color.black;
				if (mode == PhenoGenoEnum.Phenotype && selectedCell != null) {
					color = selectedCell.GetOutputFromUnit(arrow.tailUnit, arrow.tailUnitSlot) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
				} else {
					color = ColorScheme.instance.signalOff;
				}
				arrow.GetComponent<Image>().color = ColorUtil.SetAlpha(color, 0.5f);
			}
			isDirtySignal = false;
		}
	}

	public static List<Nerve> GetNervesToHighliteGenotype(Genotype genotype, Gene selectedGene) {
		ViewXput? viewXputNullable = GenePanel.instance.cellAndGenePanel.viewXput;

		List<Nerve> nerveToHighlite = null;

		if (viewXputNullable != null) {
			ViewXput viewXput = ((ViewXput)viewXputNullable);
			List<List<Nerve>> nerveTwinBundleList = genotype.GetNerveTwinBundles(selectedGene, viewXput.signalUnitEnum, viewXput.xputType);
			if (nerveTwinBundleList.Count > 0) {
				//for (int i = 0; i < nerveTwinBundleList.Count; i++) {
				//	Debug.Log("twin bundle: " + i + ", length: " + nerveTwinBundleList[i].Count);
				//}
				int nerveTwinBundleIndex = viewXput.index == 0 ? -1 : (viewXput.index - 1) % nerveTwinBundleList.Count;
				//Debug.Log("Index: " + (nerveTwinBundleIndex == -1 ? "EVERYTHING" : nerveTwinBundleIndex.ToString()));

				nerveToHighlite = new List<Nerve>();
				if (nerveTwinBundleIndex == -1) {
					foreach (List<Nerve> nerveList in nerveTwinBundleList) {
						foreach (Nerve nerve in nerveList) {
							nerveToHighlite.Add(nerve);
						}
					}
				} else {
					nerveToHighlite = nerveTwinBundleList[nerveTwinBundleIndex]; // there should at leas be one on index 0
				}
			}
		}
		return nerveToHighlite;
	}

	public static List<Nerve> GetNervesToHighlitePhenotype(Cell selectedCell) {
		ViewXput? viewXputNullable = CellPanel.instance.cellAndGenePanel.viewXput;

		List<Nerve> nerveToHighlite = null;

		if (viewXputNullable != null) {
			ViewXput viewXput = ((ViewXput)viewXputNullable);

			List<Nerve> nerveInSignalUnit = null;
			if (viewXput.xputType == XputEnum.Input) {
				nerveInSignalUnit = selectedCell.GetSignalUnit(viewXput.signalUnitEnum).GetInputNervesGenotypePhenotype();
			} else /* Output */ {
				nerveInSignalUnit = selectedCell.GetSignalUnit(viewXput.signalUnitEnum).GetOutputNervesGenotypePhenotype();
			}

			int nerveTwinBundleIndex = viewXput.index == 0 ? -1 : (viewXput.index - 1) % nerveInSignalUnit.Count;

			nerveToHighlite = new List<Nerve>();
			if (nerveTwinBundleIndex == -1) {
				foreach (Nerve nerve in nerveInSignalUnit) {
					nerveToHighlite.Add(nerve);
				}
			} else {
				nerveToHighlite.Add(nerveInSignalUnit[nerveTwinBundleIndex]); // there should at least be one on index 0
			}

		}
		return nerveToHighlite;
	}

	public static bool IsNervesHighliteAllModeGenotype() {
		ViewXput? viewXputNullable = GenePanel.instance.cellAndGenePanel.viewXput;
		if (viewXputNullable != null) {
			ViewXput viewXput = ((ViewXput)viewXputNullable);
			return viewXput.index == 0;
		}
		return false;

	}

	public static bool IsNervesHighliteAllModePhenotype() {
		ViewXput? viewXputNullable = CellPanel.instance.cellAndGenePanel.viewXput;
		if (viewXputNullable != null) {
			ViewXput viewXput = ((ViewXput)viewXputNullable);
			return viewXput.index == 0;
		}
		return false;

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

	private void HighliteArrow(HudSignalArrow arrow) {
		arrow.GetComponent<Image>().color = ColorScheme.instance.signalViewed;
		arrow.transform.SetAsLastSibling(); // transforming coordinate doesn't affect draw order 
	}

	// Used for inputs
	private void ShowTextAtArrowHead(HudSignalArrow arrow, bool show) {
		arrow.showHeadLabel = show;
	}

	private void ShowTextAtArrowTail(HudSignalArrow arrow, bool show) {
		arrow.showTailLabel = show;
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
