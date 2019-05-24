using UnityEngine;
using UnityEngine.UI;

public class MuscleCellPanel : MetabolismCellPanel {
	public Text productionEffectText;

	public Text frequenzy;

	public Text idleWhenAttachedText;
	public Toggle idleWhenAttachedToggle;

	public void OnIdleWhenAttachedToggleChanged() {
		if (ignoreSliderMoved) {
			return;
		}

		selectedGene.muscleCellIdleWhenAttached = idleWhenAttachedToggle.isOn;
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			MakeCreatureChanged();
		}
		MakeDirty();
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			if (mode == PhenoGenoEnum.Phenotype) {
				if (CellPanel.instance.selectedCell != null) {
					productionEffectText.text = productionEffectPhenotypeString;
					frequenzy.text = string.Format("Frequenzy: {0:F2} Hz", selectedCell.creature.phenotype.originCell.originPulseFequenzy);

					idleWhenAttachedText.color = ColorScheme.instance.grayedOutGenotype;
					idleWhenAttachedToggle.interactable = false;
				}
			} else if (mode == PhenoGenoEnum.Genotype) {
				float fMin = GlobalSettings.instance.phenotype.originPulseFrequenzyMin;
				float fMax = GlobalSettings.instance.phenotype.originPulseFrequenzyMax;
				productionEffectText.text = string.Format("Production Effect: 0.00 - [muscle frequenzy ({0:F2}...{1:F2})] * {2:F2} W", fMin, fMax, GlobalSettings.instance.phenotype.muscleCellEffectCostPerHz);

				frequenzy.text = string.Format("Frequenzy: -");
				frequenzy.color = ColorScheme.instance.grayedOutPhenotype;

				idleWhenAttachedText.color = Color.black;
				idleWhenAttachedToggle.interactable = isUnlocked();
			}


			if (selectedGene != null) {
				ignoreSliderMoved = true;

				idleWhenAttachedToggle.isOn = selectedGene.muscleCellIdleWhenAttached;

				ignoreSliderMoved = false;
			}

			isDirty = false; 
		}
	}
}
