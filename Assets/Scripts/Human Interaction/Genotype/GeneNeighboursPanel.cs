using UnityEngine;

public class GeneNeighboursPanel : MonoBehaviour {
	public GameObject circles;

	private bool isDirty = true;
	public void MakeDirty() {
		isDirty = true;
	}

	public ReferenceGraphics[] referenceGraphics;
	public ReferenceGraphics centerReferenceGraphics;

	public ArrangementPanel arrangementPanelTemplate;
	private ArrangementPanel[] arrangementPanels = new ArrangementPanel[3];

	public void Init() {
		RectTransform originalTransform = arrangementPanelTemplate.GetComponent<RectTransform>();

		for (int index = 0; index < arrangementPanels.Length; index++) {
			arrangementPanels[index] = Instantiate(arrangementPanelTemplate, transform);
			arrangementPanels[index].gameObject.SetActive(true);
			arrangementPanels[index].name = "Arrangement Panel " + index;
			RectTransform spawnTransform = arrangementPanels[index].GetComponent<RectTransform>();
			spawnTransform.position = originalTransform.position + Vector3.right * index * (originalTransform.rect.width + 2);

			arrangementPanels[index].genePanel = this;
		}

		arrangementPanelTemplate.gameObject.SetActive(false);
		arrangementPanelTemplate.arrangementButtons.SetActive(false);
	}

	private ArrangementPanel arrangementPanelAskingForReference;
	public void SetAskingForGeneReference(ArrangementPanel arrangementPanel) {
		arrangementPanelAskingForReference = arrangementPanel;
	}

	public void GiveAnswerGeneReference(Gene gene) {
		arrangementPanelAskingForReference.SetGeneReference(gene);

		MakeDirty();
		GenomePanel.instance.MakeDirty();
		CreatureSelectionPanel.instance.soloSelected.MakeDirtyGraphics();
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate)
				Debug.Log("Update GeneNeighbourPanel");

			//Nothing to represent
			if (GenePanel.instance.selectedGene == null) {
				for (int index = 0; index < arrangementPanels.Length; index++) {
					if (arrangementPanels[index] != null) {
						arrangementPanels[index].arrangement = null;
					}
				}

				circles.SetActive(false);

				isDirty = false;
				return;
			}

			circles.SetActive(true);

			for (int index = 0; index < arrangementPanels.Length; index++) {
				if (arrangementPanels[index] != null) {
					arrangementPanels[index].arrangement = GenePanel.instance.selectedGene.arrangements[index];
				}
			}

			//perifier
			for (int cardinalIndex = 0; cardinalIndex < 6; cardinalIndex++) {
				referenceGraphics[cardinalIndex].reference = GenePanel.instance.selectedGene.GetFlippableReference(cardinalIndex, GenotypePanel.instance.viewedFlipSide);
			}
			centerReferenceGraphics.reference = new GeneReference(GenePanel.instance.selectedGene, GenotypePanel.instance.viewedFlipSide);

			isDirty = false;
		}
	}
}