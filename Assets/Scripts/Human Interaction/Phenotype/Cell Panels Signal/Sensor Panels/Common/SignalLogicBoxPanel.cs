using UnityEngine;
using UnityEngine.UI;

public class SignalLogicBoxPanel : MonoBehaviour {
	public Text outputLabel;
	public SignalLogicBoxGatePanel gateTemplate;

	private const int slotCount = 5;
	private Vector2 layerSize = new Vector2(270f, 40f);

	private SignalLogicBoxGatePanel gateLayer0;
	private SignalLogicBoxGatePanel[] gatesLayer1 = new SignalLogicBoxGatePanel[slotCount / 2];
	private SignalLogicBoxGatePanel[] gatesLayer2 = new SignalLogicBoxGatePanel[slotCount / 2];

	public GeneLogicBox geneLogicBox;

	[HideInInspector]
	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;

	private PhenoGenoEnum GetMode() {
		return mode;
	}

	public void Initialize(PhenoGenoEnum mode) {
		this.mode = mode;


		float width = 100;
		float height = 40;

		gateLayer0 = GameObject.Instantiate(gateTemplate, transform);
		gateLayer0.transform.position = gateTemplate.transform.position + Vector3.right * 0f * width + Vector3.down * 0f * height;
		gateLayer0.transform.SetAsFirstSibling();
		gateLayer0.Initialize(mode);

		gateTemplate.gameObject.SetActive(false);


		MakeDirty();
		// TODO: istantiate enough of the gates to the other layers as well
	}


	[HideInInspector]
	public string outputText;

	private bool isDirty = false;
	public void MakeDirty() {
		isDirty = true;

	}



	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update Signal logic box");
			}

			// put all gates in place

			// update all gates (depending on respective input)
			UpdateReferencesToGene();
			gateLayer0.MakeDirty();

			outputLabel.text = outputText;

			isDirty = false;
		}
	}

	private void UpdateReferencesToGene() {
		if (geneLogicBox != null) {
			gateLayer0.geneLogicBoxGate = geneLogicBox.gateLayer0;
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
