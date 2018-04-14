using System.Collections.Generic;
using UnityEngine;


public class EdgePool : MonoBehaviour {
	public Edge edgePrefab;
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

	private Queue<Edge> storedQueue = new Queue<Edge>();

	public Edge Borrow() {
		if (!GlobalSettings.instance.pooling.edge) {
			return Instantiate();
		}
		Edge borrowEdge = null;
		if (storedQueue.Count > 0) {
			borrowEdge = PopEdge();
		} else {
			borrowEdge = Instantiate();
		}
		m_loanedCount++;
		return borrowEdge;
	}

	//Note: make sure there are no object out there with references to this returned cell
	public void Recycle(Edge edge) {
		if (!GlobalSettings.instance.pooling.edge) {
			Destroy(edge.gameObject);
			return;
		}
		edge.OnRecycle();
		edge.transform.parent = transform;
		edge.gameObject.SetActive(false);
		storedQueue.Enqueue(edge);
		m_loanedCount--;
	}

	private Edge PopEdge() {
		if (storedQueue.Count > 0) {
			Edge edge = storedQueue.Dequeue();
			edge.gameObject.SetActive(true); // Causes: Assertion failed: Invalid SortingGroup index set in Renderer
			return edge;
		}
		return null;
	}

	private Edge Instantiate() {
		Edge cell = (Instantiate(edgePrefab, Vector3.zero, Quaternion.identity) as Edge);
		cell.name = "Edge" + serialNumber++;
		cell.transform.parent = transform;
		return cell;
	}

}
