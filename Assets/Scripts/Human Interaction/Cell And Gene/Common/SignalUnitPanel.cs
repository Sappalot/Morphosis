using System;
using System.Collections.Generic;
using UnityEngine;

// a panel that can change genotype and handles signals

// Me <== LogicBoxPanel, AxonPanel, ConstantSensorPanel, EnergySensorPanel, EffectSensorPanel, AttachmentSensorPanel, SizeSensorPanel
public abstract class SignalUnitPanel : ComponentPanel {
	[HideInInspector]
	public bool isGhost = false; // Can't be used for this gene/geneCell (will be grayed out)

	public SignalLocations locations = new SignalLocations();

	public OutputPanel[] outputPanels; // TODO: Make LogixBoxPanel and AxonPanel use output panel (also make all output panel handle early and late (late = early foor leaf node sensors))

	public RectTransform settingsPanel; // Not all sensor panels use settings panel

	[HideInInspector]
	public SignalUnitEnum signalUnitEnum;

	public virtual void Initialize(PhenoGenoEnum mode, SignalUnitEnum signalUnitEnum, CellAndGenePanel cellAndGenePanel) {
		base.Initialize(mode, cellAndGenePanel);
		if (viewOutputPanel != null) {
			viewOutputPanel.Initialize(mode, XputEnum.Output, signalUnitEnum);
		}
		

		this.signalUnitEnum = signalUnitEnum;

		if (settingsPanel != null) {
			settingsPanel.gameObject.SetActive(true); // Not all sensor panels use settings panel
		}

		for (int i = 0; i < outputPanels.Length; i++) {
			outputPanels[i].Initialize(mode, signalUnitEnum, IndexToSignalUnitSlotEnum(i), this, cellAndGenePanel);
		}

		MakeDirty();
	}

	public ViewXputPanel viewOutputPanel;
	public ViewXputPanel viewInputPanel;

	public override ViewXput? viewXput {
		get {
			if (viewOutputPanel != null && viewOutputPanel.isMouseHovering) {
				return viewOutputPanel.xput;
			}
			if (viewInputPanel != null && viewInputPanel.isMouseHovering) {
				return viewInputPanel.xput;
			}
			return null;
		}
	}

	[Serializable]
	public struct SignalLocations {
		public RectTransform inputA;
		public RectTransform inputB;
		public RectTransform inputC;
		public RectTransform inputD;
		public RectTransform inputE;
		public RectTransform inputF;

		public RectTransform outputEarlyA;
		public RectTransform outputLateA;

		public RectTransform outputEarlyB;
		public RectTransform outputLateB;

		public RectTransform outputEarlyC;
		public RectTransform outputLateC;

		public RectTransform outputEarlyD;
		public RectTransform outputLateD;

		public RectTransform outputEarlyE;
		public RectTransform outputLateE;

		public RectTransform outputEarlyF;
		public RectTransform outputLateF;
	}

	public RectTransform GetLocation(SignalUnitSlotEnum slot) {
		if (slot == SignalUnitSlotEnum.inputA) {
			return locations.inputA;
		} else if (slot == SignalUnitSlotEnum.inputB) {
			return locations.inputB;
		} else if (slot == SignalUnitSlotEnum.inputC) {
			return locations.inputC;
		} else if (slot == SignalUnitSlotEnum.inputD) {
			return locations.inputD;
		} else if (slot == SignalUnitSlotEnum.inputE) {
			return locations.inputE;
		} else if (slot == SignalUnitSlotEnum.inputF) {
			return locations.inputF;
		} else if (slot == SignalUnitSlotEnum.outputEarlyA) {
			return locations.outputEarlyA;
		} else if (slot == SignalUnitSlotEnum.outputLateA) {
			return locations.outputLateA;
		} else if (slot == SignalUnitSlotEnum.outputEarlyB) {
			return locations.outputEarlyB;
		} else if (slot == SignalUnitSlotEnum.outputLateB) {
			return locations.outputLateB;
		} else if (slot == SignalUnitSlotEnum.outputEarlyC) {
			return locations.outputEarlyC;
		} else if (slot == SignalUnitSlotEnum.outputLateC) {
			return locations.outputLateC;
		} else if (slot == SignalUnitSlotEnum.outputEarlyD) {
			return locations.outputEarlyD;
		} else if (slot == SignalUnitSlotEnum.outputLateD) {
			return locations.outputLateD;
		} else if (slot == SignalUnitSlotEnum.outputEarlyE) {
			return locations.outputEarlyE;
		} else if (slot == SignalUnitSlotEnum.outputLateE) {
			return locations.outputLateE;
		} else if (slot == SignalUnitSlotEnum.outputEarlyF) {
			return locations.outputEarlyF;
		} else if (slot == SignalUnitSlotEnum.outputLateF) {
			return locations.outputLateF;
		}
		return null;
	}

	public SignalUnitSlotEnum IndexToSignalUnitSlotEnum(int index) {
		if (index == 0) {
			return SignalUnitSlotEnum.outputLateA;
		} else if (index == 1) {
			return SignalUnitSlotEnum.outputLateB;
		} else if (index == 2) {
			return SignalUnitSlotEnum.outputLateC;
		} else if (index == 3) {
			return SignalUnitSlotEnum.outputLateD;
		} else if (index == 4) {
			return SignalUnitSlotEnum.outputLateE;
		} else if (index == 5) {
			return SignalUnitSlotEnum.outputLateF;
		}
		return SignalUnitSlotEnum.outputLateA; // error
	}

	public SignalUnit affectedSignalUnit {
		get {
			if (selectedCell != null) {
				return selectedCell.GetSignalUnit(signalUnitEnum);
			}

			return null;
		}
	}

	public GeneSignalUnit affectedGeneSignalUnit {
		get {
			if (selectedGene != null) {
				return selectedGene.GetGeneSignalUnit(signalUnitEnum);
			}

			return null;
		}
	}

	public SignalUnit anyAffectedSignalUnitGenotype {
		get {
			Creature soloSelected = CreatureSelectionPanel.instance.soloSelected;
			if (soloSelected == null) {
				return null;
			}
			List<Cell> geneCell = soloSelected.genotype.GetGeneCellsWithGene(selectedGene);
			return geneCell[0].GetSignalUnit(signalUnitEnum);

		}
	}

	public List<SignalUnit> allAffectedSignalUnitsGenotype {
		get {
			Creature soloSelected = CreatureSelectionPanel.instance.soloSelected;
			if (soloSelected == null) {
				return null;
			}
			List<Cell> geneCells = soloSelected.genotype.GetGeneCellsWithGene(selectedGene);
			List<SignalUnit> signalUnits = new List<SignalUnit>();
			foreach (Cell geneCell in geneCells) {
				signalUnits.Add(geneCell.GetSignalUnit(signalUnitEnum));
			}
			return signalUnits;
		}
	}

	public bool isAnyAffectedSignalUnitsRootedGenotype {
		get {
			if (allAffectedSignalUnitsGenotype != null) {
				return allAffectedSignalUnitsGenotype.Exists(r => r.rootnessEnum == RootnessEnum.Rooted);
			}
			return false;
		}
		
	}

	public virtual void Update() {
		if (isDirty) {
			if (settingsPanel != null) {
				settingsPanel.gameObject.SetActive(!isGhost);
			}

			for (int i = 0; i < outputPanels.Length; i++) {
				outputPanels[i].isGhost = isGhost;
				outputPanels[i].MakeDirty();
			}

			if (viewOutputPanel != null) {
				viewOutputPanel.isGhost = isGhost;
				viewOutputPanel.MakeDirty();
			}

			if (viewInputPanel != null) {
				viewInputPanel.isGhost = isGhost;
				viewInputPanel.MakeDirty();
			}

			isDirty = false;
		}
	}
}
