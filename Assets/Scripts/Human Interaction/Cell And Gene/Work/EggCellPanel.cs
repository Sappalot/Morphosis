using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EggCellPanel : ComponentPanel {
	public Button fertilizeButton;

	public LogicBoxPanel fertilizeLogicBoxPanel;
	public LogicBoxPanel dummyLogicBoxBPanel;
	public EnergySensorPanel fertilizeEnergySensorPanel;
	public AttachmentSensorPanel fertilizeAttachmentSensorPanel;
	public SignalUnitPanel dummySensorCPanel;
	public SignalUnitPanel dummySensorDPanel;


	public override void Initialize(PhenoGenoEnum mode, CellAndGenePanel cellAndGenePanel) {
		base.Initialize(mode, cellAndGenePanel);

		// logic box A
		fertilizeLogicBoxPanel.Initialize(mode, SignalUnitEnum.WorkLogicBoxA, cellAndGenePanel);
		fertilizeLogicBoxPanel.isGhost = false;

		// logic box B
		dummyLogicBoxBPanel.Initialize(mode, SignalUnitEnum.WorkLogicBoxB, cellAndGenePanel);
		dummyLogicBoxBPanel.isGhost = true;

		// sensor A
		fertilizeEnergySensorPanel.Initialize(mode, SignalUnitEnum.WorkSensorA, cellAndGenePanel);
		fertilizeEnergySensorPanel.isGhost = false;

		// sensor B
		fertilizeAttachmentSensorPanel.Initialize(mode, SignalUnitEnum.WorkSensorB, cellAndGenePanel);
		fertilizeAttachmentSensorPanel.isGhost = false;

		// sensor C
		dummySensorCPanel.Initialize(mode, SignalUnitEnum.WorkSensorC, cellAndGenePanel);
		dummySensorCPanel.isGhost = true;

		// sensor D
		dummySensorDPanel.Initialize(mode, SignalUnitEnum.WorkSensorD, cellAndGenePanel);
		dummySensorDPanel.isGhost = true;

		MakeDirty();
	}

	public override ViewXput? viewXput {
		get {
			if (fertilizeLogicBoxPanel.viewXput != null) {
				return fertilizeLogicBoxPanel.viewXput;
			}

			if (fertilizeEnergySensorPanel.viewXput != null) {
				return fertilizeEnergySensorPanel.viewXput;
			}

			if (fertilizeAttachmentSensorPanel.viewXput != null) {
				return fertilizeAttachmentSensorPanel.viewXput;
			}

			return null;
		}
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
			if (GlobalSettings.instance.debug.debugLogMenuUpdate) {
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