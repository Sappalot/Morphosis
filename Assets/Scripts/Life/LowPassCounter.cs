public class LowPassCounter {
	private int cursorIndex = 0;
	private float[] array;

	private int count;

	public LowPassCounter(int length) {
		array = new float[length];
	}

	public void IncreaseCounter() {
		count++;
	}

	public void Clear() {
		for (int i = 0; i < array.Length; i++) {
			array[i] = 0f;
		}
		cursorIndex = 0;
	}

	private int GetAndEmptyCount() {
		int b = count;
		count = 0;
		return b;
	}

	public float GetAndStepLowPassCount() {
		array[cursorIndex] = GetAndEmptyCount();
		cursorIndex++;
		if (cursorIndex >= array.Length) {
			cursorIndex = 0;
		}

		float c = 0f;
		for (int i = 0; i < array.Length; i++) {
			c += array[i];
		}

		return c / array.Length;
	}
}

