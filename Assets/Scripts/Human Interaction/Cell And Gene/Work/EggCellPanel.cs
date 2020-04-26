using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EggCellPanel : ComponentPanel {
	public Button fertilizeButton;

	public LogicBoxPanel fertilizeLogicBoxPanel;
	public EnergySensorPanel fertilizeEnergySensorPanel;
	public AttachmentSensorPanel fertilizeAttachmentSensorPanel;

	public override void Initialize(PhenoGenoEnum mode, CellAndGenePanel cellAndGenePanel) {
		fertilizeLogicBoxPanel.Initialize(mode, SignalUnitEnum.WorkLogicBoxA, cellAndGenePanel);
		fertilizeEnergySensorPanel.Initialize(mode, SignalUnitEnum.WorkSensorA, cellAndGenePanel);
		fertilizeAttachmentSensorPanel.Initialize(mode, SignalUnitEnum.WorkSensorB, cellAndGenePanel);

		MakeDirty();

		base.Initialize(mode, cellAndGenePanel);
	}

	public override void MakeDirty() {
		base.MakeDirty();
		fertilizeLogicBoxPanel.MakeDirty();
		fertilizeEnergySensorPanel.MakeDirty();
		fertilizeAttachmentSensorPanel.MakeDirty();
	}

	public override List<IGeneInput> GetAllGeneInputs() {
		return fertilizeLogicBoxPanel.GetAllGeneInputs();
	}

	public void OnClickFertilize() {
		if (CreatureSelectionPanel.instance.hasSoloSelected && GetMode() == PhenoGenoEnum.Phenotype) {
			World.instance.life.FertilizeCreature(cellAndGenePanel.cell, true, World.instance.worldTicks, true);
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
			componentFooterPanel.SetProductionEffectText(0f, GlobalSettings.instance.phenotype.eggCell.effectProductionDown);

			fertilizeLogicBoxPanel.outputText = "Fertilize asexually";

			isDirty = false;
		}
	}
}