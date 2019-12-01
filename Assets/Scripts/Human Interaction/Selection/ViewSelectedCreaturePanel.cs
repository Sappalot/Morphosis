using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewSelectedCreaturePanel : MonoSingleton<ViewSelectedCreaturePanel> {

	public CameraController cameraController;

	public Text viewAllLabel;
	public Text viewPreviousLabel;
	public Text viewNextLabel;

	private bool isDirty;
	private int viewedIndex;

	public void OnPressedViewAllSelectedCreatures() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			MoveCameraToBoundsOfCreatures(CreatureSelectionPanel.instance.selection, HUD.instance.worldViewportPanel.bottomAndRightPanelsBlocking);
		} else {
			MoveCameraToBoundsOfCreatures(CreatureSelectionPanel.instance.selection, HUD.instance.worldViewportPanel.bottomPanelBlocking);
		}

	}

	public void OnPressedViewPreviousSelectedCreature() {
		viewedIndex--;
		if (viewedIndex < 0) {
			viewedIndex = CreatureSelectionPanel.instance.selectionCount - 1;
		}

		MoveCameraToBoundsOfCreature(CreatureSelectionPanel.instance.selection[viewedIndex], HUD.instance.worldViewportPanel.bottomAndRightPanelsBlocking);
	}

	public void OnPressedViewNextSelectedCreature() {
		viewedIndex++;
		viewedIndex %= CreatureSelectionPanel.instance.selectionCount;

		MoveCameraToBoundsOfCreature(CreatureSelectionPanel.instance.selection[viewedIndex], HUD.instance.worldViewportPanel.bottomAndRightPanelsBlocking);
	}

	private void MoveCameraToBoundsOfCreature(Creature creature, RectTransform panel) {
		List<Creature> listOfOne = new List<Creature>();
		listOfOne.Add(creature);
		MoveCameraToBoundsOfCreatures(listOfOne, panel);
	}

	private void MoveCameraToBoundsOfCreatures(List<Creature> creatures, RectTransform panel) {
		Bounds groupAABB = new Bounds(float.MaxValue, float.MinValue, float.MaxValue, float.MinValue);
		foreach (Creature c in creatures) {

			Bounds aabb = c.phenotype.AABB;
			groupAABB.xMin = Mathf.Min(aabb.xMin, groupAABB.xMin);
			groupAABB.xMax = Mathf.Max(aabb.xMax, groupAABB.xMax);
			groupAABB.yMin = Mathf.Min(aabb.yMin, groupAABB.yMin);
			groupAABB.yMax = Mathf.Max(aabb.yMax, groupAABB.yMax);
		}

		float width = groupAABB.width;
		float height = groupAABB.height;

		float marginPercentage = 0.1f;
		float marginMeters = 5f;

		groupAABB.xMin -= Mathf.Max(width * marginPercentage, marginMeters);
		groupAABB.xMax += Mathf.Max(width * marginPercentage, marginMeters);
		groupAABB.yMin -= Mathf.Max(height * marginPercentage, marginMeters);
		groupAABB.yMax += Mathf.Max(height * marginPercentage, marginMeters);

		cameraController.MoveToBounds(groupAABB, HUD.instance.hudSize, HUD.instance.WorldViewportBounds(panel));
	}

	private void MoveCameraToCenterOnCreature() {

	}


	public void MakeDirty() {
		isDirty = true;
	}


	private void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			OnPressedViewAllSelectedCreatures();
		} else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
			OnPressedViewPreviousSelectedCreature();
		} else if (Input.GetKeyDown(KeyCode.RightArrow)) {
			OnPressedViewNextSelectedCreature();
		}

		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update ViewSelectedCreaturePanel");
			}

			if (CreatureSelectionPanel.instance.hasSelection) {
				viewAllLabel.color = ColorScheme.instance.normalText;
				if (CreatureSelectionPanel.instance.hasSoloSelected) {
					viewAllLabel.text = "[ This ]";
					viewPreviousLabel.color = ColorScheme.instance.grayedOut;
					viewNextLabel.color = ColorScheme.instance.grayedOut;
				} else {
					viewAllLabel.text = "[ Group ]";
					viewPreviousLabel.color = ColorScheme.instance.normalText;
					viewNextLabel.color = ColorScheme.instance.normalText;
				}
			} else {
				viewAllLabel.text = "[ Group ]";
				viewAllLabel.color = ColorScheme.instance.grayedOut;
				viewPreviousLabel.color = ColorScheme.instance.grayedOut;
				viewNextLabel.color = ColorScheme.instance.grayedOut;
			}



			isDirty = false;
		}
	}
}
