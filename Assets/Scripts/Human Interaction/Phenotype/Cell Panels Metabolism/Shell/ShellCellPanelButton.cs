using UnityEngine;
using UnityEngine.UI;

public class ShellCellPanelButton : MonoBehaviour {
	public ShellCellPanel shellCellPanel;

	public Button button;
	public Text text;
	public Image background;
	public Image buttonImage;

	public int armorClass { get; private set; }
	public int transparencyClass { get; private set; }

	public void Init(int armorClass, int transparencyClass) {
		this.armorClass = armorClass;
		this.transparencyClass = transparencyClass;

		background.color = ShellCell.GetColor(armorClass, transparencyClass);

		if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.shell) {
			background.color = ShellCell.GetStrongerColor(armorClass, transparencyClass);
		} else {
			background.color = ShellCell.GetColor(armorClass, transparencyClass);
		}

		text.text = string.Format("-{0:F2}", ShellCell.GetEffectCost(armorClass, transparencyClass));
		buttonImage.fillCenter = false;
		hasSelectedFrame = false;
	}

	public bool hasSelectedFrame {
		set {
			buttonImage.color = value ? (CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype ? ColorScheme.instance.selectedButton : ColorScheme.instance.grayedOutGenotype) : //selected
										(CreatureEditModePanel.instance.mode == PhenoGenoEnum.Genotype ? ColorScheme.instance.notSelectedButton : Color.clear); // not selected
		}
	}

	public void OnClicked() {
		shellCellPanel.SelectButton(this);
	}

}
