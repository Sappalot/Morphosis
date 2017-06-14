using System.Collections.Generic;
using UnityEngine;

public class Arrangement {
    public bool isEnabled = true;
    private ArrangementTypeEnum m_type = ArrangementTypeEnum.Side;
    public ArrangementTypeEnum type {
        get {
            return m_type;
        }
        set {
            m_type = value;
            SnapToLegalValues();
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
        }
    }

    private int m_arrowIndex = 0; //ALL: +- 30 degrees per step, 0 is straight up (possitive y), negative on white side possitive on black side  
    public int arrowIndex {
        get {
            return m_arrowIndex;
        }
        set {
            m_arrowIndex = value;
            m_arrowIndex = AngleUtil.WarpArrowIndex(m_arrowIndex);
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
        }
    }

    public bool flipPairsEnabled = false; //MIRROR4 & STAR6

    public ArrangementFlipSmOpTypeEnum flipTypeSameOpposite = ArrangementFlipSmOpTypeEnum.Same; // SIDE and STAR use Same/Opposite, Mirror use BlackToArrow/WhiteToArrow 

    public ArrangementFlipBtaWtaTypeEnum flipTypeBlackWhiteToArrow = ArrangementFlipBtaWtaTypeEnum.BlackToArrow;
    public Gene referenceGene;

    public Arrangement() {
        //SnapToLegalValues();
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
    }

    public void IncreasRefCount() {
        if (m_type == ArrangementTypeEnum.Side) {
            if (m_referenceCount == -2) {
                m_referenceCount = 1;
            } else {
                m_referenceCount++;
            }
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
    }

    public void DecreasseRefCount() {
        if (m_type == ArrangementTypeEnum.Side) {
            if (m_referenceCount == 1) {
                m_referenceCount = -2;
            } else {
                m_referenceCount--;
            }
            if (m_referenceCount < -5) {
                m_referenceCount = -5;
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
    }

    public void TurnArrowCounterClowkwise() {
        if (m_type == ArrangementTypeEnum.Side) {
            m_arrowIndex += 2;
        } else if (m_type == ArrangementTypeEnum.Mirror) {
            m_arrowIndex += 2;
        } else if (m_type == ArrangementTypeEnum.Star) {
            m_arrowIndex += 2;
        }
        m_arrowIndex = AngleUtil.WarpArrowIndex(m_arrowIndex);
    }

    public void TurnArrowClowkwise() {
        if (m_type == ArrangementTypeEnum.Side) {
            m_arrowIndex -= 2;
        } else if (m_type == ArrangementTypeEnum.Mirror) {
            m_arrowIndex -= 2;
        } else if (m_type == ArrangementTypeEnum.Star) {
            m_arrowIndex -= 2;
        }
        m_arrowIndex = AngleUtil.WarpArrowIndex(m_arrowIndex);
    }

    public void IncreaseGap() {
        if (m_type == ArrangementTypeEnum.Mirror) {
            m_gap++;
        }
        SnapToLegalValues();
    }

    public void DecreseGap() {
        if (m_type == ArrangementTypeEnum.Mirror) {
            m_gap--;
        }
        SnapToLegalValues();
    }

    //reference location 0-5
    //heading 0-5
    //flip side BW or WB
    public GeneReference GetFlippableReference(int referenceCardinalIndex, FlipSideEnum viewedFlipSide) {
        int referenceCardinalIndexFlippable = AngleUtil.GetFlipableCardinalIndex(referenceCardinalIndex, viewedFlipSide); // look on flipped side if nessesary

        if (m_type == ArrangementTypeEnum.Side) {
            for (int index = 0; index < Mathf.Abs(m_referenceCount); index++) {
                if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + ((m_referenceCount > 0) ? index * 2 : -index * 2))) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
            }
        } else if (m_type == ArrangementTypeEnum.Mirror) {
            if (m_referenceCount == 2) {
                if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + m_gap + 1)) {
                    return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex - m_gap - 1)) {
                    return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
            } else if (m_referenceCount == 4) {
                if (flipPairsEnabled) {
                    if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + m_gap + 1)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                    } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex - m_gap - 1)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + m_gap + 3)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex - m_gap - 3)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                    }
                } else {
                    if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + m_gap + 1)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                    } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex - m_gap - 1)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + m_gap + 3)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                    } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex - m_gap - 3)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    }
                }
            } else if (m_referenceCount == 6) {
                if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + 1)) {
                    return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex - 1)) {
                    return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + 3)) {
                    return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex - 3)) {
                    return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + 5)) {
                    return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex - 5)) {
                    return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
            }
        } else if (m_type == ArrangementTypeEnum.Star) {
            if (m_referenceCount == 2) {
                if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + 3)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex - 3)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
            }
            else if (m_referenceCount == 3) {
                if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + 2)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex - 2)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + 6)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
            } else if (m_referenceCount == 4) {
                if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + 2)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + 4)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex - 2)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex - 4)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
            } else if (m_referenceCount == 6) {
                if (flipPairsEnabled) {
                    if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex - 1)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + 1)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                    } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex - 3)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                    } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + 3)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex - 5)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + 5)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipBtaWtaTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                    }
                } else { 
                    if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex - 1)) {
                        return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + 1)) {
                        return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex - 3)) {
                        return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + 3)) {
                        return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex - 5)) {
                        return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipSmOpTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } else if (AngleUtil.ToArrowIndex(referenceCardinalIndexFlippable) == AngleUtil.WarpArrowIndex(m_arrowIndex + 5)) {
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
        m_referenceCount = Mathf.Clamp(m_referenceCount, -5, 5);
    }

    private void SnapToLegalMirror() {
        m_referenceCount = Mathf.Abs(m_referenceCount);
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
        m_referenceCount = Mathf.Abs(m_referenceCount);
        if (m_referenceCount == 1) {
            m_referenceCount = 2;
        }
        else if (m_referenceCount == 5) {
            m_referenceCount = 4;
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
            m_arrowIndex = AngleUtil.WarpArrowIndex(m_arrowIndex + 1);
        }
    }

    //odd ==> even: snap towards white
    private void SnapArrowToEvenAngles() {
        if (Mathf.Abs(m_arrowIndex) % 2 == 1) {
            m_arrowIndex = AngleUtil.WarpArrowIndex(m_arrowIndex - 1);
        }
    }
}
