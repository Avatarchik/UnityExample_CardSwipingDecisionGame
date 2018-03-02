#define SECURED // 사용자가 정의한 기호.

using UnityEngine;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// ??????
/// 
/// </summary>
public static class SecurePlayerPrefs
{
	public static void SetString(string key, string value)
	{
		#if (SECURED)
		string hashedKey = GenerateMD5 (key);
		string checkKey = GenerateMD5 (key + "asdf");
		string encryptedValue = xorEncryptDecrypt (value);
		string checkVal = GenerateMD5 (encryptedValue);
		checkVal = xorEncryptDecrypt (checkVal);
		PlayerPrefs.SetString (hashedKey, encryptedValue);
		PlayerPrefs.SetString (checkKey, checkVal);
		#else
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

	public static bool HasKey(string key)
	{
		#if(SECURED)
		string hashedKey = GenerateMD5 (key);
		bool hasKey = PlayerPrefs.HasKey (hashedKey);
		return hasKey;
		#else
		bool hasKey = PlayerPrefs.HasKey (key);
		return hasKey;
		#endif
	}

	#if (SECURED)
	// some fast xor encryptor
	public static int key = 129;
	public static string xorEncryptDecrypt(string text){
		StringBuilder inSb = new StringBuilder (text);
		StringBuilder outSB = new StringBuilder (text.Length);
		char c;
		for(int i = 0; i<text.Length; i++){
			c = inSb [i];
			c = (char)(c ^ key);
			outSB.Append (c);
		}
		return outSB.ToString ();
	}

    /// <summary>
    /// 지정된 텍스트의 MD5 해시를 생성합니다.
    /// 경고: 비밀번호 저장방식으로는 안전하지 않습니다.
    /// </summary>
    /// <returns>MD5 해시 문자열</returns>
    /// <param name="text">The text to hash</param>
    static string GenerateMD5(string text)
	{
        /// 씨샵에서 제공하는 MD5 해시 알고리즘 타입의 해시 객체를 할당한다.
		var md5 = MD5.Create();

		byte[] inputBytes = Encoding.UTF8.GetBytes(text);
		byte[] hash = md5.ComputeHash(inputBytes);

		// step 2, convert byte array to hex string
		var sb = new StringBuilder();
		for (int i = 0; i < hash.Length; i++)
		{
			sb.Append(hash[i].ToString("X2"));
		}
		return sb.ToString();
	}

	#endif
}