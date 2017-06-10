using System.Collections.Generic;
using UnityEngine;

public class Arrangement {
    public bool isEnabled = true;
    public ArrangementTypeEnum type = ArrangementTypeEnum.Side;

    public int referenceCount = 1; //ALL: negative value indicated cells on the white side (when arrangementtype = Side)
    public int arrowAngle; //ALL: +- 30 degrees per step, 0 is straight up (possitive y), negative on white side possitive on black side  
    public int gap = 0; //MIRROR: number of cells in the gap
    public bool flipPairsEnabled = false; //MIRROR & STAR
    public ArrangementFlipTypeEnum flipTypeSameOpposite = ArrangementFlipTypeEnum.Same; // SIDE and STAR use Same/Opposite, Mirror use BlackToArrow/WhiteToArrow 
    public ArrangementFlipTypeEnum flipTypeBlackWhiteToArrow = ArrangementFlipTypeEnum.BlackToArrow;
    public Gene referenceGene;

    //----------------------------------------------
    private static Dictionary<int, int> cardinalToArrangement = new Dictionary<int, int>();
    private static Dictionary<int, int> cardinalToFlipCardinal = new Dictionary<int, int>();

    static Arrangement() {
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
    }

    public void SnapToLegalSide() {
        SnapArrowToEvenAngles();
        referenceCount = Mathf.Min(5, referenceCount);
    }

    public void SnapToLegalMirror() {
        referenceCount = Mathf.Abs(referenceCount);
        if (referenceCount == 1 || referenceCount == 3 || referenceCount == 5) {
            referenceCount++;
        }
    }

    public void SnapToLegalStar() {
        //Adjust Reference Count if nessesary
        referenceCount = Mathf.Abs(referenceCount);
        if (referenceCount == 1) {
            referenceCount = 2;
        } else if (referenceCount == 5) {
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

        } else if (type == ArrangementTypeEnum.Star) {
            if (referenceCount < 2) {
                referenceCount = 2;
            } else if(referenceCount <= 3) {
                referenceCount++;
            } else if (referenceCount == 4) {
                referenceCount = 6;
            }
        }
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

        } else if (type == ArrangementTypeEnum.Star) {
            if (referenceCount == 3 || referenceCount == 4) {
                referenceCount--;
            } else if (referenceCount >= 6) {
                referenceCount = 4;
            }            
        }
        Debug.Log("count: " + referenceCount);
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

        } else if (type == ArrangementTypeEnum.Star) {
            if (referenceCount == 2) {
                if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 3)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
                else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 3)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
            }
            else if (referenceCount == 3) {
                if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 2)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
                else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 2)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
                else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 6)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
            }
            else if (referenceCount == 4) {
                if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 2)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
                else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 4)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
                else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 2)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
                else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 4)) {
                    return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                }
            } else if (referenceCount == 6) {

                if (flipPairsEnabled) {
                    if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 1)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    }
                    else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 1)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                    }
                    else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 3)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                    }
                    else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 3)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    }
                    else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 5)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    }
                    else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 5)) {
                        return new GeneReference(referenceGene, flipTypeBlackWhiteToArrow == ArrangementFlipTypeEnum.BlackToArrow ? GetOppositeFlipSide(viewedFlipSide) : viewedFlipSide);
                    }
                } else { 
                    if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 1)) {
                        return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    }
                    else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 1)) {
                        return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    }

                    else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 3)) {
                        return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    }
                    else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 3)) {
                        return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    }

                    else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle - 5)) {
                        return new GeneReference(referenceGene, flipTypeSameOpposite == ArrangementFlipTypeEnum.Same ? viewedFlipSide : GetOppositeFlipSide(viewedFlipSide));
                    }
                    else if (CardinalIndexToArrangementAngle(referenceCardinalIndexFlippable) == WarpArrAngle(arrowAngle + 5)) {
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
}
