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

			if (mode == PhenoGenoEnum.Phenotype) {
				// phenotype
				// TODO: make phenotype work as well

				isDirtyConnections = false;
				return;
			}

			Genotype genotype = CreatureSelectionPanel.instance.soloSelected.genotype;
			List<Nerve> uniqueNerves = genotype.GetAllUniqueNervesGenotype(selectedGene);

			if (uniqueNerves.Count == 0) {
				// nothing to draw.... done here... bye
				isDirtyConnections = false;
				return;
			}

			List<Nerve> nervesToHighlite = GetNervesToHighliteGenotype(genotype, selectedGene);

			// inputs
			foreach (Nerve nerve in uniqueNerves) {
				//When it comes to input, they all look the same so we just pich the first one, which we are sure to have

				if (nerve.nerveStatusEnum == NerveStatusEnum.Output_GenotypeLocal ||
					nerve.nerveStatusEnum == NerveStatusEnum.Output_GenotypeExternal) {

					// We draw internal output arrows (from their tail side), though they have been drawn (from their head side) allready
					// The reason for this is so that we can highlite them from the tail side as well

					HudSignalArrow arrow = GetArrowLikeNerve(nerve);

					Vector2 tail = cellAndGenePanel.TotalPanelOffset(nerve.tailSignalUnitEnum, nerve.tailSignalUnitSlotEnum);
					// external output
					Vector2 head;

					if (nerve.nerveStatusEnum == NerveStatusEnum.Output_GenotypeLocal) {
						// Local input
						head = cellAndGenePanel.TotalPanelOffset(nerve.headSignalUnitEnum, nerve.headSignalUnitSlotEnum);
					} else {
						// External input (short arrow, pointing up, with head at input )
						head = tail + Vector2.up * 20f;
					}

					SetArrowTransforms(arrow, head, tail);
					arrow.GetComponent<Image>().color = ColorScheme.instance.signalOff;

					// Highlite
					if (nervesToHighlite != null) {
						if (nervesToHighlite.Find(n => Nerve.AreTwinNerves(n, nerve, true)) != null) {
							HighliteArrow(arrow);
						}
					}

					arrowList.Add(arrow);
				} else if (nerve.nerveStatusEnum == NerveStatusEnum.Input_GenotypeLocal ||
					nerve.nerveStatusEnum == NerveStatusEnum.Input_GenotypeExternal /* ||
					nerve.nerveStatusEnum == NerveStatusEnum.Input_GenotypeExternalVoid */) {

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
					arrow.GetComponent<Image>().color = ColorScheme.instance.signalOff;

					// Highlite
					if (nervesToHighlite != null) {
						if (nervesToHighlite.Find(n => Nerve.AreTwinNerves(n, nerve, true)) != null) {
							HighliteArrow(arrow);
						}
					}

					arrowList.Add(arrow);
				}
			}

			isDirtyConnections = false;
		}

		//// Update signal TODO: update only when dirty, that is post signal update in creature
		//if (isDirtySignal) {
		//	foreach (HudSignalArrow arrow in arrowList) {
		//		Color color = Color.black;
		//		if (mode == PhenoGenoEnum.Phenotype && selectedCell != null) {
		//			color = selectedCell.GetOutputFromUnit(arrow.tailUnit, arrow.tailUnitSlot) ? ColorScheme.instance.signalOn : ColorScheme.instance.signalOff;
		//		} else {
		//			color = ColorScheme.instance.signalOff;
		//		}
		//		arrow.GetComponent<Image>().color = new Color(color.r, color.g, color.b, 0.5f);
		//	}
		//	isDirtySignal = false;
		//}
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
		arrow.GetComponent<Image>().color = Color.magenta;
		arrow.transform.SetAsLastSibling(); // transforming coordinate doesn't affect draw order 
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
