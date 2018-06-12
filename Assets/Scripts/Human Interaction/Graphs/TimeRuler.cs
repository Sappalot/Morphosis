using UnityEngine;
using UnityEngine.UI;

public class TimeRuler : MonoBehaviour {
	public Text explanation;
	public Text[] numbers;
	public Canvas canvas;


	public void UpdateCanvas(Rect graphArea) {
		canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(graphArea.width, graphArea.height);
		canvas.GetComponent<RectTransform>().position = new Vector2(graphArea.center.x, graphArea.center.y);
	}

	public void UpdateGraphics(Rect graphArea, float scale) {
		if (scale > 40f) {
			explanation.text = "s ago";

			numbers[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
			numbers[0].text = "now";

			numbers[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 1f, 0f);
			numbers[1].text = "1";

			numbers[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 2f, 0f);
			numbers[2].text = "2";

			numbers[3].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 3f, 0f);
			numbers[3].text = "3";

			numbers[4].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 4f, 0f);
			numbers[4].text = "4";

			numbers[5].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 5f, 0f);
			numbers[5].text = "5";

			numbers[6].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 6f, 0f);
			numbers[6].text = "6";

			numbers[7].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 7f, 0f);
			numbers[7].text = "7";

			numbers[8].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 8f, 0f);
			numbers[8].text = "8";

			numbers[9].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 9f, 0f);
			numbers[9].text = "9";

			numbers[10].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 10f, 0f);
			numbers[10].text = "10";

		} else if (scale > 5f) {
			explanation.text = "s ago";

			numbers[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
			numbers[0].text = "now";

			numbers[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 10f, 0f);
			numbers[1].text = "10";

			numbers[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 20f, 0f);
			numbers[2].text = "20";

			numbers[3].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 30f, 0f);
			numbers[3].text = "30";

			numbers[4].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 40f, 0f);
			numbers[4].text = "40";

			numbers[5].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 50f, 0f);
			numbers[5].text = "50";

			numbers[6].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 60f, 0f);
			numbers[6].text = "60";

			numbers[7].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 120f, 0f);
			numbers[7].text = "120";

			numbers[8].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 180f, 0f);
			numbers[8].text = "180";

			numbers[9].text = "";
			numbers[10].text = "";
			numbers[11].text = "";
			numbers[12].text = "";
			numbers[13].text = "";
			numbers[14].text = "";
			numbers[15].text = "";
			numbers[16].text = "";
			numbers[17].text = "";
			numbers[18].text = "";
			numbers[19].text = "";
			numbers[20].text = "";
			numbers[21].text = "";
			numbers[22].text = "";
			numbers[23].text = "";
			numbers[24].text = "";
		} else if (scale > 0.5f) {
			explanation.text = "m ago";

			numbers[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
			numbers[0].text = "now";

			numbers[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 1f * 60f, 0f);
			numbers[1].text = "1";

			numbers[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 2f * 60f, 0f);
			numbers[2].text = "2";

			numbers[3].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 3f * 60f, 0f);
			numbers[3].text = "3";

			numbers[4].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 4f * 60f, 0f);
			numbers[4].text = "4";

			numbers[5].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 5f * 60f, 0f);
			numbers[5].text = "5";

			numbers[6].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 6f * 60f, 0f);
			numbers[6].text = "6";

			numbers[7].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 7f * 60f, 0f);
			numbers[7].text = "7";

			numbers[8].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 8f * 60f, 0f);
			numbers[8].text = "8";

			numbers[9].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 9f * 60f, 0f);
			numbers[9].text = "9";

			numbers[10].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 10f * 60f, 0f);
			numbers[10].text = "10";

			numbers[11].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 20f * 60f, 0f);
			numbers[11].text = "20";

			numbers[12].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 30f * 60f, 0f);
			numbers[12].text = "30";

			numbers[13].text = "";
			numbers[14].text = "";
			numbers[15].text = "";
			numbers[16].text = "";
			numbers[17].text = "";
			numbers[18].text = "";
			numbers[19].text = "";
			numbers[20].text = "";
			numbers[21].text = "";
			numbers[22].text = "";
			numbers[23].text = "";
			numbers[24].text = "";
		} else if (scale > 0.1f) {
			explanation.text = "m ago";

			numbers[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
			numbers[0].text = "now";

			numbers[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 10f * 60f, 0f);
			numbers[1].text = "10";

			numbers[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 20f * 60f, 0f);
			numbers[2].text = "20";

			numbers[3].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 30f * 60f, 0f);
			numbers[3].text = "30";

			numbers[4].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 40f * 60f, 0f);
			numbers[4].text = "40";

			numbers[5].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 50f * 60f, 0f);
			numbers[5].text = "50";

			numbers[6].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 60f * 60f, 0f);
			numbers[6].text = "60";

			numbers[7].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 120f * 60f, 0f);
			numbers[7].text = "120";

			numbers[8].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 180f * 60f, 0f);
			numbers[8].text = "180";

			numbers[9].text = "";
			numbers[10].text = "";
			numbers[11].text = "";
			numbers[12].text = "";
			numbers[13].text = "";
			numbers[14].text = "";
			numbers[15].text = "";
			numbers[16].text = "";
			numbers[17].text = "";
			numbers[18].text = "";
			numbers[19].text = "";
			numbers[20].text = "";
			numbers[21].text = "";
			numbers[22].text = "";
			numbers[23].text = "";
			numbers[24].text = "";
		} else if (scale > 0.006f) {
			explanation.text = "h ago";

			numbers[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
			numbers[0].text = "now";

			numbers[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 1f * 3600f, 0f);
			numbers[1].text = ".";

			numbers[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 2f * 3600f, 0f);
			numbers[2].text = "2";

			numbers[3].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 3f * 3600f, 0f);
			numbers[3].text = ".";

			numbers[4].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 4f * 3600f, 0f);
			numbers[4].text = "4";

			numbers[5].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 5f * 3600f, 0f);
			numbers[5].text = ".";

			numbers[6].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 6f * 3600f, 0f);
			numbers[6].text = "6";

			numbers[7].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 7f * 3600f, 0f);
			numbers[7].text = ".";

			numbers[8].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 8f * 3600f, 0f);
			numbers[8].text = "8";

			numbers[9].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 9f * 3600f, 0f);
			numbers[9].text = ".";

			numbers[10].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 10f * 3600f, 0f);
			numbers[10].text = "10";

			numbers[11].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 11f * 3600f, 0f);
			numbers[11].text = ".";

			numbers[12].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 12f * 3600f, 0f);
			numbers[12].text = "12";

			numbers[13].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 13f * 3600f, 0f);
			numbers[13].text = ".";

			numbers[14].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 14f * 3600f, 0f);
			numbers[14].text = "14";

			numbers[15].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 15f * 3600f, 0f);
			numbers[15].text = ".";

			numbers[16].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 16f * 3600f, 0f);
			numbers[16].text = "16";

			numbers[17].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 17f * 3600f, 0f);
			numbers[17].text = ".";

			numbers[18].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 18f * 3600f, 0f);
			numbers[18].text = "18";

			numbers[19].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 19f * 3600f, 0f);
			numbers[19].text = ".";

			numbers[20].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 20f * 3600f, 0f);
			numbers[20].text = "20";

			numbers[21].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 21f * 3600f, 0f);
			numbers[21].text = ".";

			numbers[22].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 22f * 3600f, 0f);
			numbers[22].text = "22";

			numbers[23].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 23f * 3600f, 0f);
			numbers[23].text = ".";

			numbers[24].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 24f * 3600f, 0f);
			numbers[24].text = "24";

		} else if (scale > 0f) {
			explanation.text = "d ago";

			numbers[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
			numbers[0].text = "now";

			numbers[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 1f * 24f * 3600f, 0f);
			numbers[1].text = "1";

			numbers[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 2f * 24f * 3600f, 0f);
			numbers[2].text = "2";

			numbers[3].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 3f * 24f * 3600f, 0f);
			numbers[3].text = "3";

			numbers[4].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 4f * 24f * 3600f, 0f);
			numbers[4].text = "4";

			numbers[5].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 5f * 24f * 3600f, 0f);
			numbers[5].text = "5";

			numbers[6].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 6f * 24f * 3600f, 0f);
			numbers[6].text = "6";

			numbers[7].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 7f * 24f * 3600f, 0f);
			numbers[7].text = "7";

			numbers[8].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 8f * 24f * 3600f, 0f);
			numbers[8].text = "8";

			numbers[9].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 9f * 24f * 3600f, 0f);
			numbers[9].text = "9";

			numbers[10].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 10f * 24f * 3600f, 0f);
			numbers[10].text = "10";

			numbers[11].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 11f * 24f * 3600f, 0f);
			numbers[11].text = "11";

			numbers[12].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 12f * 24f * 3600f, 0f);
			numbers[12].text = "12";

			numbers[13].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 13f * 24f * 3600f, 0f);
			numbers[13].text = "13";

			numbers[14].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 14f * 24f * 3600f, 0f);
			numbers[14].text = "14";

			numbers[15].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 15f * 24f * 3600f, 0f);
			numbers[15].text = "15";

			numbers[16].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 16f * 24f * 3600f, 0f);
			numbers[16].text = "16";

			numbers[17].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 17f * 24f * 3600f, 0f);
			numbers[17].text = "17";

			numbers[18].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 18f * 24f * 3600f, 0f);
			numbers[18].text = "18";

			numbers[19].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 19f * 24f * 3600f, 0f);
			numbers[19].text = "19";

			numbers[20].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 20f * 24f * 3600f, 0f);
			numbers[20].text = "20";

			numbers[21].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 30f * 24f * 3600f, 0f);
			numbers[21].text = "30";

			numbers[22].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 40f * 24f * 3600f, 0f);
			numbers[22].text = "40";

			numbers[23].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 50f * 24f * 3600f, 0f);
			numbers[23].text = "50";

			numbers[24].GetComponent<RectTransform>().anchoredPosition = new Vector2(-scale * 100f * 24f * 3600f, 0f);
			numbers[24].text = "100";

		}

	}
}
