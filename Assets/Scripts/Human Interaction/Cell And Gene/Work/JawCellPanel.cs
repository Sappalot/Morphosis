using UnityEngine;
using UnityEngine.UI;

public class JawCellPanel : ComponentPanel {
	public Text prayCellCount;

	public Text cannibalizeText;

	public Toggle cannibalizeKinToggle;
	public Text cannibalizeKinText;

	public Toggle cannibalizeMotherToggle;
	public Text cannibalizeMotherText;

	public Toggle cannibalizeFatherToggle;
	public Text cannibalizeFatherText;

	public Toggle cannibalizeSiblingsToggle;
	public Text cannibalizeSiblingsText;

	public Toggle cannibalizeChildrenToggle;
	public Text cannibalizeChildrenText;

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

	public void OnChangedCannibalizeKin() {
		if (ignoreHumanInput) {
			return;
		}
		gene.jawCellCannibalizeKin = cannibalizeKinToggle.isOn;
		OnGenomeChanged();
	}

	public void OnChangedCannibalizeMother() {
		if (ignoreHumanInput) {
			return;
		}
		gene.jawCellCannibalizeMother = cannibalizeMotherToggle.isOn;
		OnGenomeChanged();
	}

	public void OnChangedCannibalizeFather() {
		if (ignoreHumanInput) {
			return;
		}
		gene.jawCellCannibalizeFather = cannibalizeFatherToggle.isOn;
		OnGenomeChanged();
	}

	public void OnChangedCannibalizeSiblings() {
		if (ignoreHumanInput) {
			return;
		}
		gene.jawCellCannibalizeSiblings = cannibalizeSiblingsToggle.isOn;
		OnGenomeChanged();
	}

	public void OnChangedCannibalizeChildren() {
		if (ignoreHumanInput) {
			return;
		}
		gene.jawCellCannibalizeChildren = cannibalizeChildrenToggle.isOn;
		OnGenomeChanged();
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			if (GetMode() == PhenoGenoEnum.Phenotype) {
				if (cellAndGenePanel.cell != null) {
					componentFooterPanel.SetProductionEffectText(selectedCell.effectProductionPredPrayUp, GlobalSettings.instance.phenotype.jawCell.effectProductionDown);
					prayCellCount.text = "Pray count: " + (cellAndGenePanel.cell as JawCell).prayCount;
					cannibalizeKinToggle.interactable = false;
					cannibalizeMotherToggle.interactable = false;
					cannibalizeFatherToggle.interactable = false;
					cannibalizeSiblingsToggle.interactable = false;
					cannibalizeChildrenToggle.interactable = false;
				}
			} else if (GetMode() == PhenoGenoEnum.Genotype) {
				componentFooterPanel.SetProductionEffectText(string.Format("Production Effect: [pray count (0...6)] * {0:F2} - {1:F2} W <color=#808080ff>(@ normal pray armor)</color>", GlobalSettings.instance.phenotype.jawCell.effectProductionUpAtSpeed.Evaluate(20f) * GlobalSettings.instance.phenotype.jawCell.effectProductionUpKeepFactor, GlobalSettings.instance.phenotype.jawCell.effectProductionDown));
				prayCellCount.text = "Pray Count: -";
				cannibalizeKinToggle.interactable = IsUnlocked();
				cannibalizeMotherToggle.interactable = IsUnlocked();
				cannibalizeFatherToggle.interactable = IsUnlocked();
				cannibalizeSiblingsToggle.interactable = IsUnlocked();
				cannibalizeChildrenToggle.interactable = IsUnlocked();
			}

			if (gene != null) {
				ignoreHumanInput = true;

				cannibalizeKinToggle.isOn = gene.jawCellCannibalizeKin;
				cannibalizeMotherToggle.isOn = gene.jawCellCannibalizeMother;
				cannibalizeFatherToggle.isOn = gene.jawCellCannibalizeFather;
				cannibalizeSiblingsToggle.isOn = gene.jawCellCannibalizeSiblings;
				cannibalizeChildrenToggle.isOn = gene.jawCellCannibalizeChildren;

				ignoreHumanInput = false;
			}

			isDirty = false;
		}
	}
}
