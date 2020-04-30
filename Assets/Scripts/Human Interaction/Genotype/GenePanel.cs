using UnityEngine;
using UnityEngine.UI;

public class GenePanel : MonoSingleton<GenePanel> {
	public Text typeHeadingLabel;
	public CellAndGenePanel cellAndGenePanel;

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

	public void Initialize() {
		cellAndGenePanel.Initialize(PhenoGenoEnum.Genotype, false);
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
				Debug.Log("Update GeneCellPanel");
			}
			if (selectedGene == null || !CreatureSelectionPanel.instance.hasSoloSelected) {
				typeHeadingLabel.text = "Gene:";
			} else {
				typeHeadingLabel.text = "Gene: " + selectedGene.type.ToString();
			}
			isDirty = false;
		}
	}
}