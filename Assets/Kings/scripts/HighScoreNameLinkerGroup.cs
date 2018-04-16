using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 하이스코어 점수를 낸 유저를 관리하는 클래스???? 
/// </summary>
public class HighScoreNameLinkerGroup : MonoBehaviour {

    /// <summary>
    /// 나 자신의 객체를 만든다.
    /// </summary>
	public static HighScoreNameLinkerGroup instance;

    /// <summary>
    /// 나 자신의 객체에 나를 할당한다.
    /// </summary>
	void Awake(){
		instance = this;
	}


	public HighScoreNameLinker[] hsnl;
	// Use this for initialization
	void Start () {
		//hsnl = GetComponentsInChildren<HighScoreNameLinker> ();
	}

	public void generateLinks(){
		foreach (HighScoreNameLinker a in hsnl) {
			a.generateHighScoreNameLink ();
		}
	}

	// Update is called once per frame
	void Update () {
	}
}
