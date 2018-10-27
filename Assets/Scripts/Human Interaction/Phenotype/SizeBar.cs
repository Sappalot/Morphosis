﻿using UnityEngine;
using UnityEngine.UI;

public class SizeBar : MonoBehaviour {
	public Image squareTemplate;
	public Transform panel;

	public Color colorBuiltOther;
	public Color colorEmptyOther;
	public Color colorBuiltEgg;
	public Color colorEmptyEgg;
	public Color colorOccupiedChild;

	private Image[] images = new Image[100];

	private void Start() {
		squareTemplate.gameObject.SetActive(true);
		for (int y = 0; y < 2; y++) {
			for (int x = 0; x < 50; x++) {
				images[y* 50 + x] = Instantiate(squareTemplate, panel);
				images[y * 50 + x].rectTransform.anchoredPosition = new Vector2(x *7f, -y * 9f);
			}
		}
		squareTemplate.gameObject.SetActive(false);
	}

	public void UpdateBar(int totalSize, int builtSize, int totalEggCellCount, int builtEggCount, int attachedChildrenCount) {
		for (int index = 0; index < 100; index++) {
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
			for (int index = 0; index < 100; index++) {
				images[index].gameObject.SetActive(value);
			}
		}
	}
}