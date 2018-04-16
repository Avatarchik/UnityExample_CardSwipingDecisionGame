using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class valueDefinitions : MonoBehaviour {
    
    /// <summary>
    /// 게임의 모든 가치를 정의한 열거형. 이름, 성별, 나라, 카리스마등 게임에서 사용되는 모든 종류의 분류를 정의해놓은 열거형이다. 
    /// 유니티에서 열거형 변수는 인스펙터에서 드롭 다운 메뉴를 표시되기때문에 사용자가 열거형에 요소를 쉽게 선택해서 지정할 수 있다.
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
