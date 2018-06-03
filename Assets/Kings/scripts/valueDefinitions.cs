using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueDefinitions : MonoBehaviour {

    /// <summary>
    /// 게임에서 사용되는 모든 가치값을 정의해놓은 열거형.
    /// 유니티에서 열거형 변수는 인스펙터에서 드롭 다운 메뉴를 표시되기때문에 프로그래머가 인스펙터에서 목록의 요소를 쉽게 선택해서 지정할 수 있게 하기 위해 본 게임에서 사용되는 모든 값들을 열거형으로 정의해놨다.
    /// </summary>
    public enum 값정의
    {
        이름, // 이름
        서브네임, // 성
        성별, // 성별
        국가, // 나라
        통치기간, // 통치기간
        군대, // 군대수치
        국민, // 국민수치
        종교, // 종교수치
        돈, // 돈
        권위, // 권위
        지성, // 지성
        카리스마, // 카리스마
        행운, // 행운
        독창성, // 독창성
        look, //
        건강, // 건강
        결혼, // 결혼
        기혼, // 기혼
        광고준비, // 광고준비
        게임플레이중 // 게임플레이중
    }
}
