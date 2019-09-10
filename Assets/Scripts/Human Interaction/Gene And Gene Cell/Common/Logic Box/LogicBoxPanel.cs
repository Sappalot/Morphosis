using UnityEngine;
using UnityEngine.UI;

public class LogicBoxPanel : MonoBehaviour {
	private static Vector2 rowSize = new Vector2(270f, 40f);
	public static float cellWidth = rowSize.x * (1f / 5f);
	public static float cellHeight = 40;

	[HideInInspector]
	public string outputText;

	public Text outputLabel;
	public LogicBoxGatePanel gateTemplate;

	private LogicBoxGatePanel gateRow0;
	private LogicBoxGatePanel[] gatesRow1 = new LogicBoxGatePanel[GeneLogicBox.maxGatesPerRow];
	private LogicBoxGatePanel[] gatesRow2 = new LogicBoxGatePanel[GeneLogicBox.maxGatesPerRow];
	public LogicBoxInputPanel[] inputRow3 = new LogicBoxInputPanel[GeneLogicBox.maxGatesPerRow];

	private GeneLogicBox geneLogicBox;

	public Vector3 gateGridOrigo {
		get {
			return gateTemplate.transform.position;
		}
	}

	public void UpdateConnections() {
		geneLogicBox.UpdateConnections();
	}

	[HideInInspector]
	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;

	private PhenoGenoEnum GetMode() {
		return mode;
	}

	public void Initialize(PhenoGenoEnum mode) {
		this.mode = mode;

		gateRow0 = GameObject.Instantiate(gateTemplate, transform);
		gateRow0.transform.position = gateTemplate.transform.position + Vector3.right * 0f * cellWidth + Vector3.down * 0f * cellHeight;
		gateRow0.transform.SetAsFirstSibling();
		gateRow0.Initialize(mode, this);

		// create small gate pool
		for (int row = 1; row < GeneLogicBox.rowCount; row++) {
			for (int column = 0; column < GeneLogicBox.maxGatesPerRow; column++) {
				LogicBoxGatePanel gate = GameObject.Instantiate(gateTemplate, transform);
				gate.GetComponent<RectTransform>().sizeDelta = new Vector2(cellWidth, cellHeight);
				gate.transform.position = gateTemplate.transform.position + Vector3.right * column * cellWidth + Vector3.down * row * cellHeight;
				gate.transform.SetAsFirstSibling();
				gate.Initialize(mode, this);
				gate.gameObject.SetActive(true);

				if (row == 1) {
					gatesRow1[column] = gate;
				} else {
					gatesRow2[column] = gate;
				}
			}
		}
		gateTemplate.gameObject.SetActive(false);

		// Initialize input boxes
		foreach (LogicBoxInputPanel s in inputRow3) {
			s.Initialize(mode, this);
		}

		MakeDirty();
	}

	public void ConnectToGeneLogic(GeneLogicBox geneLogicBox) {
		this.geneLogicBox = geneLogicBox;
		if (gateRow0 != null) {
			gateRow0.geneLogicBoxGate = geneLogicBox.gateRow0;
			for (int i = 0; i < geneLogicBox.gateRow1.Length; i++) {
				gatesRow1[i].geneLogicBoxGate = geneLogicBox.gateRow1[i]; // just map strait off, doesn't really matter
			}
			for (int i = 0; i < geneLogicBox.gateRow2.Length; i++) {
				gatesRow2[i].geneLogicBoxGate = geneLogicBox.gateRow2[i];
			}
			for (int i = 0; i < geneLogicBox.inputRow3.Length; i++) {
				inputRow3[i].geneLogicBoxInput = geneLogicBox.inputRow3[i];
			}
		}
	}

	public void MarkAsNewForge() {
		CreatureSelectionPanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
		}
	}

	public void OnClickedAddGateRow1() {
		if (mode == PhenoGenoEnum.Genotype && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome && geneLogicBox.TryCreateGate(1, LogicOperatorEnum.And)) {
			UpdateConnections();
			MarkAsNewForge();
			MakeDirty();
		}
	}

	public void OnClickedAddGateRow2() {
		if (mode == PhenoGenoEnum.Genotype && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome && geneLogicBox.TryCreateGate(2, LogicOperatorEnum.And)) {
			UpdateConnections();
			MarkAsNewForge();
			MakeDirty();
		}
	}

	private bool isDirty = false;
	public void MakeDirty() {
		isDirty = true;
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Signal logic box");
			}

			gateRow0.MakeDirty();
			for (int i = 0; i < gatesRow1.Length; i++) {
				gatesRow1[i].MakeDirty();
			}
			for (int i = 0; i < gatesRow2.Length; i++) {
				gatesRow2[i].MakeDirty();
			}
			for (int i = 0; i < inputRow3.Length; i++) {
				inputRow3[i].MakeDirty();
			}

			outputLabel.text = outputText;
			isDirty = false;
		}
	}

	public Gene selectedGene {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return CellPanel.instance.selectedCell != null ? CellPanel.instance.selectedCell.gene : null;
			} else {
				return GeneCellPanel.instance.selectedGene;
			}
		}
	}

	public Cell selectedCell {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return CellPanel.instance.selectedCell;
			} else {
				return null; // there could be many cells selected for the same gene
			}
		}
	}
}
