#define SECURED /// 사용자가 정의한 기호. 만약 보안이 필요한 상황일때 전처리에서 프로그래머가 SECURED를 지정하면 컴파일할때 #if SECURED 코드에 작성한 코드가 포함된다..

using UnityEngine;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// 플레이어프랩스에 데이터를 입출력할때 보안처리를 하느냐마느냐 관련 클래스.
/// </summary>
public static class SecurePlayerPrefs
{

    /// <summary>
    /// 플레이어 프랩스에 문자형 데이터를 저장하는 메서드. 보안이 필요한경우 플레이어프랩스 구분하는 키를 MD5 해시값으로 바꿔서 키로 만들고, 데이터를 저장한다. 
    /// </summary>
    /// <param name="키이름">플레이어프랩스에 구분할 키이름으로 사용할 데이터</param>
    /// <param name="저장데이터">저장할 데이터. 문자열</param>
	public static void 플레이어프랩스에데이터저장(string 키이름, string 저장데이터)
	{
		#if (SECURED) /// 보안이 필요한 경우라면 

        /// 플레이어프랩스에 저장할때 구분할 키이름을 MD5 해시형태로 바꾼다.
		string 해시키 = MD5해시생성 (키이름);
        /// 의미없는 문자를 덧붙여서 체크키로 저장한다.
		string 체크키 = MD5해시생성 (키이름 + "asdf");
        /// 암호화된 값을 저장한다.
		string 암호화된값 = xorEncryptDecrypt (저장데이터);

		string checkVal = MD5해시생성 (암호화된값);
		checkVal = xorEncryptDecrypt (checkVal);
		PlayerPrefs.SetString (해시키, 암호화된값);
		PlayerPrefs.SetString (체크키, checkVal);
		#else /// 보안이 필요하지 않을 상황이라면, 플레이어 프랩스에 
		PlayerPrefs.SetString (key, value);
		#endif
	}

	public static string 플레이어프랩스에데이터가져오기(string 키이름)
	{
		#if (SECURED)
		string hashedKey = MD5해시생성 (키이름);
		if (PlayerPrefs.HasKey (hashedKey)) {
			string encryptedValue = PlayerPrefs.GetString (hashedKey);
			string checkKey = MD5해시생성 (키이름 + "asdf");
			string readCheckVal = PlayerPrefs.GetString (checkKey);
			readCheckVal = xorEncryptDecrypt (readCheckVal);
			string checkVal = MD5해시생성 (encryptedValue);

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
		플레이어프랩스에데이터저장 (key, value.ToString ());
	}
	public static float GetFloat(string key){
		float value;
		if (float.TryParse (플레이어프랩스에데이터가져오기 (key), out value)) {
			return value;
		} else {
			return 0f;
		}
	}
	public static void SetDouble(string key, double value){
		플레이어프랩스에데이터저장 (key, value.ToString ());
	}
	public static double GetDouble(string key){
		double value;
		if (double.TryParse (플레이어프랩스에데이터가져오기 (key), out value)) {
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
		플레이어프랩스에데이터저장 (key, value.ToString ());
	}
	public static int GetInt(string key){
		int value;
		if (int.TryParse (플레이어프랩스에데이터가져오기 (key), out value)) {
			return value;
		} else {
			return 0;
		}
	}

	public static string GetString(string key, string defaultValue)
	{
		if (HasKey(key))
		{
			return 플레이어프랩스에데이터가져오기(key);
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
		string hashedKey = MD5해시생성 (key);
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
    /// <param name="텍스트"></param>
    /// <returns></returns>
	public static string xorEncryptDecrypt(string 텍스트)
    {
        /// 문자열을 수정할 수 있는 안으로 들어오는 StringBuilder 객체를 만들고, 파라미터로 받은 텍스트를 할당한다.
		StringBuilder 들어오는스트링빌더객체 = new StringBuilder (텍스트);
        /// 문자열을 수정할 수 있는 밖으로 나가는 StringBuilder 객체를 만들고, 파라미터로 받은 텍스트 길이를 용량으로 할당한다.
		StringBuilder 나가는스트링빌더객체 = new StringBuilder (텍스트.Length);
        
        /// 문자를 넣을 로컬 변수
		char 문자;
        
        /// 파라미터의 입력받은 텍스트 길이만큼 루프를 돌면서
		for (int i = 0; i < 텍스트.Length; i++)
        {
            /// 파라미터로 입력받은 텍스트를 한글짜씩 떼어서 할당
            문자 = 들어오는스트링빌더객체[i];

            /// key값이 십진수 129이니깐 이진수로는 10000001이다. 이것을 2진수 자리수로 대입해서 현재 로컬변수에 들어 있는 값이 같으면 0, 서로 다른값이면 1을 반환해서 그걸 텍스트로 전환해서 넣는다.
            문자 = (char)(문자 ^ key);
            /// 밖으로 뽑는 스트링 빌더값에 계속 추가한다.
            나가는스트링빌더객체.Append(문자);
        }
        /// 비교한 값을 반환한다.
		return 나가는스트링빌더객체.ToString ();
	}

    /// <summary>
    /// 파라미터로 입력한 텍스트를 MD5 해시값으로 바꿔서 리턴하는메서드.
    /// MD5 해시 방식이 아주 안전하지 않기때문에 비밀번호 저장방식으로는 안전하지 않기때문에 비밀번호 용도로는 쓰지 말것.
    /// </summary>
    /// <returns>MD5 해시 문자열</returns>
    /// <param name="해시값으로바꿀텍스트">해시값으로 바꿀 텍스트</param>
    static string MD5해시생성(string 해시값으로바꿀텍스트)
	{
        /// step 1, 씨샵에서 제공하는 MD5 해시 알고리즘 타입의 해시 객체를 할당한다.
		var md5 = MD5.Create();

        /// UTF8 인코딩 방식으로 텍스트를 바이트로 변환해서 할당.
		byte[] 바이트로쪼갠문자 = Encoding.UTF8.GetBytes(해시값으로바꿀텍스트);
        /// 지정된 바이트 배열에 대한 해시값을 계산한다.
		byte[] 해시 = md5.ComputeHash(바이트로쪼갠문자);

		/// step 2, byte array을 16진수 문자열로 변경
		var sb = new StringBuilder();
		for (int i = 0; i < 해시.Length; i++)
		{
			sb.Append(해시[i].ToString("X2"));
		}

        /// 생성된 해시 문자열을 반환.
		return sb.ToString();
	}

	#endif
}