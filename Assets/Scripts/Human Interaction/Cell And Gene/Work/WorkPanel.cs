using UnityEngine;
using UnityEngine.UI;

public class WorkPanel : MonoBehaviour {
	public EggCellPanel eggPanel;
	public FungalCellPanel fungalPanel;
	public JawCellPanel jawPanel;
	public LeafCellPanel leafPanel;
	public MuscleCellPanel musclePanel;
	public RootCellPanel rootPanel;
	public ShellCellPanel shellPanel;
	public VeinCellPanel veinPanel;

	public NerveLocationsPanel nerveLocationsPanel;
	
	public Dropdown typeDropdown;
	public Image typeDropdownImageShow;
	public Image typeDropdownImageList;

	private ComponentPanel[] workPanels = new ComponentPanel[8];
	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;
	private bool isDirty = true;
	private bool ignoreMenuChange;

	public void Initialize(PhenoGenoEnum mode) {
		this.mode = mode;

		workPanels[0] = eggPanel;
		workPanels[1] = fungalPanel;
		workPanels[2] = jawPanel;
		workPanels[3] = leafPanel;
		workPanels[4] = musclePanel;
		workPanels[5] = rootPanel;
		workPanels[6] = shellPanel;
		workPanels[7] = veinPanel;
		foreach (ComponentPanel m in workPanels) {
			m.Initialize(mode);
		}

		typeDropdownImageShow.color = ColorScheme.instance.selectedChanged;
		typeDropdownImageList.color = ColorScheme.instance.selectedChanged;

		MakeDirty();
	}

	public void MakeDirty() {
		if (selectedGene != null) {
			workPanels[(int)selectedGene.type].MakeDirty(); // only the visible one
		}
		isDirty = true;
	}
	
	public void OnMetabolismCellTypeDropdownChanged() {
		if (ignoreMenuChange) {
			return;
		}

		if (mode == PhenoGenoEnum.Phenotype) {
			return;
		}

		selectedGene.type = (CellTypeEnum)typeDropdown.value; // keep values in order!!

		GenePanel.instance.MakeDirty();
		GenotypePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.genotype.geneCellsDiffersFromGenome = true;
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
		}
		MakeDirty();
	}



	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			if ((mode == PhenoGenoEnum.Phenotype && selectedCell == null) || (mode == PhenoGenoEnum.Genotype && selectedGene == null)) {
				// no menu
				isDirty = false;
				return;
			}

			typeDropdown.interactable = IsUnlocked();

			ignoreMenuChange = true;
			typeDropdown.value = (int)selectedGene.type;
			ignoreMenuChange = false;

			eggPanel.gameObject.SetActive(selectedGene.type == CellTypeEnum.Egg);
			fungalPanel.gameObject.SetActive(selectedGene.type == CellTypeEnum.Fungal);
			jawPanel.gameObject.SetActive(selectedGene.type == CellTypeEnum.Jaw);
			leafPanel.gameObject.SetActive(selectedGene.type == CellTypeEnum.Leaf);
			musclePanel.gameObject.SetActive(selectedGene.type == CellTypeEnum.Muscle);
			rootPanel.gameObject.SetActive(selectedGene.type == CellTypeEnum.Root);
			shellPanel.gameObject.SetActive(selectedGene.type == CellTypeEnum.Shell);
			veinPanel.gameObject.SetActive(selectedGene.type == CellTypeEnum.Vein);
		}
	}

	public ComponentPanel currentWorkPanel {
		get {
			if (cellType == CellTypeEnum.Egg) {
				return eggPanel;
			} else if (cellType == CellTypeEnum.Fungal) {
				return fungalPanel;
			} else if (cellType == CellTypeEnum.Jaw) {
				return jawPanel;
			} else if (cellType == CellTypeEnum.Leaf) {
				return leafPanel;
			} else if (cellType == CellTypeEnum.Muscle) {
				return musclePanel;
			} else if (cellType == CellTypeEnum.Root) {
				return rootPanel;
			} else if (cellType == CellTypeEnum.Shell) {
				return shellPanel;
			} else if (cellType == CellTypeEnum.Vein) {
				return veinPanel;
			}
			return null;
		}
	}

	[HideInInspector]
	public CellTypeEnum cellType {
		get {
			return selectedGene.type;
		}
	}

	public Gene selectedGene {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return CellPanel.instance.selectedCell != null ? CellPanel.instance.selectedCell.gene : null;
			} else {
				return GenePanel.instance.selectedGene;
			}
		}
	}

	public Cell selectedCell {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return CellPanel.instance.selectedCell;
			} else {
				return null; // there could be many cells selected for the same gene
			}
		}
	}

	private bool IsUnlocked() {
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome;
	}
}
