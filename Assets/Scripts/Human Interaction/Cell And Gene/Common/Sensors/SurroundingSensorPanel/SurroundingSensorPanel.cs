using UnityEngine;
using UnityEngine.UI;

public class SurroundingSensorPanel : SignalUnitPanel {
	// ..channels ...
	public Image channelViewAButtonImage; // the images on the buttons, the ones which can be selected to view
	public Image channelViewBButtonImage;
	public Image channelViewCButtonImage;
	public Image channelViewDButtonImage;
	public Image channelViewEButtonImage;
	public Image channelViewFButtonImage;

	public Text channelLabelA; // the text inside of the white squared selected channel button
	public Text channelLabelB;
	public Text channelLabelC;
	public Text channelLabelD;
	public Text channelLabelE;
	public Text channelLabelF;

	public int viewedChannel = 1; // 1 = A

	// ^ channel ^

	public Dropdown channelSensorTypeDropdown;
	public Image channelSensorTypeDropdownImageShow;
	public Image channelSensorImageList;

	// The different panels in the same order as in drop down AND SurroundingSensorChannelSensorTypeEnum
	public SurroundingSensorCreatureCellFovCovPanel creatureCellFovCovPanel;
	public SurroundingSensorTerrainRockFovCovPanel terrainRockFovCovPanel;
	private SurroundingSensorChannelSensorPanel[] channelSensorPanels = new SurroundingSensorChannelSensorPanel[2];

	public GameObject CreatureCellFovCovRoot;
	public GameObject TerrainRockFovCovRoot;

	public Text directionSliderLabel;
	public Slider directionSlider;

	public Text fieldOfViewSliderLabel;
	public Slider fieldOfViewSlider;

	public Text rangeFarSliderLabel;
	public Slider rangeFarSlider;

	public Text rangeNearSliderLabel;
	public Slider rangeNearSlider;

	public override void MakeDirty() {
		base.MakeDirty();
		creatureCellFovCovPanel.MakeDirty();
		terrainRockFovCovPanel.MakeDirty();
	}

	public override void Initialize(PhenoGenoEnum mode, SignalUnitEnum signalUnit, CellAndGenePanel cellAndGenePanel) {
		base.Initialize(mode, signalUnit, cellAndGenePanel);

		channelSensorPanels[0] = creatureCellFovCovPanel;
		channelSensorPanels[1] = terrainRockFovCovPanel;

		foreach (SurroundingSensorChannelSensorPanel m in channelSensorPanels) {
			m.Initialize(mode, cellAndGenePanel, this);
		}

		channelSensorTypeDropdownImageShow.color = ColorScheme.instance.selectedChanged;
		channelSensorImageList.color = ColorScheme.instance.selectedChanged;
	}

	public void OnClickedChannelView(int channel) {
		viewedChannel = channel;
		MakeDirty();
	}

	public void OnChannelTypeDropdownChanged() {
		if (ignoreHumanInput) {
			return;
		}

		(affectedGeneSignalUnit as GeneSurroundingSensor).SetSensorTypeAtChannel(viewedChannel, (SurroundingSensorChannelSensorTypeEnum)channelSensorTypeDropdown.value);

		OnGenomeChanged();
	}

	public void OnDirectionSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		(affectedGeneSignalUnit as GeneSurroundingSensor).direction = (float)directionSlider.value;
		OnGenomeChanged();
	}

	public void OnFieldOfViewSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		(affectedGeneSignalUnit as GeneSurroundingSensor).fieldOfView = (float)fieldOfViewSlider.value;
		OnGenomeChanged();
	}

	public void OnRangeFarSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		(affectedGeneSignalUnit as GeneSurroundingSensor).rangeFar = (float)rangeFarSlider.value;
		OnGenomeChanged();
	}

	public void OnRangeNearSliderMoved() {
		if (ignoreHumanInput) {
			return;
		}

		(affectedGeneSignalUnit as GeneSurroundingSensor).rangeNear = (float)rangeNearSlider.value;
		OnGenomeChanged();
	}

	public override void Update() {
		if (isDirty) {
			base.Update();

			if (GlobalSettings.instance.debug.debugLogMenuUpdate) {
				Debug.Log("Update Surrounding Sensor Panel");
			}

			if (selectedGene != null && affectedGeneSignalUnit != null) {
				ignoreHumanInput = true;

				// .. ghost .. they dont care if they are ghosts at the moment
				creatureCellFovCovPanel.isGhost = isGhost;
				terrainRockFovCovPanel.isGhost = isGhost;
				// ^ ghost ^

				// ... selected channel ...
				channelViewAButtonImage.color = viewedChannel == 1 ? ColorScheme.instance.selectedViewed : ColorScheme.instance.notSelectedViewed;
				channelViewBButtonImage.color = viewedChannel == 2 ? ColorScheme.instance.selectedViewed : ColorScheme.instance.notSelectedViewed;
				channelViewCButtonImage.color = viewedChannel == 3 ? ColorScheme.instance.selectedViewed : ColorScheme.instance.notSelectedViewed;
				channelViewDButtonImage.color = viewedChannel == 4 ? ColorScheme.instance.selectedViewed : ColorScheme.instance.notSelectedViewed;
				channelViewEButtonImage.color = viewedChannel == 5 ? ColorScheme.instance.selectedViewed : ColorScheme.instance.notSelectedViewed;
				channelViewFButtonImage.color = viewedChannel == 6 ? ColorScheme.instance.selectedViewed : ColorScheme.instance.notSelectedViewed;
				// ^ selected channel ^

				// ... channel sensor panel ...
				channelLabelA.text = channelSensorPanels[(int)(affectedGeneSignalUnit as GeneSurroundingSensor).SensorTypeAtChannel(1)].shortName;
				channelLabelB.text = channelSensorPanels[(int)(affectedGeneSignalUnit as GeneSurroundingSensor).SensorTypeAtChannel(2)].shortName;
				channelLabelC.text = channelSensorPanels[(int)(affectedGeneSignalUnit as GeneSurroundingSensor).SensorTypeAtChannel(3)].shortName;
				channelLabelD.text = channelSensorPanels[(int)(affectedGeneSignalUnit as GeneSurroundingSensor).SensorTypeAtChannel(4)].shortName;
				channelLabelE.text = channelSensorPanels[(int)(affectedGeneSignalUnit as GeneSurroundingSensor).SensorTypeAtChannel(5)].shortName;
				channelLabelF.text = channelSensorPanels[(int)(affectedGeneSignalUnit as GeneSurroundingSensor).SensorTypeAtChannel(6)].shortName;

				channelSensorTypeDropdown.value = (int)(affectedGeneSignalUnit as GeneSurroundingSensor).SensorTypeAtChannel(viewedChannel);
				channelSensorTypeDropdown.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				SurroundingSensorChannelSensorTypeEnum viewedChannelType = (affectedGeneSignalUnit as GeneSurroundingSensor).SensorTypeAtChannel(viewedChannel);
				creatureCellFovCovPanel.gameObject.SetActive(viewedChannelType == SurroundingSensorChannelSensorTypeEnum.CreatureCellFovCov);
				terrainRockFovCovPanel.gameObject.SetActive(viewedChannelType == SurroundingSensorChannelSensorTypeEnum.TerrainRockFovCov);
				
				// ^ channel sensor panel ^

				directionSliderLabel.text = string.Format("Direction: {0:F1} °", (affectedGeneSignalUnit as GeneSurroundingSensor).direction);
				directionSlider.value = (affectedGeneSignalUnit as GeneSurroundingSensor).direction;
				directionSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				fieldOfViewSliderLabel.text = string.Format("Field Of View: {0:F1} °", (affectedGeneSignalUnit as GeneSurroundingSensor).fieldOfView);
				fieldOfViewSlider.value = (affectedGeneSignalUnit as GeneSurroundingSensor).fieldOfView;
				fieldOfViewSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				rangeFarSliderLabel.text = string.Format("Range far: {0:F1} m", (affectedGeneSignalUnit as GeneSurroundingSensor).rangeFar);
				rangeFarSlider.value = (affectedGeneSignalUnit as GeneSurroundingSensor).rangeFar;
				rangeFarSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				rangeNearSliderLabel.text = string.Format("Range near: {0:F1} m", (affectedGeneSignalUnit as GeneSurroundingSensor).rangeNear);
				rangeNearSlider.value = (affectedGeneSignalUnit as GeneSurroundingSensor).rangeNear;
				rangeNearSlider.interactable = IsUnlocked() && mode == PhenoGenoEnum.Genotype;

				ignoreHumanInput = false;
			}

			isDirty = false;
		}
	}
}