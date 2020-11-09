using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputPanel {

	void MakeDirty();

	// We call this one as we pick an output for our nerve tail inside the geneCell panel
	void TrySetNerveInputLocally(SignalUnitEnum inputUnit, SignalUnitSlotEnum inputUnitSlot);

	// We call this one uppon pick of a gene cell for the "nerve tail" 
	void TrySetNerveInputExternally(SignalUnitEnum inputUnit, SignalUnitSlotEnum inputUnitSlot, Vector2i nerveVectorLocal);

	// We call this one as we abort nerve assignation and want nerve to go back to state before starting assignation
	void TrySetNerve(SignalUnitEnum inputUnit, SignalUnitSlotEnum inputUnitSlot, SignalUnitEnum outputUnit, SignalUnitSlotEnum outputUnitSlot, Vector2i nerveVector);

	void ShowNerveInputExternally(Vector2i nerveVectorLocal);

	void MakeMotherPanelDirty();
}
