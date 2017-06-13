using System.Collections.Generic;

public static class AngleUtil {

    private static Dictionary<int, CardinalDirectionEnum> cardinalIndexToCardinalDirection = new Dictionary<int, CardinalDirectionEnum>();
    private static Dictionary<CardinalDirectionEnum, int> cardinalDirectionsToCardinalIndex = new Dictionary<CardinalDirectionEnum, int>();
    private static Dictionary<int, float> cardinalIndexToAngle = new Dictionary<int, float>();
    private static Dictionary<int, int> cardinalIndexToArrowIndex = new Dictionary<int, int>();
    private static Dictionary<int, int> cardinalIndexToFlipCardinalIndex = new Dictionary<int, int>();


    static AngleUtil() {
        cardinalIndexToCardinalDirection.Add(0, CardinalDirectionEnum.northEast);
        cardinalIndexToCardinalDirection.Add(1, CardinalDirectionEnum.north);
        cardinalIndexToCardinalDirection.Add(2, CardinalDirectionEnum.northWest);
        cardinalIndexToCardinalDirection.Add(3, CardinalDirectionEnum.southWest);
        cardinalIndexToCardinalDirection.Add(4, CardinalDirectionEnum.south);
        cardinalIndexToCardinalDirection.Add(5, CardinalDirectionEnum.southEast);

        cardinalDirectionsToCardinalIndex.Add(CardinalDirectionEnum.northEast, 0);
        cardinalDirectionsToCardinalIndex.Add(CardinalDirectionEnum.north, 1);
        cardinalDirectionsToCardinalIndex.Add(CardinalDirectionEnum.northWest, 2);
        cardinalDirectionsToCardinalIndex.Add(CardinalDirectionEnum.southWest, 3);
        cardinalDirectionsToCardinalIndex.Add(CardinalDirectionEnum.south, 4);
        cardinalDirectionsToCardinalIndex.Add(CardinalDirectionEnum.southEast, 5);

        cardinalIndexToAngle.Add(0, 30f);
        cardinalIndexToAngle.Add(1, 90f);
        cardinalIndexToAngle.Add(2, 150f);
        cardinalIndexToAngle.Add(3, 210f);
        cardinalIndexToAngle.Add(4, 270f);
        cardinalIndexToAngle.Add(5, 330f);
        cardinalIndexToAngle.Add(6, 390f);
        cardinalIndexToAngle.Add(7, 450f);
        cardinalIndexToAngle.Add(8, 510f);
        cardinalIndexToAngle.Add(9, 570f);
        cardinalIndexToAngle.Add(10, 630f);
        cardinalIndexToAngle.Add(11, 690f);

        cardinalIndexToArrowIndex.Add(0, -2);
        cardinalIndexToArrowIndex.Add(1, 0);
        cardinalIndexToArrowIndex.Add(2, 2);
        cardinalIndexToArrowIndex.Add(3, 4);
        cardinalIndexToArrowIndex.Add(4, 6);
        cardinalIndexToArrowIndex.Add(5, -4);

        cardinalIndexToFlipCardinalIndex.Add(0, 2);
        cardinalIndexToFlipCardinalIndex.Add(1, 1);
        cardinalIndexToFlipCardinalIndex.Add(2, 0);
        cardinalIndexToFlipCardinalIndex.Add(3, 5);
        cardinalIndexToFlipCardinalIndex.Add(4, 4);
        cardinalIndexToFlipCardinalIndex.Add(5, 3);
    }

    public static int ToCardinalDirectionIndex(CardinalDirectionEnum cardinalDirection) {
        return cardinalDirectionsToCardinalIndex[cardinalDirection];
    }

    public static CardinalDirectionEnum ToCardinalDirection(int cardinalIndex) {
        return cardinalIndexToCardinalDirection[cardinalIndex];
    }

    public static float ToAngle(int cardinalIndex) {
        return cardinalIndexToAngle[cardinalIndex];
    }

    public static float ToAngle(CardinalDirectionEnum cardinalDirection) {
        return ToAngle(ToCardinalDirectionIndex(cardinalDirection));
    }

    public static float ToAngle(int arrowIndex, FlipSideEnum flipSide) {
        if (flipSide == FlipSideEnum.BlackWhite) {
            return arrowIndex * 30f + 90f;
        }
        return 180f - (arrowIndex * 30f + 90f);
    }

    //Angle the shortest angle, that is <= 180
    public static float GetAngleDifference(int cardinalIndexA, int cardinalIndexB) {
        if (cardinalIndexA <= cardinalIndexB)
        {
            return ToAngle(cardinalIndexB) - ToAngle(cardinalIndexA);
        }
        else
        {
            return GetAngleDifference(cardinalIndexB, cardinalIndexA);
        }
    }

    //Angle the shortest angle, that is <= 180
    public static float GetAngleDifference(float angle1, float angle2) {
        float diff = (angle2 - angle1 + 180f) % 360f - 180f;
        return diff < -180f ? diff + 360f : diff;
    }

    public static int GetFlipableCardinalIndex(int cardinalIndex, FlipSideEnum flipSide) {
        if (flipSide == FlipSideEnum.BlackWhite) {
            return cardinalIndex;
        }
        return cardinalIndexToFlipCardinalIndex[cardinalIndex];
    }

    public static int ToArrowIndex(int cardinalIndex) {
        return cardinalIndexToArrowIndex[cardinalIndex];
    }

    public static int WarpArrowIndex(int arrowIndex) {
        if (arrowIndex < -5) {
            return arrowIndex + 12;
        }
        if (arrowIndex > 6) {
            return arrowIndex - 12;
        }
        return arrowIndex;
    }
}