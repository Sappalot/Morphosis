﻿using UnityEngine;
using UnityEngine.UI;

public class RootCellPanel : ComponentPanel {

	public LogicBoxPanel dummyLogicBoxAPanel;
	public LogicBoxPanel dummyLogicBoxBPanel;
	public SensorPanel dummySensorAPanel;
	public SensorPanel dummySensorBPanel;
	public SensorPanel dummySensorCPanel;
	public SensorPanel dummySensorDPanel;

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
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			componentFooterPanel.SetProductionEffectText(0f, GlobalSettings.instance.phenotype.rootCell.effectProductionDown);

			isDirty = false; 
		}
	}
}
