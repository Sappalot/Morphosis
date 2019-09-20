using UnityEngine;
using UnityEngine.UI;

public class CellPanel : MonoSingleton<CellPanel> {
	// Overview
	public Text typeHeading;

	//----- Shold be same as the top of GeneCellPanel 
	[Header("Metabolism")]
	public EnergyBar energyBar;
	public Text effect;
	public Text armour;

	public Text effectFromNeighbours; //kill me
	public Text effectToNeighbours; //kill me
	public Text effectFromMother; //kill me
	public Text effectToChildren; //kill me

	public Text isOrigo;
	public Text isPlacenta;
	public Text neighboursCount; //own cells + attached cells (mother + children)
	public Text connectedVeinsCount; //number of veins connected to me, non placenta + children
	public Text connectionGroupCount;
	public Text apexAngle;
	public Text eatingOnMeCount; //number of Jaw cells eating on me

	public Button healButton;
	public Button hurtButton;
	public Button deleteButton;

	public Dropdown cellWorkTypeDropdown;

	

	//----- ^ Shold be same as the top of GeneCellPanel ^

	// Metabolism -> specific
	public CellAndGeneComponentPanel eggCellPanel;
	public CellAndGeneComponentPanel fungalCellPanel;
	public CellAndGeneComponentPanel jawCellPanel;
	public CellAndGeneComponentPanel leafCellPanel;
	public CellAndGeneComponentPanel muscleCellPanel;
	public CellAndGeneComponentPanel rootCellPanel;
	public CellAndGeneComponentPanel shellCellPanel;
	public CellAndGeneComponentPanel veinCellPanel;
	private CellAndGeneComponentPanel[] metabolismCellPanels = new CellAndGeneComponentPanel[8];

	public CellAndGeneAxonComponentPanel axonCellPanel;
	public CellAndGeneSignalUnitPanel effectSensorPanel;
	// TODO: more sensor panels

	public CellAndGeneBuildPriorityComponentPanel cellBuildPriorityPanel;
	public CellAndGeneOriginComponentPanel originCellPanel;

	public GeneNeighboursComponentPanel geneNeighbourPanel;

	public CellAndGenePanel cellAndGenePanel;

	override public void Init() {
		isDirty = true;
		metabolismCellPanels[0] = eggCellPanel;
		metabolismCellPanels[1] = fungalCellPanel;
		metabolismCellPanels[2] = jawCellPanel;
		metabolismCellPanels[3] = leafCellPanel;
		metabolismCellPanels[4] = muscleCellPanel;
		metabolismCellPanels[5] = shellCellPanel;
		metabolismCellPanels[6] = rootCellPanel;
		metabolismCellPanels[7] = veinCellPanel;
		foreach (CellAndGeneComponentPanel m in metabolismCellPanels) {
			m.Initialize(PhenoGenoEnum.Phenotype);
		}

		axonCellPanel.Initialize(PhenoGenoEnum.Phenotype);

		cellBuildPriorityPanel.mode = PhenoGenoEnum.Phenotype;
		originCellPanel.mode = PhenoGenoEnum.Phenotype;

		geneNeighbourPanel.gameObject.SetActive(false);

		cellAndGenePanel.Initialize(PhenoGenoEnum.Phenotype);

		MakeDirty();
	}

	private bool isDirty = true;
	public void MakeDirty() {
		isDirty = true;

		foreach (CellAndGeneComponentPanel m in metabolismCellPanels) {
			m.MakeDirty();
		}

		axonCellPanel.MakeDirty();

		//foreach (CellSensorPanel s in sensorPanels) {
		//	s.MakeDirty();
		//}
		
		cellBuildPriorityPanel.MakeDirty();
		originCellPanel.MakeDirty();
	}

	private Cell m_selectedCell;
	public Cell selectedCell {
		get {
			if (m_selectedCell != null) {
				return m_selectedCell;
			}
			if (CreatureSelectionPanel.instance.hasSoloSelected && CreatureSelectionPanel.instance.soloSelected.phenotype.isAlive) {
				if (CreatureSelectionPanel.instance.soloSelected.phenotype.originCell != null) {
					return CreatureSelectionPanel.instance.soloSelected.phenotype.originCell;
				}
				return null;
			}
			return null;
		}
		set {
			m_selectedCell = value;
			MakeDirty();
		}
	}

	public void OnClickDelete() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			World.instance.life.KillCellSafe(selectedCell, World.instance.worldTicks);

			CreatureSelectionPanel.instance.MakeDirty();
			PhenotypePanel.instance.MakeDirty();
			MakeDirty();
		}
	}

	public void OnClickHeal() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			selectedCell.energy = Mathf.Min(selectedCell.energy + 5f, GlobalSettings.instance.phenotype.cellMaxEnergy);

			PhenotypePanel.instance.MakeDirty();
			MakeDirty();
		}
	}

	public void OnClickHurt() {
		if (CreatureSelectionPanel.instance.hasSoloSelected) {
			selectedCell.energy -= 5f;

			PhenotypePanel.instance.MakeDirty();
			MakeDirty();
		}
	}

	private void Update() {
		if (isDirty) {
			if (GlobalSettings.instance.printoutAtDirtyMarkedUpdate) {
				Debug.Log("Update CellPanel");
			}

			//All off, we may turn on 1 of them later 
			foreach (CellAndGeneComponentPanel m in metabolismCellPanels) {
				m.gameObject.SetActive(false);
			}

			cellWorkTypeDropdown.interactable = false; //can't change cell type

			energyBar.effectMeasure = EffectTempEnum.None;
			if (PhenotypeGraphicsPanel.instance.graphicsCell == PhenotypeGraphicsPanel.CellGraphicsEnum.effect) {
				if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellTotal) {
					energyBar.effectMeasure = EffectTempEnum.Total;
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellProduction) {
					energyBar.effectMeasure = EffectTempEnum.Production;
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellFlux) {
					energyBar.effectMeasure = EffectTempEnum.Flux;
				}
			}

			//Nothing to represent
			if (selectedCell == null || !CreatureSelectionPanel.instance.hasSoloSelected) {
				typeHeading.text = "Cell:";

				energyBar.isOn = false;
				if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellTotal || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureTotal) {
					effect.text = "Total Effect: ";
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellProduction || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureProduction) {
					effect.text = "Production Effect: ";
				} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellFlux || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureFlux) {
					effect.text = "Flux Effect: ";
				}


				armour.text = "Armour: ";

				effectFromNeighbours.text = "P me <= neighbours:";
				effectToNeighbours.text = "P me => neighbours:";
				effectFromMother.text = "P me <= mother:";
				effectToChildren.text = "P me => children:";
				isOrigo.text = "-";
				isPlacenta.text = "-";
				neighboursCount.text = "Neighbours:";
				connectionGroupCount.text = "Connection Groups:";
				apexAngle.text = "Apex angle:";
				connectedVeinsCount.text = "Veins:";
				eatingOnMeCount.text = "Eating on me:";

				isDirty = false;
				return;
			}
			typeHeading.text = "Cell: " + selectedCell.GetCellType().ToString();

			energyBar.isOn = true;
			energyBar.fullness = selectedCell.energyFullness;
			energyBar.effectTotal = selectedCell.GetEffect(true, true, true, true);
			energyBar.effectProd = selectedCell.GetEffect(true, false, false, false);
			energyBar.effectStress = selectedCell.GetEffect(false, true, false, false);
			energyBar.effectFlux = selectedCell.GetEffect(false, false, true, true);

			if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellTotal || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureTotal) {
				effect.text = string.Format("Total Effect: {0:F2} - {1:F2} = {2:F2}W", selectedCell.GetEffectUp(true, true, true), selectedCell.GetEffectDown(true, true, true, true), selectedCell.GetEffect(true, true, true, true));
			} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellProduction || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureProduction) {
				effect.text = string.Format("Production Effect: {0:F2} - {1:F2} = {2:F2}W", selectedCell.GetEffectUp(true, false, false), selectedCell.GetEffectDown(true, false, false, false), selectedCell.GetEffect(true, false, false, false));
			} else if (PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CellFlux || PhenotypeGraphicsPanel.instance.effectMeasure == PhenotypeGraphicsPanel.EffectMeasureEnum.CreatureFlux) {
				effect.text = string.Format("Flux Effect: {0:F2} - {1:F2} = {2:F2}W", selectedCell.GetEffectUp(false, true, true), selectedCell.GetEffectDown(false, false, true, true), selectedCell.GetEffect(false, false, true, true));
			}

			if (selectedCell != null) {
				armour.text = string.Format("Armour: {0:F2} ==> Stress effect: {1:F2} W", selectedCell.gene.armour, GlobalSettings.instance.phenotype.jawCellEatEffectAtSpeed.Evaluate(20f) / selectedCell.armour);
			}

			effectFromNeighbours.text = string.Format("P me <= neighbours: {0:F2}W", selectedCell.effectFluxFromSelf); //kill me
			effectToNeighbours.text =   string.Format("P me => neighbours: {0:F2}W", selectedCell.effectFluxToSelf); //kill me
			effectFromMother.text =     string.Format("P me <= mother: {0:F2}W",     selectedCell.effectFluxFromMotherAttached); //kill me
			effectToChildren.text =     string.Format("P me => children: {0:F2}W",   selectedCell.effectFluxToChildrenAttached); //kill me

			isOrigo.text = selectedCell.isOrigin ? "Origin" : "...";
			isPlacenta.text = selectedCell.isPlacenta ? "Placenta" : "...";
			neighboursCount.text = "Neighbours: " + (selectedCell.neighbourCountAll - selectedCell.neighbourCountConnectedRelatives) + ((selectedCell.neighbourCountConnectedRelatives > 0) ? (" + "  + selectedCell.neighbourCountConnectedRelatives + " rel.") : "");
			connectionGroupCount.text = "Connection Groups: " + selectedCell.groups;
			apexAngle.text = "Apex angle: " + selectedCell.apexAngle.ToString();
			connectedVeinsCount.text = "Veins: " + selectedCell.creature.phenotype.NonPlacentaVeinsConnectedToCellCount(selectedCell) + (selectedCell.creature.phenotype.PlacentaVeinsConnectedToCellCount(selectedCell) > 0 ? (" + " + selectedCell.creature.phenotype.PlacentaVeinsConnectedToCellCount(selectedCell) + " children") : "") ; 
			eatingOnMeCount.text = "Eating on me: " + selectedCell.predatorCount;

			healButton.gameObject.SetActive(true);
			hurtButton.gameObject.SetActive(true);
			deleteButton.gameObject.SetActive(true);

			if (selectedCell.GetCellType() == CellTypeEnum.Egg) {
				cellWorkTypeDropdown.value = 0;
				eggCellPanel.gameObject.SetActive(true);
				eggCellPanel.MakeDirty();
			} else if (selectedCell.GetCellType() == CellTypeEnum.Fungal) {
				cellWorkTypeDropdown.value = 1;
				fungalCellPanel.gameObject.SetActive(true);
				fungalCellPanel.MakeDirty();
			} else if (selectedCell.GetCellType() == CellTypeEnum.Jaw) {
				cellWorkTypeDropdown.value = 2;
				jawCellPanel.gameObject.SetActive(true);
				jawCellPanel.MakeDirty();
			} else if (selectedCell.GetCellType() == CellTypeEnum.Leaf) {
				cellWorkTypeDropdown.value = 3;
				leafCellPanel.gameObject.SetActive(true);
				leafCellPanel.MakeDirty();
			} else if (selectedCell.GetCellType() == CellTypeEnum.Muscle) {
				cellWorkTypeDropdown.value = 4;
				muscleCellPanel.gameObject.SetActive(true);
				muscleCellPanel.MakeDirty();
			} else if (selectedCell.GetCellType() == CellTypeEnum.Root) {
				cellWorkTypeDropdown.value = 5;
				rootCellPanel.gameObject.SetActive(true);
				rootCellPanel.MakeDirty();
			} else if (selectedCell.GetCellType() == CellTypeEnum.Shell) {
				cellWorkTypeDropdown.value = 6;
				shellCellPanel.gameObject.SetActive(true);
				shellCellPanel.MakeDirty();
			} else if (selectedCell.GetCellType() == CellTypeEnum.Vein) {
				cellWorkTypeDropdown.value = 7;
				veinCellPanel.gameObject.SetActive(true);
				veinCellPanel.MakeDirty();
			}

			if (selectedCell.isOrigin) {
				originCellPanel.gameObject.SetActive(true);
				originCellPanel.MakeDirty();

				cellBuildPriorityPanel.gameObject.SetActive(false);

			} else {
				originCellPanel.gameObject.SetActive(false);

				cellBuildPriorityPanel.gameObject.SetActive(true);
				cellBuildPriorityPanel.MakeDirty();
			}

			HudSignalArrowHandler.MakeDirtyConnections();

			isDirty = false;
		}
	}
}