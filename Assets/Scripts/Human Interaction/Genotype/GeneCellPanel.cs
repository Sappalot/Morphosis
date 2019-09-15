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
	public Text apexAngle;
	public Text eatingOnMeCount; //number of Jaw cells eating on me

	public Button healButton;
	public Button hurtButton;
	public Button deleteButton;

	public Dropdown metabolismCellTypeDropdown;

	//----- ^ Shold be same as the top of CellPanel ^

	// Work -> specific
	public CellComponentPanel eggCellPanel;
	public CellComponentPanel fungalCellPanel;
	public CellComponentPanel jawCellPanel;
	public CellComponentPanel leafCellPanel;
	public CellComponentPanel muscleCellPanel;
	public CellComponentPanel rootCellPanel;
	public CellComponentPanel shellCellPanel;
	public CellComponentPanel veinCellPanel;
	private CellComponentPanel[] metabolismCellPanels = new CellComponentPanel[8];

	public CellAxonComponentPanel axonCellPanel;
	private CellSensorPanel[] sensorPanels = new CellSensorPanel[1];
	//public CellSensorPanel effectSensorPanel;
	// TODO: more sensor panels

	public CellBuildPriorityComponentPanel cellBuildPriorityPanel;
	public OriginCellComponentPanel originCellPanel;

	public GeneNeighboursComponentPanel geneNeighbourPanel;

	public SignalArrowHandler signalArrowHandler;

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

		foreach (CellComponentPanel m in metabolismCellPanels) {
			m.Initialize(PhenoGenoEnum.Genotype);
		}

		axonCellPanel.Initialize(PhenoGenoEnum.Genotype);

		//sensorPanels[0] = effectSensorPanel;
		//foreach (CellSensorPanel s in sensorPanels) {
			//s.SetMode(PhenoGenoEnum.Genotype);
		//}

		cellBuildPriorityPanel.mode = PhenoGenoEnum.Genotype;
		originCellPanel.mode = PhenoGenoEnum.Genotype;

		geneNeighbourPanel.Init();

		signalArrowHandler.Initialize(PhenoGenoEnum.Genotype);

		MakeDirty();
	}

	private bool isDirty = true;
	public void MakeDirty() {
		isDirty = true;

		foreach (CellComponentPanel m in metabolismCellPanels) {
			m.MakeDirty();
		}

		axonCellPanel.MakeDirty();

		foreach (CellSensorPanel s in sensorPanels) {
			//s.MakeDirty();
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
			foreach (CellComponentPanel m in metabolismCellPanels) {
				m.gameObject.SetActive(false);
			}

			foreach (CellSensorPanel s in sensorPanels) {
				//s.gameObject.SetActive(false);
			}

			metabolismCellTypeDropdown.interactable = false;

			//All metabolism stuff off
			effect.text = "Effect: -";

			if (selectedGene != null) {
				armour.text = string.Format("Armour: {0:F2} ==> Stress effect: {1:F2} W", selectedGene.armour, GlobalSettings.instance.phenotype.jawCellEatEffectAtSpeed.Evaluate(20f) / selectedGene.armour);
			}

			effectFromNeighbours.text = "P me <= neighbours: -";
			effectToNeighbours.text = "P me => neighbours: -";
			effectFromMother.text = "P me <= mother: -";
			effectToChildren.text = "P me => children: -";
			isOrigin.text = "-";
			isPlacenta.text = "-";
			neighboursCount.text = "Neighbours: -";
			connectedVeinsCount.text = "Veins: - ";
			connectionGroupCount.text = "Connection Groups: -";
			apexAngle.text = "Apex angle: -";
			eatingOnMeCount.text = "Eating on me: -";

			energyBar.isOn = false;

			healButton.gameObject.SetActive(false);
			hurtButton.gameObject.SetActive(false);
			deleteButton.gameObject.SetActive(false);

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

			//// Sensor ...
			//if (selectedGene.sensorType == SensorTypeEnum.Effect) {
			//	effectSensorPanel.gameObject.SetActive(true);
			//	effectSensorPanel.MakeDirty();
			//}
			//// ^ Sensor ^

			if (selectedGene.isOrigin) {
				originCellPanel.gameObject.SetActive(true);
				originCellPanel.MakeDirty();

				cellBuildPriorityPanel.gameObject.SetActive(false);

			} else {
				originCellPanel.gameObject.SetActive(false);

				cellBuildPriorityPanel.gameObject.SetActive(true);
				cellBuildPriorityPanel.MakeDirty();
			}

			signalArrowHandler.MakeDirty();

			isDirty = false;
		}
	}
}