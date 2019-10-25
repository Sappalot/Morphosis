using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EggCellPanel : CellAndGeneComponentPanel {
	public Button fertilizeButton;

	public LogicBoxPanel fertilizeLogicBoxPanel;
	public EnergySensorPanel fertilizeEnergySensorPanel;
	public AttachmentSensorPanel fertilizeAttachmentSensorPanel;

	public override void Initialize(PhenoGenoEnum mode) {
		fertilizeLogicBoxPanel.Initialize(mode, SignalUnitEnum.WorkLogicBoxA);
		fertilizeEnergySensorPanel.Initialize(mode, SignalUnitEnum.WorkSensorA);
		fertilizeAttachmentSensorPanel.Initialize(mode, SignalUnitEnum.WorkSensorB);

		MakeDirty();

		base.Initialize(mode);
	}

	public override void MakeDirty() {
		base.MakeDirty();
		fertilizeLogicBoxPanel.MakeDirty();
		fertilizeEnergySensorPanel.MakeDirty();
		fertilizeAttachmentSensorPanel.MakeDirty();
	}

	public override List<GeneLogicBoxInput> GetAllGeneGeneLogicBoxInputs() {
		return fertilizeLogicBoxPanel.GetAllGeneGeneLogicBoxInputs();
	}

	public void OnClickFertilize() {
		if (CreatureSelectionPanel.instance.hasSoloSelected && GetMode() == PhenoGenoEnum.Phenotype) {
			World.instance.life.FertilizeCreature(CellPanel.instance.selectedCell, true, World.instance.worldTicks, true);
		}
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			if (GetMode() == PhenoGenoEnum.Phenotype) {
				fertilizeButton.gameObject.SetActive(true);
			} else if (GetMode() == PhenoGenoEnum.Genotype) {
				fertilizeButton.gameObject.SetActive(false);
			}
			footerPanel.SetProductionEffectText(0f, GlobalSettings.instance.phenotype.eggCellEffectCost);

			fertilizeLogicBoxPanel.outputText = "Fertilize asexually";

			isDirty = false;
		}
	}
}