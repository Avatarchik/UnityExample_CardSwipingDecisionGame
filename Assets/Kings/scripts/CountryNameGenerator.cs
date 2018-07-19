
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 국가 이름 생성기는 무작위 국가에 따라 임의의 주어진 이름과 성을 조합하여 생성합니다. 이 스크립트는 주로 게임용으로 작성되었습니다.
/// 플레이어에게 정체성을 부여하기 위해 'Swipe my life'. 주어진 이름은 성별(두개의 성별)과 결합되어 있습니다.성별 그림을 보려면 'Gender Generator'스크립트를 추가로 사용하십시오.
/// 값의 무작위 화는 값 스크립트에 따라 조합을 저장합니다.
/// GAme -> Values 
/// </summary>
public class CountryNameGenerator : TranslatableContent {

	public static CountryNameGenerator instance;

	/// <summary>
    /// 성별을 정의한 열거형. 남자, 여자 성별이 있음.
    /// </summary>
	[System.Serializable] /// 인스펙터에 노출하기 위한 직렬화
	public enum 성별
    {
		남성,
		여성
	}

    /// <summary>
    /// 모르겠다.
    /// </summary>
	[Tooltip("플레이의 실제 성별입니다.")]
	[ReadOnlyInspector] /// 인스펙터에서 읽기 전용으로 프로그래머가 만든 애트리뷰트
    public 성별 성구분;

    /// <summary>
    /// 게임이 시작되면 플레이어에게 랜덤으로 주어지는 국가와 왕의 이름, 칭호,성별을 인스펙터에서 미리 등록할수 있는 변수들을 규정한 클래스. 
    /// </summary>
    [System.Serializable] /// 인스펙터에 노출하기 위한 직렬화 애트리뷰트
    public class SubStringList
    {
        /// <summary>
        /// 게임이 시작하면 플레이어에게 랜덤으로 정해지는 국가명.
        /// 인스펙터에서 미리 국가 이름을 넣어둔다.
        /// </summary>
        [Tooltip("게임시작시 랜덤으로 정해지는 국가명을 미리 등록합니다.")]
        public string 국가명;

        /// <summary>
        /// 게임이 시작하면 플레이어에게 랜덤으로 정해지는 왕의 이름과 성별.
        /// 인스펙터에서 미리 해당국가에 맞는 왕의 이름과 성별을 넣어둔다.
        /// </summary>
        [Tooltip("게임시작시 랜덤으로 정해지는 이름과 성별의 가능한 조합 목록을 미리 등록합니다.")]
        public nameGenderLink[] 국왕이름과성별;

        /// <summary>
        /// 게임이 시작하면 플레이어에게 랜덤으로 정해지는 왕의 칭호(위대한 무슨무슨 왕 이런식으로).
        /// 인스펙터에서 미리 해당국가에 맞는 왕의 칭호를 넣어둔다.
        /// </summary>
        [Tooltip("게임시작시 랜덤으로 정해지는 왕의 칭호의 가능한 조합 목록을 미리 등록합니다.")]
        public string[] 칭호;
    }

    /// <summary>
    /// 게임이 시작되면 플레이어에게 랜덤으로 주어지는 국가를 인스펙터에서 세팅하기 위한 변수.
    /// 인스펙터에서 이 변수를 통해 4가지 정보를 등록할 수 있다. 국가명칭, 왕의 이름, 칭호, 성별.
    /// 일단 3개 국가정도만 세팅하자.
    /// </summary>
    [Tooltip("플레이어에게 랜덤으로 주어지는 국가 및 국가별 왕의 이름과칭호, 성별을 작성하세요.")]
    public SubStringList[] 국가들;

    /// <summary>
    /// 게임 상단에 국가를 표시하는 UGUI 텍스트 영역.
    /// Game -> StatsCanvas -> PlayerPanel -> CountryText
    /// </summary>
	[Tooltip("게임씬의 UGUI에서 국가를 표시하는 텍스트 필드할당하세요.")]
    public Text 국가명표시텍스트;

    /// <summary>
    /// 게임 상단에 왕의 이름과 별칭을 표시하는 UGUI 텍스트 영역.
    /// Game -> StatsCanvas -> PlayerPanel -> PlayerNameText
    /// </summary>
	[Tooltip("게임씬의 UGUI에서 왕의 이름과 별칭을 표시하는 텍스트 필드를 할당하세요.")]
    public Text 국왕이름별칭표시텍스트;

    /// <summary>
    /// 인스펙터에서 드롭다운 목록에서 열거형으로 정의한 목록 중 '국가'를 선택.
    /// </summary>
	[Tooltip("국가를 정의하는 값 유형입니다.")]
    public ValueDefinitions.값정의 vs_type_country;

    ValueScript vs_country;

    /// <summary>
    /// 인스펙터에서 드롭다운 목록에서 열거형으로 정의한 목록 중 '이름'을 선택.
    /// </summary>
	[Tooltip("지정된 이름을 정의하는 값 유형입니다.")]
    public ValueDefinitions.값정의 vs_type_givenName;

    /// <summary>
    /// 모르겠다.
    /// </summary>
    ValueScript vs_name;

    /// <summary>
    /// 인스펙터에서 드롭다운 목록에서 열거형으로 정의한 목록 중 '칭호'를 선택.
    /// </summary>
    [Tooltip("성씨를 정의하는 값 유형입니다.")]
    public ValueDefinitions.값정의 vs_type_surname;

    ValueScript vs_surname;

    /// <summary>
    /// 인스펙터에서 드롭다운 목록에서 열거형으로 정의한 목록 중 Gender를 선택.
    /// </summary>
    [Tooltip("성별을 포함하는 값 유형입니다. 이 스크립트의 값은 'Country Name Generator'에 의해 정의되며, 그렇지 않은 경우는 정의되지 않습니다.")]
    public ValueDefinitions.값정의 vs_type_gender;

    ValueScript vs_gender;

    /// <summary>
    /// 왕의 이름과 성별을 인스펙터에서 입력할 수 있게 하기 위한 클래스.
    /// </summary>
	[System.Serializable] /// 인스펙터에서 클래스를 노출하기 위한 애트리뷰트
	public class nameGenderLink
    {
        /// <summary>
        /// 왕의 이름. 인스펙터에서 여기에 쓴 내용은 배열 요소명이 된다. 
        /// </summary>
		public string name;

        /// <summary>
        /// 인스펙터에서 왕의 성별을 선택할 수 있게 하기 위한 열거형. 
        /// </summary>
		public 성별 gender;
    }

    /// <summary>
    /// 이름과 국가로 텍스트 필드를 구현하는 메서드
    /// </summary>
    /// <param name="force"></param>
    public void actualizeTexts(bool force = false)
    {

        if (GameStateManager.instance.gamestate == GameStateManager.Gamestate.gameActive || force == true)
        {
            if (국가명표시텍스트 != null)
            {
                국가명표시텍스트.text = GetCountryTranslatedStringFromValue();
            }
            if (국왕이름별칭표시텍스트 != null)
            {
                nameGenderLink ngl = getNameAndGenderFromValue();
                if (ngl != null)
                {
                    국왕이름별칭표시텍스트.text = ngl.name + " " + getSurnameFromValue();
                }
                else
                {
                    국왕이름별칭표시텍스트.text = "";
                }
            }

            /// 아파치 헬리콥터를 원하면 이것을 확장하십시오. 아파치 그림을 보려면 'Gender Generator'스크립트를 수정하십시오.
            성구분 = getNameAndGenderFromValue().gender;
            if (성구분 != null)
            {
                if (성구분 == 성별.남성)
                {
                    vs_gender.새로운값저장(0f);
                }
                else if (성구분 == 성별.여성)
                {
                    vs_gender.새로운값저장(1f);
                }
            }
        }
    }

    /// <summary>
    /// 왕의 이름과 성별을 반환하는 메서드. value 스크립트에서 주어진 이름과 성별을 로드하십시오.
    /// </summary>
    /// <returns></returns>
    public nameGenderLink getNameAndGenderFromValue()
    {
        if (vs_name != null)
        {
            int index = Mathf.RoundToInt(vs_name.플레이어프랩스데이터);
            if (index >= 국가들[getCountryIndexFromValue()].국왕이름과성별.Length)
            {
                index = 국가들[getCountryIndexFromValue()].국왕이름과성별.Length - 1;
            }
            nameGenderLink nameGender = 국가들[getCountryIndexFromValue()].국왕이름과성별[index];

            return nameGender;
        }
        return null;
    }
    /*
	 * Load the surname from its value script 
	 */
    public string getSurnameFromValue()
    {
        if (vs_surname != null)
        {
            int index = Mathf.RoundToInt(vs_surname.플레이어프랩스데이터);
            if (index >= 국가들[getCountryIndexFromValue()].칭호.Length)
            {
                index = 국가들[getCountryIndexFromValue()].칭호.Length - 1;
            }
            return 국가들[getCountryIndexFromValue()].칭호[index];
        }
        return "";
    }
    /*
	 * Load the country index from its value script 
	 */
    public int getCountryIndexFromValue()
    {
        if (vs_country != null)
        {
            int index = Mathf.RoundToInt(vs_country.플레이어프랩스데이터);
            if (index >= 국가들.Length)
            {
                index = 국가들.Length - 1;
            }
            return index;
        }
        return -1;
    }

    /*
	 * Get the given name, surname and country as one combined string.
	 */
    public string getCountryNameText()
    {
        string retText = "";
        nameGenderLink ngl = getNameAndGenderFromValue();
        if (ngl != null)
        {
            retText += ngl.name + " " + getSurnameFromValue() + ", " + GetCountryTranslatedStringFromValue();
        }
        else
        {
            retText = "";
        }
        return retText;
    }

    /// <summary>
    /// 가능한 경우 번역 된 국가 이름을 가져옵니다.
    /// </summary>
    /// <returns></returns>
    public string GetCountryTranslatedStringFromValue()
    {
        return TranslationManager.translateIfAvail(국가들[getCountryIndexFromValue()].국가명);
    }

    /*
	 * Return all possible translatable terms
	 */
    public override List<string> getTranslatableTerms()
    {
        List<string> terms = new List<string>();
        terms.Clear();
        EventScript es;

        foreach (SubStringList ssl in 국가들)
        {
            terms.Add(ssl.국가명);
            //don't translate given name and surname?
        }

        return terms;
    }


    void Awake()
    {
        instance = this;
    }

    void Start(){
		clearUI ();
		StartCoroutine (oneFrame ());
		TranslationManager.instance.registerTranslateableContentScript (this);
	}

	void clearUI(){
		if (국가명표시텍스트 != null) {
			국가명표시텍스트.text = "";
		}
		if (국왕이름별칭표시텍스트 != null) {
			국왕이름별칭표시텍스트.text = "";
		}
	}

    /// <summary>
    /// 'Value' 스크립트는 Start()에서 등록되고, 로드되기 때문에 스크립트 관리자 값에서 등록된 스크립트를 가져오려면 하나의 프레임 지연이 필요하다.
    /// </summary>
    /// <returns></returns>
    IEnumerator oneFrame()
    {
        yield return null; /// 다음프레임까지 대기.
        createValueScriptLinks();
        actualizeTexts();
    }

    /// <summary>
    /// 값 스크립트 링크 만들기.
    /// </summary>
	void createValueScriptLinks(){
		vs_gender = ValueManager.나자신.첫번째피팅값가져오기 (vs_type_gender);
		vs_name = ValueManager.나자신.첫번째피팅값가져오기 (vs_type_givenName);
		vs_surname = ValueManager.나자신.첫번째피팅값가져오기 (vs_type_surname);
		vs_country = ValueManager.나자신.첫번째피팅값가져오기 (vs_type_country);
	}

	
}



	

