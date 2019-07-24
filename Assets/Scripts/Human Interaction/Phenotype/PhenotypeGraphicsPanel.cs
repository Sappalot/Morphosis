using UnityEngine;
using UnityEngine.UI;

public class PhenotypeGraphicsPanel : MonoSingleton<PhenotypeGraphicsPanel> {
	//Graphics

	public Dropdown graphicsCellDropdown;
	public enum CellGraphicsEnum {
		type,
		energy,
		effect,
		flux,
		leafExposure,
		childCountCreature,
		predatorPray,
		typeAndPredatorPray,
		update,
		creation,
		individual,
		pulse,
		age,
		shell,
		buildPriority, // selected only
		isSleeping,
	}
	[HideInInspector]
	public CellGraphicsEnum graphicsCell {
		get {
			return (CellGraphicsEnum)graphicsCellDropdown.value;
		}
	}

	[HideInInspector]
	public bool isGraphicsCellEnergyRelated {
		get {
			return 
				graphicsCell == CellGraphicsEnum.energy ||
				graphicsCell == CellGraphicsEnum.effect ||
				graphicsCell == CellGraphicsEnum.flux;
		}
	}

	public Dropdown effectMeasuredDropdown;
	public enum EffectMeasureEnum {
		CellTotal,
		CellProduction,
		CellFlux,
		CreatureTotal,
		CreatureProduction,
		CreatureFlux,
	}
	[HideInInspector]
	public EffectMeasureEnum effectMeasure {
		get {
			return (EffectMeasureEnum)effectMeasuredDropdown.value;
		}
	}


}