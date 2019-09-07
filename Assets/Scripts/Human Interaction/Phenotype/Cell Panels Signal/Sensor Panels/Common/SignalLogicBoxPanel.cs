using UnityEngine;
using UnityEngine.UI;

public class SignalLogicBoxPanel : MonoBehaviour {
	private static Vector2 layerSize = new Vector2(270f, 40f);
	public static float cellWidth = layerSize.x * (1f / 5f);
	public static float cellHeight = 40;

	[HideInInspector]
	public string outputText;

	public Text outputLabel;
	public SignalLogicBoxGatePanel gateTemplate;

	private SignalLogicBoxGatePanel gateLayer0;
	private SignalLogicBoxGatePanel[] gatesLayer1 = new SignalLogicBoxGatePanel[GeneLogicBox.maxGatesPerLayer];
	private SignalLogicBoxGatePanel[] gatesLayer2 = new SignalLogicBoxGatePanel[GeneLogicBox.maxGatesPerLayer];

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

		gateLayer0 = GameObject.Instantiate(gateTemplate, transform);
		gateLayer0.transform.position = gateTemplate.transform.position + Vector3.right * 0f * cellWidth + Vector3.down * 0f * cellHeight;
		gateLayer0.transform.SetAsFirstSibling();
		gateLayer0.Initialize(mode, this);

		// create small gate pool
		for (int row = 1; row < GeneLogicBox.rowCount; row++) {
			for (int column = 0; column < GeneLogicBox.maxGatesPerLayer; column++) {
				SignalLogicBoxGatePanel gate = GameObject.Instantiate(gateTemplate, transform);
				gate.GetComponent<RectTransform>().sizeDelta = new Vector2(cellWidth, cellHeight);
				gate.transform.position = gateTemplate.transform.position + Vector3.right * column * cellWidth + Vector3.down * row * cellHeight;
				gate.transform.SetAsFirstSibling();
				gate.Initialize(mode, this);
				gate.gameObject.SetActive(true);

				if (row == 1) {
					gatesLayer1[column] = gate;
				} else {
					gatesLayer2[column] = gate;
				}

			}
		}

		gateTemplate.gameObject.SetActive(false);

		MakeDirty();
		// TODO: istantiate enough of the gates to the other layers as well
	}

	public void ConnectToGeneLogic(GeneLogicBox geneLogicBox) {
		this.geneLogicBox = geneLogicBox;
		if (gateLayer0 != null) {
			gateLayer0.geneLogicBoxGate = geneLogicBox.gateRow0;
			for (int i = 0; i < geneLogicBox.gateRow1.Length; i++) {
				gatesLayer1[i].geneLogicBoxGate = geneLogicBox.gateRow1[i]; // just map strait off, doesn't really matter
			}
			for (int i = 0; i < geneLogicBox.gateRow2.Length; i++) {
				gatesLayer2[i].geneLogicBoxGate = geneLogicBox.gateRow2[i];
			}
		}
	}

	public void OnClickedAddGateRow1() {
		if (geneLogicBox.TryCreateGate(1, LogicOperatorEnum.And) && mode == PhenoGenoEnum.Genotype && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome) {
			UpdateConnections();
			MarkAsNewForge();
			MakeDirty();
		}
	}

	public void OnClickedAddGateRow2() {
		if (geneLogicBox.TryCreateGate(2, LogicOperatorEnum.And) && mode == PhenoGenoEnum.Genotype && CreatureSelectionPanel.instance.soloSelected.allowedToChangeGenome) {
			UpdateConnections();
			MarkAsNewForge();
			MakeDirty();
		}
	}

	private void MarkAsNewForge() {
		CreatureSelectionPanel.instance.MakeDirty();
		GenomePanel.instance.MakeDirty();
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			CreatureSelectionPanel.instance.soloSelected.creation = CreatureCreationEnum.Forged;
			CreatureSelectionPanel.instance.soloSelected.generation = 1;
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

			gateLayer0.MakeDirty();
			for (int i = 0; i < GeneLogicBox.maxGatesPerLayer; i++) {
				gatesLayer1[i].MakeDirty();
				gatesLayer2[i].MakeDirty();
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
