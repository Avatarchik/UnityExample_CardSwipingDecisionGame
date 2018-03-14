using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class valueDefinitions : MonoBehaviour {
    
    /// <summary>
    /// 게임의 모든 가치를 정의한 열거형. 이름, 성별, 나라, 카리스마등 게임에서 사용되는 모든 종류의 분류를 정의해놓은 열거형이다. 
    /// 여러 스크립트에서 인스펙터에서 쉽게 드롭 다운 메뉴를 표시하는데 사용된다.
    /// 가령 이름 변수일경우 인스펙터 드롭다운 메뉴에서 name을 지정하는 방식으로 사용한다.
    /// </summary>
    public enum values
    {
        name,
        surname,
        gender,
        country,
        years,
        army,
        people,
        religion,
        money,
        authority,
        intelligence,
        charisma,
        luck,
        creativity,
        look,
        health,
        marriage,
        married,
        adready,
        gamesPlayed
    }
}
