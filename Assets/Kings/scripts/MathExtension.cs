using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ???? 유니티의 UGUI에 슬라이더에 사용되는 계산을 하는 메서드를 가지고 있는 클래스.
/// </summary>
public class MathExtension : MonoBehaviour  {

    /// <summary>
    /// ???? 뭔가 늘어나는것을 보간하는 것 같은데.
    /// </summary>
    /// <param name="실제"></param>
    /// <param name="타겟"></param>
    /// <param name="스텝"></param>
    /// <returns></returns>
	public static float 늘어나는것보간하기(float 실제, float 타겟, float 스텝)
	{
		float retVal = 0f;

        if (실제 < (타겟 - 스텝))
        {
			retVal = 실제 + 스텝;

			if (retVal >= 타겟)
            {
				retVal = 타겟;
			}
		}
        else if (실제 > (타겟 + 스텝))
        {
			retVal = 실제 - 스텝;

			if (retVal <= 타겟)
            {
				retVal = 타겟;
			}
		}
        else
        {
			retVal = 타겟;
		}

		return retVal;
	
	}
}
