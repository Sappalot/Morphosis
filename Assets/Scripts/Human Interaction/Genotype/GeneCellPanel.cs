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
	public MetabolismCellPanel fungalCellPanel;
	public MetabolismCellPanel jawCellPanel;
	public MetabolismCellPanel leafCellPanel;
	public MetabolismCellPanel muscleCellPanel;
	public MetabolismCellPanel rootCellPanel;
	public MetabolismCellPanel shellCellPanel;
	public MetabolismCellPanel veinCellPanel;
	private MetabolismCellPanel[] metabolismCellPanels = new MetabolismCellPanel[8];

	public AxonCellPanel axonCellPanel;
	public OriginCellPanel originCellPanel;

	public GeneNeighboursPanel geneNeighbourPanel;

	override public void Init() {
		isDirty = true;
		metabolismCellPanels[0] = eggCellPanel;
		metabolismCellPanels[1] = fungalCellPanel;
		metabolismCellPanels[2] = jawCellPanel;
		metabolismCellPanels[3] = leafCellPanel;
		metabolismCellPanels[4] = muscleCellPanel;
		metabolismCellPanels[5] = shellCellPanel;
		metabolismCellPanels[6] = rootCellPanel;
		metabolismCellPanels[7] = veinCellPanel;

		foreach (MetabolismCellPanel m in metabolismCellPanels) {
			m.mode = PhenoGenoEnum.Genotype;
		}

		axonCellPanel.mode = PhenoGenoEnum.Genotype;
		originCellPanel.mode = PhenoGenoEnum.Genotype;

		geneNeighbourPanel.Init();

		MakeDirty();
	}

	private bool isDirty = true;
	public void MakeDirty() {
		isDirty = true;
		foreach (MetabolismCellPanel m in metabolismCellPanels) {
			m.MakeDirty();
		}
		axonCellPanel.MakeDirty();
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

		geneNeighbourPanel.MakeDirty();
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

			energy.color = ColorScheme.instance.grayedOut;

			//All metabolism stuff off
			effect.text = "Effect: -";
			effect.color = ColorScheme.instance.grayedOutPhenotype;

			effectFromNeighbours.text = "P me <= neighbours: -";
			effectFromNeighbours.color = ColorScheme.instance.grayedOutPhenotype;

			effectToNeighbours.text = "P me => neighbours: -";
			effectToNeighbours.color = ColorScheme.instance.grayedOutPhenotype;

			effectFromMother.text = "P me <= mother: -";
			effectFromMother.color = ColorScheme.instance.grayedOutPhenotype;

			effectToChildren.text = "P me => children: -";
			effectToChildren.color = ColorScheme.instance.grayedOutPhenotype;

			isOrigin.text = "-";
			isOrigin.color = ColorScheme.instance.grayedOutPhenotype;

			isPlacenta.text = "-";
			isPlacenta.color = ColorScheme.instance.grayedOutPhenotype;

			neighboursCount.text = "Neighbours: -";
			neighboursCount.color = ColorScheme.instance.grayedOutPhenotype;

			connectedVeinsCount.text = "Veins: - ";
			connectedVeinsCount.color = ColorScheme.instance.grayedOutPhenotype;

			connectionGroupCount.text = "Connection Groups: -";
			connectionGroupCount.color = ColorScheme.instance.grayedOutPhenotype;

			eatingOnMeCount.text = "Eating on me: -";
			eatingOnMeCount.color = ColorScheme.instance.grayedOutPhenotype;

			deleteButtonText.color = ColorScheme.instance.grayedOutPhenotype;
			healButtonText.color = ColorScheme.instance.grayedOutPhenotype;
			hurtButtonText.color = ColorScheme.instance.grayedOutPhenotype;

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
			} else if (GenePanel.instance.selectedGene.type == CellTypeEnum.Fungal) {
				metabolismCellTypeDropdown.value = 1;
				fungalCellPanel.gameObject.SetActive(true);
				fungalCellPanel.MakeDirty();
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
				muscleCellPanel.gameObject.SetActive(true);
				muscleCellPanel.MakeDirty();
			} else if (GenePanel.instance.selectedGene.type == CellTypeEnum.Root) {
				metabolismCellTypeDropdown.value = 5;
				rootCellPanel.gameObject.SetActive(true);
				rootCellPanel.MakeDirty();
			} else if (GenePanel.instance.selectedGene.type == CellTypeEnum.Shell) {
				metabolismCellTypeDropdown.value = 6;
				shellCellPanel.gameObject.SetActive(true);
				shellCellPanel.MakeDirty();
			} else if (GenePanel.instance.selectedGene.type == CellTypeEnum.Vein) {
				metabolismCellTypeDropdown.value = 7;
				veinCellPanel.gameObject.SetActive(true);
				veinCellPanel.MakeDirty();
			}
			ignoreMenuChange = false;

			originCellPanel.MakeDirty();

			isDirty = false;
		}
	}

}