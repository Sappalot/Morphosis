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
			cameraController.TryUnlockCamera();
			MoveCameraToBoundsOfCreatures(CreatureSelectionPanel.instance.selection, HUD.instance.worldViewportPanel.bottomAndRightPanelsBlocking);
		} else {
			cameraController.TryUnlockCamera();
			MoveCameraToBoundsOfCreatures(CreatureSelectionPanel.instance.selection, HUD.instance.worldViewportPanel.bottomPanelBlocking);
		}

	}

	public void OnPressedViewPreviousSelectedCreature() {
		viewedIndex--;
		if (viewedIndex < 0) {
			viewedIndex = CreatureSelectionPanel.instance.selectionCount - 1;
		}
		cameraController.TryUnlockCamera();
		MoveCameraToBoundsOfCreature(CreatureSelectionPanel.instance.selection[viewedIndex], HUD.instance.worldViewportPanel.bottomAndRightPanelsBlocking);
	}

	public void OnPressedViewNextSelectedCreature() {
		viewedIndex++;
		viewedIndex %= CreatureSelectionPanel.instance.selectionCount;
		cameraController.TryUnlockCamera();
		MoveCameraToBoundsOfCreature(CreatureSelectionPanel.instance.selection[viewedIndex], HUD.instance.worldViewportPanel.bottomAndRightPanelsBlocking);
	}

	public void MoveCameraToBoundsOfCreature(Creature creature, RectTransform panel) {
		List<Creature> listOfOne = new List<Creature>();
		listOfOne.Add(creature);
		MoveCameraToBoundsOfCreatures(listOfOne, panel);
	}

	public static Bounds BoundsOfCreatures(List<Creature> creatures) {
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

		return groupAABB;
	}

	public void MoveCameraToBoundsOfCreatures(List<Creature> creatures, RectTransform panel) {
		cameraController.MoveToBounds(BoundsOfCreatures(creatures), HUD.instance.hudSize, HUD.instance.WorldViewportBounds(panel));
	}

	public static Vector2 CenterOfBounds(Bounds worldRect, Vector2i screentBoundsHUD, Bounds viewportBoundsHUD) {

		float worldRectAspect = worldRect.width / worldRect.height;
		float worldViewportBoundsAspect = viewportBoundsHUD.width / viewportBoundsHUD.height;

		float enclosingWidth = 0f;
		float enclosingHeight = 0f;

		if (worldRectAspect < worldViewportBoundsAspect) {
			// world rect is touching floor and ceiling of viewport
			enclosingHeight = worldRect.height;
			enclosingWidth = worldRect.height * worldViewportBoundsAspect;
		} else {
			// world rect is touching left and rigth wall of viewport
			enclosingWidth = worldRect.width;
			enclosingHeight = worldRect.width / worldViewportBoundsAspect;
		}

		float enclosingWidthHalf = enclosingWidth / 2f;
		float enclosingHeightHalf = enclosingHeight / 2f;

		Bounds viewportBoundsWorld = new Bounds(worldRect.center.x - enclosingWidthHalf,
											worldRect.center.x + enclosingWidthHalf,
											worldRect.center.y - enclosingHeightHalf,
											worldRect.center.y + enclosingHeightHalf);

		Bounds screenBoundsWorld = new Bounds(viewportBoundsWorld.xMin - viewportBoundsHUD.xMin * (viewportBoundsWorld.width / viewportBoundsHUD.width),
												viewportBoundsWorld.xMax + (screentBoundsHUD.x - viewportBoundsHUD.xMax) * (viewportBoundsWorld.width / viewportBoundsHUD.width),
												viewportBoundsWorld.yMin - viewportBoundsHUD.yMin * (viewportBoundsWorld.height / viewportBoundsHUD.height),
												viewportBoundsWorld.yMax + (screentBoundsHUD.y - viewportBoundsHUD.yMax) * (viewportBoundsWorld.height / viewportBoundsHUD.height));

		return screenBoundsWorld.center;
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
				DebugUtil.Log("Update ViewSelectedCreaturePanel");
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
