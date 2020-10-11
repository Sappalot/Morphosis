using UnityEngine;
using UnityEngine.UI;

public class MuscleCellPanel : ComponentPanel {
	public Text frequenzy;

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
			if (GlobalSettings.instance.debug.debugLogMenuUpdate) {
				DebugUtil.Log("Update CellPanel");
			}

			if (GetMode() == PhenoGenoEnum.Phenotype) {
				if (cellAndGenePanel.cell != null) {
					componentFooterPanel.SetProductionEffectText(0f, selectedCell.effectProductionInternalDown);
					frequenzy.text = string.Format("Frequenzy: {0:F2} Hz", selectedCell.creature.phenotype.originCell.originPulseFequenzy);
				}
			} else if (GetMode() == PhenoGenoEnum.Genotype) {
				componentFooterPanel.SetProductionEffectText(string.Format("Production Effect: -{0:F2} W (- {1:F2} J per contraction)", GlobalSettings.instance.phenotype.muscleCell.effectProductionDown, GlobalSettings.instance.phenotype.muscleCell.energyProductionDownPerContraction));

				frequenzy.text = string.Format("Frequenzy: -");
			}

			if (selectedGene != null) {
				ignoreHumanInput = true;

				ignoreHumanInput = false;
			}

			isDirty = false;
		}
	}
}
