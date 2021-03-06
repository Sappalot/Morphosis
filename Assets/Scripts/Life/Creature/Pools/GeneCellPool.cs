﻿using System.Collections.Generic;
using UnityEngine;


public class GeneCellPool : MonoBehaviour {
	public EggCell eggCellPrefab;
	public FungalCell fungalCellPrefab;
	public JawCell jawCellPrefab;
	public LeafCell leafCellPrefab;
	public MuscleCell muscleCellPrefab;
	public RootCell rootCellPrefab;
	public ShellCell shellCellPrefab;
	public VeinCell veinCellPrefab;

	public int GetStoredCellCount(CellTypeEnum type) {
		return storedQueues[type].Count;
	}

	//We are expecting to gett all of these back if all GeneCells were recycled
	public int GetLoanedCellCount(CellTypeEnum type) {
		return loanedCount[type];
	}

	private Queue<Cell> storedEgg = new Queue<Cell>();
	private Queue<Cell> storedFungal = new Queue<Cell>();
	private Queue<Cell> storedJaw = new Queue<Cell>();
	private Queue<Cell> storedLeaf = new Queue<Cell>();
	private Queue<Cell> storedMuscle = new Queue<Cell>();
	private Queue<Cell> storedRoot = new Queue<Cell>();
	private Queue<Cell> storedShell = new Queue<Cell>();
	private Queue<Cell> storedVein = new Queue<Cell>();

	private Dictionary<CellTypeEnum, Queue<Cell>> storedQueues = new Dictionary<CellTypeEnum, Queue<Cell>>();
	private Dictionary<CellTypeEnum, int> loanedCount = new Dictionary<CellTypeEnum, int>(); //How many cells of a specific type are we expected to get back, if they ere all deleted from the world
	private Dictionary<CellTypeEnum, int> serialNumber = new Dictionary<CellTypeEnum, int>();

	private void Awake() {
		storedQueues.Add(CellTypeEnum.Egg,    storedEgg);
		storedQueues.Add(CellTypeEnum.Fungal, storedFungal);
		storedQueues.Add(CellTypeEnum.Jaw,    storedJaw);
		storedQueues.Add(CellTypeEnum.Leaf,   storedLeaf);
		storedQueues.Add(CellTypeEnum.Muscle, storedMuscle);
		storedQueues.Add(CellTypeEnum.Root,   storedRoot);
		storedQueues.Add(CellTypeEnum.Shell,  storedShell);
		storedQueues.Add(CellTypeEnum.Vein,   storedVein);

		loanedCount.Add(CellTypeEnum.Egg,    0);
		loanedCount.Add(CellTypeEnum.Fungal, 0);
		loanedCount.Add(CellTypeEnum.Jaw,    0);
		loanedCount.Add(CellTypeEnum.Leaf,   0);
		loanedCount.Add(CellTypeEnum.Muscle, 0);
		loanedCount.Add(CellTypeEnum.Root,   0);
		loanedCount.Add(CellTypeEnum.Shell,  0);
		loanedCount.Add(CellTypeEnum.Vein,   0);

		serialNumber.Add(CellTypeEnum.Egg, 0);
		serialNumber.Add(CellTypeEnum.Fungal, 0);
		serialNumber.Add(CellTypeEnum.Jaw, 0);
		serialNumber.Add(CellTypeEnum.Leaf, 0);
		serialNumber.Add(CellTypeEnum.Muscle, 0);
		serialNumber.Add(CellTypeEnum.Root, 0);
		serialNumber.Add(CellTypeEnum.Shell, 0);
		serialNumber.Add(CellTypeEnum.Vein, 0);
	}

	public Cell Borrow(CellTypeEnum type) {
		if (!GlobalSettings.instance.pooling.geneCell) {
			return Instantiate(type);
		}
		Cell borrowCell = null;
		Cell poppedCell = PopCell(storedQueues[type]);

		if (poppedCell != null) {
			borrowCell = poppedCell;
		} else {
			borrowCell = Instantiate(type);
		}

		borrowCell.OnBorrowToWorld();
		loanedCount[type]++;
		return borrowCell;
	}

	//Note: make sure there are no object out there with references to this returned cell
	public void Recycle(Cell geneCell) {
		if (!GlobalSettings.instance.pooling.geneCell) {
			Destroy(geneCell.gameObject);
			return;
		}
		geneCell.transform.parent = transform;
		geneCell.gameObject.SetActive(false);
		storedQueues[geneCell.GetCellType()].Enqueue(geneCell);
		loanedCount[geneCell.GetCellType()]--;
	}

	private Cell PopCell(Queue<Cell> queue) {
		if (queue.Count > 0) {
			Cell cell = queue.Dequeue();
			cell.gameObject.SetActive(true); // Causes: Assertion failed: Invalid SortingGroup index set in Renderer
			return cell;
		}
		return null;
	}

	private Cell Instantiate(CellTypeEnum type) {
		Cell cell = null;
		if (type == CellTypeEnum.Egg) {
			cell = (Instantiate(eggCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
		} else if (type == CellTypeEnum.Fungal) {
			cell = (Instantiate(fungalCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
		} else if (type == CellTypeEnum.Jaw) {
			cell = (Instantiate(jawCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
		} else if (type == CellTypeEnum.Leaf) {
			cell = (Instantiate(leafCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
		} else if (type == CellTypeEnum.Muscle) {
			cell = (Instantiate(muscleCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
		} else if (type == CellTypeEnum.Root) {
			cell = (Instantiate(rootCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
		} else if (type == CellTypeEnum.Shell) {
			cell = (Instantiate(shellCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
		} else if (type == CellTypeEnum.Vein) {
			cell = (Instantiate(veinCellPrefab, Vector3.zero, Quaternion.identity) as Cell);
		}
		cell.Initialize(PhenoGenoEnum.Genotype);

		cell.name = type.ToString() + serialNumber[type]++;
		cell.transform.parent = transform;

		return cell;
	}

	public void ClearPool() {
		foreach (Queue <Cell> queue in storedQueues.Values) {
			Cell[] array = queue.ToArray();
			foreach(Cell c in array) {
				Destroy(c.gameObject);
			}
			queue.Clear();
		}
	}

}
