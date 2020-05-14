using System.Collections.Generic;
using UnityEngine;

public class NerveArrowPool : MonoBehaviour {
	public NerveArrow nerveArrowPrefab;
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

	private Queue<NerveArrow> storedQueue = new Queue<NerveArrow>();

	public NerveArrow Borrow() {
		if (!GlobalSettings.instance.pooling.nerveArrow) {
			return Instantiate();
		}
		NerveArrow borrowVein = null;
		if (storedQueue.Count > 0) {
			borrowVein = PopVein();
		} else {
			borrowVein = Instantiate();
		}
		m_loanedCount++;
		return borrowVein;
	}

	//Note: make sure there are no object out there with references to this returned cell
	public void Recycle(NerveArrow nerveArrow) {
		if (!GlobalSettings.instance.pooling.nerveArrow) {
			Destroy(nerveArrow.gameObject);
			return;
		}
		nerveArrow.OnRecycle();
		nerveArrow.transform.parent = transform;
		nerveArrow.gameObject.SetActive(false);
		storedQueue.Enqueue(nerveArrow);
		m_loanedCount--;
	}

	private NerveArrow PopVein() {
		if (storedQueue.Count > 0) {
			NerveArrow nerveArrow = storedQueue.Dequeue();
			nerveArrow.gameObject.SetActive(true); // Causes: Assertion failed: Invalid SortingGroup index set in Renderer
			return nerveArrow;
		}
		return null;
	}

	private NerveArrow Instantiate() {
		NerveArrow nerveArrow = (Instantiate(nerveArrowPrefab, Vector3.zero, Quaternion.identity) as NerveArrow);
		nerveArrow.name = "NerveArrow " + serialNumber++;
		nerveArrow.transform.parent = transform;
		return nerveArrow;
	}
}
