using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 'GenderGenerator'스크립트는 값 스크립트의 값에 따라 그림과 텍스트를 표시합니다. 
/// 또한 값에 의존하는 그림을 보여주는 데 사용할 수 있으며 성별에 국한되지 않습니다. 
/// 값은 인덱스에 캐스트되고 정의 된 목록에서 표시 할 특성을 선택합니다.
/// Game -> Gender
/// </summary>
public class GenderGenerator : TranslatableContent {

	public static GenderGenerator instance;

	[Tooltip("성별 값을 보유하는 값 유형을 정의하십시오. 이 값 유형은 또한 'Country Name Generator'에 연결되어야 합니다..")]
	public ValueDefinitions.값정의 valueType;
	ValueScript vs;

	void Awake(){
		instance = this;
	}

	void Start(){
		clearUI ();
		StartCoroutine (frameDelay());
		TranslationManager.instance.registerTranslateableContentScript (this);
	}

	/// <summary>
    /// Start()에서 로드되는 'value' 스크립트에 대한 의존성 때문에 UI를 구현하기 위해서는 하나의 프레임 지연이 필요합니다.
    /// </summary>
    /// <returns></returns>
	IEnumerator frameDelay(){
		yield return null; /// 다음 프레임까지 대기
		vs = ValueManager.나자신.첫번째피팅값가져오기 (valueType);
		actualizeUI ();
	}


	[Tooltip("사용 가능한 성별에 대한 이름과 그림을 정의하십시오.")]
	public gendStringList[] genders;

	[Tooltip("실제 성별을 나타내는 텍스트 입력란을 정의하십시오.")]
	public Text outText;
	[Tooltip("실제 성별 그림을 보여주는 이미지를 정의하십시오.")]
	public Image outImg;


	[System.Serializable]
	public class gendStringList{
		public string gender;
		public Sprite picto;
	}


	Color originalSpriteColor;
	bool colorCopied = false;
	void clearUI(){
		if (outText != null) {
			outText.text = "";
		}
		if (outImg != null) {
			if (colorCopied == false) {
				originalSpriteColor = outImg.color;
				colorCopied = true;
			}
			outImg.color = Color.clear;
		}
	}

	public void actualizeUI(){
		if (vs != null) {
			if (outText != null) {
				outText.text = getGenderText ();
			}
			if (outImg != null) {
				if (colorCopied == true) {
					outImg.color = originalSpriteColor;
				}
				outImg.sprite = getGenderSprite ();
			}
		}
	}


	public string getGenderText()
	{
		if (vs != null) {
			int index = Mathf.RoundToInt (vs.플레이어프랩스데이터);
			if (index >= genders.Length) {
				index = genders.Length - 1;
			}
			return TranslationManager.translateIfAvail (genders [index].gender);
		}
		return null;
	}

	public Sprite getGenderSprite()
	{
		if (vs != null) {
			int index = Mathf.RoundToInt (vs.플레이어프랩스데이터);
			if (index >= genders.Length) {
				index = genders.Length - 1;
			}
			return genders [index].picto;
		}
		return null;
	}

	/*
	 * Return all possible translatable terms
	 */
	public override List<string> getTranslatableTerms ()
	{
		List<string> terms = new List<string> ();
		terms.Clear ();
		EventScript es;

		foreach (gendStringList gsl in genders) {
			terms.Add (gsl.gender);
		}

		return terms;
	}


}
