using UnityEngine;
using UnityEngine.UI;

public class ViewXputPanel : MonoBehaviour {
	[HideInInspector]
	public bool isGhost = false; // Can't be used for this gene/geneCell (will be grayed out)


	public Image backgroundImage;
	public Text label;


	private int viewIndex;

	private bool isDirty;
	private PhenoGenoEnum mode;

	public ViewXput? xput {
		get {
			if (isMouseHovering) {
				return new ViewXput(xputType, signalUnitEnum, viewIndex);
			}
			return null;
		}
	}
	private XputEnum xputType;
	private SignalUnitEnum signalUnitEnum;

	public virtual void MakeDirty() {
		isDirty = true;
	}

	public void Initialize(PhenoGenoEnum mode, XputEnum xputType, SignalUnitEnum signalUnitEnum) {
		this.mode = mode;
		this.xputType = xputType;
		this.signalUnitEnum = signalUnitEnum;
		MakeDirty();
	}

	public bool isMouseHovering;

	public void OnPointerEnter() {
		if (isGhost) {
			return;
		}
		viewIndex = 0;
		backgroundImage.color = ColorScheme.instance.signalViewed;
		isMouseHovering = true;
		MakeDirty();
		MakeArrowsDirty();
	}

	public void OnPointerExit() {
		if (isGhost) {
			return;
		}

		backgroundImage.color = Color.gray;
		isMouseHovering = false;
		MakeDirty();
		MakeArrowsDirty();
	}

	public void OnClicked() {
		if (isGhost) {
			return;
		}
		viewIndex++;
		MakeDirty();
		MakeArrowsDirty();
	}

	private void MakeArrowsDirty() {
		// TODO: Also in world
		if (mode == PhenoGenoEnum.Genotype) {
			GenePanel.instance.cellAndGenePanel.hudSignalArrowHandler.MakeDirtyConnections();
		} else {
			CellPanel.instance.cellAndGenePanel.hudSignalArrowHandler.MakeDirtyConnections();
		}
		
	}

	private void Update() {
		if (isDirty) {
			label.text = (xputType == XputEnum.Output ? "Output" : "Input");

			if (isGhost) {
				label.color = ColorScheme.instance.grayedOut;
				backgroundImage.color = Color.gray;
				return;
			}

			if (isMouseHovering) {
				backgroundImage.color = ColorScheme.instance.signalViewed;
			} else {
				backgroundImage.color = Color.gray;
			}

			label.color = ColorScheme.instance.normalText;
			

			isDirty = false;
		}
	}
}