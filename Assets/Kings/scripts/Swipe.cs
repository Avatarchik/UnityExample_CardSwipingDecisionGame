using UnityEngine;
using System.Collections;
using UnityEngine.Events; // 유니티 이벤트를 사용하기 위한 네임스페이스.


/// <summary>
/// 사용자가 마우스를 클릭해서 카드를 넘기거나 는 기능을 구현한 클래스.
/// Game -> Scripts
/// </summary>
public class Swipe : MonoBehaviour {

    /// <summary>
    /// 카드 넘기는 기능을 구현하기 위해 사용자가 드래그를 시작하는 지점.
    /// </summary>
	private Vector3 startPosition;

    /// <summary>
    /// 카드 넘기는 기능을 구현하기 위해 사용자가 드래그를 끝내는 지점.
    /// </summary>
	private Vector3 stopPosition;

	private Vector3 swipe;

    private Vector3 lastPosition;

    /// <summary>
    /// 유니티 이벤트를 등록하기 위한 클래스 선언.
    /// </summary>
	[System.Serializable] // 클래스를 인스펙터에 노출하기 위해 직렬화해준다.
    public class 유니티이벤트 : UnityEvent {}

    /// <summary>
    /// 스와이프 관련 드래그 설정값과 실행할 이벤트 메서드등을 정의한 클래스.
    /// </summary>
	[System.Serializable] // 클래스를 인스펙터에 노출하기 위해 직렬화해준다.
	public class normSwipe
    {
        /// <summary>
        /// 사용자가 스테이츠창을 보기 위해 위아래로 스와이프할때 최소 이 값 이상을 움직여야 다음 이벤트가 실행된다.
        /// </summary>
		[Tooltip("설정한 거리 이상으로 위 / 아래로 스와이프가 되야 이벤트가 실행됩니다.")]
		[Range(0f,1f)]
        public float swipeDetectionLimit_UD = 0.3f;

        /// <summary>
        /// 사용자가 카드를 넘기기위해 왼쪽오른쪽으로 스와이프할때 최소 이 값 이상을 움직여야 다음 이벤트가 실행된다.
        /// </summary>
		[Tooltip("설정한 거리 이상으로 왼쪽 / 오른쪽으로 스와이프가 되야 이벤트가 실행됩니다")]
		[Range(0f,1f)]
        public float swipeDetectionLimit_LR = 0.3f;

        /// <summary>
        /// 사용자가 위로 스와이프할때 발생시킬 유니티 이벤트. 인스펙터에서 3개의 메서드를 등록한다.
        /// Game -> MenuCanvas의 AnimatorBool.setBool 메서드를 할당. 초기값은 true로 한다. (카드 넘기는 애니메이션이 실행)
        /// Game -> Sounds -> swipe1의 AudioSource.Play 메서드 할당 (카드 넘기는 소리가 재생된다.)
        /// Game -> Scripts -> CardStack.setCardMoveEnable 메서드 할당. 초기값은 fail로 한다.
        /// </summary>
        public 유니티이벤트 swipeUp;

        /// <summary>
        /// 사용자가 아래로 스와이프할때 발생시킬 유니티 이벤트. 인스펙터에서 3개의 메서드를 등록한다.
        /// Game -> MenuCanvas의  AnimatorBool.setBool 메서드를 할당. 초기값은 true로 한다. (카드 넘기는 애니메이션이 실행)
        /// Game -> Sounds -> swipe2의 AudioSource.Play 메서드 할당 (카드 넘기는 소리가 재생된다.)
        /// Game -> Scripts -> CardStack.setCardMoveEnable 메서드 할당. 초기값은 true로 한다.
        /// </summary>
		public 유니티이벤트 swipeDown;

        /// <summary>
        /// 사용자가 왼쪽으로 스와이프할때 발생시킬 유니티 이벤트. 인스펙터에서 2개의 메서드를 등록한다.
        /// Game -> Scripts -> CardStack.leftSwipe 메서드 할당
        /// Game -> Scripts -> GameStateManager.swipe 메서드 할당.
        /// </summary>
		public 유니티이벤트 swipeLeft;

        /// <summary>
        /// 사용자가 오른쪽으로 스와이프할때 발생시킬 유니티 이벤트. 인스펙터에서 2개의 메서드를 등록한다.
        /// Game -> Scripts -> CardStack.rightSwipe 메서드 할당
        /// Game -> Scripts -> GameStateManager.swipe 메서드 할당.
        /// </summary>
		public 유니티이벤트 swipeRight;
	}

    /// <summary>
    /// 인스펙터에서 normSwipe 클래스에 정의된 변수를 노출시키기 위한 변수.
    /// 스와이프 관련 드래그 설정값과 실행할 이벤트 메서드등이 인스펙터에서 할당되어 있다.
    /// </summary>
	public normSwipe usualSwipes;

    /// <summary>
    /// 사용자의 마우스드래그에 연동되어 카드 넘기는 값으로 사용할 값.
    /// </summary>
	Vector3 swipeVector = Vector3.zero;

    /// <summary>
    /// ???
    /// </summary>
    /// <returns></returns>
	public Vector2 getSwipeVector()
    {
		return swipeVector;
	}

	[Tooltip("'getScaledSwipeVector ()'로 가져 오기 위해 스와이프의 크기를 조정합니다. \n 이 값은 카드 이동의 블렌드 트리와 연결하는 데 사용됩니다.")]
	public Vector2 swipeScale = Vector3.zero;

    /// <summary>
    /// 사용자가 마우스 왼쪽버튼을 누르고 있는 중인지 상태 여부를 담고 있는 변수.
    /// 사용자가 왼쪽버튼을 누르고 있는 동안은 true값을 갖는다.
    /// </summary>
	[ReadOnlyInspector] // 사용자가 만든 어트리뷰트. 인스펙터에 노출된 변수를 볼수는 있는데, 인스펙터에서 수정할수는 없게 하는 기능
    public bool pressed = false;

    /// <summary>
    /// 사용자가 현재 카드를 넘기기 위해 첫 클릭을 한 상황인지, 드래그 중인 상황인지를 구현하기 위한 변수.
    /// 사용자가 현재 카드는 넘기는 중이면 true값을 가지고, 아니거나, 첫클릭이면 false값을 가진다. 
    /// </summary>
	private bool oldPressed = false;

    /// <summary>
    /// 사용자가 카드를 넘기기 위해 마우스 왼쪽버튼을 클릭해서 드래그해서 이동한 거리에 연동되서 카드가 스와이프 되는 거리.
    /// </summary>
    [ReadOnlyInspector] // 사용자가 만든 어트리뷰트. 인스펙터에 노출된 변수를 볼수는 있는데, 인스펙터에서 수정할수는 없게 하는 기능
    public float actualSwipeDistance = 0f;

	public Vector2 getScaledSwipeVector()
    {
		Vector2 retVal = Vector2.zero;
		retVal.x = swipeVector.x * swipeScale.x;
		retVal.y = swipeVector.y * swipeScale.y;
		return retVal;
	}

	void Update(){


        /// 사용자가 카드를 넘기기 위해 마우스 왼쪽버튼이 누루고 있는 순간동안만 pressed에 true값을
        if (Input.GetMouseButton(0))
        {
            pressed = true;
        }
        else /// 아니면 false값을 준다.
        {
            pressed = false;
        }

		
        /// 사용자가 카드를 넘기기 위해 클릭한 상태라면 (시작단계라면)
        if (oldPressed == false && pressed == true)
        {
            onKlick ();	
		}

        /// 사용자가 카드를 넘기기 위해 드래그중인 상태이거나, 드래그가 끝난 상태라면
        if (oldPressed == true && pressed == false)
        {
            onRelease ();
        }
		
        /// 사용자가 마우스 왼쪽 버튼을 누르고 있는 카드를 넘기는 행위를 하고 있는 동안이면, 카드가 얼마나 스와이프 해야 하는지 값을 계산하고
		if (pressed == true)
        {
            /// 시작지점과 현재 마우스위치와의 거리를 구한다. 드래그해서 이동한 거리를 구한다.
			swipeVector = Input.mousePosition - startPosition;
            /// 현재 스크린 사이즈에 비례해서 드래그한 거리를 산정해서 할당.
			swipeVector.x = swipeVector.x / Screen.width;
			swipeVector.y = swipeVector.y / Screen.height;
            /// 카드가 스와이프될 거리를 계산해서 할당.
			actualSwipeDistance = swipeVector.magnitude;
		}
        else /// 아무행위도 안하고 있으면 값을 초기화한다.
        {
			actualSwipeDistance = 0f;
			swipeVector = Vector3.zero;
		}
        
		oldPressed = pressed;
	}

    /// <summary>
    /// 현재 마우스 커서 위치를 시작지점과 끝지점에 할당한다.
    /// </summary>
    void onKlick(){

        /// 현재 마우스가 위치한 지점(클릭한 지점이 아님)의 좌표를 시작지점과 끝지점으로 할당한다.
		startPosition = Input.mousePosition;
		lastPosition = startPosition;
	}

    /// <summary>
    /// 사용자가 게임을 진행하기 위해 카드를 좌우로 넘기거나, 옵션을 보는 행위를 할때 실행하는 명령들. 
    /// </summary>
	void onRelease()
    {
        /// 현재 마우스 커서 위치를 스톱위치에 할당한다.
		stopPosition = Input.mousePosition;

        /// 사용자가 상하좌우 스와이트를 했을때 실행할 명령들을 실행.
		processSwipe ();
	}

    /// <summary>
    /// 사용자가 상하좌우 스와이프를 했을때 실행할 명령들 모음
    /// </summary>
	void processSwipe()
    {
        
        /// 사용자가 카드를 좌우로 넘기는 중이라면
		if (Mathf.Abs (swipeVector.x) > Mathf.Abs (swipeVector.y))
        {

			/// 스와이프 크기가 설정한 값보다 크다면
			if (swipeVector.magnitude > usualSwipes.swipeDetectionLimit_LR) {
				
                /// 오른쪽으로 카드를 넘기는것이라면
				if (swipeVector.x > 0f)
                {
                    /// 오른쪽으로 넘기는 기능에 등록된 것들을 실행한다.
					usualSwipes.swipeRight.Invoke ();	
				}
                else /// 왼쪽으로 카드를 넘기는 것이라면
                {
                    /// 왼쪽으로 넘기는 기능에 등록된 것들을 실행한다.
					usualSwipes.swipeLeft.Invoke ();	
				}
			}
		}
        else /// 사용자가 옵션을 보기 위해 위아래로 스와이프 중이라면
        {
            /// 스와이프 크기가 설정한 값보다 크다면
			if (swipeVector.magnitude > usualSwipes.swipeDetectionLimit_UD)
            {
				/// 위로 드래그중이라면
				if (swipeVector.y > 0f)
                {
					usualSwipes.swipeUp.Invoke ();	
				}
                else /// 아래로 드래그중이라면
                {
					usualSwipes.swipeDown.Invoke ();	
				}
			}
		}
	
		/// 스와이프 한 다음 : 위치를 0으로 재설정한다.
		startPosition = stopPosition = Vector3.zero;
	}

}
