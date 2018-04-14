using System.Collections.Generic;
using UnityEngine;

public class VeinPool : MonoBehaviour {
	public Vein veinPrefab;
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

	private Queue<Vein> storedQueue = new Queue<Vein>();

	public Vein Borrow() {
		if (!GlobalSettings.instance.pooling.vein) {
			return Instantiate();
		}
		Vein borrowVein = null;
		if (storedQueue.Count > 0) {
			borrowVein = PopVein();
		} else {
			borrowVein = Instantiate();
		}
		m_loanedCount++;
		return borrowVein;
	}

	//Note: make sure there are no object out there with references to this returned cell
	public void Recycle(Vein vein) {
		if (!GlobalSettings.instance.pooling.vein) {
			Destroy(vein.gameObject);
			return;
		}
		vein.OnRecycle();
		vein.transform.parent = transform;
		vein.gameObject.SetActive(false);
		storedQueue.Enqueue(vein);
		m_loanedCount--;
	}

	private Vein PopVein() {
		if (storedQueue.Count > 0) {
			Vein vein = storedQueue.Dequeue();
			vein.gameObject.SetActive(true); // Causes: Assertion failed: Invalid SortingGroup index set in Renderer
			return vein;
		}
		return null;
	}

	private Vein Instantiate() {
		Vein cell = (Instantiate(veinPrefab, Vector3.zero, Quaternion.identity) as Vein);
		cell.name = "Vein" + serialNumber++;
		cell.transform.parent = transform;
		return cell;
	}
}
