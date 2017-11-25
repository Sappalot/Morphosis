using UnityEngine;
using UnityEngine.UI;

public class GenePanel : MonoSingleton<GenePanel> {

	public GameObject circles;

	public Image geneReferenceImage;
	public Image flipBlackWhite;
	public Image flipWhiteBlack;
	public Text geneReferenceText;

	private bool isDirty = true;
	public void MakeDirty() {
		isDirty = true;
	}

	private Gene m_selectedGene;
	public Gene selectedGene {
		get {
			return m_selectedGene != null ? m_selectedGene : (CreatureSelectionPanel.instance.hasSoloSelected ? CreatureSelectionPanel.instance.soloSelected.genotype.originCell.gene : null);
		}
		set {
			m_selectedGene = value;
			isDirty = true;
		}
	}


	public ReferenceGraphics[] referenceGraphics;
	public ReferenceGraphics centerReferenceGraphics;

	public ArrangementPanel arrangementPanelTemplate;
	private ArrangementPanel[] arrangementPanels = new ArrangementPanel[3];

	//Gene Settings
	public Dropdown cellTypeDropdown;

	override public void Init() {
		RectTransform originalTransform = arrangementPanelTemplate.GetComponent<RectTransform>();

		for (int index = 0; index < arrangementPanels.Length; index++) {
			arrangementPanels[index] = Instantiate(arrangementPanelTemplate, transform);
			arrangementPanels[index].gameObject.SetActive(true);
			arrangementPanels[index].name = "Arrangement Panel " + index;
			RectTransform spawnTransform = arrangementPanels[index].GetComponent<RectTransform>();
			spawnTransform.position = originalTransform.position + Vector3.right * index * (originalTransform.rect.width + 2);

			arrangementPanels[index].genePanel = this;
		}

		arrangementPanelTemplate.gameObject.SetActive(false);
		arrangementPanelTemplate.arrangementButtons.SetActive(false);
	}

	//----
	public void OnCellTypeDropdownChanged() {
		bool trueChange = (int)selectedGene.type != cellTypeDropdown.value;

		if (cellTypeDropdown.value == 0) {
			selectedGene.type = CellTypeEnum.Egg;
		} else if (cellTypeDropdown.value == 1) {
			selectedGene.type = CellTypeEnum.Fungal;
		} else if (cellTypeDropdown.value == 2) {
			selectedGene.type = CellTypeEnum.Jaw;
		} else if (cellTypeDropdown.value == 3) {
			selectedGene.type = CellTypeEnum.Leaf;
		} else if (cellTypeDropdown.value == 4) {
			selectedGene.type = CellTypeEnum.Muscle;
		} else if (cellTypeDropdown.value == 5) {
			selectedGene.type = CellTypeEnum.Root;
		} else if (cellTypeDropdown.value == 6) {
			selectedGene.type = CellTypeEnum.Shell;
		} else if (cellTypeDropdown.value == 7) {
			selectedGene.type = CellTypeEnum.Vein;
		}

		if (trueChange && CreatureEditModePanel.instance.mode == CreatureEditModeEnum.Genotype) {
			GenotypePanel.instance.MakeDirty();
			GenomePanel.instance.MakeDirty();
			if (CreatureSelectionPanel.instance.hasSoloSelected) {
				CreatureSelectionPanel.instance.soloSelected.genotype.geneCellsDiffersFromGenome = true;
			}
			isDirty = true;
		}
	}

	private ArrangementPanel arrangementPanelAskingForReference;
	public void SetAskingForGeneReference(ArrangementPanel arrangementPanel) {
		arrangementPanelAskingForReference = arrangementPanel;
	}

	public void GiveAnswerGeneReference(Gene gene) {
		arrangementPanelAskingForReference.SetGeneReference(gene);

		MakeDirty();
		GenomePanel.instance.MakeDirty();
		//GenomePanel.instance.MakeScrollDirty();
		CreatureSelectionPanel.instance.soloSelected.MakeDirtyGraphics();
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate)
				Debug.Log("Update GenePanel");
			//Nothing to represent
			if (selectedGene == null) {
				for (int index = 0; index < arrangementPanels.Length; index++) {
					if (arrangementPanels[index] != null) {
						arrangementPanels[index].arrangement = null;
					}
				}

				circles.SetActive(false);

				cellTypeDropdown.gameObject.SetActive(false);
				isDirty = false;
				return;
			}

			circles.SetActive(true);

			for (int index = 0; index < arrangementPanels.Length; index++) {
				if (arrangementPanels[index] != null) {
					arrangementPanels[index].arrangement = selectedGene.arrangements[index];
				}
			}

			//perifier
			for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
				referenceGraphics[cardinalIndex].reference = selectedGene.GetFlippableReference(cardinalIndex, GenotypePanel.instance.viewedFlipSide);
			}
			centerReferenceGraphics.reference = new GeneReference(selectedGene, GenotypePanel.instance.viewedFlipSide);

			//Gene Settings
			cellTypeDropdown.gameObject.SetActive(true);

			if (selectedGene.type == CellTypeEnum.Egg) {
				cellTypeDropdown.value = 0;
			} else if (selectedGene.type == CellTypeEnum.Fungal) {
				cellTypeDropdown.value = 1;
			} else if (selectedGene.type == CellTypeEnum.Jaw) {
				cellTypeDropdown.value = 2;
			} else if (selectedGene.type == CellTypeEnum.Leaf) {
				cellTypeDropdown.value = 3;
			} else if (selectedGene.type == CellTypeEnum.Muscle) {
				cellTypeDropdown.value = 4;
			} else if (selectedGene.type == CellTypeEnum.Root) {
				cellTypeDropdown.value = 5;
			} else if (selectedGene.type == CellTypeEnum.Shell) {
				cellTypeDropdown.value = 6;
			} else if (selectedGene.type == CellTypeEnum.Vein) {
				cellTypeDropdown.value = 7;
			}

			isDirty = false;
		}
	}
}