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

	private readonly float externalArrowLength = 30f;

	private void Update() {
		// Update connections
		if (!CreatureSelectionPanel.instance.hasSoloSelected) {
			return;
		}

		List<Nerve> uniqueNerves = null;
		List<Nerve> nervesToHighlite = null;
		bool isNervesHighliteAllMode = false;
		if (isDirtyConnections || isDirtySignal) {
			if (mode == PhenoGenoEnum.Genotype) {
				Genotype genotype = CreatureSelectionPanel.instance.soloSelected.genotype;
				uniqueNerves = genotype.GetAllUniqueNerves(selectedGene);
				nervesToHighlite = GetNervesToHighliteGenotype(genotype, selectedGene);
				isNervesHighliteAllMode = IsNervesHighliteAllModeGenotype();
			} else /*Phenotype*/{
				uniqueNerves = selectedCell.GetAllNervesGenotypePhenotype();
				nervesToHighlite = GetNervesToHighlitePhenotype(selectedCell);
				isNervesHighliteAllMode = IsNervesHighliteAllModePhenotype();
			}
		}


		if (isDirtyConnections) {
			if ((mode == PhenoGenoEnum.Genotype && CreatureSelectionPanel.instance.soloSelected.genotype.isInterGeneCellDirty) ||
				(mode == PhenoGenoEnum.Phenotype && CreatureSelectionPanel.instance.soloSelected.phenotype.isInterCellDirty)) {
				DebugUtil.Log("Ooooops, not ready to refresh yet");
				return; // Ooooops, not ready to refresh yet
			}

			if ((mode == PhenoGenoEnum.Phenotype && selectedCell == null) || (mode == PhenoGenoEnum.Genotype && selectedGene == null)) {
				// no menu
				isDirtyConnections = false;
				return;
			}

			//DebugUtil.Log("Updating Hud Signal Arrow Handler");

			foreach (HudSignalArrow arrow in arrowList) {
				hudSignalArrowPool.Recycle(arrow);
			}
			arrowList.Clear();

			if (uniqueNerves == null || uniqueNerves.Count == 0) {
				// nothing to draw.... done here... bye
				isDirtyConnections = false;
				return;
			}

			Dictionary<string, List<HudSignalArrow>> outputArrowDictionary = new Dictionary<string, List<HudSignalArrow>>();

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
						head = tail + Vector2.up * externalArrowLength;

						arrow.headPosition = head;
						arrow.tailPosition = tail;
					
						// Store potentialy overlapping output arrows, so that we can spread them out later
						List<HudSignalArrow> outputSlotArrowList;
						if (outputArrowDictionary.TryGetValue(arrow.ToTailString(), out outputSlotArrowList)) {
							outputSlotArrowList.Add(arrow);
						} else {
							outputSlotArrowList = new List<HudSignalArrow>();
							outputSlotArrowList.Add(arrow);
							outputArrowDictionary.Add(arrow.ToTailString(), outputSlotArrowList);
						}
					}

					SetArrowTransforms(arrow, head, tail);
					if (mode == PhenoGenoEnum.Phenotype && (selectedCell.GetSignalUnit(arrow.tailUnit).rootnessEnum == RootnessEnum.Rootable || selectedCell.GetSignalUnit(arrow.headUnit).rootnessEnum == RootnessEnum.Rootable)) {
						arrow.GetComponent<Image>().color = ColorScheme.instance.signalRootable;
					} else /* Rooted */{
						arrow.GetComponent<Image>().color = ColorScheme.instance.signalOff;
					}
					ShowTextAtArrowHead(arrow, false);
					ShowTextAtArrowTail(arrow, false);

					// Highlite
					if (nervesToHighlite != null ) {
						if (nervesToHighlite.Find(n => n == nerve) != null) { // Nerve.AreTwinNerves(n, nerve, true)) != null
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

					if (mode == PhenoGenoEnum.Phenotype && (selectedCell.GetSignalUnit(arrow.tailUnit) == null || selectedCell.GetSignalUnit(arrow.tailUnit).rootnessEnum == RootnessEnum.Rootable || selectedCell.GetSignalUnit(arrow.headUnit) == null || selectedCell.GetSignalUnit(arrow.headUnit).rootnessEnum == RootnessEnum.Rootable)) {
						arrow.GetComponent<Image>().color = ColorScheme.instance.signalRootable;
					} else /* Rooted */{
						arrow.GetComponent<Image>().color = ColorScheme.instance.signalOff;
					}

					if (nerve.tailSignalUnitEnum == SignalUnitEnum.Void) {
						arrow.GetComponent<Image>().color = ColorScheme.instance.signalOff;
						tail = head + Vector2.down * externalArrowLength;

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

			float outputButtonWidth = 40f;

			// spread out output arrows to make it look pretty
			foreach (List<HudSignalArrow> arrowList in outputArrowDictionary.Values) {
				for (int i = 0; i < arrowList.Count; i++) {
					HudSignalArrow arrow = arrowList[i];
					float step = outputButtonWidth / (1 + arrowList.Count);
					float x = -outputButtonWidth * 0.5f + step * (i + 1);
					SetArrowTransforms(arrow, arrow.headPosition + Vector2.right * x, arrow.tailPosition + Vector2.right * x);
				}
			}

			isDirtyConnections = false;
		}

		// Update signal TODO: update only when dirty, that is post signal update in creature
		if (isDirtySignal && nervesToHighlite == null) {
			foreach (HudSignalArrow arrow in arrowList) {
				Color color = Color.black;
				if (mode == PhenoGenoEnum.Phenotype) {
					if (arrow.tailCell == null || arrow.tailCell.GetSignalUnit(arrow.tailUnit) == null || arrow.tailCell.GetSignalUnit(arrow.tailUnit).rootnessEnum == RootnessEnum.Rootable || arrow.headCell == null || arrow.headCell.GetSignalUnit(arrow.headUnit) == null || arrow.headCell.GetSignalUnit(arrow.headUnit).rootnessEnum == RootnessEnum.Rootable) {
						// Rootable - Head or tail is not rooted, because there are cells unbuilt
						color = ColorScheme.instance.signalRootable;
					} else {
						// Rooted - head and tail are built (show signal on / off)
						color = arrow.tailCell.GetOutputFromUnit(arrow.tailUnit, arrow.tailUnitSlot) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
					}
				} else {
					// Genotype - same for all "signal off" allways
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
				//	DebugUtil.Log("twin bundle: " + i + ", length: " + nerveTwinBundleList[i].Count);
				//}
				int nerveTwinBundleIndex = viewXput.index == 0 ? -1 : (viewXput.index - 1) % nerveTwinBundleList.Count;
				//DebugUtil.Log("Index: " + (nerveTwinBundleIndex == -1 ? "EVERYTHING" : nerveTwinBundleIndex.ToString()));

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

			if (nerveInSignalUnit.Count > 0) {
				int index = viewXput.index == 0 ? -1 : (viewXput.index - 1) % nerveInSignalUnit.Count;

				nerveToHighlite = new List<Nerve>();
				if (index == -1) {
					foreach (Nerve nerve in nerveInSignalUnit) {
						nerveToHighlite.Add(nerve);
					}
				} else {
					nerveToHighlite.Add(nerveInSignalUnit[index]); // there should at least be one on index 0
				}
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

	private HudSignalArrow GetArrowLikeNerve(Nerve nerve) {
		HudSignalArrow arrow = hudSignalArrowPool.Borrow();
		arrow.gameObject.SetActive(true);

		arrow.headUnit = nerve.headSignalUnitEnum;
		arrow.headUnitSlot = nerve.headSignalUnitSlotEnum;
		arrow.tailUnit = nerve.tailSignalUnitEnum;
		arrow.tailUnitSlot = nerve.tailSignalUnitSlotEnum;

		arrow.headCell = nerve.headCell;
		arrow.tailCell = nerve.tailCell;


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
