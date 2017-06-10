using System.Collections.Generic;

public static class CardinalDirectionUtil {

    private static Dictionary<int, CardinalDirectionEnum> index_Directions = new Dictionary<int, CardinalDirectionEnum>();
    private static Dictionary<CardinalDirectionEnum, int> directions_Index = new Dictionary<CardinalDirectionEnum, int>();
    private static Dictionary<int, float> index_Angle = new Dictionary<int, float>();

    static CardinalDirectionUtil() {
        index_Directions.Add(0, CardinalDirectionEnum.northEast);
        index_Directions.Add(1, CardinalDirectionEnum.north);
        index_Directions.Add(2, CardinalDirectionEnum.northWest);
        index_Directions.Add(3, CardinalDirectionEnum.southWest);
        index_Directions.Add(4, CardinalDirectionEnum.south);
        index_Directions.Add(5, CardinalDirectionEnum.southEast);

        directions_Index.Add(CardinalDirectionEnum.northEast,   0);
        directions_Index.Add(CardinalDirectionEnum.north,       1);
        directions_Index.Add(CardinalDirectionEnum.northWest,   2);
        directions_Index.Add(CardinalDirectionEnum.southWest,   3);
        directions_Index.Add(CardinalDirectionEnum.south,       4);
        directions_Index.Add(CardinalDirectionEnum.southEast,   5);

        index_Angle.Add(0, 30f);
        index_Angle.Add(1, 90f);
        index_Angle.Add(2, 150f);
        index_Angle.Add(3, 210f);
        index_Angle.Add(4, 270f);
        index_Angle.Add(5, 330f);
        index_Angle.Add(6, 390f);
        index_Angle.Add(7, 450f);
        index_Angle.Add(8, 510f);
        index_Angle.Add(9, 570f);
        index_Angle.Add(10, 630f);
        index_Angle.Add(11, 690f);
    }

    public static int ToIndex(CardinalDirectionEnum direction) {
        return directions_Index[direction];
    }

    public static CardinalDirectionEnum ToCardinalDirection(int index)
    {
        return index_Directions[index];
    }

    public static float ToAngle(int index) {
        return index_Angle[index];
    }

    public static float ToAngle(CardinalDirectionEnum direction) {
        return ToAngle(ToIndex(direction));
    }

    //Angle the shortest angle, that is <= 180
    public static float GetAngleBetween(int indexA, int indexB)
    {
        if (indexA <= indexB)
        {
            return ToAngle(indexB) - ToAngle(indexA);
        }
        else
        {
            return GetAngleBetween(indexB, indexA);
        }
    }
}