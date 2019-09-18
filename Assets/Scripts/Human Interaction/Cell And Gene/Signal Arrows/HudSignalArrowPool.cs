using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudSignalArrowPool : MonoBehaviour {
	public HudSignalArrow hudSignalArrowPrefab;
	private int serialNumber = 0;

	public int storedCount {
		get {
			return storedQueue.Count;
		}
	}

	//We are expecting to gett all of these back if all edges were recycled
	private int m_loanedCount;
	public int loanedCount {
		get {
			return m_loanedCount;
		}
	}

	private Queue<HudSignalArrow> storedQueue = new Queue<HudSignalArrow>();

	public HudSignalArrow Borrow() {
		HudSignalArrow borrowArrow = null;
		if (storedQueue.Count > 0) {
			borrowArrow = PopArrow();
		} else {
			borrowArrow = Instantiate();
		}
		m_loanedCount++;
		return borrowArrow;
	}

	//Note: make sure there are no object out there with references to this returned cell
	public void Recycle(HudSignalArrow arrow) {
		arrow.OnRecycle();
		arrow.transform.parent = transform;
		arrow.gameObject.SetActive(false);
		storedQueue.Enqueue(arrow);
		m_loanedCount--;
	}

	private HudSignalArrow PopArrow() {
		if (storedQueue.Count > 0) {
			HudSignalArrow arrow = storedQueue.Dequeue();
			arrow.gameObject.SetActive(true); // Causes: Assertion failed: Invalid SortingGroup index set in Renderer
			return arrow;
		}
		return null;
	}

	private HudSignalArrow Instantiate() {
		HudSignalArrow arrow = (Instantiate(hudSignalArrowPrefab, Vector3.zero, Quaternion.identity) as HudSignalArrow);
		arrow.name = "HudSignalArrow" + serialNumber++;
		arrow.transform.parent = transform;
		return arrow;
	}
}
