using System.Collections.Generic;
using UnityEngine;

public class Arrangement {
    public bool isEnabled = true;
    public ArrangementTypeEnum type = ArrangementTypeEnum.Side;

    public int referenceCount { get; private set; } //ALL: negative value indicated cells on the white side (when arrangementtype = Side)
    public int arrowAngle { get; private set; } //ALL: +- 30 degrees per step, 0 is straight up (possitive y), negative on white side possitive on black side  
    public int gap { get; private set; } //MIRROR: number of cells in the gap
    public bool flipPairsEnabled { get; private set; } //MIRROR & STAR
    public ArrangementFlipTypeEnum flipTypeSameOpposite { get; private set; } // SIDE and STAR use Same/Opposite, Mirror use BlackToArrow/WhiteToArrow 
    public ArrangementFlipTypeEnum flipTypeBlackWhiteToArrow { get; private set; }
    public Gene referenceGene;

    //----------------------------------------------
    private static Dictionary<int, int> cardinalToArrangement = new Dictionary<int, int>();
    private static Dictionary<int, int> cardinalToFlipCardinal = new Dictionary<int, int>();

    private static bool hasFilledDictionaries;
    public Arrangement() {
        if (!hasFilledDictionaries) {
            cardinalToArrangement.Add(0, -2);
            cardinalToArrangement.Add(1, 0);
            cardinalToArrangement.Add(2, 2);
            cardinalToArrangement.Add(3, 4);
            cardinalToArrangement.Add(4, 6);
            cardinalToArrangement.Add(5, -4);

            cardinalToFlipCardinal.Add(0, 2);
            cardinalToFlipCardinal.Add(1, 1);
            cardinalToFlipCardinal.Add(2, 0);
            cardinalToFlipCardinal.Add(3, 5);
            cardinalToFlipCardinal.Add(4, 4);
            cardinalToFlipCardinal.Add(5, 3);

            hasFilledDictionaries = true;
        }

        SnapToLegalValues();
        referenceCount = 1;
        flipTypeSameOpposite = ArrangementFlipTypeEnum.Same;
        flipTypeBlackWhiteToArrow = ArrangementFlipTypeEnum.BlackToArrow;
    }

    public void CycleArrangementType() {
        if (type == ArrangementTypeEnum.Side) {
            type = ArrangementTypeEnum.Mirror;
        }
        else if (type == ArrangementTypeEnum.Mirror) {
            type = ArrangementTypeEnum.Star;
        }
        else if (type == ArrangementTypeEnum.Star) {
            type = ArrangementTypeEnum.Side;
        }
        SnapToLegalValues();
    }

    public void IncreaseGap() {
        if (type == ArrangementTypeEnum.Mirror) {
            gap++;
        }
        SnapToLegalValues();
    }

    public void DecreseGap() {
        if (type == ArrangementTypeEnum.Mirror) {
            gap--;
        }
        SnapToLegalValues();
    }

    public void IncreasRefCount() {
        if (type == ArrangementTypeEnum.Side) {
            if (referenceCount == -2) {
                referenceCount = 1;
            }
            else {
                referenceCount++;
            }
            if (referenceCount > 5) {
                referenceCount = 5;
            }
        } else if (type == ArrangementTypeEnum.Mirror) {
            if (referenceCount < 4) {
                referenceCount = 4;
            } else if (referenceCount < 6) {
                referenceCount = 6;
            }
        } else if (type == ArrangementTypeEnum.Star) {
            if (referenceCount < 2) {
                referenceCount = 2;
            } else if(referenceCount <= 3) {
                referenceCount++;
            } else if (referenceCount == 4) {
                referenceCount = 6;
            }
        }
        SnapToLegalValues();
    }

    public void DecreasseRefCount() {
        if (type == ArrangementTypeEnum.Side) {
            if (referenceCount == 1) {
                referenceCount = -2;
            } else {
                referenceCount--;
            }
            if (referenceCount < -5) {
                referenceCount = -5;
            }
        } else if (type == ArrangementTypeEnum.Mirror) {
            if (referenceCount > 4) {
                referenceCount = 4;
            } else if (referenceCount > 2) {
                referenceCount = 2;
            } 
        } else if (type == ArrangementTypeEnum.Star) {
            if (referenceCount == 3 || referenceCount == 4) {
                referenceCount--;
            } else if (referenceCount >= 6) {
                referenceCount = 4;
            }            
        }
        SnapToLegalValues();
    }
    public void TurnArrowCounterClowkwise() {
        if (type == ArrangementTypeEnum.Side) {
            arrowAngle += 2;
        } else if (type == ArrangementTypeEnum.Mirror) {
            arrowAngle += 2;
        } else if (type == ArrangementTypeEnum.Star) {
            arrowAngle += 2;
        }
        arrowAngle = WarpArrAngle(arrowAngle);
    }

    public void TurnArrowClowkwise() {
        if (type == ArrangementTypeEnum.Side) {
            arrowAngle -= 2;
        } else if (type == ArrangementTypeEnum.Mirror) {
            arrowAngle -= 2;
        } else if (type == ArrangementTypeEnum.Star) {
            arrowAngle -= 2;
        }
        arrowAngle = WarpArrAngle(arrowAngle);
    }

    public void SetFlipSame() {
        flipTypeSameOpposite = ArrangementFlipTypeEnum.Same;
    }

    public void SetFlipOpposite() {
        flipTypeSameOpposite = ArrangementFlipTypeEnum.Opposite;
    }

    public void SetFlipBlackToArrow() {
        flipTypeBlackWhiteToArrow = ArrangementFlipTypeEnum.BlackToArrow;
    }

    public void SetFlipWhiteToArrow() {
        flipTypeBlackWhiteToArrow = ArrangementFlipTypeEnum.WhiteToArrow;
    }

    public void SetEnablePairs(bool enable) {
        if (type == ArrangementTypeEnum.Side) {

        }
        else if (type == ArrangementTypeEnum.Mirror) {
            flipPairsEnabled = enable;
        }
        else if (type == ArrangementTypeEnum.Star) {
            flipPairsEnabled = enable;
        }
    }

    //reference location 0-5
    //heading 0-5
    //flip side BW or WB
    public GeneReference GetFlippableReference(int referenceCardinalIndex, FlipSideEnum viewedFlipSide) {
        int referenceCardinalIndexFlippable = GetFlipableCardinalIndex(referenceCardinalIndex, viewedFlipSide); // look on flipped side if nessesary

        if (type == ArrangementTypeEnum.Side) {
            for (int index = 0; index < Mathf.Abs(referenceCount); index++) {
                if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + ((referenceCount > 0) ? index * 2 : -index * 2))) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
            }
        } else if (type == ArrangementTypeEnum.Mirror) {
            if (referenceCount == 2) {
                if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + gap + 1)) {
                    return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - gap - 1)) {
                    return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
            } else if (referenceCount == 4) {
                if (flipPairsEnabled) {
                    if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + gap + 1)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                    } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - gap - 1)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + gap + 3)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - gap - 3)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                    }
                } else {
                    if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + gap + 1)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                    } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - gap - 1)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + gap + 3)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                    } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - gap - 3)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    }
                }
            } else if (referenceCount == 6) {
                if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 1)) {
                    return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 1)) {
                    return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 3)) {
                    return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 3)) {
                    return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 5)) {
                    return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 5)) {
                    return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
            }
        } else if (type == ArrangementTypeEnum.Star) {
            if (referenceCount == 2) {
                if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 3)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 3)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
            }
            else if (referenceCount == 3) {
                if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 2)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 2)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 6)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
            } else if (referenceCount == 4) {
                if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 2)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 4)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 2)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 4)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
            } else if (referenceCount == 6) {
                if (flipPairsEnabled) {
                    if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 1)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 1)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                    } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 3)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                    } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 3)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 5)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 5)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                    }
                } else { 
                    if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 1)) {
                        return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 1)) {
                        return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 3)) {
                        return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 3)) {
                        return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 5)) {
                        return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    } else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 5)) {
                        return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    }
                }
            }
        }

        return null;
    }


    //Mathematical angle 0 at possitive x
    public float GetFlipableMathAngle(FlipSideEnum flipSide) {
        if (flipSide == FlipSideEnum.BlackWhite) {
            return arrowAngle * 30f + 90f;
        }
        return 180f - (arrowAngle * 30f + 90f);
    }

    private static FlipSideEnum GetOppositeFlipSide(FlipSideEnum flipSide) {
        if (flipSide == FlipSideEnum.BlackWhite) {
            return FlipSideEnum.WhiteBlack;
        }
        return FlipSideEnum.BlackWhite;
    }

    private static int GetFlipableCardinalIndex(int cardinalIndex, FlipSideEnum flipSide) {
        if (flipSide == FlipSideEnum.BlackWhite) {
            return cardinalIndex;
        }
        return cardinalToFlipCardinal[cardinalIndex];
    }

    private static int CardinalIndexToArrangementAngle(int cardinalIndex) {
        return cardinalToArrangement[cardinalIndex];
    }

    //-----------Utils
    public static int WarpArrAngle(int angle) {
        if (angle < -5) {
            return angle + 12;
        }
        if (angle > 6) {
            return angle - 12;
        }
        return angle;
    }

    //---------
    private void SnapToLegalValues() {
        if (type == ArrangementTypeEnum.Side) {
            SnapToLegalSide();
        } else if (type == ArrangementTypeEnum.Mirror) {
            SnapToLegalMirror();
        } else if (type == ArrangementTypeEnum.Star) {
            SnapToLegalStar();
        }
    }

    private void SnapToLegalSide() {
        SnapArrowToEvenAngles();
        referenceCount = Mathf.Clamp(referenceCount, -5, 5);
    }

    private void SnapToLegalMirror() {
        referenceCount = Mathf.Abs(referenceCount);
        if (referenceCount == 1 || referenceCount == 3 || referenceCount == 5) {
            referenceCount++;
        }

        if (referenceCount == 2) {
            gap = Mathf.Clamp(gap, 0, 4);
        } else if (referenceCount == 4) {
            gap = Mathf.Clamp(gap, 0, 2);
        } else if (referenceCount == 6) {
            gap = 0;
        }

        if (gap == 0 || gap == 2 || gap == 4) {
            SnapArrowToOddAngles();
        } else {
            SnapArrowToEvenAngles();
        }

        Debug.Log("Count: " + referenceCount + ", Gap: " + gap);
    }

    private void SnapToLegalStar() {
        //Adjust Reference Count if nessesary
        referenceCount = Mathf.Abs(referenceCount);
        if (referenceCount == 1) {
            referenceCount = 2;
        }
        else if (referenceCount == 5) {
            referenceCount = 4;
        }

        //Main Arrow
        //Adjust ArrowAngle if nessesary
        if (referenceCount == 2 || referenceCount == 6) {
            SnapArrowToOddAngles();
        }
        else {
            SnapArrowToEvenAngles();
        }
    }

    //even ==> odd: snap towards black
    private void SnapArrowToOddAngles() {
        if (Mathf.Abs(arrowAngle) % 2 == 0) {
            arrowAngle = WarpArrAngle(arrowAngle + 1);
        }
    }

    //odd ==> even: snap towards white
    private void SnapArrowToEvenAngles() {
        if (Mathf.Abs(arrowAngle) % 2 == 1) {
            arrowAngle = WarpArrAngle(arrowAngle - 1);
        }
    }
}
