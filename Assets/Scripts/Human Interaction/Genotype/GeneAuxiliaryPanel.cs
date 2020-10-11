using UnityEngine;
using UnityEngine.UI;

public class GeneAuxiliaryPanel : MonoSingleton<GeneAuxiliaryPanel> {
	public Text typeHeadingLabel;
	public CellAndGenePanel cellAndGenePanel;

	private Gene m_viewedGene;
	public Gene viewedGene {
		get {
			return m_viewedGene;
		}
		set {
			m_viewedGene = value;
			MakeDirty();
		}
	}

	private Cell m_viewedGeneCell; // So we override which cell is used as well, though we are dealing with genes (build priority in genotypePanel depends on cell)
	public Cell viewedCell {
		get {
			return m_viewedGeneCell;
		}
		set {
			m_viewedGeneCell = value;
			MakeDirty();
		}
	}

	public void Initialize() {
		cellAndGenePanel.Initialize(PhenoGenoEnum.Genotype, true);
		MakeDirty();
	}

	private bool isDirty = true;
	public void MakeDirty() {
		isDirty = true;
		cellAndGenePanel.MakeDirty();
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				DebugUtil.Log("Update GeneCellPanel");
			}
			if (viewedGene == null || !CreatureSelectionPanel.instance.hasSoloSelected) {
				typeHeadingLabel.text = "Auxiliary Gene:";
			} else {
				typeHeadingLabel.text = "Auxiliary Gene: " + viewedGene.type.ToString();
			}
			isDirty = false;
		}
	}
}