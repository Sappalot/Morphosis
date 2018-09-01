using UnityEngine;
using UnityEngine.UI;

public class GeneEditModePanel : MonoSingleton<GeneEditModePanel> {
	public Image cellModeImage;
	public Image neighboursModeImage;

	private bool isDirty = true;

	private GeneEditModeEnum m_mode;
	public GeneEditModeEnum mode {
		get {
			return m_mode;
		}
	}

	public void Start() {
		m_mode = GeneEditModeEnum.Cell;
		isDirty = true;
	}

	public void Restart() {
		m_mode = GeneEditModeEnum.Cell;
		isDirty = true;
	}

	public void OnClickedCellEditMode() {
		m_mode = GeneEditModeEnum.Cell;
		isDirty = true;
	}

	public void OnClickedNeighboursEditMode() {
		m_mode = GeneEditModeEnum.Neighbours;
		isDirty = true;
	}
	
	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate)
				Debug.Log("Update GeneEditModePanel");

			cellModeImage.color = (mode == GeneEditModeEnum.Cell) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;
			neighboursModeImage.color = (mode == GeneEditModeEnum.Neighbours) ? ColorScheme.instance.selectedButton : ColorScheme.instance.notSelectedButton;

			GeneCellPanel.instance.gameObject.SetActive(mode == GeneEditModeEnum.Cell);
			GeneNeighboursPanel.instance.gameObject.SetActive(mode == GeneEditModeEnum.Neighbours);

			isDirty = false;
		}
	}
}
