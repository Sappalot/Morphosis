using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class CardinalDirectionHelper
{

    static Dictionary<int, CardinalDirection> index_Directions = new Dictionary<int, CardinalDirection>();
    static Dictionary<CardinalDirection, int> directions_Index = new Dictionary<CardinalDirection, int>();
    static Dictionary<int, float> index_Angle = new Dictionary<int, float>();

    static CardinalDirectionHelper() {
        index_Directions.Add(0, CardinalDirection.northEast);
        index_Directions.Add(1, CardinalDirection.north);
        index_Directions.Add(2, CardinalDirection.northWest);
        index_Directions.Add(3, CardinalDirection.southWest);
        index_Directions.Add(4, CardinalDirection.south);
        index_Directions.Add(5, CardinalDirection.southEast);

        directions_Index.Add(CardinalDirection.northEast,   0);
        directions_Index.Add(CardinalDirection.north,       1);
        directions_Index.Add(CardinalDirection.northWest,   2);
        directions_Index.Add(CardinalDirection.southWest,   3);
        directions_Index.Add(CardinalDirection.south,       4);
        directions_Index.Add(CardinalDirection.southEast,   5);

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

    public static int ToIndex(CardinalDirection direction) {
        return directions_Index[direction];
    }

    public static CardinalDirection ToCardinalDirection(int index)
    {
        return index_Directions[index];
    }

    public static float ToAngle(int index) {
        return index_Angle[index];
    }

    public static float ToAngle(CardinalDirection direction) {
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

/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class CardinalDirectionIndex
{

    static Dictionary<int, CardinalDirection> index_Directions = new Dictionary<int, CardinalDirection>();
    static Dictionary<int, float> index_Angle = new Dictionary<int, float>();
    static Dictionary<CardinalDirection, int> directions_Index = new Dictionary<CardinalDirection, int>();

    static CardinalDirectionIndex() {
        index_Directions.Add(0, CardinalDirection.northEast);
        index_Directions.Add(1, CardinalDirection.north);
        index_Directions.Add(2, CardinalDirection.northWest);
        index_Directions.Add(3, CardinalDirection.southWest);
        index_Directions.Add(4, CardinalDirection.south);
        index_Directions.Add(5, CardinalDirection.southEast);

        directions_Index.Add(CardinalDirection.northEast,   0);
        directions_Index.Add(CardinalDirection.north,       1);
        directions_Index.Add(CardinalDirection.northWest,   2);
        directions_Index.Add(CardinalDirection.southWest,   3);
        directions_Index.Add(CardinalDirection.south,       4);
        directions_Index.Add(CardinalDirection.southEast,   5);
        directions_Index.Add(CardinalDirection.northEast,   6);
        directions_Index.Add(CardinalDirection.north,       7);
        directions_Index.Add(CardinalDirection.northWest,   8);
        directions_Index.Add(CardinalDirection.southWest,   9);
        directions_Index.Add(CardinalDirection.south,      10);
        directions_Index.Add(CardinalDirection.southEast,  11);

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

    public static int ToIndex(CardinalDirection direction) {
        return directions_Index[direction];
    }

    public static CardinalDirection ToCardinalDirection(int index)
    {
        return index_Directions[index];
    }

    public static float GetAngle(int index) {
        return index_Angle[index];
    }

    public static float GetAngle(CardinalDirection direction) {
        return index_Angle[ToIndex(direction)];
    }

    public static float GetAngleBetween(int indexA, int indexB) {
        if (indexA <= indexB && indexB <= indexA + 3) {
            return GetAngle(indexA) - GetAngle(indexB);            
        }
        else {
            return GetAngleBetween(indexB, indexA);
        }
    }
}
*/
