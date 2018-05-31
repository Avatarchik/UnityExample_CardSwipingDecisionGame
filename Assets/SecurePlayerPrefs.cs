#define SECURED /// 사용자가 정의한 기호. 만약 보안이 필요한 상황일때 전처리에서 프로그래머가 SECURED를 지정하면 컴파일할때 #if SECURED 코드에 작성한 코드가 포함된다..

using UnityEngine;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// 플레이어프랩스에 데이터를 저장하고, 가져오는 처리를하는 클래스. 전처리 옵션에 따라 암호화 보안처리를 한다.
/// </summary>
public static class SecurePlayerPrefs
{

    /// <summary>
    /// 플레이어 프랩스에 문자형 데이터를 저장하는 메서드. 보안이 필요한경우 플레이어프랩스 구분하는 키를 MD5 해시값으로 바꿔서 키로 만들고, 데이터를 저장한다. 
    /// </summary>
    /// <param name="구분할키이름">플레이어프랩스에서 구분하는 키이름</param>
    /// <param name="저장할데이터">저장할 데이터. 문자열</param>
	public static void 플레이어프랩스에데이터저장(string 구분할키이름, string 저장할데이터)
	{
		#if (SECURED) /// 디파인을 보안이 필요한 경우로 지정한 경우 실행되는 코드 

        /// 플레이어프랩스에 저장할때 구분하는 키이름을 MD5 해시형태로 바꾼다.
		string 저장할키이름을해시로만듦 = MD5해시생성 (구분할키이름);
        /// 저장할 데이터 자체도 암호화 한다.
		string 비트암호화된저장할데이터 = Xor암호화또는풀기(저장할데이터);
        /// 플레이프랩스에 저장할 키와 데이터를 해시키와 비트암호화된데이터로 바꿔서 저장한다.
        PlayerPrefs.SetString(저장할키이름을해시로만듦, 비트암호화된저장할데이터);
        

        /// 보안을 풀기 더 어렵게 의미없는 문자를 덧붙여서 나중에 플레이어프랩스에서 해당 데이터를 꺼내올깨 비교할 체크키를 하나 만든다.
		string 키이름에의미없는문자결합해서해시로만듦것 = MD5해시생성 (구분할키이름 + "asdf");
        
        /// 비트암호화된 저장할 데이터 자체도 해시형태로 바꾼다.
		string 체크에사용되는저장할데이트 = MD5해시생성 (비트암호화된저장할데이터);
        /// 해시형태를 또 다시 비트암호화해서 저장한다.
        체크에사용되는저장할데이트 = Xor암호화또는풀기 (체크에사용되는저장할데이트);
        
        /// 나중에 플레이어프랩스에서 해당 데이터를 꺼내올때 비교할 체크키와 체크데이터로 저장한다.
		PlayerPrefs.SetString (키이름에의미없는문자결합해서해시로만듦것, 체크에사용되는저장할데이트);
		
        #else /// 디파인을 보안이 필요하지 않은 상황으로 지정한 경우 실행되는 코드 
		
        /// 플레이어프랩스에 데이터를 저장한다.
        PlayerPrefs.SetString (저장할키이름, 저장할데이터);
		
        #endif
	}

    /// <summary>
    /// ????
    /// </summary>
    /// <param name="구분할키이름">플레이어프랩스에서 데이터를 저장할때 사용할 키값</param>
    /// <returns></returns>
	public static string 플레이어프랩스에데이터가져오기(string 구분할키이름)
	{
        #if (SECURED) /// 디파인을 보안이 필요한 경우로 지정한 경우 실행되는 코드 

        /// 플레이어프랩스에서 커내올 데이터의 키이름을 해시로 만듦  
        string 구분할키이름을해시로만듦 = MD5해시생성 (구분할키이름);

        /// 만약 플레이어프랩스에 가져오고자하는 키값이 해시로 있다면, 즉 데이터가 있다면
        if (PlayerPrefs.HasKey (구분할키이름을해시로만듦))
        {
            /// 가지고 올 데이터를 플레이어 프랩스에서 꺼내와서 
			string 비트암호화된저장된데이터 = PlayerPrefs.GetString (구분할키이름을해시로만듦);



            /// 플레이어 프랩스에서 체크키를 꺼내 옴.
            string 키이름에의미없는문자결합해서해시로만듦것 = MD5해시생성 (구분할키이름 + "asdf");


            string 플레이어프랩스에서꺼내온비교데이터 = PlayerPrefs.GetString (키이름에의미없는문자결합해서해시로만듦것);

            플레이어프랩스에서꺼내온비교데이터 = Xor암호화또는풀기 (플레이어프랩스에서꺼내온비교데이터);

            string 플레이어프랩스에서꺼내온키이름을해시로만듦 = MD5해시생성 (비트암호화된저장된데이터);

			string 플레이프랩스에서꺼내온원본데이터;

            플레이프랩스에서꺼내온원본데이터 = Xor암호화또는풀기 (비트암호화된저장된데이터);

			if (string.Equals (플레이어프랩스에서꺼내온비교데이터, 플레이어프랩스에서꺼내온키이름을해시로만듦)) {

                return 플레이프랩스에서꺼내온원본데이터;
			}
            else
            {
				return "";
			}
		}
        else /// 플레이어프랩스에 가져오고자하는 키값의 해시가 없다면, 즉 데이터가 없다면
        {
            /// 빈문자열을 리턴한다.
			return "";
		}

        #else /// 디파인을 보안이 필요하지 않은 상황으로 지정한 경우 실행되는 코드
        /// 플레이어프랩스에서 데이터를 가져온다.
        return PlayerPrefs.GetString (구분할키이름);
        
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
   
    /// 비트 연산을 통해 이진수 숫자를 흩트려놓기 위한 그냥 프로그래머가 넣은 아무 숫자. xor 암호를 하기 위해.
    /// 이진수차원에서 이값과 비트연산을 통해 원래 값을 흩트려놓기 때문에 다른 사람이 이값을 알지 못하면 다시 비트 연산을 한다고 해도 원래 글자를 알수 없을 것이다.
    public static int 아무숫자 = 129;

    /// <summary>
    /// 문자를 파라미터로 입력받아서, 파라미터로 입력받은 문자값을 아무 숫자를 이용해서 XOR 비트연산해서, 이진수 차원에서 파라미터로 입력받은 값을 흩트려놓아서, 리턴하는 값은 전혀 다른 값을 내놓게 되는 메서드이다.
    /// 즉, 
    /// 본 메서드를 암호화하는데 사용한다면, 원래 값을 프로그래머가 정한 아무숫자를 이용해서 흩트려놓은 값을 반환해서 암호화가 된다. 
    /// 반대로 본 메서드를 통해 흩트려놓은 값을 다시 본 메서드의 파라미터 값으로 집어넣으면 원래 값으로 다시 환원된다.
    /// </summary>
    /// <param name="텍스트"></param>
    /// <returns>암호화된 값</returns>
    public static string Xor암호화또는풀기(string 텍스트)
    {
        /// 스트링빌더 객체를 2개 만드는데,
        /// 한개는 파라미터로 받는 텍스트를 저장할 스트링빌더 객체고
        /// 한개는 위 객체와 동일한 크기의 반환값을 저장할 스트링빌더 객체를 만든다.
		StringBuilder 들어오는스트링빌더객체 = new StringBuilder (텍스트);
		StringBuilder 나가는스트링빌더객체 = new StringBuilder (텍스트.Length);
        
        /// 문자를 넣을 로컬 변수
		char 문자;
        
        /// 파라미터의 입력받은 텍스트 길이만큼 루프를 돌면서
		for (int i = 0; i < 텍스트.Length; i++)
        {
            /// 파라미터로 입력받은 텍스트를 한글짜씩 떼어서 할당
            문자 = 들어오는스트링빌더객체[i];

            /// 아무숫자값이 십진수 129이니깐 이진수로는 10000001이다. 이것을 2진수 자리수로 대입해서 현재 로컬변수에 들어 있는 값이 같으면 0, 서로 다른값이면 1을 반환해서 그걸 텍스트로 전환해서 넣는다.
            문자 = (char)(문자 ^ 아무숫자);

            /// 밖으로 뽑는 스트링 빌더값에 계속 추가한다.
            나가는스트링빌더객체.Append(문자);
        }
        /// 이진수가 흩트려진 값(프로그래머가 임의로 암호한 값)을 반환한다. 문자열
		return 나가는스트링빌더객체.ToString ();
	}

    /// <summary>
    /// 파라미터로 입력한 텍스트를 한글자씩 쪼개서, 쪼개진 한글자씩을 MD5 해시값으로 바꿔서 쭈욱 붙여서 문자열로 만들어서 리턴하는메서드.
    /// MD5 해시 방식이 아주 안전하지 않기때문에 비밀번호 저장방식으로는 안전하지 않기때문에 비밀번호 용도로는 쓰지 말것.
    /// </summary>
    /// <returns>MD5 해시 문자열</returns>
    /// <param name="해시값으로바꿀텍스트">해시값으로 바꿀 텍스트</param>
    static string MD5해시생성(string 해시값으로바꿀텍스트)
	{
        /// step 1, 씨샵에서 제공하는 MD5 해시 알고리즘 타입의 해시 객체를 할당한다.
		var md5객체 = MD5.Create();

        /// UTF8 인코딩 방식으로 파라미터로 받은 텍스트를 한글자씩 쪼개서 바이트로 변환해서 할당.
		byte[] 바이트로쪼갠문자 = Encoding.UTF8.GetBytes(해시값으로바꿀텍스트);
        /// 한글자씩 쪼갠 바이트를 해시값으로 변환해서 배열에 한개씩 할당.
		byte[] 해시 = md5객체.ComputeHash(바이트로쪼갠문자);

		/// step 2, 해시값이 담긴 배열을 루프를 돌면서 16진수 문자열로 변경
		var 리턴값 = new StringBuilder();

        for (int i = 0; i < 해시.Length; i++)
		{
            /// 해시 배열을 16진수값으로 바꾼다.
			리턴값.Append(해시[i].ToString("X2"));
		}

        /// 생성된 해시 문자열을 반환.
		return 리턴값.ToString();
	}

	#endif
}