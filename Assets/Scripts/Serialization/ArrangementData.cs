using System;
[Serializable]
public class ArrangementData {
	public bool isEnabled = true;
	public int referenceGeneIndex = -1;
	public ArrangementFlipSmOpTypeEnum flipTypeSameOpposite = ArrangementFlipSmOpTypeEnum.Same; // SIDE and STAR use Same/Opposite, Mirror use BlackToArrow/WhiteToArrow 
	public ArrangementFlipBtaWtaTypeEnum flipTypeBlackWhiteToArrow = ArrangementFlipBtaWtaTypeEnum.BlackToArrow;
	public bool flipPairsEnabled = false; //MIRROR4 & STAR6
	public ArrangementTypeEnum type = ArrangementTypeEnum.Side;
	public int referenceCount = 1;
	public int arrowIndex = 0;
	public int gap = 0;
	public ArrangementReferenceSideEnum referenceSide = ArrangementReferenceSideEnum.Black; //SIDE
}