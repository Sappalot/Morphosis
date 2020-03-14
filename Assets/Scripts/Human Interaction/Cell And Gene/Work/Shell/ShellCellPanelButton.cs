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

		//background.color = ShellCell.GetColor(armorClass, transparencyClass);

		if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.shell) {
			//background.color = ShellCell.GetStrongerColor(armorClass, transparencyClass);
		} else {
			//background.color = ShellCell.GetColor(armorClass, transparencyClass);
		}

		//text.text = string.Format("-{0:F2}", ShellCell.GetEffectCost(armorClass, transparencyClass));
		//text.color = new Color(1f - ShellCell.GetColor(armorClass, transparencyClass).r, 1f - ShellCell.GetColor(armorClass, transparencyClass).g, 1f - ShellCell.GetColor(armorClass, transparencyClass).b);

		buttonImage.fillCenter = false;
		hasSelectedFrame = false;
	}

	public bool hasSelectedFrame {
		set {
			buttonImage.color = value ? ColorScheme.instance.selectedChanged : ColorScheme.instance.notSelectedChanged;
		}
	}

	public void OnClicked() {
		shellCellPanel.SelectButton(this);
	}
}