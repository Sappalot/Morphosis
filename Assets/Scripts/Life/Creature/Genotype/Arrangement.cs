using System.Collections.Generic;
using UnityEngine;

public class Arrangement {
	public bool m_isEnabled = true;
	public bool isEnabled {
		get {
			return m_isEnabled;
		}
		set {
			m_isEnabled = value;
			genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
		}
	}

	public int m_referenceGeneIndex;
	public int referenceGeneIndex {
		get {
			return m_referenceGeneIndex;
		}
		set {
			m_referenceGeneIndex = value;
			genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
		}
	}

	private Gene m_referenceGene;
	public Gene referenceGene {
		get {
			return m_referenceGene;
		}
		set {
			m_referenceGene = value;
			referenceGeneIndex = m_referenceGene.index;
			genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
		}
	}

	private ArrangementFlipSmOpTypeEnum m_flipTypeSameOpposite = ArrangementFlipSmOpTypeEnum.Same; // SIDE & STAR
	public ArrangementFlipSmOpTypeEnum flipTypeSameOpposite { 
		get {
			return m_flipTypeSameOpposite;
		}
		set {
			m_flipTypeSameOpposite = value;
			genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
		}
	}


	private ArrangementFlipBtaWtaTypeEnum m_flipTypeBlackWhiteToArrow; // MIRROR
	public ArrangementFlipBtaWtaTypeEnum flipTypeBlackWhiteToArrow {
		get {
			return m_flipTypeBlackWhiteToArrow;
		}
		set {
			m_flipTypeBlackWhiteToArrow = value;
			genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
		}
	}

	private bool m_isFlipPairsEnabled = false; //MIRROR4 & STAR6
	public bool isFlipPairsEnabled { 
		get {
			return m_isFlipPairsEnabled;
		}
		set {
			m_isFlipPairsEnabled = value;
			genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
		}
	}

	private ArrangementReferenceSideEnum m_referenceSide = ArrangementReferenceSideEnum.Black; //SIDE
	public ArrangementReferenceSideEnum referenceSide { 
		get {
			return m_referenceSide;
		}
		set {
			m_referenceSide = value;
			genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
		}
	}

	private ArrangementTypeEnum m_type = ArrangementTypeEnum.Side;
	public ArrangementTypeEnum type {
		get {
			return m_type;
		}
		set {
			m_type = value;
			SnapToLegalValues();
			genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
		}
	}

	private int m_referenceCount = 1; //ALL: negative value indicated cells on the white side (when arrangementtype = Side)
	public int referenceCount {
		get {
			return m_referenceCount;
		}
		set {
			m_referenceCount = value;
			SnapToLegalValues();
			genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
		}
	}

	private int m_arrowIndex = 0; //ALL: +- 30 degrees per step, 0 is straight up (possitive y), negative on white side possitive on black side  
	public int arrowIndex {
		get {
			return m_arrowIndex;
		}
		set {
			m_arrowIndex = value;
			SnapToLegalValues();
			genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
		}
	}

	private int m_gap = 0; //MIRROR: number of cells in the gap
	public int gap {
		get {
			return m_gap;
		}
		set {
			m_gap = value;
			SnapToLegalValues();
			genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
		}
	}

	private IGenotypeDirtyfy genotypeDirtyfy;

	public Arrangement(int index, IGenotypeDirtyfy genotypeDirtyfy) {
		this.genotypeDirtyfy = genotypeDirtyfy;
		Defaultify(index);
	}

	public void Defaultify(int index) {
		isEnabled = false;
		referenceGeneIndex = Mathf.Min(Genotype.genomeLength - 1, index + 1); // each gene reffer to th nex on in the order
		flipTypeSameOpposite = ArrangementFlipSmOpTypeEnum.Same; // SIDE & STAR
		flipTypeBlackWhiteToArrow = ArrangementFlipBtaWtaTypeEnum.BlackToArrow; // MIRROR
		isFlipPairsEnabled = false; //MIRROR4 & STAR6
		type = ArrangementTypeEnum.Side;
		referenceCount = 1;
		arrowIndex = 0;
		gap = 0;
		referenceSide = ArrangementReferenceSideEnum.Black; //SIDE

		genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
	}

	public void Randomize() {
		isEnabled = Random.Range(0, 3) == 0;
		referenceGeneIndex = Random.Range(1, Genotype.genomeLength);
		flipTypeSameOpposite = Random.Range(0, 2) == 0 ? ArrangementFlipSmOpTypeEnum.Same : ArrangementFlipSmOpTypeEnum.Opposite;
		flipTypeBlackWhiteToArrow = Random.Range(0, 2) == 0 ? ArrangementFlipBtaWtaTypeEnum.BlackToArrow : ArrangementFlipBtaWtaTypeEnum.WhiteToArrow;
		isFlipPairsEnabled = Random.Range(0, 2) == 0;
		type = (ArrangementTypeEnum)Random.Range(0, 3);
		if (type == ArrangementTypeEnum.Side) {
			referenceCount = Random.Range(1, 6);
		} else {
			referenceCount = Random.Range(1, 7);
		}
		arrowIndex = Random.Range(-5, 7);
		gap = Random.Range(0, 5);
		referenceSide = Random.Range(0, 2) == 0 ? ArrangementReferenceSideEnum.Black : ArrangementReferenceSideEnum.White;

		genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
	}

	public void Mutate(float strength) {
		GlobalSettings gs = GlobalSettings.instance;
		float rnd;

		// isEnabled
		rnd = Random.Range(0, gs.mutation.isEnabledToggle * strength + 1000f);
		if (rnd < gs.mutation.isEnabledToggle * strength) {
			isEnabled = !isEnabled;
		}

		// reference
		rnd = Random.Range(0, gs.mutation.referenceChange * strength + 1000f);
		if (rnd < gs.mutation.referenceChange * strength) {
			referenceGeneIndex = Random.Range(1, Genotype.genomeLength);
		}

		// flipTypeSameOpposite
		rnd = Random.Range(0, gs.mutation.flipTypeSameOppositeToggle * strength + 1000f);
		if (rnd < gs.mutation.flipTypeSameOppositeToggle * strength) {
			if (flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same) {
				flipTypeSameOpposite = ArrangementFlipSmOpTypeEnum.Opposite;
			} else {
				flipTypeSameOpposite = ArrangementFlipSmOpTypeEnum.Same;
			}
		}

		// flipTypeBlackWhiteToArrow
		rnd = Random.Range(0, gs.mutation.flipTypeBlackWhiteToArrowToggle * strength + 1000f);
		if (rnd < gs.mutation.flipTypeBlackWhiteToArrowToggle * strength) {
			if (flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow) {
				flipTypeBlackWhiteToArrow = ArrangementFlipBtaWtaTypeEnum.WhiteToArrow;
			} else {
				flipTypeBlackWhiteToArrow = ArrangementFlipBtaWtaTypeEnum.BlackToArrow;
			}
		}

		// flipPairsEnabled
		rnd = Random.Range(0, gs.mutation.isflipPairsEnabledToggle * strength + 1000f);
		if (rnd < gs.mutation.isflipPairsEnabledToggle * strength) {
			isFlipPairsEnabled = !isFlipPairsEnabled;
		}

		// type
		rnd = Random.Range(0, gs.mutation.typeChange * strength + 1000f);
		if (rnd < gs.mutation.typeChange * strength) {
			int t = Random.Range(0, 2);
			if (type == ArrangementTypeEnum.Side) {
				if (t == 0) {
					type = ArrangementTypeEnum.Mirror;
				} else {
					type = ArrangementTypeEnum.Star;
				}
			} else if (type == ArrangementTypeEnum.Mirror) {
				if (t == 0) {
					type = ArrangementTypeEnum.Side;
				} else {
					type = ArrangementTypeEnum.Star;
				}
			} else if (type == ArrangementTypeEnum.Star) {
				if (t == 0) {
					type = ArrangementTypeEnum.Side;
				} else {
					type = ArrangementTypeEnum.Mirror;
				}
			}
		}

		// referenceCount
		float referenceChange =
			  gs.mutation.referenceCountDecrease1 * strength
			+ gs.mutation.referenceCountDecrease2 * strength
			+ gs.mutation.referenceCountDecrease3 * strength
			+ gs.mutation.referenceCountIncrease1 * strength
			+ gs.mutation.referenceCountIncrease2 * strength
			+ gs.mutation.referenceCountIncrease3 * strength;
		rnd = Random.Range(0, referenceChange * strength + 1000f);

		// -1
		float ra =     gs.mutation.referenceCountDecrease1 * strength;
		// -2
		float rb = ra + gs.mutation.referenceCountDecrease2 * strength;
		// -3
		float rc = rb + gs.mutation.referenceCountDecrease3 * strength;
		// +1
		float rd = rc + gs.mutation.referenceCountIncrease1 * strength;
		// +2
		float re = rd + gs.mutation.referenceCountIncrease2 * strength;
		// +3
		float rf = re + gs.mutation.referenceCountIncrease3 * strength;

		if (rnd < ra) {
			referenceCount -= 1;
		} else if (rnd >= ra && rnd < rb) {
			referenceCount -= 2;
		} else if (rnd >= rb && rnd < rc) {
			referenceCount -= 3;
		} else if (rnd >= rc && rnd < rd) {
			referenceCount += 1;
		} else if (rnd >= rd && rnd < re) {
			referenceCount += 2;
		} else if (rnd >= re && rnd < rf) {
			referenceCount += 3;
		}

		// Arrow Index
		float arrowIndexChange =
			  gs.mutation.arrowIndexDecrease1 * strength
			+ gs.mutation.arrowIndexDecrease2 * strength
			+ gs.mutation.arrowIndexDecrease3 * strength
			+ gs.mutation.arrowIndexIncrease1 * strength
			+ gs.mutation.arrowIndexIncrease2 * strength
			+ gs.mutation.arrowIndexIncrease3 * strength;
		rnd = Random.Range(0, arrowIndexChange * strength + 1000f);

		// -1
		float aa = gs.mutation.arrowIndexDecrease1 * strength;
		// -2
		float ab = ra + gs.mutation.arrowIndexDecrease2 * strength;
		// -3
		float ac = rb + gs.mutation.arrowIndexDecrease3 * strength;
		// +1
		float ad = rc + gs.mutation.arrowIndexIncrease1 * strength;
		// +2
		float ae = rd + gs.mutation.arrowIndexIncrease2 * strength;
		// +3
		float af = re + gs.mutation.arrowIndexIncrease3 * strength;

		if (rnd < aa) {
			arrowIndex -= 1;
		} else if (rnd >= aa && rnd < ab) {
			arrowIndex -= 2;
		} else if (rnd >= ab && rnd < ac) {
			arrowIndex -= 3;
		} else if (rnd >= ac && rnd < ad) {
			arrowIndex += 1;
		} else if (rnd >= ad && rnd < ae) {
			arrowIndex += 2;
		} else if (rnd >= ae && rnd < af) {
			arrowIndex += 3;
		}
		arrowIndex = AngleUtil.ArrowIndexRawToArrowIndexSafe(arrowIndex);

		// gap
		float gapChange =
			  gs.mutation.gapDecrease1 * strength
			+ gs.mutation.gapDecrease2 * strength
			+ gs.mutation.gapDecrease3 * strength
			+ gs.mutation.gapIncrease1 * strength
			+ gs.mutation.gapIncrease2 * strength
			+ gs.mutation.gapIncrease3 * strength;
		rnd = Random.Range(0, gapChange * strength + 1000f);

		// -1
		float ga = gs.mutation.gapDecrease1 * strength;
		// -2
		float gb = ra + gs.mutation.gapDecrease2 * strength;
		// -3
		float gc = rb + gs.mutation.gapDecrease3 * strength;
		// +1
		float gd = rc + gs.mutation.gapIncrease1 * strength;
		// +2
		float ge = rd + gs.mutation.gapIncrease2 * strength;
		// +3
		float gf = re + gs.mutation.gapIncrease3 * strength;

		if (rnd < ga) {
			gap -= 1;
		} else if (rnd >= ga && rnd < gb) {
			gap -= 2;
		} else if (rnd >= gb && rnd < gc) {
			gap -= 3;
		} else if (rnd >= gc && rnd < gd) {
			gap += 1;
		} else if (rnd >= gd && rnd < ge) {
			gap += 2;
		} else if (rnd >= ge && rnd < gf) {
			gap += 3;
		}

		// referenceSide
		rnd = Random.Range(0, gs.mutation.referenceSideToggle * strength + 1000f);
		if (rnd < gs.mutation.referenceSideToggle * strength) {
			if (referenceSide == ArrangementReferenceSideEnum.Black) {
				referenceSide = ArrangementReferenceSideEnum.White;
			} else {
				referenceSide = ArrangementReferenceSideEnum.Black;
			}
		}

		SnapToLegalValues();

		genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
	}

	public void SetReferenceGeneFromReferenceGeneIndex(Gene[] genes) {
		referenceGene = genes[referenceGeneIndex];
	}

	public void CycleArrangementType() {
		if (m_type == ArrangementTypeEnum.Side) {
			m_type = ArrangementTypeEnum.Mirror;
		}
		else if (m_type == ArrangementTypeEnum.Mirror) {
			m_type = ArrangementTypeEnum.Star;
		}
		else if (m_type == ArrangementTypeEnum.Star) {
			m_type = ArrangementTypeEnum.Side;
		}
		SnapToLegalValues();
		genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
	}

	public void IncreasRefCount() {
		if (m_type == ArrangementTypeEnum.Side) {
			m_referenceCount++;
			if (m_referenceCount > 5) {
				m_referenceCount = 5;
			}
		} else if (m_type == ArrangementTypeEnum.Mirror) {
			if (m_referenceCount < 4) {
				m_referenceCount = 4;
			} else if (m_referenceCount < 6) {
				m_referenceCount = 6;
			}
		} else if (m_type == ArrangementTypeEnum.Star) {
			if (m_referenceCount < 2) {
				m_referenceCount = 2;
			} else if (m_referenceCount <= 3) {
				m_referenceCount++;
			} else if (m_referenceCount == 4) {
				m_referenceCount = 6;
			}
		}
		SnapToLegalValues();
		genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
	}

	public void DecreaseRefCount() {
		if (m_type == ArrangementTypeEnum.Side) {
			m_referenceCount--;
			if (m_referenceCount < 1) {
				m_referenceCount = 1;
			}
		} else if (m_type == ArrangementTypeEnum.Mirror) {
			if (m_referenceCount > 4) {
				m_referenceCount = 4;
			} else if (m_referenceCount > 2) {
				m_referenceCount = 2;
			}
		} else if (m_type == ArrangementTypeEnum.Star) {
			if (m_referenceCount == 3 || m_referenceCount == 4) {
				m_referenceCount--;
			} else if (m_referenceCount >= 6) {
				m_referenceCount = 4;
			}
		}
		SnapToLegalValues();
		genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
	}

	public void TurnArrowCounterClowkwise() {
		if (m_type == ArrangementTypeEnum.Side) {
			m_arrowIndex += 2;
		} else if (m_type == ArrangementTypeEnum.Mirror) {
			m_arrowIndex += 2;
		} else if (m_type == ArrangementTypeEnum.Star) {
			m_arrowIndex += 2;
		}
		m_arrowIndex = AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex);
		genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
	}

	public void TurnArrowClowkwise() {
		if (m_type == ArrangementTypeEnum.Side) {
			m_arrowIndex -= 2;
		} else if (m_type == ArrangementTypeEnum.Mirror) {
			m_arrowIndex -= 2;
		} else if (m_type == ArrangementTypeEnum.Star) {
			m_arrowIndex -= 2;
		}
		m_arrowIndex = AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex);
		genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
	}

	public void IncreaseGap() {
		if (m_type == ArrangementTypeEnum.Mirror) {
			m_gap++;
		}
		SnapToLegalValues();
		genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
	}

	public void DecreseGap() {
		if (m_type == ArrangementTypeEnum.Mirror) {
			m_gap--;
		}
		SnapToLegalValues();
		genotypeDirtyfy.ReforgeGeneCellPatternAndForward();
	}

	//reference location 0-5
	//heading 0-5
	//flip side BW or WB
	public GeneReference GetFlippableReference(int referenceCardinalIndex, FlipSideEnum viewedFlipSide) {
		int referenceCardinalIndexFlippable = AngleUtil.GetFlipableCardinalIndex(referenceCardinalIndex, viewedFlipSide); // look on flipped side if nessesary

		if (m_type == ArrangementTypeEnum.Side) {
			for (int index = 0; index < referenceCount; index++) {
				if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + ((referenceSide == ArrangementReferenceSideEnum.Black) ? index * 2 : -index * 2))) {
					return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
				}
			}
			//for (int index = 0; index < Mathf.Abs(m_referenceCount); index++) {
			//	if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + ((m_referenceCount > 0) ? index * 2 : -index * 2))) {
			//		return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
			//	}
			//}
		} else if (m_type == ArrangementTypeEnum.Mirror) {
			if (m_referenceCount == 2) {
				if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + m_gap + 1)) {
					return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
				} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex - m_gap - 1)) {
					return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
				}
			} else if (m_referenceCount == 4) {
				if (isFlipPairsEnabled) {
					if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + m_gap + 1)) {
						return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
					} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex - m_gap - 1)) {
						return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
					} if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + m_gap + 3)) {
						return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
					} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex - m_gap - 3)) {
						return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
					}
				} else {
					if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + m_gap + 1)) {
						return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
					} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex - m_gap - 1)) {
						return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
					} if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + m_gap + 3)) {
						return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
					} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex - m_gap - 3)) {
						return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
					}
				}
			} else if (m_referenceCount == 6) {
				if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + 1)) {
					return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
				} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex - 1)) {
					return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
				} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + 3)) {
					return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
				} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex - 3)) {
					return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
				} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + 5)) {
					return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
				} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex - 5)) {
					return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
				}
			}
		} else if (m_type == ArrangementTypeEnum.Star) {
			if (m_referenceCount == 2) {
				if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + 3)) {
					return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
				} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex - 3)) {
					return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
				}
			}
			else if (m_referenceCount == 3) {
				if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + 2)) {
					return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
				} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex - 2)) {
					return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
				} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + 6)) {
					return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
				}
			} else if (m_referenceCount == 4) {
				if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + 2)) {
					return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
				} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + 4)) {
					return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
				} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex - 2)) {
					return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
				} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex - 4)) {
					return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
				}
			} else if (m_referenceCount == 6) {
				if (isFlipPairsEnabled) {
					if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex - 1)) {
						return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
					} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + 1)) {
						return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
					} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex - 3)) {
						return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
					} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + 3)) {
						return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
					} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex - 5)) {
						return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
					} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + 5)) {
						return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
					}
				} else { 
					if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex - 1)) {
						return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
					} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + 1)) {
						return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
					} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex - 3)) {
						return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
					} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + 3)) {
						return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
					} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex - 5)) {
						return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
					} else if (AngleUtil.CardinalIndexToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + 5)) {
						return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
					}
				}
			}
		}

		return null;
	}

	//Mathematical angle 0 at possitive x
	public float GetFlipableArrowAngle(FlipSideEnum flipSide) {
		return AngleUtil.ToAngle(m_arrowIndex, flipSide);
	}

	private static FlipSideEnum GetOppositeFlipSide(FlipSideEnum flipSide) {
		if (flipSide == FlipSideEnum.BlackWhite) {
			return FlipSideEnum.WhiteBlack;
		}
		return FlipSideEnum.BlackWhite;
	}

	private void SnapToLegalValues() {
		if (m_type == ArrangementTypeEnum.Side) {
			SnapToLegalSide();
		} else if (m_type == ArrangementTypeEnum.Mirror) {
			SnapToLegalMirror();
		} else if (m_type == ArrangementTypeEnum.Star) {
			SnapToLegalStar();
		}
	}

	private void SnapToLegalSide() {
		SnapArrowToEvenAngles();
		m_referenceCount = Mathf.Clamp(m_referenceCount, 1, 5);
	}

	private void SnapToLegalMirror() {
		m_referenceCount = (int)Mathf.Clamp(m_referenceCount, 1, 6);
		if (m_referenceCount == 1 || m_referenceCount == 3 || m_referenceCount == 5) {
			m_referenceCount++;
		}

		if (m_referenceCount == 2) {
			m_gap = Mathf.Clamp(m_gap, 0, 4);
		} else if (m_referenceCount == 4) {
			m_gap = Mathf.Clamp(m_gap, 0, 2);
		} else if (m_referenceCount == 6) {
			m_gap = 0;
		}

		if (m_gap == 0 || m_gap == 2 || m_gap == 4) {
			SnapArrowToOddAngles();
		} else {
			SnapArrowToEvenAngles();
		}
	}

	private void SnapToLegalStar() {
		//Adjust Reference Count if nessesary
		if (m_referenceCount <= 1) {
			m_referenceCount = 2;
		}
		if (m_referenceCount == 5 || m_referenceCount > 6) {
			m_referenceCount = 6;
		}

		//Main Arrow
		//Adjust ArrowAngle if nessesary
		if (m_referenceCount == 2 || m_referenceCount == 6) {
			SnapArrowToOddAngles();
		}
		else {
			SnapArrowToEvenAngles();
		}
	}

	//even ==> odd: snap towards black
	private void SnapArrowToOddAngles() {
		if (Mathf.Abs(m_arrowIndex) % 2 == 0) {
			m_arrowIndex = AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex + 1);
		}
	}

	//odd ==> even: snap towards white
	private void SnapArrowToEvenAngles() {
		if (Mathf.Abs(m_arrowIndex) % 2 == 1) {
			m_arrowIndex = AngleUtil.ArrowIndexRawToArrowIndexSafe(m_arrowIndex - 1);
		}
	}

	public Arrangement GetClone() {
		Arrangement clone = new Arrangement(1, genotypeDirtyfy);
		clone.ApplyData(UpdateData());
		return clone;
	}

	//data
	private ArrangementData arrangementData = new ArrangementData();
	public ArrangementData UpdateData() { // Save: We have all genes and their data allready
		arrangementData.isEnabled = isEnabled;
		arrangementData.referenceGeneIndex = referenceGeneIndex; // referenceGene.index; //Have to use index cant use reference
		arrangementData.flipTypeSameOpposite = flipTypeSameOpposite; // SIDE and STAR use Same/Opposite, Mirror use BlackToArrow/WhiteToArrow 
		arrangementData.flipTypeBlackWhiteToArrow = flipTypeBlackWhiteToArrow;
		arrangementData.flipPairsEnabled = isFlipPairsEnabled; //MIRROR4 & STAR6        
		arrangementData.type = type;
		arrangementData.referenceCount = referenceCount;
		arrangementData.arrowIndex = arrowIndex;
		arrangementData.gap = gap;
		arrangementData.referenceSide = referenceSide; //SIDE
		return arrangementData;
	}

	public void ApplyData(ArrangementData arrangementData) {
		isEnabled = arrangementData.isEnabled;
		referenceGeneIndex = arrangementData.referenceGeneIndex;
		//referenceGene = arrangementData.referenceGene; //Cant set it here
		flipTypeSameOpposite = arrangementData.flipTypeSameOpposite;
		flipTypeBlackWhiteToArrow = arrangementData.flipTypeBlackWhiteToArrow;
		isFlipPairsEnabled = arrangementData.flipPairsEnabled;
		type = arrangementData.type;
		referenceCount = arrangementData.referenceCount;
		arrowIndex = arrangementData.arrowIndex;
		gap = arrangementData.gap;
		referenceSide = arrangementData.referenceSide; //SIDE
	}
}
