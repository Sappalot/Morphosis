using UnityEngine;

public abstract class SurroundingSensorChannelSensorPanel : MonoBehaviour {
	public string shortName;

	[HideInInspector]
	protected PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;
	[HideInInspector]
	public bool isGhost = false; // Can't be used for this gene/geneCell (will be grayed out)

	protected bool ignoreHumanInput = false;
	protected bool isDirty = false;
	protected CellAndGenePanel cellAndGenePanel;
	protected SurroundingSensorPanel motherPanel;

	virtual public void Initialize(PhenoGenoEnum mode, CellAndGenePanel cellAndGenePanel, SurroundingSensorPanel motherPanel) {
		this.mode = mode;
		this.cellAndGenePanel = cellAndGenePanel;
		this.motherPanel = motherPanel;
	}

	public virtual void MakeDirty() {
		isDirty = true;
	}

	public bool IsUnlocked() {
		return CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome && !cellAndGenePanel.isAuxiliary;
	}

	public void OnGenomeChanged() {
		CreatureSelectionPanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
		MakeDirty();
	}
}