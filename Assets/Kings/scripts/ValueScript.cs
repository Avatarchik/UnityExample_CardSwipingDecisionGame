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
        /// 나 자신 클래스를 static ValueManager 싱글톤 객체의 리스트에 등록.
        /// 결과적으로 Game씬의 Values오브젝트 밑의 자식 오브젝트들이 모두 리스트에 등록되게 된다.
		ValueManager.나자신.ValueScript리스트에추가 (this);

    }

    

    /// <summary>
    /// 유니티니의 UGUI에 값을 넣어주는 메서드.  
    /// </summary>
    /// <param name="ui값"></param>
	public void UI실현(float ui값)
    {
		if (UI조절.스크롤바 != null)
        {
			UI조절.스크롤바.size = ui값 / 범위.최대값;
		}
		if (UI조절.스탯수치텍스트 != null)
        {
			UI조절.스탯수치텍스트.text = ui값.ToString(UI조절.표시포맷);
		}
		if (UI조절.슬라이더 != null)
        {
			UI조절.슬라이더.value = ui값 / 범위.최대값;
		}
	}


    /// <summary>
    /// 게임내의 상단 메뉴나, '당신의스탯' UGUI창의 슬라이더의 수치등을 조절하기 위해 필요한 변수를 정의한 클래스.
    /// </summary>
	[System.Serializable]
	public class UI구성
    {
		[Tooltip("유니티의 UGUI의 스크롤바를 연결하는 변수. 스크롤바는 스크롤링을 통해 막대를 움직일수 있다.")]
		public Scrollbar 스크롤바;

        [Tooltip("유니티의 UGUI의 슬라이더를 연결하는 변수. 슬라이더바는 정해진 범위상의 숫자값을 통해 막대를 움직일 수 있다.")]
		public Slider 슬라이더;

        /// <summary>
        /// 스크롤바 게이지가 변경될때의 속도이다.
        /// </summary>
        [Tooltip("값이 변경되면 바를 채우는 속도입니다.")]
		[Range(0.1f,100f)] // 인스펙터에서 슬라이더형태로 조절하기 위한 어트리뷰트.
        public float 게이지속도 = 10f;

        /// <summary>
        /// UGUI에 숫자를 소수점까지 표시할지, 소수점없이 표시할지등의 포맷을 인스펙터에서 지정하는 변수.
        /// 예를 들어 통치기간(Years)을 표시할때는 '0'으로 인스펙터에서 표시해야 한다.
        /// </summary>
        [Tooltip("값을 표시하기 위한 형식을 정의하십시오. \n예: 0 또는 # 을 입력하십시오. \n예 : #.00을 입력하면 두자리 숫자가 표시됩니다.")]
		public string 표시포맷 = "0.##";

        /// <summary>
        /// 100%를 기준으로 현재 보유한 값이 몇퍼센트인지 인스펙터에서 읽기전용으로 보여주는 변수.
        /// </summary>
        [Tooltip("실제 lerped/filling 값.")]
		[ReadOnlyInspector] // 실수로 수정하면 안되니깐 읽기전용 어트리뷰트 사용
        public float 현재퍼센트 = 0f;

        /// <summary>
        /// '당신의스탯'에 있는 권위, 지성, 카리스마, 행운등의 수치를 UGUI에서 보여주기 위한 Text타입 변수
        /// </summary>
        [Tooltip("당신의 스탯에서 텍스트로 표시되는 스탯부분의 텍스트UGUI를 연결하세요.")]
		public Text 스탯수치텍스트;

        /// <summary>
        /// ???? 이용자게게 값변경을 보여질때 방식을 정할때 사용하는 옵션인것 같은데 잘 모르겠다.
        /// </summary>
		[Tooltip("값 관리자는 'showActualization'에 따라 값 변경을 사용자에게 표시할 수 있습니다.")]
		public bool showActualization = true;

        /// <summary>
        /// '당신의 스탯' UGUI창에서 아이콘으로 사용할 스프라이트 이미지를 연결하기 위한 변수.
        /// 권위, 지성...등등의 각 스탯에 맞게 제작된 아이콘 이미지를 연결한다.
        /// </summary>
        [Tooltip("'당신의 스탯'에서 표시할 아이콘을 연결하세요.")]
		public Sprite 아이콘이미지;
	}

    /// <summary>
    /// 게임창의 군대 수치나 통치기간등과 '당신의 스탯'에 들어 있는 권위, 지성, 카리스마 같은 스크롤바 UGUI를 인스펙터에서 연결하기 위한 변수.
    /// 내가 붙어 있는 객체의 종류가 권위, 지성, 카리스마, 행운, 창의력, Look, 건강, 결혼 일경우 인스펙터에서 게임신의 '당신의 스탯' UGUI에 만들어놓은 각각의 슬라이더를 여기에 연결한다.
    /// </summary>
	[Tooltip("게임에서 사용할 UI의 포맷은 각 오브젝트마다 UI의 값의 세팅이 틀리기때문에 인스펙터에서 세부 정보를 세팅하세요.")]
	public UI구성 UI조절;




	void Update(){
		
        //UserInterface.lerpedValue = Mathf.Lerp (UserInterface.lerpedValue, value, UserInterface.lerpSpeed * Time.deltaTime);

        /// UGUI의 슬라이더방의 게이지의 현재 퍼센트를 보정해서 매프레임 값을 넣어주고.
		UI조절.현재퍼센트 = MathExtension.늘어나는것보간하기 (UI조절.현재퍼센트, 플레이어프랩스데이터, UI조절.게이지속도 * Time.deltaTime);

        /// 보정된 현재퍼센트를 UGUI의 슬라이더 게이지에 반영한다.
		UI실현 (UI조절.현재퍼센트);
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
    /// 내 정체 밸류값이 내가 설정한 범위를 넘지 못하도록 하고, 설정한 범위를 넘을 경우 최소값 또는 최대값을 할당한다. 그리고 최소값 그리고 최대값일때 실행할 유니티이벤트를 실행한다.
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
	public float addValue (float increment)
    {

		if (debugValueChanges == true)
        {
			Debug.Log ("Value '" + 내역활.ToString () + "': " + 플레이어프랩스데이터.ToString () + " increment by " + increment.ToString ());
		}

		플레이어프랩스데이터 += increment;

		설정값으로실행 ();

        if (increment >= 0f) {
			실행할이벤트.증가할때.Invoke ();
		} else {
			실행할이벤트.감소할때.Invoke ();
		}
		값저장 ();

		if (debugValueChanges == true) {
			Debug.Log ("Value '" + 내역활.ToString () + "' is now " + 플레이어프랩스데이터.ToString () + " (after limiter)");
		}

		return 플레이어프랩스데이터;
	}

    /// <summary>
    /// 플레이어프랩스데이터에서 가져온 데이터에 파라미터로 입력한 값을 넣는다. (단 설정값을 넘지않게 해서 값을 저장한다.)
    /// </summary>
    /// <param name="새로운값">플레이어프랩스 데이터에 저장할 새로운 값</param>
    /// <returns></returns>
    public float 새로운값저장(float 새로운값)
    {
        플레이어프랩스데이터 = 새로운값;

        설정값으로실행();

        값저장();

        return 플레이어프랩스데이터;
    }


    /// <summary>
    /// 새로운 게임 시작시 랜덤으로 값을 지정하기 위한 옵션. false이면 랜덤으로 값을 세팅한다. 
    /// </summary>
	[Tooltip("'keepValue'는 새로운 게임 시작시 값의 임의화를 차단합니다. 게임을 처음 시작할때 값은 'Limits.RandomMin' 와 'Limits.RandomMax' (인스펙터에서 액세스 가능) 사이에서 무작위로 지정됩니다..")]
	public bool 값옵션 = false;


    /// <summary>
    /// 내 정체를 랜덤으로 값을 지정하는 메서드. 값을 지정할때 최소과 최대값을 넘지 못하도록 하고 있으며, 최소과 최대값일때 실행할 유니티이벤트를 실행한다. 그리고 플레이어프랩스에 데이터를 저장한다.
    /// </summary>
    /// <returns>랜덤으로 지정한 값을 반환</returns>
    public float 랜덤값세팅()
    {
        /// 랜덤으로 지정한 값을 배정하고
        플레이어프랩스데이터 = Random.Range (범위.랜덤최소값, 범위.랜덤최대값);

        /// 내 정체 밸류값이 내가 설정한 범위를 넘지 못하도록 하고, 설정한 범위를 넘을 경우 최소값 또는 최대값을 할당한다.
        /// 그리고 최소값 & 최대값일때 실행할 유니티이벤트를 실행한다
        설정값으로실행();

        /// 플레이어프랩스에 데이터 저장
		값저장();

        /// 랜덤으로 지정한 값을 반환
        return 플레이어프랩스데이터;
	}


    /// <summary>
    /// 새로운게임실행시 나한테 부여되는 값을 랜덤으로 부여하는 메서드.
    /// </summary>
	public void 새로운게임실행()
    {
        if (값옵션 == false)
        {
            랜덤값세팅();
        }
	}

    /// <summary>
    /// ??? 멀티플레이일때 나한테 부여되는 값을 저장하는 것과 관련된 메서드인것 같은데, 해당 코드가 사용되는 곳이 없음.
    /// </summary>
    /// <param name="multiplier"></param>
    /// <returns></returns>
    public float MultiplyValue(float multiplier)
    {
        플레이어프랩스데이터 *= multiplier;

        설정값으로실행();
        if (multiplier >= 1f)
        {
            실행할이벤트.증가할때.Invoke();
        }
        else
        {
            실행할이벤트.감소할때.Invoke();
        }
        값저장();
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
	void 값저장()
    {
        /// 플레이어프랩스에 데이터를 저장한다.
		SecurePlayerPrefs.저장float (플레이어프랩스키값, 플레이어프랩스데이터);
	}

    /// <summary>
    /// 최대값과 최소값 저장하기.
    /// </summary>
	public void 최소최대값저장()
    {
        /// 플레이어프랩스에 저장된 최소값과 최대값을 가져와서 할당한다.
		float 플레이어프랩스최소값 = SecurePlayerPrefs.얻기float (플레이어프랩스키값+"_최소");
		float 플레이어프랩스최대값 = SecurePlayerPrefs.얻기float (플레이어프랩스키값+"_최대");

        /// 만약 플레이어프랩스키값중 "뭐뭐뭐_최소"라고 저장된 키값이 존재하면
        if (SecurePlayerPrefs.키값존재여부(플레이어프랩스키값 + "_최소"))
        {
            /// 만약 플레이어프랩스데이터값이 플레이어프랩스최소값보다 작을 경우
            if (플레이어프랩스데이터 < 플레이어프랩스최소값)
            {
                /// 해당 플레이어프랩스데이터를 "뭐뭐뭐_최대"라는 키값으로 플레이어프랩스에 저장한다.
                SecurePlayerPrefs.저장float(플레이어프랩스키값 + "_최대", 플레이어프랩스데이터);
            }
        }
        else /// 만약 플레이어프랩스키값중 "뭐뭐뭐_최소"라고 저장된 키값이 존재하지 않으면
        {
            /// 플레이어프랩스에 ‘뭐뭐뭐_최소’라는 키값으로 데이터를 float 데이터를 저장한다.
            SecurePlayerPrefs.저장float(플레이어프랩스키값 + "_최소", 플레이어프랩스데이터);
        }
        /// 만약 플레이어프랩스데이터가 플레이어프랩스최대값에 크면
        if (플레이어프랩스데이터 > 플레이어프랩스최대값)
        {
            /// 플레이어프랩스에 ‘뭐뭐뭐_최대’라는 키값으로 데이터를 float 데이터를 저장한다.
            SecurePlayerPrefs.저장float(플레이어프랩스키값 + "_최대", 플레이어프랩스데이터);
        }
	}

    /// <summary>
    /// 플레이어프랩스에 ‘뭐뭐뭐_최대’키값으로 저장된 값을 가져오는 메서드.
    /// </summary>
    /// <returns>플레이어프랩스에 ‘뭐뭐뭐_최대’로 키값으로 저장된 값을 반환</returns>
    public float 최대값얻기()
    {
        return SecurePlayerPrefs.얻기float(플레이어프랩스키값 + "_최대");
    }

    /// <summary>
    /// 플레이어프랩스에 ‘뭐뭐뭐_최소’ 키값으로 저장된 값을 가져오는 메서드.
    /// </summary>
    /// <returns>플레이어프랩스에 ‘뭐뭐뭐_최소’키값으로 저장된 값 반환</returns>
    public float 최소값얻기()
    {
        return SecurePlayerPrefs.얻기float(플레이어프랩스키값 + "_최소");
    }

    void OnDestroy()
    {
        값저장();
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
