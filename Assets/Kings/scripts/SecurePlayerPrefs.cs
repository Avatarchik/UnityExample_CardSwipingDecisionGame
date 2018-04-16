#define SECURED /// 사용자가 정의한 기호. 만약 보안이 필요한 상황일때 전처리에서 프로그래머가 SECURED를 지정하면 컴파일할때 #if SECURED 코드에 작성한 코드가 포함된다..

using UnityEngine;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// ?????
/// </summary>
public static class SecurePlayerPrefs
{

    /// <summary>
    /// 플레이어 프랩스에 문자형 데이터를 저장하는 메서드. 보안이 필요한경우 플레이어프랩스 구분하는 키를 MD5 해시값으로 바꿔서 키로 만들고, 데이터를 저장한다. 
    /// </summary>
    /// <param name="key">플레이어프랩스에 구분할 키값으로 사용할 텍스트</param>
    /// <param name="value">저장할 데이터. 문자열</param>
	public static void SetString(string key, string value)
	{
		#if (SECURED) /// 보안이 필요한 경우라면 

        /// 플레이어프랩스에 저장할때 구분할 키이름을 MD5 해시형태의 문자열을 바꾼다.
		string hashedKey = GenerateMD5 (key);
        /// 의미없는 문자를 체크키로 저장한다.
		string checkKey = GenerateMD5 (key + "asdf");
        /// 암호화된 값을 저장한다.
		string encryptedValue = xorEncryptDecrypt (value);

		string checkVal = GenerateMD5 (encryptedValue);
		checkVal = xorEncryptDecrypt (checkVal);
		PlayerPrefs.SetString (hashedKey, encryptedValue);
		PlayerPrefs.SetString (checkKey, checkVal);
		#else /// 보안이 필요하지 않을 상황이라면, 플레이어 프랩스에 
		PlayerPrefs.SetString (key, value);
		#endif
	}

	public static string GetString(string key)
	{
		#if (SECURED)
		string hashedKey = GenerateMD5 (key);
		if (PlayerPrefs.HasKey (hashedKey)) {
			string encryptedValue = PlayerPrefs.GetString (hashedKey);
			string checkKey = GenerateMD5 (key + "asdf");
			string readCheckVal = PlayerPrefs.GetString (checkKey);
			readCheckVal = xorEncryptDecrypt (readCheckVal);
			string checkVal = GenerateMD5 (encryptedValue);

			string decryptedValue;
			decryptedValue = xorEncryptDecrypt (encryptedValue);

			if (string.Equals (readCheckVal, checkVal)) {
				return decryptedValue;
			}else {
				return "";
			}
		} else {
			return "";
		}
		#else
		return PlayerPrefs.GetString (key);
		#endif
	}
	public static void SetFloat(string key, float value){
		SetString (key, value.ToString ());
	}
	public static float GetFloat(string key){
		float value;
		if (float.TryParse (GetString (key), out value)) {
			return value;
		} else {
			return 0f;
		}
	}
	public static void SetDouble(string key, double value){
		SetString (key, value.ToString ());
	}
	public static double GetDouble(string key){
		double value;
		if (double.TryParse (GetString (key), out value)) {
			return value;
		} else {
			return 0;
		}
	}

	public static void SetBool(string key, bool value){
		if (value) {
			SetInt (key, 1);
		} else {
			SetInt (key, 0);
		}
	}
	public static bool GetBool(string key){
		if(GetInt(key)==1)
		{
			return true;	
		}
		return false;
	}
	public static void SetInt(string key, int value){
		SetString (key, value.ToString ());
	}
	public static int GetInt(string key){
		int value;
		if (int.TryParse (GetString (key), out value)) {
			return value;
		} else {
			return 0;
		}
	}

	public static string GetString(string key, string defaultValue)
	{
		if (HasKey(key))
		{
			return GetString(key);
		}
		else
		{
			return defaultValue;
		}
	}

    /// <summary>
    /// 파라미터로 받은 값이 플레이어프랩스에 이미 존재하는 키값인지 확인해서 존재하면 true를, 존재하지 않으면 false를 반환한다.
    /// </summary>
    /// <param name="key">????</param>
    /// <returns></returns>
	public static bool HasKey(string key)
	{
		#if(SECURED) /// 전처리기 지시문에서 프로그래머가 보안이 필요한 상황이라고 작성했다면 처리되는 코드
        /// 파라미터로 입력받은 값을 MD5 해시값으로 바꿔서 저장
		string hashedKey = GenerateMD5 (key);
        /// MD5로 바꾼값이 플레이어프랩스에 존재하는 지를 확인한다
		bool hasKey = PlayerPrefs.HasKey (hashedKey);
        /// 확인한 값을 리턴한다.
		return hasKey;
		#else /// 전처리기 지시문에서 프로그래머가 보안이 필요하지 않은 상황으로 작성했다면 처리되는 코드
        /// 파라미터로 입력받은 값이 플레이어프랩스에 존재하는지를 확인한다.
		bool hasKey = PlayerPrefs.HasKey (key);
        /// 확인한 값을 리턴한다.
		return hasKey;
		#endif
	}

    #if (SECURED) /// 보안이 필요한 경우라면.
    // 일부 빠른 xor 암호화기
    public static int key = 129;

    

    /// <summary>
    /// 129라는 키값과, 파라미터로 입력받은 값을 비교해서 어떻게 어떻게 암호화 하는 메서드인듯.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
	public static string xorEncryptDecrypt(string text)
    {
        /// 문자열을 수정할 수 있는 안으로 들어오는 StringBuilder 객체를 만들고, 파라미터로 받은 텍스트를 할당한다.
		StringBuilder inSb = new StringBuilder (text);
        /// 문자열을 수정할 수 있는 밖으로 나가는 StringBuilder 객체를 만들고, 파라미터로 받은 텍스트 길이를 용량으로 할당한다.
		StringBuilder outSB = new StringBuilder (text.Length);
        /// 문자를 넣을 로컬 변수
		char c;
        /// 파라미터의 텍스트 길이만큼 루프를 돌면서
		for (int i = 0; i < text.Length; i++)
        {
            /// 한글짜씩 떼어서 할당해서
            c = inSb[i];
            /// key값이 십진수 129이니깐 이진수로는 10000001이다. 이것을 2진수 자리수로 대입해서 현재 로컬변수에 들어 있는 값이 같으면 0, 서로 다른값이면 1을 반환해서 그걸 텍스트로 전환해서 넣는다.
            c = (char)(c ^ key);
            /// 밖으로 뽑는 스트링 빌더값에 계속 추가한다.
            outSB.Append(c);
        }
        /// 비교한 값을 반환한다.
		return outSB.ToString ();
	}

    /// <summary>
    /// 파라미터로 입력한 텍스트를 MD5 해시값으로 바꿔서 리턴합니다.
    /// 경고: 비밀번호 저장방식으로는 안전하지 않습니다.
    /// </summary>
    /// <returns>MD5 해시 문자열</returns>
    /// <param name="text">해시값으로 바꿀 텍스트</param>
    static string GenerateMD5(string text)
	{
        /// 첫단계. 씨샵에서 제공하는 MD5 해시 알고리즘 타입의 해시 객체를 할당한다.
		var md5 = MD5.Create();

		byte[] inputBytes = Encoding.UTF8.GetBytes(text);
		byte[] hash = md5.ComputeHash(inputBytes);

		// step 2, byte array을 16진수 문자열로 변경
		var sb = new StringBuilder();
		for (int i = 0; i < hash.Length; i++)
		{
			sb.Append(hash[i].ToString("X2"));
		}

        /// 생성된 해시 문자열을 반환.
		return sb.ToString();
	}

	#endif
}