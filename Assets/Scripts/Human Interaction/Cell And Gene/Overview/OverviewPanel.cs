using UnityEngine;
using UnityEngine.UI;

// general most important Cell- and (Gene) information
public class OverviewPanel : MonoBehaviour {
	public EnergyBar energyBar;
	public Text effectLabel;
	public Text armourLabel;
	public Text transparencyLabel;

	public Text effectFromNeighboursLabel; //kill me
	public Text effectToNeighboursLabel; //kill me
	public Text effectFromMotherLabel; //kill me
	public Text effectToChildrenLabel; //kill me

	public Text isOrigoLabel;
	public Text isPlacentaLabel;
	public Text neighboursCountLabel; //own cells + attached cells (mother + children)
	public Text connectedVeinsCountLabel; //number of veins connected to me, non placenta + children
	public Text connectionGroupCountLabel;
	public Text apexAngleLabel;
	public Text eatingOnMeCountLabel; //number of Jaw cells eating on me
	public ComponentFooterPanel footerPanel; //footer

	public Button healButton;
	public Button hurtButton;
	public Button deleteButton;

	private PhenoGenoEnum mode = PhenoGenoEnum.Phenotype;
	private bool isDirty = true;
	public void MakeDirty() {
		isDirty = true;
	}

	public void Initialize(PhenoGenoEnum mode) {
		this.mode = mode;
		MakeDirty();
	}

	public void OnClickDelete() {
		if (CreatureSelectionPanel.instance.hasSoloSelected && mode == PhenoGenoEnum.Phenotype) {
			World.instance.life.KillCellSafe(CellPanel.instance.selectedCell, World.instance.worldTicks);

			CreatureSelectionPanel.instance.MakeDirty();
			PhenotypePanel.instance.MakeDirty();
			MakeDirty();
		}
	}

	public void OnClickHeal() {
		if (CreatureSelectionPanel.instance.hasSoloSelected && mode == PhenoGenoEnum.Phenotype) {
			CellPanel.instance.selectedCell.energy = Mathf.Min(CellPanel.instance.selectedCell.energy + 5f, GlobalSettings.instance.phenotype.cellMaxEnergy);

			PhenotypePanel.instance.MakeDirty();
			MakeDirty();
		}
	}

	public void OnClickHurt() {
		if (CreatureSelectionPanel.instance.hasSoloSelected && mode == PhenoGenoEnum.Phenotype) {
			CellPanel.instance.selectedCell.energy -= 5f;

			PhenotypePanel.instance.MakeDirty();
			MakeDirty();
		}
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			energyBar.effectMeasure = EffectTempEnum.None;
			if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.effect) {
				if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellTotal) {
					energyBar.effectMeasure = EffectTempEnum.Total;
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellProduction) {
					energyBar.effectMeasure = EffectTempEnum.Production;
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellExternal) {
					energyBar.effectMeasure = EffectTempEnum.External;
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellFlux) {
					energyBar.effectMeasure = EffectTempEnum.Flux;
				}
			}

			//Nothing to represent
			if ((mode == PhenoGenoEnum.Phenotype && selectedCell == null) || (mode == PhenoGenoEnum.Genotype && selectedGene == null) || !CreatureSelectionPanel.instance.hasSoloSelected) {

				//energyBar.isOn = false;
				//if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellTotal || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureTotal) {
				//	effectLabel.text = "Total Effect: ";
				//} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellProduction || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureProduction) {
				//	effectLabel.text = "Production Effect: ";
				//} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellFlux || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureFlux) {
				//	effectLabel.text = "Flux Effect: ";
				//}
				//armourLabel.text = "Armour: ";

				//effectFromNeighboursLabel.text = "P me <= neighbours:";
				//effectToNeighboursLabel.text = "P me => neighbours:";
				//effectFromMotherLabel.text = "P me <= mother:";
				//effectToChildrenLabel.text = "P me => children:";
				//isOrigoLabel.text = "-";
				//isPlacentaLabel.text = "-";
				//neighboursCountLabel.text = "Neighbours:";
				//connectionGroupCountLabel.text = "Connection Groups:";
				//apexAngleLabel.text = "Apex angle:";
				//connectedVeinsCountLabel.text = "Veins:";
				//eatingOnMeCountLabel.text = "Eating on me:";

				isDirty = false;
				return;
			}

			if (mode == PhenoGenoEnum.Phenotype) {
				energyBar.isOn = true;
				energyBar.fullness = selectedCell.energyFullness;
				energyBar.effectTotal = selectedCell.Effect(true, true, true, true);
				energyBar.effectProd = selectedCell.Effect(true, false, false, false);
				energyBar.effectExternal = selectedCell.Effect(false, true, false, false);
				energyBar.effectFlux = selectedCell.Effect(false, false, true, true);

				if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellTotal || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureTotal) {
					effectLabel.text = string.Format("Total Effect: {0:F2} - {1:F2} = {2:F2}W", selectedCell.EffectUp(true, true, true), selectedCell.EffectDown(true, true, true, true), selectedCell.Effect(true, true, true, true));
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellProduction || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureProduction) {
					effectLabel.text = string.Format("Production Effect: {0:F2} - {1:F2} = {2:F2}W", selectedCell.EffectUp(true, false, false), selectedCell.EffectDown(true, false, false, false), selectedCell.Effect(true, false, false, false));
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellExternal || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureExternal) {
					effectLabel.text = string.Format("External Effect: {0:F2} - {1:F2} = {2:F2}W", 0f, selectedCell.EffectDown(false, true, false, false), selectedCell.EffectDown(false, true, false, false));
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellFlux || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureFlux) {
					effectLabel.text = string.Format("Flux Effect: {0:F2} - {1:F2} = {2:F2}W", selectedCell.EffectUp(false, true, true), selectedCell.EffectDown(false, false, true, true), selectedCell.Effect(false, false, true, true));
				}

				effectFromNeighboursLabel.text = string.Format("P me <= neighbours: {0:F2}W", selectedCell.effectFluxFromSelf); //kill me
				effectToNeighboursLabel.text = string.Format("P me => neighbours: {0:F2}W", selectedCell.effectFluxToSelf); //kill me
				effectFromMotherLabel.text = string.Format("P me <= mother: {0:F2}W", selectedCell.effectFluxFromMotherAttached); //kill me
				effectToChildrenLabel.text = string.Format("P me => children: {0:F2}W", selectedCell.effectFluxToChildrenAttached); //kill me

				isOrigoLabel.text = selectedCell.isOrigin ? "Origin" : "...";
				isPlacentaLabel.text = selectedCell.isPlacenta ? "Placenta" : "...";
				neighboursCountLabel.text = "Neighbours: " + (selectedCell.neighbourCountAll - selectedCell.neighbourCountConnectedRelatives) + ((selectedCell.neighbourCountConnectedRelatives > 0) ? (" + " + selectedCell.neighbourCountConnectedRelatives + " rel.") : "");
				connectionGroupCountLabel.text = "Connection Groups: " + selectedCell.groups;
				apexAngleLabel.text = "Apex angle: " + selectedCell.apexAngle.ToString();
				connectedVeinsCountLabel.text = "Veins: " + selectedCell.creature.phenotype.NonPlacentaVeinsConnectedToCellCount(selectedCell) + (selectedCell.creature.phenotype.PlacentaVeinsConnectedToCellCount(selectedCell) > 0 ? (" + " + selectedCell.creature.phenotype.PlacentaVeinsConnectedToCellCount(selectedCell) + " children") : "");
				eatingOnMeCountLabel.text = "Eating on me: " + selectedCell.predatorCount;

				footerPanel.SetProductionEffectText(selectedCell.EffectUp(true, false, false), selectedCell.EffectDown(true, false, false, false));

				if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellProduction || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureProduction) {
					footerPanel.productionEffectLabel.color = Color.gray; // since we have it presented above allready, confusing to show it twice
				} else {
					footerPanel.productionEffectLabel.color = Color.black;
				}
			} else {
				energyBar.isOn = false;
				if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellTotal || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureTotal) {
					effectLabel.text = "Total Effect: ";
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellProduction || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureProduction) {
					effectLabel.text = "Production Effect: ";
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellExternal || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureExternal) {
					effectLabel.text = "External Effect: ";
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellFlux || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureFlux) {
					effectLabel.text = "Flux Effect: ";
				}

				effectFromNeighboursLabel.text = "P me <= neighbours: -";
				effectToNeighboursLabel.text = "P me => neighbours: -";
				effectFromMotherLabel.text = "P me <= mother: -";
				effectToChildrenLabel.text = "P me => children: -";
				isOrigoLabel.text = "-";
				isPlacentaLabel.text = "-";
				neighboursCountLabel.text = "Neighbours: -";
				connectionGroupCountLabel.text = "Connection Groups: -";
				apexAngleLabel.text = "Apex angle: -";
				connectedVeinsCountLabel.text = "Veins: -";
				eatingOnMeCountLabel.text = "Eating on me: -";

				footerPanel.SetProductionEffectText("Production Effect: todo [1.00...4.00] - [1.00...2.00] = [-1.00...3.00] W");
			}

			armourLabel.text = string.Format("Armour: {0:F2} ==> Stress effect: {1:F2} W", selectedGene.armour, GlobalSettings.instance.phenotype.jawCell.effectProductionUpAtSpeed.Evaluate(20f) / selectedGene.armour);
			transparencyLabel.text = string.Format("Transparency: {0:F2}", selectedGene.transparancy);

			healButton.gameObject.SetActive(mode == PhenoGenoEnum.Phenotype);
			hurtButton.gameObject.SetActive(mode == PhenoGenoEnum.Phenotype);
			deleteButton.gameObject.SetActive(mode == PhenoGenoEnum.Phenotype);
			
		}
	}

	public Gene selectedGene {
		get {
			if (mode == PhenoGenoEnum.Phenotype) {
				return CellPanel.instance.selectedCell != null ? CellPanel.instance.selectedCell.gene : null;
			} else {
				return GenePanel.instance.selectedGene;
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