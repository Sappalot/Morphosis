using System.Collections.Generic;
using UnityEngine;


public class CreaturePool : MonoSingleton<CreaturePool> {
	//public Creature creaturePrefab;
	//private int serialNumber = 0;

	//public int storedCount {
	//	get {
	//		return storedQueue.Count;
	//	}
	//}

	////We are expecting to gett all of these back if all edges were recycled
	//private int m_loanedCount;
	//public int loanedCount {
	//	get {
	//		return m_loanedCount;
	//	}
	//}

	//private Queue<Creature> storedQueue = new Queue<Creature>();

	//public Creature Borrow() {
	//	Creature borrowCreature = null;
	//	if (storedQueue.Count > 0) {
	//		borrowCreature = PopCreature();
	//	} else {
	//		borrowCreature = Instantiate();
	//	}
	//	m_loanedCount++;
	//	return borrowCreature;
	//}

	////Note: make sure there are no object out there with references to this returned cell
	//public void Recycle(Creature creature) {
	//	creature.OnRecycle();
	//	creature.transform.parent = transform;
	//	creature.gameObject.SetActive(false);
	//	storedQueue.Enqueue(creature);
	//	m_loanedCount--;
	//}

	//private Creature PopCreature() {
	//	if (storedQueue.Count > 0) {
	//		Creature creature = storedQueue.Dequeue();
	//		creature.gameObject.SetActive(true); // Causes: Assertion failed: Invalid SortingGroup index set in Renderer
	//		return creature;
	//	}
	//	return null;
	//}

	//private Creature Instantiate() {
	//	Creature creature = (Instantiate(creaturePrefab, Vector3.zero, Quaternion.identity) as Creature);
	//	creature.name = "Creature" + serialNumber++;
	//	creature.transform.parent = transform;
	//	return creature;
	//}
}
