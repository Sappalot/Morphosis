using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicBox : Sensor {
	
	public LogicBox(SignalUnitEnum outputUnit) {
		this.signalUnit = outputUnit;
	}

	public override bool GetOutput() {
		return output;
	}

	public override void UpdateOutputs(Cell hostCell, int deltaTicks, ulong worldTicks) {
		if (signalUnit == SignalUnitEnum.WorkEggFertilizeLogicBox) {
			// TODO: let input go through gates to form an answer output
			GeneLogicBoxInput input = hostCell.gene.eggCellFertilizeLogic.inputRow3[0];
			output = hostCell.GetOutputFromUnit(input.internalInput); // hack connection
		} else if (signalUnit == SignalUnitEnum.WorkEggFertilizeLogicBox) {
			// TODO: implement
		}

	}
}