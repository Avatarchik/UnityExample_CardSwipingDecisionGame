using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

/*
 * The script 'HighScoreNameLinker' takes a high score (from multible possible sources)
 * and combines it with the player identity where it origins from. 
 * The highscores and the identity are shown some frames after the start of the game.
 */

public class HighScoreNameLinker : MonoBehaviour {

	[System.Serializable]
	public class highScoreNamePair{

		public string countryName;
		public int highScore;
		public bool valid = false;
	}

	[Tooltip("Loaded/actual highscore pair.")]
	[ReadOnlyInspector]public highScoreNamePair hsnPair;

	public enum hsnSelection
	{
		valueScriptActual,
		valueScriptMinimal,
		valueScriptMaximal,
		scoreCounter
	}
	[Tooltip("Select the source of the score.")]
	public hsnSelection highScoreSource;
	[Tooltip("Save key is auto generated from the linked scripts.")]
	[HideInInspector]public string key = "saveKey";
	public scoreCounter sc;
	ValueScript vs;
	public ValueDefinitions.값정의 valueType;
	[Tooltip("Select text field to show the player identity with the score.")]
	public Text countryNameText;
	[Tooltip("Select text field to show the score.")]
	public Text highScoreText;

	void Awake(){
	}

	void Start(){
		clearUI ();
		StartCoroutine (delayed ());
	}

	IEnumerator delayed(){
		yield return null;
		yield return null;
		yield return null;
		generateSaveKey ();
		load ();
		displayHighScorePair ();
	}

	void getVSscript(){
		vs = ValueManager.나자신.getFirstFittingValue (valueType);
	}

	void generateSaveKey(){
		getVSscript ();
		if (highScoreSource == hsnSelection.valueScriptActual){
			key = vs.내역활.ToString() + "_HS_pair_act";
		} else if (highScoreSource == hsnSelection.valueScriptMaximal) {
			key = vs.내역활.ToString() + "_HS_pair_max";
		} else if (highScoreSource == hsnSelection.valueScriptMinimal) {
			key = vs.내역활.ToString() + "_HS_pair_min";
		} else if (highScoreSource == hsnSelection.scoreCounter) {
			key = sc.key + "_HS_pair";
		}
	}

	public void displayHighScorePair(){
		if (countryNameText != null) {
			if (hsnPair.valid == true) {
				countryNameText.text = hsnPair.countryName;
			} else {
				countryNameText.text = "no entry yet";
			}
		}
		if (highScoreText != null) {
			if (hsnPair.valid == true) {
				highScoreText.text = hsnPair.highScore.ToString ();
			} else {
				highScoreText.text = "0";
			}
		}
	}

	public void clearUI(){
		if (countryNameText != null) {
			countryNameText.text = "";
		}
		if (highScoreText != null) {
			highScoreText.text = "";
		}
	}

	void save(){
		PlayerPrefs.SetString(key, JsonUtility.ToJson(hsnPair));
	}

	void load(){
		if (PlayerPrefs.HasKey (key)) {
			string json = PlayerPrefs.GetString (key);
			JsonUtility.FromJsonOverwrite (json, hsnPair);
		} else {
			//no save exists, generate one.
			hsnPair.countryName = "none";
			if (highScoreSource == hsnSelection.valueScriptMinimal) {
				hsnPair.highScore = 9999;
			} else {
				hsnPair.highScore = 0;
			}
			hsnPair.valid = false;
			save ();
		}
	}


	/*
	 * Generate the actual pair of the high score and the player identity.
	 * The new pair is only saved, if a new high score is achieved.
	 * This function is called by 'HighScoreNameLinkerGroup' by the method 'generateLinks()' to
	 * generate all at once.
	 */
	public void generateHighScoreNameLink(){
		int score = 0;
		bool newHighScore = false;

		generateSaveKey ();
		load ();

		getVSscript ();
		if (highScoreSource == hsnSelection.valueScriptActual) {
			score = Mathf.RoundToInt (vs.플레이어프랩스데이터);
		} else if (highScoreSource == hsnSelection.valueScriptMaximal) {
			score = Mathf.RoundToInt (vs.getMaxValue ());
		} else if (highScoreSource == hsnSelection.valueScriptMinimal) {
			score = Mathf.RoundToInt (vs.getMinValue ());
		} else if (highScoreSource == hsnSelection.scoreCounter) {
			score = sc.getScore();
		}

		//two cases: save the highest or the lowest value;
		if (highScoreSource == hsnSelection.valueScriptMinimal) {
			//getting new lowest value
			if (score < hsnPair.highScore) {
				newHighScore = true;
			} 
		} else {
			//getting new highest value
			if(score > hsnPair.highScore){
				newHighScore = true;
			}
		}

		if (newHighScore == true) {
			hsnPair.highScore = score;
			hsnPair.countryName = CountryNameGenerator.instance.getCountryNameText ();
			hsnPair.valid = true;
			save ();
			//Debug.Log ("new " + key);
		} else {
			//Debug.Log ("no " + key);
		}
	}
}
