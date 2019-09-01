using UnityEngine;
using UnityEngine.UI;

public class MuscleCellPanel : MetabolismCellPanel {
	public Text productionEffectText;

	public HibernatePanel hibernatePanel;

	public Text frequenzy;

	public override void Initialize(PhenoGenoEnum mode) {
		hibernatePanel.SetMode(mode);
		base.Initialize(mode);
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			if (GetMode() == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					productionEffectText.text = productionEffectPhenotypeString;
					frequenzy.text = string.Format("Frequenzy: {0:F2} Hz", selectedCell.creature.phenotype.originCell.originPulseFequenzy);
				}
			} else if (GetMode() == PhenoGenoEnum.Genotype) {
				productionEffectText.text = string.Format("Production Effect: 0.00 - {0:F2} W - {1:F2} J per contraction", GlobalSettings.instance.phenotype.cellHibernateEffectCost, GlobalSettings.instance.phenotype.muscleCellEnergyCostPerContraction);

				frequenzy.text = string.Format("Frequenzy: -");
			}

			if (selectedGene != null) {
				ignoreSliderMoved = true;

				ignoreSliderMoved = false;
			}

			// Subpanels
			hibernatePanel.MakeDirty();

			isDirty = false; 
		}
	}
}
