using UnityEngine;
using System.Collections.Generic;

public class RelationArrows : MonoBehaviour {
	
	public LineRenderer arrowTemplateMother;
	public LineRenderer arrowTemplateFather;
	public LineRenderer arrowTemplateChild;
	public List<LineRenderer> arrows = new List<LineRenderer>();


	[HideInInspector]
	public Creature creature;

	private void Awake() {
		arrowTemplateMother.gameObject.SetActive(false);
		arrowTemplateFather.gameObject.SetActive(false);
		arrowTemplateChild.gameObject.SetActive(false);
	}

	private const float hoverDistance = -20f;

	public void UpdateGraphics() {
		if (!GlobalPanel.instance.graphicsRelationsToggle.isOn || creature == null) {
			gameObject.SetActive(false);
			return;
		}
		gameObject.SetActive(true);

		//Mother
		LineRenderer motherArrow = null;
		if (arrows.Count >= 1) {
			motherArrow = arrows[0];
		} else {
			motherArrow = Instantiate(arrowTemplateMother);
			motherArrow.transform.parent = transform;
			motherArrow.name = "Arrow from mother";
			//motherArrow.material = materialMother;
			motherArrow.gameObject.SetActive(true);
			arrows.Add(motherArrow);
		}
		if (creature.HasMotherAlive()) {
			motherArrow.gameObject.SetActive(true);
			Vector2 mother = creature.GetMotherAlive().GetOriginPosition(CreatureEditModePanel.instance.mode);
			motherArrow.SetPosition(0, new Vector3(mother.x, mother.y, hoverDistance)); // tail

			Vector2 me = creature.GetOriginPosition(CreatureEditModePanel.instance.mode);
			motherArrow.SetPosition(1, new Vector3(me.x, me.y, hoverDistance)); //head
		} else {
			motherArrow.gameObject.SetActive(false);
		}

		//TODO: Father
		LineRenderer fatherArrow = null;
		if (arrows.Count >= 2) {
			fatherArrow = arrows[1];
		} else {
			fatherArrow = Instantiate(arrowTemplateFather);
			fatherArrow.transform.parent = transform;
			fatherArrow.name = "Arrow from father";
			//fatherArrow.material = materialFather;
			fatherArrow.gameObject.SetActive(true);
			arrows.Add(fatherArrow);
		}
		fatherArrow.gameObject.SetActive(false);

		//Children
		List<Creature> children = creature.GetChildrenAlive();
		for (int childIndex = 0; childIndex < children.Count; childIndex++) {
			LineRenderer childArrow = null;
			if (arrows.Count >= 3 + childIndex) {
				childArrow = arrows[2 + childIndex];
			} else {
				childArrow = Instantiate(arrowTemplateChild);
				childArrow.transform.parent = transform;
				childArrow.name = "Arrow to child " + childIndex;

				childArrow.gameObject.SetActive(true);
				arrows.Add(childArrow);
			}

			childArrow.gameObject.SetActive(true);
			Vector2 me = creature.GetOriginPosition(CreatureEditModePanel.instance.mode);
			childArrow.SetPosition(0, new Vector3(me.x, me.y, hoverDistance)); // tail

			Vector2 child = children[childIndex].GetOriginPosition(CreatureEditModePanel.instance.mode);
			childArrow.SetPosition(1, new Vector3(child.x, child.y, hoverDistance)); //head

			//float b = 0.5f + 0.3f * ((childIndex + 1f) / children.Count);
			//childArrow.sharedMaterial.color = new Color(b, b, b, 1f);
		}

		// Hide the rest
		for (int i = 2 + children.Count; i < arrows.Count; i++) {
			arrows[i].gameObject.SetActive(false);
		}
	}
}