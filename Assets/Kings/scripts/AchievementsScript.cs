using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AchievementsScript : TranslatableContent {
	
	public static AchievementsScript instance;

	void Awake(){
		instance = this;
	}

	/*
	 * Specification of all the possible achivements.
	 */
	public enum achievementTyp{
		marry,
		rule20
	}

	[Tooltip("If an achivement was triggered it can be shown with an animator. Link this animator here.")]
	public Animator achievementAnimator;
	[Tooltip("If an achivement was triggered it can be shown with an animator. Specify the trigger for the animator here.")]
	public string triggerOnAchievement = "show";
	[Tooltip("If an achivement was triggered it can update information about the achivement. Specify the placeholder title text display here.")]
	public Text anim_titleText;
	[Tooltip("If an achivement was triggered it can update information about the achivement. Specify the placeholder description text display here.")]
	public Text anim_descriptionText;
	[Tooltip("If an achivement was triggered it can update information about the achivement. Specify the placeholder image display here.")]
	public Image anim_achievementImage;

	[Tooltip("The achievement progress ('3/20') can be displayed. Specify the placeholder text display here.")]
	public Text achievementProgressText;

	[System.Serializable]
	public class achievement{
		public achievementTyp typ;
		public int achievementCnt;
	}

	[System.Serializable]
	public class achievementConfig{
		[Tooltip("Specification the type of achivement here. For expanding the list modify the structure 'achievementTyp' within this script.")]
		public achievementTyp typ;
		[Tooltip("The number, how often the achivement was met. The animator for displaying the achivement is only triggered the first time.")]
		[ReadOnlyInspector]public int achievementCnt;
		[Tooltip("If the achivement was met, a gameobject can be activated. E.g. display the achievement in a gallery.")]
		public GameObject achievementGameobject;
		[Tooltip("The title text for the achievement. If you want to use translations, this text can be used as term.")]
		public string title;
		[Tooltip("The description text for the achievement. If you want to use translations, this text can be used as term.")]
		public string description;
		[Tooltip("The sprite for the achievement. If you want to use translations, this text can be used as term.")]
		public Sprite sprite;

		public void save(){
			PlayerPrefs.SetInt("achievement_" + typ.ToString(), achievementCnt);
		}

		public void load(){
			achievementCnt = PlayerPrefs.GetInt("achievement_" + typ.ToString());
		}
	}

	public achievementConfig[] achievements;

	void load(){
		foreach (achievementConfig ac in achievements) {
			ac.load ();
		}

		countAndShowAchieventProgesss ();
	}

	public void countAndShowAchieventProgesss(){
		int achievementsOverall = 0;
		int achievementsDone = 0;
		foreach (achievementConfig ac in achievements) {
			achievementsOverall++;
			if (ac.achievementCnt > 0 ) {
				achievementsDone++;
			}
		}

		if (achievementProgressText != null) {
			achievementProgressText.text = achievementsDone.ToString () + "/" + achievementsOverall.ToString ();
		}
	}

	public void activatGameObjects(){
		foreach (achievementConfig ac in achievements) {
			if (ac.achievementCnt > 0 && ac.achievementGameobject.activeSelf == false) {
				ac.achievementGameobject.SetActive (true);
			}
		}
	}
		
	void Start () {
		load ();
		activatGameObjects ();	//activate the gameobjects e.g. in gallery for the already scored achievements
		TranslationManager.instance.registerTranslateableContentScript (this);
	}

	public void addAchievement(achievementTyp typ){
		load ();
		foreach (achievementConfig ac in achievements) {
			if (ac.typ == typ) {
				ac.achievementCnt++;
				ac.save ();

				if (ac.achievementCnt == 1) {
					showAchievementAnimation (ac);
				}

				activatGameObjects ();
				countAndShowAchieventProgesss ();
			}
		}
	}

	void showAchievementAnimation(achievementConfig ac){
		if (achievementAnimator != null) {
			if (anim_descriptionText != null) {
				anim_descriptionText.text = TranslationManager.translateIfAvail(ac.description);
			}
			if (anim_titleText != null) {
				anim_titleText.text = TranslationManager.translateIfAvail(ac.title);
			}
			if (anim_achievementImage != null) {
				anim_achievementImage.sprite = ac.sprite;
			}
			achievementAnimator.SetTrigger (triggerOnAchievement);
		}
	}

	/*
	 * Return all possible translatable terms
	 */
	public override List<string> getTranslatableTerms ()
	{
		List<string> terms = new List<string> ();
		terms.Clear ();
		EventScript es;

		foreach (achievementConfig ac in achievements) {
			terms.Add (ac.description);
			terms.Add (ac.title);
		}

		return terms;
	}
}
