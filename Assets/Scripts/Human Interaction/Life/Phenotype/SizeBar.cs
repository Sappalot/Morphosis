using UnityEngine;
using UnityEngine.UI;

public class SizeBar : MonoBehaviour {
	public Image squareTemplate;
	public Transform panel;

	public Color colorBuiltOther;
	public Color colorEmptyOther;
	public Color colorBuiltEgg;
	public Color colorEmptyEgg;
	public Color colorOccupiedChild;

	private const int maxCount = 40;
	private Image[] images = new Image[maxCount];

	private void Start() {
		squareTemplate.gameObject.SetActive(true);

			for (int x = 0; x < maxCount; x++) {
				images[x] = Instantiate(squareTemplate, panel);
				images[x].rectTransform.anchoredPosition = new Vector2(x * 11f, 0f);
			}

		squareTemplate.gameObject.SetActive(false);
	}

	public void UpdateBar(int totalSize, int builtSize, int totalEggCellCount, int builtEggCount, int attachedChildrenCount) {
		for (int index = 0; index < maxCount; index++) {
			images[index].gameObject.SetActive(index < totalSize);
			int unbuiltEggLimit = totalEggCellCount - builtEggCount - attachedChildrenCount;
			int builtEggLimit = totalEggCellCount - attachedChildrenCount;
			int fertilizedEggLimit = totalEggCellCount;
			int builtOtherLimit = builtSize + (totalEggCellCount - builtEggCount);

			Color color = Color.black;
			if (index < unbuiltEggLimit) {
				color = colorEmptyEgg;
			}
			else if (index < builtEggLimit) {
				color = colorBuiltEgg;
			}
			else if (index < fertilizedEggLimit) {
				color = colorOccupiedChild;
			}
			else if (index < builtOtherLimit) {
				color = colorBuiltOther;
			}
			else {
				color = colorEmptyOther;
			}
			images[index].color = color;
		}
	}
	
	public bool isOn {
		set {
			for (int index = 0; index < maxCount; index++) {
				images[index].gameObject.SetActive(value);
			}
		}
	}
}