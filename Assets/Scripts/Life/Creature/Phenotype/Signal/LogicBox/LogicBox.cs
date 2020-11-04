using System.Collections.Generic;

public class LogicBox : SignalUnit {
	private bool outputLate; // phenotype
	private bool outputEarly; // phenotype
	
	public Nerve[] inputNerves = new Nerve[GeneLogicBox.columnCount]; // a, b, c, d, e, f

	public LogicBox(SignalUnitEnum signalUnitEnum, Cell hostCell) : base(hostCell) {
		base.signalUnitEnum = signalUnitEnum;
		for (int i = 0; i < GeneLogicBox.columnCount; i++) {
			inputNerves[i] = new Nerve();
		}
	}

	public override void UpdateInputNervesGenotype(Genotype genotype) {
		GeneLogicBox geneLogicBox = (GeneLogicBox)hostCell.gene.GetGeneSignalUnit(signalUnitEnum);
		for (int i = 0; i < GeneLogicBox.columnCount; i++) {
			if (geneLogicBox.GetInput(i).valveMode == SignalValveModeEnum.Pass || geneLogicBox.GetInput(i).valveMode == SignalValveModeEnum.PassInverted) {
				inputNerves[i].headCell = hostCell;
				inputNerves[i].headSignalUnitEnum = signalUnitEnum;
				inputNerves[i].headSignalUnitSlotEnum = SignalUnit.IndexToSignalInputSlotUnit(i);

				GeneNerve geneNerve = geneLogicBox.GetInput(i).geneNerve;
				inputNerves[i].tailSignalUnitEnum = geneNerve.tailUnitEnum;
				inputNerves[i].tailSignalUnitSlotEnum = geneNerve.tailUnitSlotEnum;
				if (geneNerve.isLocal) {
					inputNerves[i].tailCell = hostCell;
					inputNerves[i].nerveStatusEnum = NerveStatusEnum.InputLocal;
				} else {
					inputNerves[i].toTailVector = geneNerve.nerveVector;
					inputNerves[i].tailCell = GeneNerve.GetGeneCellAtNerveTail(hostCell, geneNerve, genotype);

					inputNerves[i].nerveStatusEnum = NerveStatusEnum.InputExternal;
				}
			} else {
				// blocked input ==> void nerve
				inputNerves[i].nerveStatusEnum = NerveStatusEnum.Void;
			}
		}
	}

	// Assume all input nerves are updated at this stage
	public override void RootRecursivlyGenotypePhenotype(Nerve nerve, bool addOutputNere) {
		bool wasAllreadyRooted = rootnessEnum == RootnessEnum.Rooted;
		base.RootRecursivlyGenotypePhenotype(nerve, addOutputNere); // roots me!

		if (wasAllreadyRooted) {
			return;
		}

		// reach out through input nerves
		for (int i = 0; i < GeneLogicBox.columnCount; i++) {
			if (inputNerves[i].nerveStatusEnum == NerveStatusEnum.InputLocal) {
				// ask to which this genes unit where tail is "pointing"
				SignalUnit childSignalUnit = hostCell.GetSignalUnit(inputNerves[i].tailSignalUnitEnum);
				if (childSignalUnit != null) {
					childSignalUnit.RootRecursivlyGenotypePhenotype(inputNerves[i], addOutputNere);
				}
			} else if (inputNerves[i].nerveStatusEnum == NerveStatusEnum.InputExternal) {
				// ask external unit where tail is pointing
				Cell childCell = inputNerves[i].tailCell;
				if (childCell != null) {
					SignalUnit childSignalUnit = childCell.GetSignalUnit(inputNerves[i].tailSignalUnitEnum);
					if (childSignalUnit != null) {
						childSignalUnit.RootRecursivlyGenotypePhenotype(inputNerves[i], addOutputNere);
					}
				}
			}
		}
	}

	public override List<Nerve> GetAllNervesGenotypePhenotype() {
		List<Nerve> nerves = new List<Nerve>();
		foreach (Nerve n in inputNerves) {
			if (n.nerveStatusEnum != NerveStatusEnum.Void) {
				nerves.Add(n);
			}
		}
		nerves.AddRange(base.GetAllNervesGenotypePhenotype());
		return nerves;
	}

	public override List<Nerve> GetInputNervesGenotypePhenotype() {
		List<Nerve> nerves = new List<Nerve>();
		foreach (Nerve n in inputNerves) {
			if (n.nerveStatusEnum != NerveStatusEnum.Void) {
				nerves.Add(n);
			}
		}
		return nerves;
	}

	public override void CloneNervesFromGenotypeToPhenotype(Cell geneCell, Phenotype phenotype) {
		base.CloneNervesFromGenotypeToPhenotype(geneCell, phenotype);
		// output in base ^

		// input only
		Nerve[] inputNervesGenotype = ((LogicBox)geneCell.GetSignalUnit(signalUnitEnum)).inputNerves;

		for (int i = 0; i < inputNerves.Length; i++) {
			inputNerves[i].Set(inputNervesGenotype[i]);
			inputNerves[i].headCell = hostCell;

			if (inputNervesGenotype[i].tailCell != null) {
				inputNerves[i].tailCell = phenotype.GetCellAtMapPosition(inputNervesGenotype[i].tailCell.mapPosition);
			} else {
				inputNerves[i].tailCell = null;
			}
		}
	}

	public override void Clear() {
		outputEarly = false;
		outputLate = false;
	}

	public override bool GetOutput(SignalUnitSlotEnum signalUnitSlot) {
		if (signalUnitSlot == SignalUnitSlotEnum.outputEarlyA) {
			return outputEarly;
		} else if (signalUnitSlot == SignalUnitSlotEnum.outputLateA) {
			return outputLate;
		}
		return false;
	}

	public override void FeedSignal() {
		outputLate = outputEarly;
	}

	public override void ComputeSignalOutput(int deltaTicks) {
		if (rootnessEnum != RootnessEnum.Rooted) {
			return;
		}

		if (hostCell.GetCellType() == CellTypeEnum.Egg && signalUnitEnum == SignalUnitEnum.WorkLogicBoxA) {
			outputEarly = ThroughGates(hostCell.gene.eggCellFertilizeLogic);
		} else if (signalUnitEnum == SignalUnitEnum.DendritesLogicBox) {
			outputEarly = ThroughGates(hostCell.gene.dendritesLogicBox);
		} else if (signalUnitEnum == SignalUnitEnum.OriginDetatchLogicBox) {
			outputEarly = ThroughGates(hostCell.gene.originDetatchLogicBox);
		}
	}

	private bool ThroughGates(GeneLogicBox geneLogicBox) {
		if (rootnessEnum != RootnessEnum.Rooted) {
			return false;
		}
		return HasSignalPostGate(geneLogicBox.GetGate(0, 0), hostCell);
	}

	// TODO: find out a way not to use static functions here
	// we need to know what logic box we are talking to in panel as we want to update signal colors

	// A gate is doing some logic, this method figures out wheter it has signal on/or off depending on gate and input
	// 2020 fall - added NAND and NOR simply by reversing the return values (if NAND or NOR) :)
	public static bool HasSignalPostGate(GeneLogicBoxGate gate, Cell hostCell) {
		if (gate.row == 0 && !gate.isTransmittingSignal) {
			return false; // If we are not transmitting anything through top gate, this box is useless. undefined but let's just turn output off
		}

		if (gate.operatorType == LogicOperatorEnum.And || gate.operatorType == LogicOperatorEnum.Nand) {
			foreach (GeneLogicBoxPart nextPart in gate.partsConnected) {
				if (nextPart.isTransmittingSignal) {
					if (nextPart is GeneLogicBoxInput) {
						if ((nextPart as GeneLogicBoxInput).valveMode == SignalValveModeEnum.Pass) {
							// TODO: find easier way to access nerve
							Nerve inputNerve = ((LogicBox)hostCell.GetSignalUnit(gate.geneLogicBox.signalUnit)).inputNerves[(nextPart as GeneLogicBoxInput).column];
							if (inputNerve.tailSignalUnitEnum == SignalUnitEnum.Void || inputNerve.tailCell == null || !inputNerve.tailCell.GetOutputFromUnit(inputNerve.tailSignalUnitEnum, inputNerve.tailSignalUnitSlotEnum)) {
								// next part turned out to be a pass input valve with its input off
								return gate.operatorType == LogicOperatorEnum.And ? false : true; // one off ==> AND is off :( (...then a layer of inversion on top when NAND)
							}
						} else if ((nextPart as GeneLogicBoxInput).valveMode == SignalValveModeEnum.PassInverted) {
							Nerve inputNerve = ((LogicBox)hostCell.GetSignalUnit(gate.geneLogicBox.signalUnit)).inputNerves[(nextPart as GeneLogicBoxInput).column];
							if (inputNerve.tailSignalUnitEnum != SignalUnitEnum.Void && inputNerve.tailCell != null && inputNerve.tailCell.GetOutputFromUnit(inputNerve.tailSignalUnitEnum, inputNerve.tailSignalUnitSlotEnum)) {
								// next part turned out to be a pass INVERTED input valve with its input on
								return gate.operatorType == LogicOperatorEnum.And ? false : true; // on ==Inverted==> off ... one off ==> AND is off :(   (...then a layer of inversion on top when NAND)
							}
						}
						// what if we have an passInverted valve 
					} else if (nextPart is GeneLogicBoxGate) {
						if (!HasSignalPostGate((nextPart as GeneLogicBoxGate), hostCell)) {
							// next part turned out to be a gate with output off
							return gate.operatorType == LogicOperatorEnum.And ? false : true; // one false ==> AND is off :(   (...then a layer of inversion on top when NAND)
						}
					}
				}
			}
			return gate.operatorType == LogicOperatorEnum.And ? true : false; // We didn't find an input or a gate with an off signal ==> all signals must have been ON :)
		} else /* (gate.operatorType == LogicOperatorEnum.OR || gate.operatorType == LogicOperatorEnum.NOR */{
			foreach (GeneLogicBoxPart nextPart in gate.partsConnected) {
				if (nextPart.isTransmittingSignal) {
					if (nextPart is GeneLogicBoxInput) {
						if ((nextPart as GeneLogicBoxInput).valveMode == SignalValveModeEnum.Pass) {
							Nerve inputNerve = ((LogicBox)hostCell.GetSignalUnit(gate.geneLogicBox.signalUnit)).inputNerves[(nextPart as GeneLogicBoxInput).column];
							if (inputNerve.tailSignalUnitEnum != SignalUnitEnum.Void && inputNerve.tailCell != null && inputNerve.tailCell.GetOutputFromUnit(inputNerve.tailSignalUnitEnum, inputNerve.tailSignalUnitSlotEnum)) {
								// next part turned out to be an pass input valve with its input on
								return gate.operatorType == LogicOperatorEnum.Or ? true : false; // one on ==> OR is on :)   (...then a layer of inversion on top when NOR)
							}
						} else if ((nextPart as GeneLogicBoxInput).valveMode == SignalValveModeEnum.PassInverted) {
							Nerve inputNerve = ((LogicBox)hostCell.GetSignalUnit(gate.geneLogicBox.signalUnit)).inputNerves[(nextPart as GeneLogicBoxInput).column];
							if (inputNerve.tailSignalUnitEnum == SignalUnitEnum.Void || inputNerve.tailCell == null || !inputNerve.tailCell.GetOutputFromUnit(inputNerve.tailSignalUnitEnum, inputNerve.tailSignalUnitSlotEnum)) {
								// next part turned out to be an pass INVERTED input valve with its input off
								return gate.operatorType == LogicOperatorEnum.Or ? true : false; // off ==Inverted==> on ... one on ==> OR is on :)     (...then a layer of inversion on top when NOR)
							}
						}
					} else if (nextPart is GeneLogicBoxGate) {
						if (HasSignalPostGate((nextPart as GeneLogicBoxGate), hostCell)) {
							// next part turned out to be a gate with output on
							return gate.operatorType == LogicOperatorEnum.Or ? true : false; // one on ==> OR is on :)   (...then a layer of inversion on top when NOR)
						}
					}
				}
			}
			return gate.operatorType == LogicOperatorEnum.Or ? false : true; // We didnt find any input or gate with an ON-signal. Conclution: all singnals must have been OFF (valve shut) :(   ==> We just set the output to off in this case (...then a layer of inversion on top when NOR)
		}
	}

	public static bool HasSignalPostInputValve(Cell hostCell, SignalUnitEnum signalUnitEnum, GeneLogicBoxInput geneLogicBoxInput) {
		if ((geneLogicBoxInput as GeneLogicBoxInput).valveMode == SignalValveModeEnum.Pass) {
			Nerve inputNerve = ((LogicBox)hostCell.GetSignalUnit(signalUnitEnum)).inputNerves[geneLogicBoxInput.column];
			if (inputNerve.tailSignalUnitEnum != SignalUnitEnum.Void && inputNerve.tailCell != null && inputNerve.tailCell.GetOutputFromUnit(inputNerve.tailSignalUnitEnum, inputNerve.tailSignalUnitSlotEnum)) {
				return true;
			}
		} else if ((geneLogicBoxInput as GeneLogicBoxInput).valveMode == SignalValveModeEnum.PassInverted) {
			Nerve inputNerve = ((LogicBox)hostCell.GetSignalUnit(signalUnitEnum)).inputNerves[geneLogicBoxInput.column];
			if (inputNerve.tailSignalUnitEnum == SignalUnitEnum.Void || inputNerve.tailCell == null || !inputNerve.tailCell.GetOutputFromUnit(inputNerve.tailSignalUnitEnum, inputNerve.tailSignalUnitSlotEnum)) {
				return true;
			}
		}
		return false;
	}

	private bool TestInput(int leftFlank) {
		return leftFlank == 0 || leftFlank == 2 || leftFlank == 4; 
	}

	// Load Save
	private LogicBoxData logicBoxData = new LogicBoxData();

	// Save
	public LogicBoxData UpdateData() {
		logicBoxData.outputEarly = outputEarly;
		logicBoxData.outputLate = outputLate;
		return logicBoxData;
	}

	// Load
	public void ApplyData(LogicBoxData logicBoxData) {
		outputEarly = logicBoxData.outputEarly;
		outputLate = logicBoxData.outputLate;
	}


	// ^ Load Save ^
}