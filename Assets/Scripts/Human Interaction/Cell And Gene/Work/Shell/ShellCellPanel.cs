using UnityEngine;
using UnityEngine.UI;

public class ShellCellPanel : ComponentPanel {
	//public ShellCellPanelButton templateButton;
	//public Text armorText;
	//public Text transparancyText;

	//private ShellCellPanelButton[,] buttonMatrix = new ShellCellPanelButton[ShellCell.armourClassCount, ShellCell.transparencyClassCount];

	public override void MakeDirty() {
		base.MakeDirty();
	}

	//private void Awake() {
	//	float width = 46;
	//	float height = 22;
	//	for (int t = 0; t < ShellCell.transparencyClassCount; t++) {
	//		for (int a = 0; a < ShellCell.armourClassCount; a++) {
	//			buttonMatrix[a, t] = GameObject.Instantiate(templateButton, transform);
	//			buttonMatrix[a, t].transform.position = templateButton.transform.position + Vector3.right * a * width + Vector3.down * t * height;
	//			buttonMatrix[a, t].transform.SetAsFirstSibling();
	//			buttonMatrix[a, t].Init(a, t);
	//		}
	//	}
	//	templateButton.gameObject.SetActive(false);
	//}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update ShellCellPanel");
			}

			if (gene != null) {
				componentFooterPanel.SetProductionEffectText(0f, GlobalSettings.instance.phenotype.shellCell.effectProductionDown);
				//componentFooterPanel.SetProductionEffectText(0f, ShellCell.GetEffectCost(selectedGene.shellCellArmorClass, GenePanel.instance.cellAndGenePanel.gene.shellCellTransparancyClass));
				
				//armorText.text = string.Format("Armour: {0:F2} (* normal)", ShellCell.GetStrength(selectedGene.shellCellArmorClass));
				//transparancyText.text = string.Format("Transparancy: {0:F0} %", ShellCell.GetTransparancy(selectedGene.shellCellTransparancyClass) * 100f);
				//UpdateButtonMatrix();
			}
			isDirty = false; 
		}
	}

	//public void UpdateButtonMatrix() {
	//	for (int t = 0; t < ShellCell.transparencyClassCount; t++) {
	//		for (int a = 0; a < ShellCell.armourClassCount; a++) {
	//			buttonMatrix[a, t].hasSelectedFrame = false;
	//		}
	//	}
	//	buttonMatrix[GenePanel.instance.cellAndGenePanel.gene.shellCellArmorClass, GenePanel.instance.cellAndGenePanel.gene.shellCellTransparancyClass].hasSelectedFrame = true;
	//}

	public void SelectButton(ShellCellPanelButton button) {
		//if (GetMode() == PhenoGenoEnum.Genotype && IsUnlocked()) {
		//	//GenePanel.instance.cellAndGenePanel.gene.shellCellArmorClass = button.armorClass;
		//	//GenePanel.instance.cellAndGenePanel.gene.shellCellTransparancyClass = button.transparencyClass;
		//	MakeDirty();
		//	if (CreatureSelectionPanel.instance.hasSoloSelected) {
		//		GenePanel.instance.MakeDirty();
		//		CreatureSelectionPanel.instance.soloSelected.MakeDirtyGraphics();
		//	}

		//	OnGenomeChanged(true);
		//}
	}
}