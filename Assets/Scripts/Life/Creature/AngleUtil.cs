using System.Collections.Generic;
using UnityEngine;

public static class AngleUtil {

	private static Dictionary<int, CardinalEnum> cardinalIndexToCardinalEnum = new Dictionary<int, CardinalEnum>();
	private static Dictionary<CardinalEnum, int> cardinalEnumToCardinalIndex = new Dictionary<CardinalEnum, int>();
	private static Dictionary<int, float> cardinalIndexToAngle = new Dictionary<int, float>();
	private static Dictionary<int, int> cardinalIndexToArrowIndex = new Dictionary<int, int>();
	private static Dictionary<int, int> cardinalIndexToFlipCardinalIndex = new Dictionary<int, int>();


	static AngleUtil() {
		cardinalIndexToCardinalEnum.Add(0, CardinalEnum.northEast);
		cardinalIndexToCardinalEnum.Add(1, CardinalEnum.north);
		cardinalIndexToCardinalEnum.Add(2, CardinalEnum.northWest);
		cardinalIndexToCardinalEnum.Add(3, CardinalEnum.southWest);
		cardinalIndexToCardinalEnum.Add(4, CardinalEnum.south);
		cardinalIndexToCardinalEnum.Add(5, CardinalEnum.southEast);

		cardinalEnumToCardinalIndex.Add(CardinalEnum.northEast, 0);
		cardinalEnumToCardinalIndex.Add(CardinalEnum.north, 1);
		cardinalEnumToCardinalIndex.Add(CardinalEnum.northWest, 2);
		cardinalEnumToCardinalIndex.Add(CardinalEnum.southWest, 3);
		cardinalEnumToCardinalIndex.Add(CardinalEnum.south, 4);
		cardinalEnumToCardinalIndex.Add(CardinalEnum.southEast, 5);

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
		cardinalIndexToAngle.Add(12, 750f);
		cardinalIndexToAngle.Add(13, 810f);
		cardinalIndexToAngle.Add(14, 870f);
		cardinalIndexToAngle.Add(15, 930f);
		cardinalIndexToAngle.Add(16, 990f);
		cardinalIndexToAngle.Add(17, 1050f);
		//How many do we need?

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

	public static int CardinalEnumToCardinalIndex(CardinalEnum cardinalDirection) {
		return cardinalEnumToCardinalIndex[cardinalDirection];
	}

	public static CardinalEnum CardinalIndexToCardinalEnum(int cardinalIndex) {
		return cardinalIndexToCardinalEnum[cardinalIndex];
	}

	public static float CardinalIndexToAngle(int cardinalIndex) {
		return cardinalIndexToAngle[cardinalIndex];
	}

	public static float CardinalEnumToAngle(CardinalEnum cardinalDirection) {
		return CardinalIndexToAngle(CardinalEnumToCardinalIndex(cardinalDirection));
	}

	public static int AngleToCardinalIndexSafe(float angle) { //angle: east = right = 0
		angle = AngleRawToSafe(angle);
		if (angle >= 0f && angle < 60f) {
			return 0;
		} else if (angle >= 60f && angle < 120f) {
			return 1;
		} else if (angle >= 120f && angle < 180f) {
			return 2;
		} else if (angle >= 180f && angle < 240f) {
			return 3;
		} else if (angle >= 240f && angle < 300f) {
			return 4;
		}
		return 5;
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
			return CardinalIndexToAngle(cardinalIndexB) - CardinalIndexToAngle(cardinalIndexA);
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

	public static int CardinalIndexToArrowIndex(int cardinalIndex) {
		return cardinalIndexToArrowIndex[cardinalIndex];
	}

	public static int ArrowIndexRawToArrowIndexSafe(int arrowIndex) {
		if (arrowIndex < -5) {
			return arrowIndex + 12;
		}
		if (arrowIndex > 6) {
			return arrowIndex - 12;
		}
		return arrowIndex;
	}

	public static float AngleRawToSafe(float angleRaw) {
		while (angleRaw < 0f) {
			angleRaw += 360f;
		}
		angleRaw %= 360f;
		Debug.Assert(angleRaw >= 0f && angleRaw <= 360f);
		return angleRaw;
	}

	public static int CardinalIndexRawToSafe(int cardinalIndex) {
		while (cardinalIndex < 0f) {
			cardinalIndex += 6;
		}
		cardinalIndex %= 6;
		Debug.Assert(cardinalIndex >= 0 && cardinalIndex <= 5);
		return cardinalIndex;
	}
}