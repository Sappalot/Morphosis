using UnityEngine;
using UnityEngine.UI;

public class ShellCellPanel : CellComponentPanel {
	public Text productionEffectText;

	public ShellCellPanelButton templateButton;
	public Text armorText;
	public Text transparancyText;

	private ShellCellPanelButton[,] buttonMatrix = new ShellCellPanelButton[ShellCell.armourClassCount, ShellCell.transparencyClassCount];

	private void Awake() {
		float width = 46;
		float height = 22;
		for (int t = 0; t < ShellCell.transparencyClassCount; t++) {
			for (int a = 0; a < ShellCell.armourClassCount; a++) {
				buttonMatrix[a, t] = GameObject.Instantiate(templateButton, transform);
				buttonMatrix[a, t].transform.position = templateButton.transform.position + Vector3.right * a * width + Vector3.down * t * height;
				buttonMatrix[a, t].transform.SetAsFirstSibling();
				buttonMatrix[a, t].Init(a, t);
			}
		}
		templateButton.gameObject.SetActive(false);
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update ShellCellPanel");
			}

			if (selectedGene != null) {
				productionEffectText.text = string.Format("Production Effect: 0.00 - {0:F2} W", ShellCell.GetEffectCost(selectedGene.shellCellArmorClass, GenePanel.instance.selectedGene.shellCellTransparancyClass));
				armorText.text = string.Format("Armour: {0:F2} (* normal)", ShellCell.GetStrength(selectedGene.shellCellArmorClass));
				transparancyText.text = string.Format("Transparancy: {0:F0} %", ShellCell.GetTransparancy(selectedGene.shellCellTransparancyClass) * 100f);
				UpdateButtonMatrix();
			}

			isDirty = false; 
		}
	}

	public void UpdateButtonMatrix() {
		for (int t = 0; t < ShellCell.transparencyClassCount; t++) {
			for (int a = 0; a < ShellCell.armourClassCount; a++) {
				buttonMatrix[a, t].hasSelectedFrame = false;
			}
		}
		buttonMatrix[GenePanel.instance.selectedGene.shellCellArmorClass, GenePanel.instance.selectedGene.shellCellTransparancyClass].hasSelectedFrame = true;
	}

	public void SelectButton(ShellCellPanelButton button) {
		if (GetMode() == PhenoGenoEnum.Genotype && IsUnlocked()) {
			GenePanel.instance.selectedGene.shellCellArmorClass = button.armorClass;
			GenePanel.instance.selectedGene.shellCellTransparancyClass = button.transparencyClass;
			MakeDirty();
			if (CreatureSelectionPanel.instance.hasSoloSelected) {
				GenePanel.instance.MakeDirty();
				CreatureSelectionPanel.instance.soloSelected.MakeDirtyGraphics();
			}

			MakeCreatureChanged();
		}
	}
}