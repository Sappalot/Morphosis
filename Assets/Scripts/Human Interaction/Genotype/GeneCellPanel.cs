using UnityEngine;
using UnityEngine.UI;

public class GeneCellPanel : MonoSingleton<GeneCellPanel> {

	// Metabolism
	[Header("Metabolism")]
	public EnergyBar energyBar;
	public Text effect;
	public Text isOrigin;
	public Text isPlacenta;
	public Text neighbours;
	public Text connectionGroupCount;
	public Text detatchThreshold;
	public Text eatingOnMe; //number of Jaw cells eating on me

	public Text healButtonText;
	public Text hurtButtonText;
	public Text deleteButtonText;

	// Metabolism -> specific
	public MetabolismCellPanel eggCellPanel;
	public MetabolismCellPanel jawCellPanel;
	public MetabolismCellPanel leafCellPanel;
	private MetabolismCellPanel[] metabolismCellPanels = new MetabolismCellPanel[3];

	public Dropdown metabolismCellTypeDropdown;

	override public void Init() {
		isDirty = true;
		metabolismCellPanels[0] = eggCellPanel;
		metabolismCellPanels[1] = jawCellPanel;
		metabolismCellPanels[2] = leafCellPanel;

		foreach (MetabolismCellPanel m in metabolismCellPanels) {
			m.mode = PhenoGenoEnum.Genotype;
		}
	}

	private bool isDirty = true;
	public void MakeDirty() {
		isDirty = true;
		foreach (MetabolismCellPanel m in metabolismCellPanels) {
			m.MakeDirty();
		}
	}

	private bool ignoreMenuChange;
	public void OnMetabolismCellTypeDropdownChanged() {
		if (ignoreMenuChange) {
			return;
		}

		if (metabolismCellTypeDropdown.value == 0) {
			GenePanel.instance.selectedGene.type = CellTypeEnum.Egg;
		} else if (metabolismCellTypeDropdown.value == 1) {
			GenePanel.instance.selectedGene.type = CellTypeEnum.Fungal;
		} else if (metabolismCellTypeDropdown.value == 2) {
			GenePanel.instance.selectedGene.type = CellTypeEnum.Jaw;
		} else if (metabolismCellTypeDropdown.value == 3) {
			GenePanel.instance.selectedGene.type = CellTypeEnum.Leaf;
		} else if (metabolismCellTypeDropdown.value == 4) {
			GenePanel.instance.selectedGene.type = CellTypeEnum.Muscle;
		} else if (metabolismCellTypeDropdown.value == 5) {
			GenePanel.instance.selectedGene.type = CellTypeEnum.Root;
		} else if (metabolismCellTypeDropdown.value == 6) {
			GenePanel.instance.selectedGene.type = CellTypeEnum.Shell;
		} else if (metabolismCellTypeDropdown.value == 7) {
			GenePanel.instance.selectedGene.type = CellTypeEnum.Vein;
		}


		GenotypePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.genotype.geneCellsDiffersFromGenome = true;
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
		}
		isDirty = true;

	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update GenePanel");
			}

			foreach (MetabolismCellPanel m in metabolismCellPanels) {
				m.gameObject.SetActive(false);
			}

			//Metabolism
			effect.text = "P cell: -";
			effect.color = Color.gray;

			isOrigin.text = "-";
			isOrigin.color = Color.gray;

			isPlacenta.text = "-";
			isPlacenta.color = Color.gray;

			neighbours.text = "Neighbours: -";
			neighbours.color = Color.gray;

			connectionGroupCount.text = "Con. Groups: -";
			connectionGroupCount.color = Color.gray;

			detatchThreshold.text = "Detatch: ?"; //todo
			detatchThreshold.color = Color.gray;

			eatingOnMe.text = "Eating on me: -";
			eatingOnMe.color = Color.gray;

			deleteButtonText.color = Color.gray;
			healButtonText.color = Color.gray;
			hurtButtonText.color = Color.gray;

			energyBar.isOn = false;

			//Nothing to represent
			if (GenePanel.instance.selectedGene == null || !CreatureSelectionPanel.instance.hasSoloSelected) {

				isDirty = false;
				return;
			}

			//allow interactionclo

			metabolismCellTypeDropdown.interactable = CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome;
			//eggGenePanel.SetInteractable(CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome);


			ignoreMenuChange = true;
			if (GenePanel.instance.selectedGene.type == CellTypeEnum.Egg) {
				metabolismCellTypeDropdown.value = 0;
				eggCellPanel.gameObject.SetActive(true);
				eggCellPanel.MakeDirty();
			} else if (GenePanel.instance.selectedGene.type == CellTypeEnum.Fungal) {
				metabolismCellTypeDropdown.value = 1;
			} else if (GenePanel.instance.selectedGene.type == CellTypeEnum.Jaw) {
				metabolismCellTypeDropdown.value = 2;
				jawCellPanel.gameObject.SetActive(true);
				jawCellPanel.MakeDirty();
			} else if (GenePanel.instance.selectedGene.type == CellTypeEnum.Leaf) {
				metabolismCellTypeDropdown.value = 3;
				leafCellPanel.gameObject.SetActive(true);
				leafCellPanel.MakeDirty();
			} else if (GenePanel.instance.selectedGene.type == CellTypeEnum.Muscle) {
				metabolismCellTypeDropdown.value = 4;
			} else if (GenePanel.instance.selectedGene.type == CellTypeEnum.Root) {
				metabolismCellTypeDropdown.value = 5;
			} else if (GenePanel.instance.selectedGene.type == CellTypeEnum.Shell) {
				metabolismCellTypeDropdown.value = 6;
			} else if (GenePanel.instance.selectedGene.type == CellTypeEnum.Vein) {
				metabolismCellTypeDropdown.value = 7;
			}
			ignoreMenuChange = false;

			isDirty = false;
		}
	}

}