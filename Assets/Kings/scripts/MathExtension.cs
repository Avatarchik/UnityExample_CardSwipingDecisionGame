using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유니티의 UGUI에 슬라이더에 사용되는 게이지를 값을 계산 하는 메서드를 가지고 있는 클래스.
/// </summary>
public class MathExtension : MonoBehaviour  {

    /// <summary>
    /// 슬라이더의 현재 게이지를 표현할때 현재퍼센트와 플레이어프랩스에 저장된 퍼센트를 비교해서 보정된 값을 리턴하는 메서드.
    /// </summary>
    /// <param name="현재퍼센트">100%를 기준으로 현재 보유한 값의 퍼센트. 즉 현재 게이지의 퍼센트 </param>
    /// <param name="플레이어프랩스에저장된퍼센트">플레이어프랩스에 저장된 100%를 기준으로 현재 보유한 값의 퍼센트.</param>
    /// <param name="게이지속도">프레임당(Time.deltaTime) 게이지 늘어나는 양</param>
    /// <returns>본 메서드를 통해 보정된 슬라이더의 게이지 퍼센트</returns>
	public static float 늘어나는것보간하기(float 현재퍼센트, float 플레이어프랩스에저장된퍼센트, float 게이지속도)
	{
        /// 본 메서드를 통해 보정된 슬라이더의 게이지 퍼센트
		float 보정된현재퍼센트 = 0f;

        if (현재퍼센트 < (플레이어프랩스에저장된퍼센트 - 게이지속도))
        {
			보정된현재퍼센트 = 현재퍼센트 + 게이지속도;

			if (보정된현재퍼센트 >= 플레이어프랩스에저장된퍼센트)
            {
				보정된현재퍼센트 = 플레이어프랩스에저장된퍼센트;
			}
		}
        else if (현재퍼센트 > (플레이어프랩스에저장된퍼센트 + 게이지속도))
        {
			보정된현재퍼센트 = 현재퍼센트 - 게이지속도;

			if (보정된현재퍼센트 <= 플레이어프랩스에저장된퍼센트)
            {
				보정된현재퍼센트 = 플레이어프랩스에저장된퍼센트;
			}
		}
        else
        {
			보정된현재퍼센트 = 플레이어프랩스에저장된퍼센트;
		}

		return 보정된현재퍼센트;
	
	}
}
