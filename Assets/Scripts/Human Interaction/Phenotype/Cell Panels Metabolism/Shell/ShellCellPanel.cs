using UnityEngine;
using UnityEngine.UI;

public class ShellCellPanel : MetabolismCellPanel {

	public ShellCellPanelButton templateButton;
	public Text productionEffectText;
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
			if (mode == PhenoGenoEnum.Phenotype) {
				productionEffectText.color = ColorScheme.instance.grayedOutGenotype;
				armorText.color = ColorScheme.instance.grayedOutGenotype;
				transparancyText.color = ColorScheme.instance.grayedOutGenotype;
			} else if (mode == PhenoGenoEnum.Genotype) {
				productionEffectText.color = Color.black;
				armorText.color = Color.black;
				transparancyText.color = Color.black;
			}


			Gene selectedGene = CellPanel.instance.selectedCell != null ? CellPanel.instance.selectedCell.gene : null;
			if (selectedGene != null) { // GeneCellPanel.instance.selectedGene
				productionEffectText.text = string.Format("Production Effect: -{0:F2} W", ShellCell.GetEffectCost(selectedGene.shellCellArmorClass, GeneCellPanel.instance.selectedGene.shellCellTransparancyClass));
				armorText.text = string.Format("Strength: {0:F2} (x Normal)", ShellCell.GetStrength(selectedGene.shellCellArmorClass));
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
		buttonMatrix[CellPanel.instance.selectedCell.gene.shellCellArmorClass, CellPanel.instance.selectedCell.gene.shellCellTransparancyClass].hasSelectedFrame = true;
	}

	public void SelectButton(ShellCellPanelButton button) {
		if (mode == PhenoGenoEnum.Genotype && isUnlocked()) {
			GeneCellPanel.instance.selectedGene.shellCellArmorClass = button.armorClass;
			GeneCellPanel.instance.selectedGene.shellCellTransparancyClass = button.transparencyClass;
			MakeDirty();
			if (CreatureSelectionPanel.instance.hasSoloSelected) {
				CreatureSelectionPanel.instance.soloSelected.MakeDirtyGraphics();
			}

			OnChanged();
		}
	}
}