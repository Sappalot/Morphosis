using UnityEngine;

public class Nerve {
	public NerveStatusEnum nerveStatusEnum;

	public NerveColorsEnum nerveColorsEnum;

	public Cell headCellLost { get; private set; }

	private Cell m_headCell;
	public Cell headCell { 
		get {
			return m_headCell;
		}
		set {
			m_headCell = value;
			if (m_headCell != null) {
				headCellLost = m_headCell;
			}
		}
	}

	public SignalUnitEnum headSignalUnitEnum;
	public SignalUnitSlotEnum headSignalUnitSlotEnum;

	public Cell tailCell;
	public SignalUnitEnum tailSignalUnitEnum;
	public SignalUnitSlotEnum tailSignalUnitSlotEnum;

	public Vector2i toTailVector; // Assume head is pointing to me: vector in cell space, from head (0, 0) to tail (x, y)

	public Vector2i toHeadVector {
		get {
			return CellMap.HexagonalMinus(new Vector2i(), toTailVector);
		}
	}

	// the owner of this nerve
	public Cell hostCell {
		get {
			if (nerveStatusEnum == NerveStatusEnum.Void) {
				Debug.Assert(false, "Can't find host cell on a void status nerve.");
			} else if (nerveStatusEnum == NerveStatusEnum.InputLocal || nerveStatusEnum == NerveStatusEnum.OutputLocal) {
				return headCell; // any will do
			} else if (nerveStatusEnum == NerveStatusEnum.InputExternal) { // TODO: phenotype
				return headCell;
			} else if (nerveStatusEnum == NerveStatusEnum.OutputExternal) { // TODO: phenotype
				return tailCell;
			}

			return null;
		}
	}

	// the cell which is reffered to by the owner
	public Cell nonHostCell {
		get {
			if (nerveStatusEnum == NerveStatusEnum.Void) {
				Debug.Assert(false, "Can't find reference cell on a void status nerve.");
			} else if (nerveStatusEnum == NerveStatusEnum.InputLocal || nerveStatusEnum == NerveStatusEnum.OutputLocal) {
				return headCell; // any will do
			} else if (nerveStatusEnum == NerveStatusEnum.InputExternal) { // TODO: phenotype
				return tailCell;
			} else if (nerveStatusEnum == NerveStatusEnum.OutputExternal) { // TODO: phenotype
				return headCell;
			}
			return null;
		}
	}

	public Nerve() {
		headCell = null;
		headSignalUnitEnum = SignalUnitEnum.Void;
		headSignalUnitSlotEnum = SignalUnitSlotEnum.inputA;

		tailCell = null;
		tailSignalUnitEnum = SignalUnitEnum.Void;
		tailSignalUnitSlotEnum = SignalUnitSlotEnum.inputA;
	}

	public Nerve(Nerve other) {
		Set(other);
	}

	public void Set(Nerve other) {
		nerveStatusEnum = other.nerveStatusEnum;

		headCell = other.headCell;
		headSignalUnitEnum = other.headSignalUnitEnum;
		headSignalUnitSlotEnum = other.headSignalUnitSlotEnum;

		tailCell = other.tailCell;
		tailSignalUnitEnum = other.tailSignalUnitEnum;
		tailSignalUnitSlotEnum = other.tailSignalUnitSlotEnum;

		toTailVector = other.toTailVector;
	}

	public bool IsOn {
		get {
			if (tailCell != null) {
				return tailCell.GetOutputFromUnit(tailSignalUnitEnum, tailSignalUnitSlotEnum);
			}
			return false;
		}
	}

	public bool isRootable { // That is unrooted
		get {
			if (headCell == null) {
				return true;
			} else {
				// there is a head cell
				if (headCell.GetSignalUnit(headSignalUnitEnum) == null) {
					// head cell is a ghost
					return true;
				}
				if (headCell.GetSignalUnit(headSignalUnitEnum).rootnessEnum == RootnessEnum.Rootable) {
					// head is at an input but not leading to root
					return true;
				}
			}

			return false;
		}
	}

	// Used with external nerves
	//public NerveColorsEnum GetNerveColor(PhenoGenoEnum phenoGenoMode) {
	//	if (phenoGenoMode == PhenoGenoEnum.Phenotype) {
	//		if (tailCell == null) {
	//			if (headCell == null) {
	//				// Both ends are in the void. Need to have either head or tail at cell
	//				return NerveColorsEnum.Error;
	//			} else {
	//				// head: cell | tail: void
	//				if (headCell.GetSignalUnit(headSignalUnitEnum) == null) {
	//					// head: cell (ghost input) | tail: void. Makes no sence 
	//					return NerveColorsEnum.Error;
	//				} else if( headCell.GetSignalUnit(headSignalUnitEnum).rootnessEnum == RootnessEnum.Rootable) {
	//					// head: cell (now unrooted but rootable) | tail: void
	//					return NerveColorsEnum.Unrooted_HeadOnInput_TailInVoid;
	//				} else if (headCell.GetSignalUnit(headSignalUnitEnum).rootnessEnum == RootnessEnum.Rooted) {
	//					// head: cell (ghost input) | tail: void. Makes no sence

	//				}
	//			}
	//		} else {
	//			if (headCell == null) {
	//				// head: void | tail: cell
	//				if (tailCell.GetSignalUnit(tailSignalUnitEnum) == null) {
	//					//head: void | tail: cell (ghost)
	//					return NerveColorsEnum.Unrooted_HeadInVoid_TailOnGhostOutput; // Not shown since 
	//				} else {
	//					//head: void | tail: cell
	//					return NerveColorsEnum.Unrooted_HeadInVoid_TailOnOutput;
	//				}
	//			} else {
	//				// head: cell | tail :cell
	//				if (tailCell.GetSignalUnit(tailSignalUnitEnum) == null) {
	//					//head: cell | tail: cell (ghost)
	//					return NerveColorsEnum.Rooted_HeadOnInput_TailOnGhost; 
	//				} else {
	//					//head: cell | tail: cell
	//					if (IsOn) {
	//						return NerveColorsEnum.Rooted_HeadOnInput_TailOnOutput_On;
	//					} else {
	//						return NerveColorsEnum.Rooted_HeadOnInput_TailOnOutput_Off;
	//					}
	//				}
	//			}
	//		}

	//	} else /* Genotype */ {
	//		return NerveColorsEnum.Error;
	//	}
	//}

	public static bool AreTwinNerves(Nerve nerveA, Nerve nerveB, bool careAboutStatus) {
		// Ignorance: call nerves with different status and tail twins, as long as heads are same


		// All we need is for the head and tail to be on the right genes with the right units and slots 
		if (careAboutStatus && nerveA.nerveStatusEnum != nerveB.nerveStatusEnum) {
			return false;
		}

		// head
		if (nerveA.headCell == null && nerveB.headCell == null) {
			// both are null ==> OK
		} else if ((nerveA.headCell == null && nerveB.headCell != null) || (nerveA.headCell != null && nerveB.headCell == null)) {
			// one pointing to a geneCell the other is not
			return false;
		} else if (nerveA.headCell.gene.index != nerveB.headCell.gene.index) {
			// both are not pointing to gene cells, but they have different genes
			return false;
		}
		// either both were null OR both pointing to gene cells with same gene

		if (nerveA.headSignalUnitEnum != nerveB.headSignalUnitEnum) {
			return false;
		}

		if (nerveA.headSignalUnitSlotEnum != nerveB.headSignalUnitSlotEnum) {
			return false;
		}

		////tail
		if (nerveA.tailCell == null && nerveB.tailCell == null) {
			// both are null ==> OK
		} else if ((nerveA.tailCell == null && nerveB.tailCell != null) || (nerveA.tailCell != null && nerveB.tailCell == null)) {
			// one pointing to a geneCell the other is not
			return false;
		} else if (nerveA.tailCell.gene.index != nerveB.tailCell.gene.index) {
			// both are not pointing to gene cells, but they have different genes
			return false;
		}
		// either both were null OR both pointing to gene cells with same gene

		if (nerveA.tailSignalUnitEnum != nerveB.tailSignalUnitEnum) {
			return false;
		}

		if (nerveA.tailSignalUnitSlotEnum != nerveB.tailSignalUnitSlotEnum) {
			return false;
		}

		return true;
	}

	public string ToTwinString() {
		return
			// Ignorance: status and tail are all the same to me

			nerveStatusEnum + "_" +
			(headCell == null ? "N" : headCell.gene.index.ToString()) + "_" + headSignalUnitEnum + "_" + headSignalUnitSlotEnum; // + "_" +
			//(tailCell == null ? "N" : tailCell.gene.index.ToString()) + "_" + tailSignalUnitEnum + "_" + tailSignalUnitSlotEnum;
	}
}
