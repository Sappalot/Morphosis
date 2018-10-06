using UnityEngine;
using UnityEngine.UI;

public class GenePanel : MonoSingleton<GenePanel> {

	public Text geneTypeHeading;

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

	override public void Init() {
		isDirty = true;
	}

	private bool isDirty = true;
	public void MakeDirty() {
		isDirty = true;
		GeneCellPanel.instance.MakeDirty();
		GeneNeighboursPanel.instance.MakeDirty();
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update GenePanel");
			}


			//Nothing to represent
			if (selectedGene == null || !CreatureSelectionPanel.instance.hasSoloSelected) {
				geneTypeHeading.text = "Gene";
				isDirty = false;
				return;
			}

			geneTypeHeading.text = "Gene: " + selectedGene.type.ToString();


			isDirty = false;
		}
	} 
}