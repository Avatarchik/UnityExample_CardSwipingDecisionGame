using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 플레이어가 카드를 좌우로 넘길때 발생시키는 효과음을 유니티이벤트 형식으로 등록해서 플레이시키는 클래스.
/// 여러개의 효과음을 유니티이벤트로 등록해놓고, 플레이어가 카드를 넘길때 랜덤으로 하나 골라서 효과음을 발생시킨다.
/// </summary>
public class RandomEvents : MonoBehaviour{

    [System.Serializable] /// 유니티 이벤트를 상속받은 클래스를 인스펙터에 노출하기 위해 직렬화 해준다.
    public class 유니티이벤트 : UnityEvent { }

	/// <summary>
    /// 인스펙터에서 효과음을 발생시키는 유니티이벤트를 여러개 등록하기 위해 배열타입 변수를 만들자. 
    /// 여기에는 플레이어가 카드를 좌우로 넘길때 발생시키는 사운드와 사운드플레이메서드를 각각 등록한다.
    /// </summary>
	public 유니티이벤트[] 효과음발생이벤트;

	/// <summary>
    /// 유니티이벤트로 등록된 효과음중 랜덤으로 하나 골라서, 해당 효과음을 플레이한다.
    /// </summary>
	public void 랜덤으로효과음발생시키기()
    {
		int 인덱스 = Random.Range (0, 효과음발생이벤트.Length); /// 이벤트로 등록된 효과음 중 랜덤으로 하나 선정해서

		효과음발생이벤트 [인덱스].Invoke (); /// 해당 효과음을 플레이시킨다.
	}
}
