using UnityEngine;
using UnityEngine.UI;

public class GeneNeighboursPanel : MonoSingleton<GeneNeighboursPanel> {
	public GameObject circles;

	// Gene panels
	//TODO move inside cell (setting) panel
	public EggGenePanel eggGenePanel;

	private bool isDirty = true;
	public void MakeDirty() {
		isDirty = true;
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
		bool trueChange = (int)GenePanel.instance.selectedGene.type != cellTypeDropdown.value;

		if (cellTypeDropdown.value == 0) {
			GenePanel.instance.selectedGene.type = CellTypeEnum.Egg;
		} else if (cellTypeDropdown.value == 1) {
			GenePanel.instance.selectedGene.type = CellTypeEnum.Fungal;
		} else if (cellTypeDropdown.value == 2) {
			GenePanel.instance.selectedGene.type = CellTypeEnum.Jaw;
		} else if (cellTypeDropdown.value == 3) {
			GenePanel.instance.selectedGene.type = CellTypeEnum.Leaf;
		} else if (cellTypeDropdown.value == 4) {
			GenePanel.instance.selectedGene.type = CellTypeEnum.Muscle;
		} else if (cellTypeDropdown.value == 5) {
			GenePanel.instance.selectedGene.type = CellTypeEnum.Root;
		} else if (cellTypeDropdown.value == 6) {
			GenePanel.instance.selectedGene.type = CellTypeEnum.Shell;
		} else if (cellTypeDropdown.value == 7) {
			GenePanel.instance.selectedGene.type = CellTypeEnum.Vein;
		}

		if (trueChange && CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype) {
			GenotypePanel.instance.MakeDirty();
			GenomePanel.instance.MakeDirty();
			if (CreatureSelectionPanel.instance.hasSoloSelected) {
				CreatureSelectionPanel.instance.soloSelected.genotype.geneCellsDiffersFromGenome = true;
				CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
				CreatureSelectionPanel.instance.soloSelected.generation = 1;
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

			eggGenePanel.gameObject.SetActive(false);

			//Nothing to represent
			if (GenePanel.instance.selectedGene == null) {
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

			//allow interactionclo
			if (CreatureSelectionPanel.instance.hasSoloSelected) {
				cellTypeDropdown.interactable = CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome;
				eggGenePanel.SetInteractable(CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome);
			}


			circles.SetActive(true);

			for (int index = 0; index < arrangementPanels.Length; index++) {
				if (arrangementPanels[index] != null) {
					arrangementPanels[index].arrangement = GenePanel.instance.selectedGene.arrangements[index];
				}
			}

			//perifier
			for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
				referenceGraphics[cardinalIndex].reference = GenePanel.instance.selectedGene.GetFlippableReference(cardinalIndex, GenotypePanel.instance.viewedFlipSide);
			}
			centerReferenceGraphics.reference = new GeneReference(GenePanel.instance.selectedGene, GenotypePanel.instance.viewedFlipSide);

			//Gene Settings
			cellTypeDropdown.gameObject.SetActive(true);

			if (GenePanel.instance.selectedGene.type == CellTypeEnum.Egg) {
				cellTypeDropdown.value = 0;
				eggGenePanel.gameObject.SetActive(true);
				eggGenePanel.MakeDirty();
				eggGenePanel.MakeSlidersDirty();
			} else if (GenePanel.instance.selectedGene.type == CellTypeEnum.Fungal) {
				cellTypeDropdown.value = 1;
			} else if (GenePanel.instance.selectedGene.type == CellTypeEnum.Jaw) {
				cellTypeDropdown.value = 2;
			} else if (GenePanel.instance.selectedGene.type == CellTypeEnum.Leaf) {
				cellTypeDropdown.value = 3;
			} else if (GenePanel.instance.selectedGene.type == CellTypeEnum.Muscle) {
				cellTypeDropdown.value = 4;
			} else if (GenePanel.instance.selectedGene.type == CellTypeEnum.Root) {
				cellTypeDropdown.value = 5;
			} else if (GenePanel.instance.selectedGene.type == CellTypeEnum.Shell) {
				cellTypeDropdown.value = 6;
			} else if (GenePanel.instance.selectedGene.type == CellTypeEnum.Vein) {
				cellTypeDropdown.value = 7;
			}

			isDirty = false;
		}
	}
}