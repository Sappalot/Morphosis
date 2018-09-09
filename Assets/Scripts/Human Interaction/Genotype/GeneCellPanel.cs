using UnityEngine;
using UnityEngine.UI;

public class GeneCellPanel : MonoSingleton<GeneCellPanel> {
	// Metabolism 

	//-----Shold be same as the top of CellPanel
	[Header("Metabolism")]
	public Text energy; // just an E 
	public EnergyBar energyBar;
	public Text effect;

	public Text effectFromNeighbours; //kill me
	public Text effectToNeighbours; //kill me
	public Text effectFromMother; //kill me
	public Text effectToChildren; //kill me

	public Text isOrigin;
	public Text isPlacenta;
	public Text neighboursCount;
	public Text connectedVeinsCount; //number of veins connected to me, non placenta + children
	public Text connectionGroupCount;
	public Text eatingOnMeCount; //number of Jaw cells eating on me

	public Text healButtonText;
	public Text hurtButtonText;
	public Text deleteButtonText;

	public Dropdown metabolismCellTypeDropdown;

	//----- ^ Shold be same as the top of CellPanel ^

	// Metabolism -> specific
	public MetabolismCellPanel eggCellPanel;
	public MetabolismCellPanel jawCellPanel;
	public MetabolismCellPanel leafCellPanel;
	private MetabolismCellPanel[] metabolismCellPanels = new MetabolismCellPanel[3];

	public OriginCellPanel originCellPanel;

	override public void Init() {
		isDirty = true;
		metabolismCellPanels[0] = eggCellPanel;
		metabolismCellPanels[1] = jawCellPanel;
		metabolismCellPanels[2] = leafCellPanel;

		foreach (MetabolismCellPanel m in metabolismCellPanels) {
			m.mode = PhenoGenoEnum.Genotype;
		}

		originCellPanel.mode = PhenoGenoEnum.Genotype;
		MakeDirty();
	}

	private bool isDirty = true;
	public void MakeDirty() {
		isDirty = true;
		foreach (MetabolismCellPanel m in metabolismCellPanels) {
			m.MakeDirty();
		}

		originCellPanel.MakeDirty();
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
				Debug.Log("Update GeneCellPanel");
			}

			foreach (MetabolismCellPanel m in metabolismCellPanels) {
				m.gameObject.SetActive(false);
			}

			metabolismCellTypeDropdown.interactable = false;

			energy.color = Color.gray;

			//All metabolism stuff off
			effect.text = "Effect: -";
			effect.color = Color.gray;

			effectFromNeighbours.text = "P me <= neighbours: -";
			effectFromNeighbours.color = Color.gray;

			effectToNeighbours.text = "P me => neighbours: -";
			effectToNeighbours.color = Color.gray;

			effectFromMother.text = "P me <= mother: -";
			effectFromMother.color = Color.gray;

			effectToChildren.text = "P me => children: -";
			effectToChildren.color = Color.gray;

			isOrigin.text = "-";
			isOrigin.color = Color.gray;

			isPlacenta.text = "-";
			isPlacenta.color = Color.gray;

			neighboursCount.text = "Neighbours: -";
			neighboursCount.color = Color.gray;

			connectedVeinsCount.text = "Veins: - ";
			connectedVeinsCount.color = Color.gray;

			connectionGroupCount.text = "Connection Groups: -";
			connectionGroupCount.color = Color.gray;

			eatingOnMeCount.text = "Eating on me: -";
			eatingOnMeCount.color = Color.gray;

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

			ignoreMenuChange = true;
			if (GenePanel.instance.selectedGene.type == CellTypeEnum.Egg) {
				metabolismCellTypeDropdown.value = 0;
				eggCellPanel.gameObject.SetActive(true);
				eggCellPanel.MakeDirty();
				//(eggCellPanel as EggCellPanel).MakeSlidersDirty();
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

			originCellPanel.MakeDirty();

			isDirty = false;
		}
	}

}