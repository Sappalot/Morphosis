using UnityEngine;
using UnityEngine.UI;

public class GeneCellPanel : MonoSingleton<GeneCellPanel> {
	// Metabolism 
	public Text geneDescriptionLabel;

	private Gene m_selectedGene;
	public Gene selectedGene {
		get {
			return m_selectedGene != null ? m_selectedGene : (CreatureSelectionPanel.instance.hasSoloSelected ? (CreatureSelectionPanel.instance.soloSelected.genotype.hasGenes ? CreatureSelectionPanel.instance.soloSelected.genotype.originCell.gene : null) : null);
		}
		set {
			m_selectedGene = value;
			MakeDirty();
		}
	}

	//-----Shold be same as the top of CellPanel
	[Header("Metabolism")]
	public EnergyBar energyBar;
	public Text effect;
	public Text armour;

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
	private SensorPanel[] sensorPanels = new SensorPanel[1];
	public SensorPanel effectSensorPanel;
	// TODO: more sensor panels

	public CellBuildPriorityPanel cellBuildPriorityPanel;
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
			m.SetMode(PhenoGenoEnum.Genotype);
		}

		axonCellPanel.SetMode(PhenoGenoEnum.Genotype);

		sensorPanels[0] = effectSensorPanel;
		foreach (SensorPanel s in sensorPanels) {
			s.SetMode(PhenoGenoEnum.Genotype);
		}

		cellBuildPriorityPanel.mode = PhenoGenoEnum.Genotype;
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

		foreach (SensorPanel s in sensorPanels) {
			s.MakeDirty();
		}

		cellBuildPriorityPanel.MakeDirty();
		originCellPanel.MakeDirty();

		geneNeighbourPanel.MakeDirty();
	}

	private bool ignoreMenuChange;
	public void OnMetabolismCellTypeDropdownChanged() {
		if (ignoreMenuChange) {
			return;
		}

		if (metabolismCellTypeDropdown.value == 0) {
			selectedGene.type = CellTypeEnum.Egg;
		} else if (metabolismCellTypeDropdown.value == 1) {
			selectedGene.type = CellTypeEnum.Fungal;
		} else if (metabolismCellTypeDropdown.value == 2) {
			selectedGene.type = CellTypeEnum.Jaw;
		} else if (metabolismCellTypeDropdown.value == 3) {
			selectedGene.type = CellTypeEnum.Leaf;
		} else if (metabolismCellTypeDropdown.value == 4) {
			selectedGene.type = CellTypeEnum.Muscle;
		} else if (metabolismCellTypeDropdown.value == 5) {
			selectedGene.type = CellTypeEnum.Root;
		} else if (metabolismCellTypeDropdown.value == 6) {
			selectedGene.type = CellTypeEnum.Shell;
		} else if (metabolismCellTypeDropdown.value == 7) {
			selectedGene.type = CellTypeEnum.Vein;
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

			//All off, we may turn on 1 of them later
			foreach (MetabolismCellPanel m in metabolismCellPanels) {
				m.gameObject.SetActive(false);
			}

			foreach (SensorPanel s in sensorPanels) {
				s.gameObject.SetActive(false);
			}

			metabolismCellTypeDropdown.interactable = false;

			//All metabolism stuff off
			effect.text = "Effect: -";
			effect.color = ColorScheme.instance.grayedOutPhenotype;

			if (selectedGene != null) {
				armour.text = string.Format("Armour: {0:F2} ==> Stress effect: {1:F2} W", selectedGene.armour, GlobalSettings.instance.phenotype.jawCellEatEffect / selectedGene.armour);
			}

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
			if (selectedGene == null || !CreatureSelectionPanel.instance.hasSoloSelected) {

				isDirty = false;
				return;
			}

			geneDescriptionLabel.text = "Gene: " + selectedGene.type.ToString();

			//allow interactionclo

			metabolismCellTypeDropdown.interactable = CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome;

			ignoreMenuChange = true;
			if (selectedGene.type == CellTypeEnum.Egg) {
				metabolismCellTypeDropdown.value = 0;
				eggCellPanel.gameObject.SetActive(true);
				eggCellPanel.MakeDirty();
			} else if (selectedGene.type == CellTypeEnum.Fungal) {
				metabolismCellTypeDropdown.value = 1;
				fungalCellPanel.gameObject.SetActive(true);
				fungalCellPanel.MakeDirty();
			} else if (selectedGene.type == CellTypeEnum.Jaw) {
				metabolismCellTypeDropdown.value = 2;
				jawCellPanel.gameObject.SetActive(true);
				jawCellPanel.MakeDirty();
			} else if (selectedGene.type == CellTypeEnum.Leaf) {
				metabolismCellTypeDropdown.value = 3;
				leafCellPanel.gameObject.SetActive(true);
				leafCellPanel.MakeDirty();
			} else if (selectedGene.type == CellTypeEnum.Muscle) {
				metabolismCellTypeDropdown.value = 4;
				muscleCellPanel.gameObject.SetActive(true);
				muscleCellPanel.MakeDirty();
			} else if (selectedGene.type == CellTypeEnum.Root) {
				metabolismCellTypeDropdown.value = 5;
				rootCellPanel.gameObject.SetActive(true);
				rootCellPanel.MakeDirty();
			} else if (selectedGene.type == CellTypeEnum.Shell) {
				metabolismCellTypeDropdown.value = 6;
				shellCellPanel.gameObject.SetActive(true);
				shellCellPanel.MakeDirty();
			} else if (selectedGene.type == CellTypeEnum.Vein) {
				metabolismCellTypeDropdown.value = 7;
				veinCellPanel.gameObject.SetActive(true);
				veinCellPanel.MakeDirty();
			}
			ignoreMenuChange = false;

			// Sensor ...
			if (selectedGene.sensorType == SensorTypeEnum.Effect) {
				effectSensorPanel.gameObject.SetActive(true);
				effectSensorPanel.MakeDirty();
			}
			// ^ Sensor ^

			if (selectedGene.isOrigin) {
				originCellPanel.gameObject.SetActive(true);
				originCellPanel.MakeDirty();

				cellBuildPriorityPanel.gameObject.SetActive(false);

			} else {
				originCellPanel.gameObject.SetActive(false);

				cellBuildPriorityPanel.gameObject.SetActive(true);
				cellBuildPriorityPanel.MakeDirty();
			}


			isDirty = false;
		}
	}

}