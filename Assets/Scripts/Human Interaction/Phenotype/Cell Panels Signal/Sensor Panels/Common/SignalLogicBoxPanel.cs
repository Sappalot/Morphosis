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

	public void OnClickedAddGateRow1() {
		if (geneLogicBox.TryCreateGate(1, LogicOperatorEnum.And)) {
			MarkAsNewForge();
			MakeDirty();
		}
	}

	public void OnClickedAddGateRow2() {
		if (geneLogicBox.TryCreateGate(2, LogicOperatorEnum.And)) {
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

			// update all gates (depending on respective input)
			gateLayer0.MakeDirty();

			for (int i = 0; i < GeneLogicBox.maxGatesPerLayer; i++) {
				gatesLayer1[i].MakeDirty();
				if (gatesLayer1[i].geneLogicBoxGate != null) {
					if (gatesLayer1[i].geneLogicBoxGate.isUsed) {
						gatesLayer1[i].transform.position = gateTemplate.transform.position + Vector3.right * gatesLayer1[i].geneLogicBoxGate.leftFlank * cellWidth + Vector3.down * 1f * cellHeight;
						gatesLayer1[i].GetComponent<RectTransform>().sizeDelta = new Vector2(cellWidth * (gatesLayer1[i].geneLogicBoxGate.rightFlank - gatesLayer1[i].geneLogicBoxGate.leftFlank), cellHeight);
					} else {
						gatesLayer1[i].transform.position = gateTemplate.transform.position + Vector3.right * 500f;
					}
				}
			}
			for (int i = 0; i < GeneLogicBox.maxGatesPerLayer; i++) {
				gatesLayer2[i].MakeDirty();
				if (gatesLayer2[i].geneLogicBoxGate != null) {
					if (gatesLayer2[i].geneLogicBoxGate.isUsed) {
						gatesLayer2[i].transform.position = gateTemplate.transform.position + Vector3.right * gatesLayer2[i].geneLogicBoxGate.leftFlank * cellWidth + Vector3.down * 2f * cellHeight;
						gatesLayer2[i].GetComponent<RectTransform>().sizeDelta = new Vector2(cellWidth * (gatesLayer2[i].geneLogicBoxGate.rightFlank - gatesLayer2[i].geneLogicBoxGate.leftFlank), cellHeight);
					} else {
						gatesLayer2[i].transform.position = gateTemplate.transform.position + Vector3.right * 500f;
					}
				}
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
