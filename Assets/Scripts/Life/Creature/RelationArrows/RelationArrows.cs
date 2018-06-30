using UnityEngine;
using System.Collections.Generic;

public class RelationArrows : MonoBehaviour {
	public LineRenderer arrowTemplate;
	public List<LineRenderer> arrows = new List<LineRenderer>();

	public Material materialMother;
	public Material materialFather;
	public Material materialChild;


	[HideInInspector]
	public Creature creature;

	private void Awake() {
		arrowTemplate.gameObject.SetActive(false);
	}

	public void UpdateGraphics() {
		if (!GlobalPanel.instance.graphicsRelations.isOn || creature == null) {
			gameObject.SetActive(false);
			return;
		}
		gameObject.SetActive(true);

		//Mother
		LineRenderer motherArrow = null;
		if (arrows.Count >= 1) {
			motherArrow = arrows[0];
		} else {
			motherArrow = Instantiate(arrowTemplate);
			motherArrow.transform.parent = transform;
			motherArrow.name = "Arrow from mother";
			motherArrow.material = materialMother;
			motherArrow.gameObject.SetActive(true);
			arrows.Add(motherArrow);
		}
		if (creature.IsDetatchedWithMotherAlive()) {
			motherArrow.gameObject.SetActive(true);
			Vector2 mother = creature.GetMotherAlive().GetOriginPosition(CreatureEditModePanel.instance.mode);
			motherArrow.SetPosition(0, new Vector3(mother.x, mother.y, -10f)); // tail

			Vector2 me = creature.GetOriginPosition(CreatureEditModePanel.instance.mode);
			motherArrow.SetPosition(1, new Vector3(me.x, me.y, -10f)); //head
		} else {
			motherArrow.gameObject.SetActive(false);
		}

		//TODO: Father
		LineRenderer fatherArrow = null;
		if (arrows.Count >= 2) {
			fatherArrow = arrows[1];
		} else {
			fatherArrow = Instantiate(arrowTemplate);
			fatherArrow.transform.parent = transform;
			fatherArrow.name = "Arrow from father";
			fatherArrow.material = materialFather;
			fatherArrow.gameObject.SetActive(true);
			arrows.Add(fatherArrow);
		}
		fatherArrow.gameObject.SetActive(false);

		//Children
		List<Creature> children = creature.GetDetatchedChildrenAlive();
		for (int childIndex = 0; childIndex < children.Count; childIndex++) {
			LineRenderer childArrow = null;
			if (arrows.Count >= 3 + childIndex) {
				childArrow = arrows[2 + childIndex];
			} else {
				childArrow = Instantiate(arrowTemplate);
				childArrow.transform.parent = transform;
				childArrow.name = "Arrow to child " + childIndex;

				

				Material m = new Material(materialChild);
				childArrow.sharedMaterial = m;

				childArrow.gameObject.SetActive(true);
				arrows.Add(childArrow);
			}

			childArrow.gameObject.SetActive(true);
			Vector2 me = creature.GetOriginPosition(CreatureEditModePanel.instance.mode);
			childArrow.SetPosition(0, new Vector3(me.x, me.y, -10f)); // tail

			Vector2 child = children[childIndex].GetOriginPosition(CreatureEditModePanel.instance.mode);
			childArrow.SetPosition(1, new Vector3(child.x, child.y, -10f)); //head

			float b = 0.5f + 0.3f * ((childIndex + 1f) / children.Count);
			childArrow.sharedMaterial.color = new Color(b, b, b, 1f);
		}

		// Hide the rest
		for (int i = 2 + children.Count; i < arrows.Count; i++) {
			arrows[i].gameObject.SetActive(false);
		}
	}
}