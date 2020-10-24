using UnityEngine;
using UnityEngine.UI;

public class LeafCellPanel : ComponentPanel {
	public Text exposure;
	public Text creatureSizeFactor;
	public Text overPopulationFactor;

	public LogicBoxPanel dummyLogicBoxAPanel;
	public LogicBoxPanel dummyLogicBoxBPanel;
	public SignalUnitPanel dummySensorAPanel;
	public SignalUnitPanel dummySensorBPanel;
	public SignalUnitPanel dummySensorCPanel;
	public SignalUnitPanel dummySensorDPanel;

	public override void Initialize(PhenoGenoEnum mode, CellAndGenePanel cellAndGenePanel) {
		base.Initialize(mode, cellAndGenePanel);

		// logic box A
		dummyLogicBoxAPanel.Initialize(mode, SignalUnitEnum.WorkLogicBoxA, cellAndGenePanel);
		dummyLogicBoxAPanel.isGhost = true;

		// logic box B
		dummyLogicBoxBPanel.Initialize(mode, SignalUnitEnum.WorkLogicBoxB, cellAndGenePanel);
		dummyLogicBoxBPanel.isGhost = true;

		// sensor A
		dummySensorAPanel.Initialize(mode, SignalUnitEnum.WorkSensorA, cellAndGenePanel);
		dummySensorAPanel.isGhost = true;

		// sensor B
		dummySensorBPanel.Initialize(mode, SignalUnitEnum.WorkSensorB, cellAndGenePanel);
		dummySensorBPanel.isGhost = true;

		// sensor C
		dummySensorCPanel.Initialize(mode, SignalUnitEnum.WorkSensorC, cellAndGenePanel);
		dummySensorCPanel.isGhost = true;

		// sensor D
		dummySensorDPanel.Initialize(mode, SignalUnitEnum.WorkSensorD, cellAndGenePanel);
		dummySensorDPanel.isGhost = true;

		MakeDirty();
	}

	private void Update() {
		if (isDirty) {
			if (selectedCell == null) {
				return;
			}

			if (GlobalSettings.instance.debug.debugLogMenuUpdate) {
				Debug.Log("Update CellPanel");
			}

			if (GetMode() == PhenoGenoEnum.Phenotype) {
				if (cellAndGenePanel.cell != null) {
					componentFooterPanel.SetProductionEffectText(selectedCell.effectProductionInternalUp, GlobalSettings.instance.phenotype.leafCell.effectProductionDown);

					exposure.text = string.Format("Exposure: {0:F2}%", (selectedCell as LeafCell).lowPassExposure * 100f);
					creatureSizeFactor.text = string.Format("Creature size factor: {0:F2}%", GlobalSettings.instance.phenotype.leafCell.exposureFactorAtBodySize.Evaluate(selectedCell.creature.cellCount) * 100f);
					overPopulationFactor.text = string.Format("Over population factor: {0:F2}%", GlobalSettings.instance.phenotype.leafCell.exposureFactorAtPopulation.Evaluate(World.instance.life.cellAliveCount) * 100f);
				}
			} else if (GetMode() == PhenoGenoEnum.Genotype) {
				componentFooterPanel.SetProductionEffectText(string.Format("Production Effect: [exposure (0...1)] * {0:F2} - {0:F2} W", GlobalSettings.instance.phenotype.leafCell.effectProductionUpMax, GlobalSettings.instance.phenotype.leafCell.effectProductionDown));

				exposure.text = "Exposure : -";
				creatureSizeFactor.text = "Creature size factor: -";
				overPopulationFactor.text = "Over population factor: -";
			}

			if (selectedGene != null) {
				ignoreHumanInput = true;

				ignoreHumanInput = false;
			}

			isDirty = false;
		}
	}
}
