using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


/// <summary>
/// 게임에서 사용되는 수치들을 계산하고 여러가지 처리등을 하기 위한 클래스.
/// 이 스크립트는 게임씬내에 빈 오브젝트들을 만들어서 연결해놓는다.  
/// Game -> Values -> adreadyValue
/// Game -> Values -> Army
/// Game -> Values -> AuthorityValue
/// Game -> Values -> CharimaValue
/// Game -> Values -> Country
/// Game -> Values -> CreativityValue
/// Game -> Values -> GamesPlayedValue
/// Game -> Values -> Gender
/// Game -> Values -> HealthValue 
/// Game -> Values -> IntelligenceValue
/// Game -> Values -> LookValue
/// Game -> Values -> LuckValue
/// Game -> Values -> MarriageValue
/// Game -> Values -> MarriedValue
/// Game -> Values -> Money
/// Game -> Values -> Name
/// Game -> Values -> People
/// Game -> Values -> Religion
/// Game -> Values -> Surname
/// Game -> Values -> Years
/// </summary>
public class ValueScript : MonoBehaviour {

    /// <summary>
    /// 현재 이 스크립트가 붙어 있는 게임오브젝트가 게임에서 어떤 것을 담당하는지 유형인지 인스펙터 드롭다운 목록에서 선택하기 위해 열거형 변수.
    /// 가령 내가 군대수치와 관련된 작동들을 할려고 하면, 게임씬에 빈 오브젝트로 만들어서 이름을 Army로 짓고, 이 클래스를 연결한다.
    /// 즉 Game -> Values -> Army이면 인스펙터 드롭다운 메뉴에서 '군대'를 선택해주어서 내가 붙어 있는 오브젝트의 성격을 규정해준다.
    /// </summary>
	[Tooltip("옆에 목록에서 내가 어떤 성격의 오브젝트인지 선택하세요.")]
	public ValueDefinitions.값정의 내역활;

    /// <summary>
    /// 플레이어프랩스에 키값으로 사용하기 위한 스트링타입 변수.
    /// 사용자가 인스펙터에서 지정한 내가 붙어 있는 객체가 무엇인지(이름, 성별, 군인수 등등)지정한 값을 문자열로 저장하고 있는 변수.
    /// 가령 군인수치를 담당하는 객체이면 프로그래머가 alueType 변수에 '군인'열거요소로 값을 할당했을 것이다. 
    /// 이 값을 본 변수에 문자(텍스트)로 저장하고 있으며, 이 값을 플레이어프랩스에 키값으로 사용하기 위해 따로 변수를 만들어둔것이다.
    /// </summary>
	private string 플레이어프랩스키값 = "빈값";

    /// <summary>
    /// 플레이어프랩스에 내 성격의 데이터가 이미 있을 경우, 해당 데이터를 가져와서 저장하는 변수
    /// </summary>
	[ReadOnlyInspector] /// 변수를 인스펙터에서 읽기 전용으로 만들기위한 프로그래머가 만든 애트리뷰트.
    public float 플레이어프랩스데이터 = 0f;

    /// <summary>
    /// 디버그 값 변경 ??????
    /// </summary>
	public bool debugValueChanges = false;

	void Awake()
    {
        /// 프로그래머가 인스펙터에서 내가 붙어 있는 객체가 무엇인지 지정한 값을 문자로 저장한다. (이름, 성별, 나라, 돈 등등)
        /// 가령 인스펙터에서 내가 붙어 있는 객체의 역할이 군인수치를 관리하는 객체라면 'Army'로 지정했을 것이고, 이때 식별자로 사용하는 변수에 'Army'텍스트 문자를 따로 저장한다.
		플레이어프랩스키값 = 내역활.ToString ();

        /// 플레이어프랩스에 값을 불러오거나 저장한다.
		플레이어프랩스저장또는불러오기();
	}

	void Start()
    {
        /// 나 자신 클래스를 싱글톤 객체의 리스트에 등록.
        /// 결과적으로 Game씬의 Values오브젝트 밑의 자식 오브젝트들이 모두 리스트에 등록되게 된다.
		ValueManager.나자신.ValueScript리스트에추가 (this);

    }




	public void actualizeUI(float uiValue)
    {
		if (유저인터페이스.uiScrollbar != null)
        {
			유저인터페이스.uiScrollbar.size = uiValue / 범위.최대값;
		}
		if (유저인터페이스.textValue != null)
        {
			유저인터페이스.textValue.text = uiValue.ToString(유저인터페이스.formatter);
		}
		if (유저인터페이스.uiSlider != null)
        {
			유저인터페이스.uiSlider.value = uiValue / 범위.최대값;
		}
	}


    /// <summary>
    /// 게임내의 결과창이나 게임상단의 UI등의 슬라이더등의 수치나 값을 조절하기 위한 클래스.
    /// </summary>
	[System.Serializable]
	public class UI구성
    {
		[Tooltip("값을 채우기 막대로 표시하는 경우에는 스크롤 막대 또는 슬라이더를 사용할 수 있습니다. 여기에서 환경 설정을 정의하십시오.")]
		public Scrollbar uiScrollbar;

        [Tooltip("값을 채우기 막대로 표시하는 경우에는 스크롤 막대 또는 슬라이더를 사용할 수 있습니다. 여기에서 환경 설정을 정의하십시오.")]
		public Slider uiSlider;

        [Tooltip("값이 변경되면 바를 채우는 속도입니다.")]
		[Range(0.1f,100f)]public float lerpSpeed = 10f;

        [Tooltip("값을 표시하기 위한 형식을 정의하십시오. \n예: 0 또는 # 을 입력하십시오. \n예 : #.00을 입력하면 두자리 숫자가 표시됩니다.")]
		public string formatter = "0.##";

        [Tooltip("실제 lerped/filling 값.")]
		[ReadOnlyInspector]public float lerpedValue = 0f;

        [Tooltip("값이 텍스트로 표시되면 여기에서 텍스트 필드를 정의하십시오.")]
		public Text textValue;

		[Tooltip("값 관리자는 'showActualization'에 따라 값 변경을 사용자에게 표시할 수 있습니다.")]
		public bool showActualization = true;

        [Tooltip("값 관리자는 미니터처스프라이트로 값의 변경을 표시할 수 있습니다.")]
		public Sprite miniatureSprite;
	}

	[Tooltip("각 오브젝트마다 UI의 값의 세팅이 틀리기때문에 인스펙터에서 세부 정보를 세팅하세요.")]
	public UI구성 유저인터페이스;




	void Update(){
		
        //UserInterface.lerpedValue = Mathf.Lerp (UserInterface.lerpedValue, value, UserInterface.lerpSpeed * Time.deltaTime);

		유저인터페이스.lerpedValue = MathExtension.linearInterpolate (유저인터페이스.lerpedValue, 플레이어프랩스데이터, 유저인터페이스.lerpSpeed * Time.deltaTime);

		actualizeUI (유저인터페이스.lerpedValue);
	}


    /// <summary>
    /// 최소값과 최대값의 범위를 지정한 클래스. 
    /// 여기서는 일단 대충 기본값으로 초기화하고, 
    /// 실제로 부여하는 값은 인스펙터에서 노출된 변수에서 프로그래머가 해당 오브젝트의 성격에 따라 최소값과 최대값을 지정한다.
    /// </summary>
	[System.Serializable]
	public class 값범위
	{
        /// <summary>
        /// 프로그래머가 설정하는 밸류값이 벗어나면 안되는 범위의 최소값
        /// </summary>
		[Tooltip("범위로 설정할 최소값을 넣어주세요")]
		public float 최소값 = 0f;

        /// <summary>
        /// 프로그래머가 설정하는 밸류값이 벗어나면 안되는 범위의 최대값
        /// </summary>
		[Tooltip("범위로 설정할 최대값을 넣어주세요")]
		public float 최대값 = 100f;

        /// <summary>
        /// 랜덤으로 값을 뽑기할때 랜덤범위의 최소값.
        /// </summary>
		[Tooltip("랜덤으로 값을 할당하는 경우 허용 가능한 최소값을 입력하세요.")]
		public float 랜덤최소값 = 0f;

        /// <summary>
        /// 랜점으로 값을 뽑기할때 랜덤범위의 최대값.
        /// </summary>
		[Tooltip("랜덤으로 값을 할당하는 경우 허용 가능한 최대값을 입력하세요.")]
		public float 랜덤최대값 = 100f;
	}

    /// <summary>
    /// 최소값과 최대값의 범위. 내가 붙어 있는 오브젝트의 성격에 따라 인스펙터에서 각각의 값의 범위를 지정한다.
    /// </summary>
	public 값범위 범위;

    /// <summary>
    /// 내 정체 밸류값이 내가 설정한 범위를 넘지 못하도록 하고, 설정한 범위를 넘을 경우 최소값 또는 최대값을 할당한다. 그리고 최소값 & 최대값일때 실행할 유니티이벤트를 실행한다.
    /// </summary>
	public void 설정값으로실행()
    {
        /// 밸류값이 내가 설정한 범위보다 작은 경우
		if (플레이어프랩스데이터 < 범위.최소값)
        {
            /// 밸류값에 내가 설정한 범위 최소값을 할당한다.
			플레이어프랩스데이터 = 범위.최소값;
            /// 최소값일때 작동할 유니티이벤트를 실행한다.
			실행할이벤트.최소값일때.Invoke();
		}
        /// 만약 밸류값이 내가 설정한 범위보다 큰 경우
		if (플레이어프랩스데이터 > 범위.최대값)
        {
            /// 밸류값에 내가 설정한 범위 최대값을 할당한다.
			플레이어프랩스데이터 = 범위.최대값;
            /// 최대값일때 작동할 유니티이벤트를 실행한다.
			실행할이벤트.최대값일때.Invoke();
		}
	}

	/// <summary>
    /// 값을 늘리거나 줄인다.
    /// </summary>
    /// <param name="increment"></param>
    /// <returns></returns>
	public float addValue (float increment){

		if (debugValueChanges == true) {
			Debug.Log ("Value '" + 내역활.ToString () + "': " + 플레이어프랩스데이터.ToString () + " increment by " + increment.ToString ());
		}

		플레이어프랩스데이터 += increment;
		설정값으로실행 ();
		if (increment >= 0f) {
			실행할이벤트.증가할때.Invoke ();
		} else {
			실행할이벤트.감소할때.Invoke ();
		}
		saveValue ();

		if (debugValueChanges == true) {
			Debug.Log ("Value '" + 내역활.ToString () + "' is now " + 플레이어프랩스데이터.ToString () + " (after limiter)");
		}

		return 플레이어프랩스데이터;
	}

	/*
	 * Set a value to an defined value.
	 */
	public float setValue(float newValue){
		플레이어프랩스데이터 = newValue;
		설정값으로실행 ();
		saveValue ();
		return 플레이어프랩스데이터;
	}

	[Tooltip("'keepValue' blocks the randomization of the value on a new game start. On the first startup of the game, the value is randomized between 'Limits.RandomMin' and 'Limits.RandomMax' (Accessable from Inspector).")]
	public bool keepValue = false;


    /// <summary>
    /// 내 정체를 랜덤으로 값을 지정하는 메서드. 값을 지정할때 최소&최대값을 넘지 못하도록 하고 있으며, 최소&최대값일때 실행할 유니티이벤트를 실행한다. 그리고 플레이어프랩스에 데이터를 저장한다.
    /// </summary>
    /// <returns></returns>
    public float 랜덤값세팅()
    {
        /// 랜덤으로 지정한 값을 배정하고
        플레이어프랩스데이터 = Random.Range (범위.랜덤최소값, 범위.랜덤최대값);

        /// 내 정체 밸류값이 내가 설정한 범위를 넘지 못하도록 하고, 설정한 범위를 넘을 경우 최소값 또는 최대값을 할당한다.
        /// 그리고 최소값 & 최대값일때 실행할 유니티이벤트를 실행한다
        설정값으로실행();

        /// 플레이어프랩스에 데이터 저장
		saveValue();

        /// 랜덤으로 지정한 값을 반환
        return 플레이어프랩스데이터;
	}

	public void newGameStart()
    {
		if (keepValue == false) {
			랜덤값세팅 ();
		}
	}


	public float multiplyValue (float multiplier){
		플레이어프랩스데이터 *= multiplier;
		설정값으로실행 ();
		if (multiplier >= 1f) {
			실행할이벤트.증가할때.Invoke();
		}else {
			실행할이벤트.감소할때.Invoke ();
		}
		saveValue ();
		return 플레이어프랩스데이터;
	}

    /// <summary>
    /// 현재 내가 붙어 있는 오브젝트 성격을 지정한 값이 기존 플레이어프랩스에 저장되어 있는 값인지 검사해서, 없으면 저장하고 있으면 불러온다.
    /// </summary>
	void 플레이어프랩스저장또는불러오기()
    {
        /// 현재 내가 붙어 있는 오브젝트이 성격을 지정한 값이 기존 플레이어프랩스에 이미 저장되어 있는 값인지 검사해서, 이미 저장되 있는 키값일 경우 
		if (SecurePlayerPrefs.키값존재여부 (플레이어프랩스키값))
        {
            /// 플레이어프랩스에 저장되어 있는 키값을 가져와서 저장한다
			플레이어프랩스데이터 = SecurePlayerPrefs.얻기float (플레이어프랩스키값);
		}
        else /// 저장되 있는 키값이 아닌 경우에는
        {
            /// 내 정체를 랜덤으로 값을 지정하는 메서드. 값을 지정할때 최소&최대값을 넘지 못하도록 하고 있으며, 최소&최대값일때 실행할 유니티이벤트를 실행한다. 그리고 플레이어프랩스에 데이터를 저장한다.
			랜덤값세팅();
		}
	}

    /// <summary>
    /// 플레이어프랩스에 데이터를 저장.
    /// </summary>
	void saveValue()
    {
        /// 플레이어프랩스에 데이터를 저장한다.
		SecurePlayerPrefs.저장float (플레이어프랩스키값, 플레이어프랩스데이터);
	}

	public void saveMinMax(){
		float min = SecurePlayerPrefs.얻기float (플레이어프랩스키값+"_min");
		float max = SecurePlayerPrefs.얻기float (플레이어프랩스키값+"_max");

		if(SecurePlayerPrefs.키값존재여부(플레이어프랩스키값+"_min")){
			if (플레이어프랩스데이터 < min) {
				SecurePlayerPrefs.저장float (플레이어프랩스키값+"_min",플레이어프랩스데이터);
			}	
		}else{
			SecurePlayerPrefs.저장float (플레이어프랩스키값+"_min",플레이어프랩스데이터);
		}

		if (플레이어프랩스데이터 > max) {
			SecurePlayerPrefs.저장float (플레이어프랩스키값 + "_max", 플레이어프랩스데이터);
		}

	}

	public float getMaxValue(){
		return SecurePlayerPrefs.얻기float (플레이어프랩스키값+"_max");
	}

	public float getMinValue(){
		return SecurePlayerPrefs.얻기float (플레이어프랩스키값+"_min");
	}

	void OnDestroy(){
		saveValue ();
	}

    /// <summary>
    /// 유니티 이벤트를 상속받은 클래스.
    /// </summary>
	[System.Serializable] /// 클래스를 인스펙터에 노출시키기 위해 직렬화해준다
    public class 유니티이벤트 : UnityEvent {}

    /// <summary>
    /// 상황에 따른 유니티 이벤트를 인스펙터에서 연결시키기 위한 클래스.
    /// </summary>
	[System.Serializable] /// 클래스를 인스펙터에 노출시키기 위해 직렬화해준다.
	public class 실행이벤트
    {
        /// <summary>
        /// 증가할때 실행할 유니티이벤트를 연결하는 변수
        /// </summary>
		public 유니티이벤트 증가할때;

        /// <summary>
        /// 감소할때 실행할 유니티이벤트를 연결하는 변수
        /// </summary>
        public 유니티이벤트 감소할때;

        /// <summary>
        ///  최대일때 실행할 유니티이벤트를 연결하는 변수
        /// </summary>
		public 유니티이벤트 최대값일때;

        /// <summary>
        /// 최소일때 실행할 유니티이벤트를 연결하는 변수
        /// </summary>
		public 유니티이벤트 최소값일때;
	}

    /// <summary>
    /// 인스펙터에서 유니티이벤트를 사용하기 위해 노출하는 변수
    /// </summary>
	[Tooltip("이벤트는 값 변경시 또는 값이 한계 중 하나에 있을때 트리거됩니다.????")]
	public 실행이벤트 실행할이벤트;


}
