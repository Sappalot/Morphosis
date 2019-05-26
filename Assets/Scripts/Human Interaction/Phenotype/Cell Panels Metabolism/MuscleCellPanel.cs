using UnityEngine;
using UnityEngine.UI;

public class MuscleCellPanel : MetabolismCellPanel {
	public Text productionEffectText;

	public HibernatePanel hibernatePanel;

	public Text frequenzy;

	public override void SetMode(PhenoGenoEnum mode) {
		hibernatePanel.SetMode(mode);
		base.SetMode(mode);
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
				float fMin = GlobalSettings.instance.phenotype.originPulseFrequenzyMin;
				float fMax = GlobalSettings.instance.phenotype.originPulseFrequenzyMax;
				productionEffectText.text = string.Format("Production Effect: 0.00 - [muscle frequenzy ({0:F2}...{1:F2})] * {2:F2} W", fMin, fMax, GlobalSettings.instance.phenotype.muscleCellEffectCostPerHz);

				frequenzy.text = string.Format("Frequenzy: -");
				frequenzy.color = ColorScheme.instance.grayedOutPhenotype;
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
