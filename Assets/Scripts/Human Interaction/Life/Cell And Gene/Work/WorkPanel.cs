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

	private CellAndGenePanel cellAndGenePanel;

	public void Initialize(PhenoGenoEnum mode, CellAndGenePanel cellAndGenePanel) {
		this.mode = mode;
		this.cellAndGenePanel = cellAndGenePanel;

		workPanels[0] = eggPanel;
		workPanels[1] = fungalPanel;
		workPanels[2] = jawPanel;
		workPanels[3] = leafPanel;
		workPanels[4] = musclePanel;
		workPanels[5] = rootPanel;
		workPanels[6] = shellPanel;
		workPanels[7] = veinPanel;
		foreach (ComponentPanel m in workPanels) {
			m.Initialize(mode, cellAndGenePanel);
		}

		typeDropdownImageShow.color = ColorScheme.instance.selectedChanged;
		typeDropdownImageList.color = ColorScheme.instance.selectedChanged;

		MakeDirty();
	}

	public ViewXput? viewXput {
		get {
			if (gene != null) {
				return workPanels[(int)gene.type].viewXput;
			}
			return null;
		}
	}

	public void MakeDirty() {
		if (gene != null) {
			workPanels[(int)gene.type].MakeDirty(); // only the visible one
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

		gene.type = (CellTypeEnum)typeDropdown.value; // keep values in order!!

		GenePanel.instance.MakeDirty();
		GenotypePanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			//CreatureSelectionPanel.instance.soloSelected.genotype.isGeneCellPatternDirty = true;
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
		}
		MakeDirty();
	}



	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.debug.debugLogMenuUpdate) {
				Debug.Log("Update CellPanel");
			}

			if ((mode == PhenoGenoEnum.Phenotype && selectedCell == null) || (mode == PhenoGenoEnum.Genotype && gene == null)) {
				// no menu
				isDirty = false;
				return;
			}

			typeDropdown.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

			ignoreMenuChange = true;
			typeDropdown.value = (int)gene.type;
			ignoreMenuChange = false;

			eggPanel.gameObject.SetActive(gene.type == CellTypeEnum.Egg);
			fungalPanel.gameObject.SetActive(gene.type == CellTypeEnum.Fungal);
			jawPanel.gameObject.SetActive(gene.type == CellTypeEnum.Jaw);
			leafPanel.gameObject.SetActive(gene.type == CellTypeEnum.Leaf);
			musclePanel.gameObject.SetActive(gene.type == CellTypeEnum.Muscle);
			rootPanel.gameObject.SetActive(gene.type == CellTypeEnum.Root);
			shellPanel.gameObject.SetActive(gene.type == CellTypeEnum.Shell);
			veinPanel.gameObject.SetActive(gene.type == CellTypeEnum.Vein);
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
			return gene.type;
		}
	}

	public Gene gene {
		get {
			return cellAndGenePanel.gene;
		}
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

	private bool IsUnlocked() {
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome && !cellAndGenePanel.isAuxiliary;
	}
}
