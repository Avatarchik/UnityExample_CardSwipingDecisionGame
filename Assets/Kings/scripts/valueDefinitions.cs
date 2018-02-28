using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class valueDefinitions : MonoBehaviour {
    
    /// <summary>
    /// 게임의 모든 가치를 정의한 열거형.
    /// 값 정의는 여러 스크립트에서 쉽게 드롭 다운 메뉴를 표시하는데 사용됩니다. 조건 또는 값 수정
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
