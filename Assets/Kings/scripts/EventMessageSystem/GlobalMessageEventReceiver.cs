using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ???? 메시지 관리자 역활을 하는 클래스인듯.
/// 'GlobalMessageEventReceiver'는 'GlobalMessageEventManager'에 의해 호출됩니다. 
/// 관리자가 없으면이 스크립트로 작성됩니다. 
/// 이 수신기를 gameobjects에 추가해야합니다.이 수신기는 특정 메시지의 영향을받습니다.조정 가능한 문자열 메시지에 따라이 수신기는 해당 단일 이벤트를 실행합니다. 
/// 리시버는 프리 패브에 추가 될 수 있으며 게임 개체가 활성화 된 후에 준비가됩니다.
/// Game -> GlobalMessages
/// </summary>
public class GlobalMessageEventReceiver : MonoBehaviour {

	[System.Serializable] /// 클래스를 인스펙터에 노출하기 위해 직렬화.
    public class mEvent : UnityEvent {}

	bool OutputReceivedMessages = true;

	[System.Serializable] /// 클래스를 인스펙터에 노출하기 위해 직렬화.
	public class MessageEvent
	{
        /// <summary>
        /// ???????
        /// 인스펙터에서 'spawnStart' 텍스트를 할당.
        /// </summary>
		public string trigger;

        /// <summary>
        /// 인스펙터에 노출되는 유니티 이벤트.
        /// 여기서는 Game -> Scripts -> CardStack.setCardMoveEnable를 할당.
        /// </summary>
		public mEvent _event;
	}	
		
	void Start()
    {
		createManagerIfNonExisting ();
		//register the receiver at the manager, to get the delegation of messages. 매니저 위의 수신자를 등록하여 메시지 위임을 얻는다.
		GlobalMessageEventManager.registerMessageReceiver (this);
	}

    /// <summary>
    /// GlobalMessageEventManager 스크립트가 붙어 있는 객체가 없는 경우 객체를 새로 만들고 나를 부착한다.
    /// </summary>
    void createManagerIfNonExisting()
    {
        /// GlobalMessageEventManager 클래스의 인스턴스에 아무것도 할당안되 있으면, 
		if (GlobalMessageEventManager.instance == null)
        {
            /// "GlobalMessageEventManager" 이름의 새로운 게임 오브젝트를 만들어서 로컬변수에 할당하고
			GameObject go = new GameObject("GlobalMessageEventManager");
            /// GlobalMessageEventManager 클래스를 부착하고
			go.AddComponent <GlobalMessageEventManager>();

			GlobalMessageEventManager gem = go.GetComponent <GlobalMessageEventManager> ();
			gem.buildAwake ();
		}
	}

	//unregister the receiver at the manager, to stop the delegation of messages
	void OnDestroy()
    {
		GlobalMessageEventManager.unregisterMessageReceiver (this);
	}

	[Tooltip("List of event - message combinations. Only relevant messages for this gameobject have to be added.")]
	public MessageEvent[] MessageEvents;


	/*
	 * 'globalMessage()' is usually called by the management script to delegate the message. 
	 */

	public void globalMessage(string trigger){

		if (OutputReceivedMessages == true) {
			Debug.Log (trigger);
		}

		int invokeCnt = 0;
		foreach (MessageEvent me in MessageEvents) {
			if (trigger == me.trigger) {	//if the message was configured..
				me._event.Invoke ();		//..execute/invoke the corresponding event
				invokeCnt++;
			}
		}
		if (invokeCnt == 0) {
			//Zero executions are possible, because of multible receivers with different triggers.
			//This time no message/trigger for this receiver was sent.
		}
	}
}
