#region ★ Introduction

/// <summary>Registry Class Introduction</summary>
///  
/// ★ Information
///  - Author : Pinelia Luna
///  - Update : 2016.02.26
///  - Version : 1.2.0
///  - Contact : pinel0102@gmail.com
///  
/// ★ Summary
///  - FileIO 클래스 사용을 보조하는 클래스입니다.
///  - 파일의 텍스트 내용을 담은 wholeString 을 new Registry() 에서 관리합니다.
///  - GetValue(), SetValue() 에 string, int, float 타입의 value 사용이 가능합니다.
///  - 자세한 사용 방법은 FileIO 클래스를 참조하면 됩니다.
///  - 이 클래스는 유니티 에서만 사용할 수 있습니다.
///  
/// ★ How to use 
///  - Registry reg = new Registry(); 를 통해 새로운 Registry 를 생성합니다.
///  - reg.MethodName(); 과 같은 방식으로 사용합니다.
///  
/// ★ Text-file Form
///  　#Section1
///  　　Key1 = Value1
///  　　Key2 = Value2
///  　　
///  　#Section2
///  　　Key1 = Value1
///  　　Key2 = Value2
///  　　
/// ★ Methods
///  - Constructor
///   - public Registry()								// 새로운 Registry 를 만듭니다.
///  
///  - Handle File
///   - public void LoadFileInDataPath()				// [persistentDataPath]/sourcePath 파일의 텍스트 내용을 Registry 에 저장합니다.
///   - public void LoadFileInAssets()					// [Assets]/sourcePath 파일의 텍스트 내용을 Registry 에 저장합니다.
///   - public void LoadFileInResources()				// [Resources]/sourcePath 파일의 텍스트 내용을 Registry 에 저장합니다.
///   - public void LoadFileInCustomPath()				// sourcePath 파일의 텍스트 내용을 Registry 에 저장합니다.
///   - public void SaveFileInDataPath()				// Registry 의 내용을 [persistentDataPath]/targetPath 파일에 저장합니다.
///   - public void SaveFileInAssets()					// Registry 의 내용을 [Assets]/targetPath 파일에 저장합니다.
///   - public void SaveFileInResources()				// Registry 의 내용을 [Resources]/targetPath 파일에 저장합니다.
///   - public void SaveFileInCustomPath()				// Registry 의 내용을 targetPath 파일에 저장합니다.
///   - public void BackupFile()						// [DataPath]/[bak] 폴더에 [DataPath]/sourcePath 파일의 백업본을 저장합니다.
///  
///  - Handle Value
///   - public void GetValue()							// Registry 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 불러옵니다.
///   - public void SetValue()							// Registry 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 변경합니다.
///   - public [string, float, int, bool] GetValue()	// Registry 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 불러옵니다.
/// 
///  - Handle Form
///   - public void AddSection()						// Registry 의 하단에 #thisSection 을 추가합니다.
///   - public void DelSection()						// #thisSection 과 하단의 모든 key=value 를 삭제합니다.
///   - public void AddKeyValue()						// #thisSection 의 하단에 thisKey=newValue 형태의 라인을 추가합니다.
///   - public void DelKeyValue()						// #thisSection 의 하단에 thisKey=newValue 형태의 라인을 삭제합니다.
///   - public bool SectionIsExist()					// Registry 에 #thisSection 이 존재하는지 확인합니다.
///   - public bool KeyIsExist()						// #thisSection 의 하단에 thisKey 가 존재하는지 확인합니다.
///  

/// <summary>FileIO Class Introduction</summary>
/// 
/// ★ Information
///  - Author : Pinelia Luna
///  - Update : 2016.02.26
///  - Version : 1.2.0 beta
///  - Contact : pinel0102@gmail.com
/// 
/// ★ Summary
///  - 텍스트 파일의 입출력과 #section:key=value 형식을 사용할 수 있는 클래스입니다.
///  - 일반적으로 앱 외부에 존재하는 [DataPath] 폴더를 사용합니다.
///  - 에디터 등의 한정된 환경에서 앱 내부에 존재하는[Resources] 폴더를 사용할 수 있습니다.
///  - 파일 읽고 쓰기시 암호화/복호화를 할 수 있습니다.
///  - #section, key=value 의 형태로 이루어진 파일을 읽고 쓸 수 있습니다.
///  - 한 라인에 하나의 key = value 만 존재할 수 있습니다.
///  - 이 클래스는 유니티 에서만 사용할 수 있습니다.
/// 
/// ★ How to use
///  - FileIO.cs 파일을 프로젝트에 임포트 합니다.
///  - FileIO.MethodName(); 과 같은 방식으로 사용합니다.
///  
/// ★ Text-file Form
///  　#Section1
///  　　Key1 = Value1
///	 　　Key2 = Value2
///  
///  　#Section2
///  　　Key1 = Value1
///  　　Key2 = Value2
/// 
/// ★ Methods
///  - Handle File
///   - public void LoadFile()                 // [pathFlag]/sourcePath 파일의 텍스트 내용을 wholeString 에 저장합니다.
///   - public void SaveFile()                 // wholeString 의 내용을 [DataPath]/targetPath 파일에 저장합니다.
///   - public void BackupFile()               // [DataPath]/[bak] 폴더에 [DataPath]/sourcePath 파일의 백업본을 저장합니다.
///
///  - Handle Value
///   - public [void, string] GetValue()       // wholeString 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 불러옵니다.
///   - public [void, string] SetValue()       // wholeString 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 변경합니다.
///
///  - Handle Form
///   - public [void, string] AddSection()     // wholeString 의 하단에 #thisSection 을 추가합니다.
///   - public [void, string] DelSection()     // #thisSection 과 하단의 모든 key=value 를 삭제합니다.
///   - public [void, string] AddKeyValue()    // #thisSection 의 하단에 thisKey=newValue 형태의 라인을 추가합니다.
///   - public [void, string] DelKeyValue()    // #thisSection 의 하단에 thisKey=newValue 형태의 라인을 삭제합니다.
///   - public bool SectionIsExist()           // wholeString 에 #thisSection 이 존재하는지 확인합니다.
///   - public bool KeyIsExist()               // #thisSection 의 하단에 thisKey 가 존재하는지 확인합니다.
///
///  - Utility
///   - private void AnalyzeFolder()           // targetPath 에 폴더가 포함되어 있을 경우 [DataPath] 에 해당 폴더를 생성합니다.
///   - private bool IsNullOrWhiteSpace()      // string 이 null 또는 공백만으로 되어있는지 검사합니다.
///   - private string ReadFromResources()     // [Resources]/referencePath 파일의 텍스트 내용을 string으로 불러옵니다.
///   - private string EncryptString()         // ClearText 의 내용을 암호화한 string 을 리턴합니다.
///   - private string DecryptString()         // EncryptedText 의 내용을 복호화한 string 을 리턴합니다.
/// 

#endregion ★ Introduction


#region ★ Namespaces

using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

#endregion ★ Namespaces


#region ★ Registry Class

/// <summary>
/// <para>──────────────────────────────</para>
/// FileIO 클래스 사용을 보조하는 클래스입니다.
/// <para>파일의 텍스트 내용을 담은 wholeString 을 new Registry() 에서 관리합니다.</para>
/// <para>GetValue(), SetValue() 에 string, int, float 타입의 value 사용이 가능합니다.</para>
/// <para>자세한 사용 방법은 <see cref="FileIO"/> 클래스를 참조하면 됩니다.</para>
/// <para>이 클래스는 유니티 에서만 사용할 수 있습니다.</para>
/// <para>──────────────────────────────</para>
/// </summary>
public class Registry
{
	#region ★ Variables

	/// <summary>
	/// Registry 의 텍스트 내용입니다. (private-working).
	/// </summary>
	private string reg = null;

	/// <summary>
	/// Registry 의 텍스트 내용입니다. (read-only).
	/// </summary>
	public string wholeString { get { return reg; } }

	#endregion ★ Variables


	#region ★ Methods (Constructor)

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// 새로운 Registry 를 만듭니다.
	/// <para>sourcePath 를 입력하면 파일의 텍스트 내용을 자동으로 새로운 Registry 에 저장합니다.</para>
	/// <para>sourcePath 파일이 없으면 [Resources]/referencePath 파일을 sourcePath 로 복사 후 재시도합니다.</para>
	/// <para>지원하는 referencePath 의 확장자는 txt/html/htm/xml/bytes/json/csv/yaml/fnt 입니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <example>
	///		Registry reg = new Registry(sourchPath); → LoadFileInDataPath(sourchPath)
	///		Registry reg = new Registry(sourchPath, referencePath); → LoadFileInDataPath(sourchPath, referencePath)
	///		Registry reg = new Registry(sourcePath, null, false); → LoadFileInResources(sourcePath)
	/// </example>
	/// <param name="sourcePath">불러을 파일의 경로.</param>
	/// <param name="referencePath">sourcePath 파일이 없을 경우 참조할 txt 파일의 [Resources] 경로.</param>
	/// <param name="pathFlag">불러올 파일의 위치 지정. (0: Application.persistentDataPath / 1: [Assets] / 2: [Resources] / 3: customPath)</param>
	public Registry(string sourcePath = null, string referencePath = null, int pathFlag = 0)
	{
		if (!string.IsNullOrEmpty(sourcePath))
		{
			if (pathFlag == 0)
			{
				LoadFileInDataPath(sourcePath, referencePath);
			}
			else if (pathFlag == 1)
			{
				LoadFileInAssets(sourcePath, referencePath);
			}
			else if (pathFlag == 2)
			{
				LoadFileInResources(sourcePath);
			}
			else if (pathFlag == 3)
			{
				LoadFileInCustomPath(sourcePath, referencePath);
			}
		}
	}

	#endregion ★ Methods (Constructor)


	#region ★ Methods (Handle File)

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// [persistentDataPath]/sourcePath 파일의 텍스트 내용을 Registry 에 저장합니다.
	/// <para>sourcePath 파일이 없으면 [Resources]/referencePath 파일을 sourcePath 로 복사 후 재시도합니다.</para>
	/// <para>지원하는 referencePath 의 확장자는 txt/html/htm/xml/bytes/json/csv/yaml/fnt 입니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="sourcePath">불러을 파일의 경로.</param>
	/// <param name="referencePath">sourcePath 파일이 없을 경우 참조할 txt 파일의 [Resources] 경로.</param>
	public void LoadFileInDataPath(string sourcePath, string referencePath = null)
	{
		FileIO.LoadFile(ref reg, sourcePath, referencePath, 0);
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// [Assets]/sourcePath 파일의 텍스트 내용을 Registry 에 저장합니다.
	/// <para>sourcePath 파일이 없으면 [Resources]/referencePath 파일을 sourcePath 로 복사 후 재시도합니다.</para>
	/// <para>지원하는 referencePath 의 확장자는 txt/html/htm/xml/bytes/json/csv/yaml/fnt 입니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="sourcePath">불러을 파일의 경로.</param>
	/// <param name="referencePath">sourcePath 파일이 없을 경우 참조할 txt 파일의 [Resources] 경로.</param>
	public void LoadFileInAssets(string sourcePath, string referencePath = null)
	{
		FileIO.LoadFile(ref reg, sourcePath, referencePath, 1);
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// [Resources]/sourcePath 파일의 텍스트 내용을 Registry 에 저장합니다.
	/// <para>지원하는 sourcePath 의 확장자는 txt/html/htm/xml/bytes/json/csv/yaml/fnt 입니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="sourcePath">불러을 파일의 경로.</param>
	public void LoadFileInResources(string sourcePath)
	{
		FileIO.LoadFile(ref reg, sourcePath, null, 2);
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// sourcePath 파일의 텍스트 내용을 Registry 에 저장합니다.
	/// <para>sourcePath 파일이 없으면 [Resources]/referencePath 파일을 sourcePath 로 복사 후 재시도합니다.</para>
	/// <para>지원하는 referencePath 의 확장자는 txt/html/htm/xml/bytes/json/csv/yaml/fnt 입니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="sourcePath">불러을 파일의 경로.</param>
	/// <param name="referencePath">sourcePath 파일이 없을 경우 참조할 txt 파일의 [Resources] 경로.</param>
	public void LoadFileInCustomPath(string sourcePath, string referencePath = null)
	{
		FileIO.LoadFile(ref reg, sourcePath, referencePath, 3);
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// Registry 의 내용을 [persistentDataPath]/targetPath 파일에 저장합니다. 
	/// <para>targetPath 파일의 기존 내용은 모두 지워집니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="targetPath">저장할 파일의 경로.</param>
	public void SaveFileInDataPath(string targetPath)
	{
		FileIO.SaveFile(reg, targetPath, 0);
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// Registry 의 내용을 [Assets]/targetPath 파일에 저장합니다. 
	/// <para>targetPath 파일의 기존 내용은 모두 지워집니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="targetPath">저장할 파일의 경로.</param>
	public void SaveFileInAssets(string targetPath)
	{
		FileIO.SaveFile(reg, targetPath, 1);
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// Registry 의 내용을 [Resources]/targetPath 파일에 저장합니다.
	/// <para>targetPath 파일의 기존 내용은 모두 지워집니다.</para>
	/// <para>유니티 에디터 등의 특수한 상황에서 사용할 수 있습니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="targetPath">저장할 파일의 경로.</param>
	public void SaveFileInResources(string targetPath)
	{
		FileIO.SaveFile(reg, targetPath, 2);
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// Registry 의 내용을 targetPath 파일에 저장합니다.
	/// <para>targetPath 파일의 기존 내용은 모두 지워집니다.</para>	
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="targetPath">저장할 파일의 경로.</param>
	public void SaveFileInCustomPath(string targetPath)
	{
		FileIO.SaveFile(reg, targetPath, 3);
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// [DataPath]/[bak] 폴더에 [DataPath]/sourcePath 파일의 백업본을 저장합니다.
	/// <para>[DataPath]는 디바이스가 제공하는 읽기/쓰기용 고유 경로입니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="sourcePath">백업할 파일의 경로.</param>
	public void BackupFIle(string sourcePath)
	{
		FileIO.BackupFile(sourcePath);
	}

	#endregion ★ Methods (Handle File)


	#region ★ Methods (Handle Value)

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// Registry 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 불러옵니다.
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">찾을 key.</param>
	/// <param name="newValue">value 를 저장할 string. (ref)</param>
	public void GetValue(string thisSection, string thisKey, ref string newValue)
	{
		FileIO.GetValue(reg, thisSection, thisKey, ref newValue);
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// Registry 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 불러옵니다.
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">찾을 key.</param>
	/// <param name="newValue">value 를 저장할 float. (ref)</param>
	public void GetValue(string thisSection, string thisKey, ref float newValue)
	{
		string strValue = null;
		FileIO.GetValue(reg, thisSection, thisKey, ref strValue);
		newValue = float.Parse(strValue);
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// Registry 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 불러옵니다.
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">찾을 key.</param>
	/// <param name="newValue">value 를 저장할 int. (ref)</param>
	public void GetValue(string thisSection, string thisKey, ref int newValue)
	{
		string strValue = null;
		FileIO.GetValue(reg, thisSection, thisKey, ref strValue);
		newValue = int.Parse(strValue);
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// Registry 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 불러옵니다.
	/// <para>(true/false), (1/0) 을 인식하며 인식하지 못하면 false 를 불러옵니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">찾을 key.</param>
	/// <param name="newValue">value 를 저장할 bool. (ref)</param>
	public void GetValue(string thisSection, string thisKey, ref bool newValue)
	{
		string strValue = null;
		FileIO.GetValue(reg, thisSection, thisKey, ref strValue);

		if (strValue != null)
		{
			try
			{
				newValue = bool.Parse(strValue);
			}
			catch
			{
				if (strValue.Equals("1"))
				{
					newValue = true;
				}
				else
				{
					newValue = false;
				}
			}
		}
		else
		{
			newValue = false;
		}
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// Registry 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 변경합니다.
	/// <para>#thisSection 또는 thisKey 가 없으면 새롭게 생성하며 newValue 를 적용합니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">찾을 key.</param>
	/// <param name="newValue">새로운 value. (string, float, int, bool)</param>
	public void SetValue(string thisSection, string thisKey, string newValue)
	{
		FileIO.SetValue(ref reg, thisSection, thisKey, newValue);
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// Registry 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 변경합니다.
	/// <para>#thisSection 또는 thisKey 가 없으면 새롭게 생성하며 newValue 를 적용합니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">찾을 key.</param>
	/// <param name="newValue">새로운 value. (string, float, int, bool)</param>
	public void SetValue(string thisSection, string thisKey, float newValue)
	{
		FileIO.SetValue(ref reg, thisSection, thisKey, newValue.ToString());
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// Registry 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 변경합니다.
	/// <para>#thisSection 또는 thisKey 가 없으면 새롭게 생성하며 newValue 를 적용합니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">찾을 key.</param>
	/// <param name="newValue">새로운 value. (string, float, int, bool)</param>
	public void SetValue(string thisSection, string thisKey, int newValue)
	{
		FileIO.SetValue(ref reg, thisSection, thisKey, newValue.ToString());
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// Registry 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 변경합니다.
	/// <para>#thisSection 또는 thisKey 가 없으면 새롭게 생성하며 newValue 를 적용합니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">찾을 key.</param>
	/// <param name="newValue">새로운 value. (string, float, int, bool)</param>
	public void SetValue(string thisSection, string thisKey, bool newValue)
	{
		FileIO.SetValue(ref reg, thisSection, thisKey, newValue.ToString());
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// Registry 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 불러옵니다.
	/// <para>→ #thisSection 의 하단에 있는 thisKey 의 value. (string)</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">찾을 key.</param>
	/// <returns>→ #thisSection 의 하단에 있는 thisKey 의 value. (string)</returns>
	public string GetValue(string thisSection, string thisKey)
	{
		return FileIO.GetValue(reg, thisSection, thisKey);
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// Registry 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 불러옵니다.
	/// <para>→ #thisSection 의 하단에 있는 thisKey 의 value. (float)</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">찾을 key.</param>
	/// <returns>→ #thisSection 의 하단에 있는 thisKey 의 value. (float)</returns>
	public float GetValueFloat(string thisSection, string thisKey)
	{
		return float.Parse(FileIO.GetValue(reg, thisSection, thisKey));
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// Registry 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 불러옵니다.
	/// <para>→ #thisSection 의 하단에 있는 thisKey 의 value. (int)</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">찾을 key.</param>
	/// <returns>→ #thisSection 의 하단에 있는 thisKey 의 value. (int)</returns>
	public int GetValueInt(string thisSection, string thisKey)
	{
		return int.Parse(FileIO.GetValue(reg, thisSection, thisKey));
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// Registry 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 불러옵니다.
	/// <para>(true/false), (1/0) 을 인식하며 인식하지 못하면 false 를 불러옵니다.</para>
	/// <para>→ #thisSection 의 하단에 있는 thisKey 의 value. (bool)</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">찾을 key.</param>
	/// <returns>→ #thisSection 의 하단에 있는 thisKey 의 value. (bool)</returns>
	public bool GetValueBool(string thisSection, string thisKey)
	{
		string strValue = null;
		strValue = FileIO.GetValue(reg, thisSection, thisKey);

		if (strValue != null)
		{
			try
			{
				return bool.Parse(strValue);
			}
			catch
			{
				if (strValue.Equals("1"))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		else
		{
			return false;
		}
	}

	#endregion ★ Methods (Handle Value)


	#region ★ Methods (Handle Form)

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// Registry 의 하단에 #thisSection 을 추가합니다.
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="thisSection">추가할 section.</param>
	public void AddSection(string thisSection)
	{
		FileIO.AddSection(ref reg, thisSection);
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// #thisSection 과 하단의 모든 key=value 를 삭제합니다.
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="thisSection">삭제할 section.</param>
	public void DelSection(string thisSection)
	{
		FileIO.DelSection(ref reg, thisSection);
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// #thisSection 의 하단에 thisKey=newValue 형태의 라인을 추가합니다.
	/// <para>#thisSection 이 없으면 wholeString 의 하단에 #thisSection 을 생성합니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">추가할 key.</param>
	/// <param name="newValue">추가할 value.</param>
	public void AddKeyValue(string thisSection, string thisKey, string newValue)
	{
		FileIO.AddKeyValue(ref reg, thisSection, thisKey, newValue);
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// #thisSection 의 하단에 thisKey=newValue 형태의 라인을 삭제합니다.
	/// <para>#thisSection 의 하단에 key가 모두 삭제되어도 #thisSection 이 삭제되지는 않습니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">삭제할 key.</param>
	public void DelKeyValue(string thisSection, string thisKey)
	{
		FileIO.DelKeyValue(ref reg, thisSection, thisKey);
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// Registry 에 #thisSection 이 존재하는지 확인합니다.
	/// <para>──────────────────────────────</para>
	/// <para>　→ #thisSection 존재 여부.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="thisSection">확인할 section.</param>
	/// <returns>→ #thisSection 존재 여부.</returns>
	public bool SectionIsExist(string thisSection)
	{
		return FileIO.SectionIsExist(reg, thisSection);
	}

	/// <summary>
	/// <para>──────────────────────────────</para>
	/// #thisSection 의 하단에 thisKey 가 존재하는지 확인합니다.
	/// <para>──────────────────────────────</para>
	/// <para>　→ thisKey 존재 여부.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="thisSection">확인할 section.</param>
	/// <param name="thisKey">확인할 key.</param>
	/// <returns>→ thisKey 존재 여부.</returns>
	public bool KeyIsExist(string thisSection, string thisKey)
	{
		return FileIO.KeyIsExist(reg, thisSection, thisKey);
	}

	#endregion ★ Methods (Handle Form)
}

#endregion ★ Registry Class


#region ★ FileIO Class

/// <summary>
/// <para>──────────────────────────────</para>
/// 텍스트 파일의 입출력과 #section:key=value 형식을 사용할 수 있는 클래스입니다.
/// <para>일반적으로 앱 외부에 존재하는 [DataPath] 폴더를 사용합니다.</para>
/// <para>에디터 등의 한정된 환경에서 앱 내부에 존재하는 [Resources] 폴더를 사용할 수 있습니다.</para>
/// <para>파일 읽고 쓰기시 암호화/복호화를 할 수 있습니다.</para>
/// <para>#section, key=value 의 형태로 이루어진 파일을 읽고 쓸 수 있습니다.</para>
/// <para>한 라인에 하나의 key=value 만 존재할 수 있습니다.</para>
/// <para>이 클래스는 유니티 에서만 사용할 수 있습니다.</para>
/// <para>──────────────────────────────</para>
/// </summary>
public static class FileIO
{
	#region ★ Variables


	/// <summary>
	/// 암호화/복호화 여부.
	/// </summary>
	private static bool isEncrypt = false;

	/// <summary>
	/// <see cref="SetValue(ref string, string, string, string)"/> 실행시 #section 이나 key 가 없을 때 자동으로 추가합니다.
	/// </summary>
	private static bool useAutoAddingMode = true;

	/// <summary>
	/// 복구 모드 사용 여부.
	/// <para>이 값이 true 일 경우 <see cref="BackupFile(string)"/> 실행시 sourcePath 파일에 이상이 있으면 백업본으로부터 복구를 시도합니다.</para>
	/// <para>해당 옵션을 사용하려면 sourcePath 파일 안에 <see cref="RecoveryModeCheck"/> 와 동일한 #section - key=value가 있어야 합니다.</para>
	/// </summary>
	private static bool useRecoveryMode = false;

	/// <summary>
	/// [Resources] 폴더에서 로드할 수 있는 파일 형식.
	/// <para>{ ".txt", ".html", ".htm", ".xml", ".bytes", ".json", ".csv", ".yaml", ".fnt" }</para>
	/// </summary>
	private static string[] SupportExtensions = new string[] { ".txt", ".html", ".htm", ".xml", ".bytes", ".json", ".csv", ".yaml", ".fnt" };

	/// <summary>
	/// 백업전 파일 검사를 위한 #section - key=value의 조합.
	/// <para>sourcePath 파일 안에 동일한 section, key, value가 있어야 합니다.</para>
	/// </summary>
	private static string[] RecoveryModeCheck = new string[] { "CRCDATA", "crcdata", "CRC1234" };

	/// <summary>
	/// section 을 시작하는 기호. 
	/// <para>한 라인에 하나의 기호만 존재할 수 있습니다.</para>
	/// <para>{ "#" }</para>
	/// </summary>
	private static string[] SectionSeperator = new string[] { "#" };

	/// <summary>
	/// key 와 value 를 구분하는 기호. 
	/// <para>한 라인에 복수의 기호가 존재할 수 있습니다. </para>
	/// <para>{ "=", " ", "\t" }</para>
	/// </summary>
	private static string[] KeyValueSeperator = new string[] { "=", " ", "\t" };

	/// <summary>
	/// 줄바꿈 기호. 
	/// <para>한 라인에 하나의 기호만 존재할 수 있습니다. </para>
	/// <para>{ "\r\n", "\n" }</para>
	/// </summary>
	private static string[] LineFeedSeperator = new string[] { "\r\n", "\n" };

	/// <summary>
	/// 암호화/복호화에 사용되는 키 1. (16)
	/// </summary>
	private static string KEY_1 = "ryojvlzmdalyglrj";

	/// <summary>
	/// 암호화/복호화에 사용되는 키 2. (32)
	/// </summary>
	private static string KEY_2 = "hcxilkqbbhczfeultgbskdmaunivmfuo";

	/// <summary>
	/// 존재하지 않는 #Section - Key 조합에 <see cref="GetValue(string, string, string, ref string)"/> 를 시도했을 때 출력.
	/// </summary>
	public static string NOT_FOUND_KEY = "not found key";


	#endregion ★ Variables


	#region ★ Methods (Handle File)


	/// <summary>
	/// <para>──────────────────────────────</para>
	/// [pathFlag]/sourcePath 파일의 텍스트 내용을 wholeString 에 저장합니다. 	
	/// <para>sourcePath 파일이 없으면 [Resources]/referencePath 파일을 sourcePath 로 복사 후 재시도합니다. </para>
	/// <para>지원하는 referencePath 의 확장자는 txt/html/htm/xml/bytes/json/csv/yaml/fnt 입니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <example>
	///		string wholeString = null;
	///		LoadFile(ref wholeString, "Save/UserData.txt", "Home/UserData.txt");
	///		
	///		[pathFlag]/Save/UserData.txt 파일이 있으면 내용을 불러옵니다.
	/// 	[pathFlag]/Save/UserData.txt 파일이 없으면 [Resources]/Home/UserData.txt 를 [pathFlag]/Save/UserData.txt 로 복사하고 내용을 불러옵니다.
	/// </example>
	/// <param name="wholeString">파일의 내용을 저장할 string. (ref)</param>
	/// <param name="sourcePath">불러을 파일의 경로.</param>
	/// <param name="referencePath">sourcePath 파일이 없을 경우 참조할 txt 파일의 [Resources] 경로.</param>
	/// <param name="pathFlag">불러올 파일의 위치 지정. (0: Application.persistentDataPath / 1: [Assets] / 2: [Resources] / 3: customPath)</param>
	public static void LoadFile(ref string wholeString, string sourcePath, string referencePath = null, int pathFlag = 0)
	{
		try
		{
			if (IsNullOrWhiteSpace(sourcePath))
			{
				Debug.LogWarning(CodeManager.GetMethodName() + "sourcePath is empty (" + sourcePath + ")");
				return;
			}

			// sourcePath 는 앞에 "/"가 있어야 함.
			if (!sourcePath.StartsWith("/"))
				sourcePath = "/" + sourcePath;

			int error = 0;

			RESTART:

			FileInfo theSourceFile = null;
			StreamReader sr = null;
			string newData = null;

			if (pathFlag == 0)
			{
				// [PersistentDataPath]/sourcePath 파일을 로드.

				Debug.Log(CodeManager.GetMethodName() + "[persistentDataPath]" + sourcePath);

				theSourceFile = new FileInfo(Application.persistentDataPath + sourcePath);

				if (theSourceFile != null && theSourceFile.Exists)
				{
					// 강제 갱신.
					//File.Delete(Application.persistentDataPath + sourcePath);
					//goto RESTART;

					sr = theSourceFile.OpenText();
				}
				else if (!IsNullOrWhiteSpace(referencePath))
				{
					// [Resources]/referencePath 파일을 로드.

					Debug.Log(CodeManager.GetMethodName() + "Create File : [persistentDataPath]" + sourcePath);

					AnalyzeFolder(sourcePath);

					//파일 생성
					FileStream file = File.Create(Application.persistentDataPath + sourcePath);
					file.Close();

					//디폴트 파일 읽기 (Read-only)
					newData = ReadFromResources(referencePath);

					//파일 쓰기
					SaveFile(newData, sourcePath, 0);

					error++;

					if (error == 1) goto RESTART;
				}
				else
				{
					Debug.LogError(CodeManager.GetMethodName() + "sourcePath == NULL && referencePath == NULL");
				}

				newData = DecryptString(sr.ReadToEnd());

				sr.Close();

				sourcePath = Application.persistentDataPath + sourcePath;
			}
			else if (pathFlag == 1)
			{
				// [DataPath]/sourcePath 파일을 로드.

				Debug.Log(CodeManager.GetMethodName() + "[Assets]" + sourcePath);

				theSourceFile = new FileInfo(Application.dataPath + sourcePath);

				if (theSourceFile != null && theSourceFile.Exists)
				{
					// 강제 갱신.
					//File.Delete(Application.persistentDataPath + sourcePath);
					//goto RESTART;

					sr = theSourceFile.OpenText();
				}
				else if (!IsNullOrWhiteSpace(referencePath))
				{
					// [Resources]/referencePath 파일을 로드.

					Debug.Log(CodeManager.GetMethodName() + "Create File : [Assets]" + sourcePath);

					AnalyzeFolder(sourcePath);

					//파일 생성
					FileStream file = File.Create(Application.dataPath + sourcePath);
					file.Close();

					//디폴트 파일 읽기 (Read-only)
					newData = ReadFromResources(referencePath);

					//파일 쓰기
					SaveFile(newData, sourcePath, 0);

					error++;

					if (error == 1) goto RESTART;
				}
				else
				{
					Debug.LogError(CodeManager.GetMethodName() + "sourcePath == NULL && referencePath == NULL");
				}

				newData = DecryptString(sr.ReadToEnd());

				sr.Close();

				sourcePath = Application.dataPath + sourcePath;
			}
			else if (pathFlag == 2)
			{
				// [Resources]/sourcePath 파일을 로드.

				Debug.Log(CodeManager.GetMethodName() + "[Resources]" + sourcePath);

				newData = ReadFromResources(sourcePath);
			}
			else if (pathFlag == 3)
			{
				// 직접 입력한 경로의 sourcePath 파일을 로드.

				// sourcePath 는 앞에 "/"가 없어야 함.
				if (sourcePath.StartsWith("/"))
					sourcePath = sourcePath.Substring(1);

				Debug.Log(CodeManager.GetMethodName() + sourcePath);

				theSourceFile = new FileInfo(sourcePath);

				if (theSourceFile != null && theSourceFile.Exists)
				{
					sr = theSourceFile.OpenText();
				}
				else
				{
					Debug.LogError(CodeManager.GetMethodName() + "sourcePath == NULL");
				}

				newData = DecryptString(sr.ReadToEnd());

				sr.Close();

			}

			if (IsNullOrWhiteSpace(newData))
			{
				Debug.LogWarning(CodeManager.GetMethodName() + "The File is Empty : " + sourcePath);
			}
			else
			{
				//Debug.Log(CodeManager.GetMethodName() + "Finished : " + sourcePath);
			}

			wholeString = newData;

			return;

		}
		catch (Exception e)
		{
			Debug.LogError(CodeManager.GetMethodName() + "Error : " + sourcePath + " : " + e);

			return;
		}
	}


	/// <summary> 
	/// <para>──────────────────────────────</para>
	/// wholeString 의 내용을 [pathFlag]/targetPath 파일에 저장합니다.
	/// <para>targetPath 파일의 기존 내용은 모두 지워집니다.</para>
	/// <para>──────────────────────────────</para>
	///	<para>　→ wholeString 의 내용이 저장된 targetPath 파일.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>	
	/// <example>	
	///		SaveFile(wholeString, "Save/UserData.txt");
	///		
	///		[pathFlag]/Save/UserData.txt 파일이 있으면 파일의 내용을 모두 지우고 wholeString 의 내용을 저장합니다.
	///		[pathFlag]/Save/UserData.txt 파일이 없으면 [pathFlag]/Save/UserData.txt 파일을 생성하고 wholeString 의 내용을 저장합니다.
	/// </example>
	/// <param name="wholeString">파일에 저장할 전체 string.</param>
	/// <param name="targetPath">저장할 파일의 경로.</param>
	/// <param name="pathFlag">파일이 저장될 위치 지정. (0: Application.persistentDataPath / 1: [Assets] / 2: [Resources] / 3: customPath)</param>
	public static void SaveFile(string wholeString, string targetPath, int pathFlag = 0)
	{
		try
		{
			if (IsNullOrWhiteSpace(targetPath))
			{
				Debug.LogWarning(CodeManager.GetMethodName() + "targetPath is empty (" + targetPath + ")");
				return;
			}

			// targetPath 는 앞에 "/"가 있어야 함.
			if (!targetPath.StartsWith("/"))
				targetPath = "/" + targetPath;

			targetPath = targetPath.Replace("\\", "/");

			if (pathFlag == 0)
			{
				AnalyzeFolder(targetPath);

				// persistentDataPath 폴더에 저장.
				Debug.Log(CodeManager.GetMethodName() + "[persistentDataPath]" + targetPath);

				targetPath = Application.persistentDataPath + targetPath;
				StreamWriter sw = new StreamWriter(targetPath);

				sw.Write(EncryptString(wholeString)); // 줄단위로 파일에 입력.
				sw.Flush();
				sw.Close();
			}
			else if (pathFlag == 1)
			{
				AnalyzeFolder(targetPath, pathFlag);

				// dataPath 폴더에 저장. (유니티 에디터의 경우 Assets 폴더).
				Debug.Log(CodeManager.GetMethodName() + "[Assets]" + targetPath);

				targetPath = Application.dataPath + targetPath;
				File.WriteAllText(targetPath, wholeString);

#if UNITY_EDITOR
				//Debug.Log("[FileIO] SaveFile : Unity Editor : Refreshing Files");
				UnityEditor.AssetDatabase.Refresh();
#endif
			}
			else if (pathFlag == 2)
			{
				// Resources 폴더에 저장.
				Debug.Log(CodeManager.GetMethodName() + "[Resources] " + targetPath);

				targetPath = Application.dataPath + "/Resources" + targetPath;
				File.WriteAllText(targetPath, wholeString);

#if UNITY_EDITOR
				//Debug.Log(CodeManager.GetMethodName() + "Unity Editor : Refreshing Files");
				UnityEditor.AssetDatabase.Refresh();
#endif
			}
			else if (pathFlag == 3)
			{
				// 직접 입력한 경로에 저장.

				// sourcePath 는 앞에 "/"가 없어야 함.
				if (targetPath.StartsWith("/"))
					targetPath = targetPath.Substring(1);

				Debug.Log(CodeManager.GetMethodName() + targetPath);

				File.WriteAllText(targetPath, wholeString);

#if UNITY_EDITOR
				//Debug.Log(CodeManager.GetMethodName() + "Unity Editor : Refreshing Files");
				UnityEditor.AssetDatabase.Refresh();
#endif
			}

			//Debug.Log(CodeManager.GetMethodName() + "Finished : " + targetPath);
		}
		catch (Exception e)
		{
			Debug.LogError(CodeManager.GetMethodName() + "Error : " + targetPath + " : " + e);
		}
	}


	/// <summary>
	/// <para>──────────────────────────────</para>
	/// [DataPath]/[bak] 폴더에 [DataPath]/sourcePath 파일의 백업본을 저장합니다.
	/// <para>[DataPath]는 디바이스가 제공하는 읽기/쓰기용 고유 경로입니다.</para>
	/// <para>──────────────────────────────</para>
	///	<para>　→ sourcePath 의 내용이 저장된 sourcePath.bak 파일.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>	
	/// <example>	
	///		BackupFile("Save/UserData.txt");
	///		
	///		[DataPath]/Save/UserData.txt 파일의 복사본을 [DataPath]/[bak]/Save/UserData.txt.bak 으로 생성합니다.
	/// </example>
	/// <param name="sourcePath">백업할 파일의 경로.</param>
	public static void BackupFile(string sourcePath)
	{
		try
		{
			if (IsNullOrWhiteSpace(sourcePath))
			{
				Debug.LogWarning(CodeManager.GetMethodName() + "sourcePath is empty (" + sourcePath + ")");
				return;
			}

			Debug.Log(CodeManager.GetMethodName() + "Backup : " + sourcePath);

			string backupFolder = "/bak";
			string backupExt = ".bak";

			if (!sourcePath.StartsWith("/"))
				sourcePath = "/" + sourcePath;

			StringBuilder backupPath = new StringBuilder();
			backupPath.Append(backupFolder);
			backupPath.Append(sourcePath);
			backupPath.Append(backupExt);

			int restart_count = 0;

			string wholeString = null;


			RESTART:

			int first_load_error = 0;

			// 복구 모드 적용시 사용.
			if (useRecoveryMode)
			{
				wholeString = null;// = LoadFile(sourcePath, null, true);
				LoadFile(ref wholeString, sourcePath, null, 0);

				// 임의의 키 값을 불러 원하는 값과 비교합니다.
				// sourcePath 파일 안에 체크를 위한 section, key, value가 있어야 합니다.
				if (GetValue(wholeString, RecoveryModeCheck[0], RecoveryModeCheck[1]) != RecoveryModeCheck[2])
					first_load_error++;
			}

			if (first_load_error == 0) // 문제가 없으면 백업본 생성.
			{
				if (!Directory.Exists(Application.persistentDataPath + backupFolder))
				{
					Debug.Log(CodeManager.GetMethodName() + "Create Directory for Backup : " + backupPath.ToString());
					AnalyzeFolder(backupFolder + sourcePath);
				}

				File.Copy(Application.persistentDataPath + sourcePath, Application.persistentDataPath + backupPath.ToString(), true);

				FileInfo theBackupFile = null;
				theBackupFile = new FileInfo(Application.persistentDataPath + backupPath.ToString());

				if (theBackupFile != null && theBackupFile.Exists)
				{ Debug.Log(CodeManager.GetMethodName() + "Finished : " + sourcePath + " → " + backupPath.ToString()); }
				else
				{ Debug.LogError(CodeManager.GetMethodName() + "Failed : " + restart_count); }
			}
			else if (restart_count == 0) // 문제가 있으면 백업본으로 복구. 
			{
				backupPath.Remove(0, backupPath.Length);
				backupPath.Append(sourcePath);

				StringBuilder tempstr = new StringBuilder();
				tempstr.Append(backupFolder);
				tempstr.Append(sourcePath);
				tempstr.Append(backupExt);

				sourcePath = null;
				sourcePath = tempstr.ToString();

				restart_count++;

				Debug.LogWarning(CodeManager.GetMethodName() + "Recovery Start : " + sourcePath + " → " + backupPath.ToString());

				goto RESTART;
			}
			else
			{
				// 백업본에도 문제가 있다.
				Debug.LogError(CodeManager.GetMethodName() + "Failed : " + restart_count);

				//File.Delete(sourcePath);
				//File.Delete(backupPath.ToString());
			}

			backupPath = null;
			wholeString = null;
		}
		catch (Exception e)
		{
			Debug.LogError(CodeManager.GetMethodName() + "Failed : " + sourcePath + " : " + e);
		}

	}


	#endregion ★ Methods (Handle File)


	#region ★ Methods (Handle Value)


	/// <summary> 
	/// <para>──────────────────────────────</para>
	///		wholeString 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 불러옵니다.
	///	<para>──────────────────────────────</para>
	/// </summary>
	/// <example>
	/// 	#options
	/// 	　play_count = 100
	/// 	
	/// 	string playCount = "0";
	/// 	GetValue(wholeString, "options", "play_count", ref playCount);
	/// 	
	/// 	playCount : "100"
	/// </example>
	/// <param name="wholeString">value 를 불러올 전체 string.</param>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">찾을 key.</param>
	/// <param name="newValue">찾은 value 를 저장할 string. (ref)</param>
	public static void GetValue(string wholeString, string thisSection, string thisKey, ref string newValue)
	{
		int load_error = 0;

		try
		{
			if (IsNullOrWhiteSpace(thisSection))
			{
				Debug.LogWarning("[FileIO] GetValue : thisSection is empty (" + thisSection + ")");
				return;// null;
			}
			if (IsNullOrWhiteSpace(thisKey))
			{
				Debug.LogWarning("[FileIO] GetValue : thisKey is empty (" + thisKey + ")");
				return;// null;
			}

			bool found = false;

			string[] arr_section = wholeString.Split(SectionSeperator, StringSplitOptions.None);

			for (int i = 1; i < arr_section.GetLength(0); i++)
			{
				string[] arr_type = arr_section[i].Split(LineFeedSeperator, StringSplitOptions.None);

				if (arr_type.Length > 0)
				{
					if (arr_type[0].Trim().Equals(thisSection, StringComparison.OrdinalIgnoreCase))
					{
						for (int j = 1; j < arr_type.GetLength(0); j++)
						{
							string[] arr_value = arr_type[j].Split(KeyValueSeperator, StringSplitOptions.RemoveEmptyEntries);

							if (arr_value.Length > 0)
							{
								//Debug.Log("arr_value[0]: [" + arr_value[0] + "] / thisKey: [" + thisKey + "]");

								if (arr_value[0].Equals(thisKey, StringComparison.OrdinalIgnoreCase))
								{
									newValue = arr_value[1];
									found = true;

									break;
								}
							}
						}

						break;
					}
				}
			}

			//Debug.Log("newValue = " + newValue);			

			if (!found)
			{
				load_error++;
				Debug.LogError("[FileIO] GetValue : Failed to find the key (" + thisSection + ", " + thisKey + ") : " + load_error);
			}
		}
		catch (Exception e)
		{
			load_error++;
			Debug.LogError("[FileIO] GetValue : Get Value Failed (" + thisSection + ", " + thisKey + ") : " + load_error + " : " + e);
		}
	}


	/// <summary>
	/// <para>──────────────────────────────</para>
	/// wholeString 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 변경합니다.
	/// <para>#thisSection 또는 thisKey 가 없으면 새롭게 생성하며 newValue 를 적용합니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>		
	/// <example>
	///		#options
	///		　play_count = 100
	///		
	///		SetValue(ref wholeString, "options", "play_count", "101");
	///		
	///		#options
	///		　play_count = 101	
	/// </example>	
	/// <param name="wholeString">value 를 변경할 전체 string. (ref)</param>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">찾을 key.</param>
	/// <param name="newValue">새로운 value.</param>		
	public static void SetValue(ref string wholeString, string thisSection, string thisKey, string newValue)
	{
		int save_error = 0;

		try
		{
			if (IsNullOrWhiteSpace(thisSection))
			{
				Debug.LogWarning("[FileIO] SetValue : thisSection is empty (" + thisSection + ")");
				return;
			}
			if (IsNullOrWhiteSpace(thisKey))
			{
				Debug.LogWarning("[FileIO] SetValue : thisKey is empty (" + thisKey + ")");
				return;
			}
			if (IsNullOrWhiteSpace(newValue))
			{
				Debug.LogWarning("[FileIO] SetValue : newValue is empty (" + newValue + ")");
				return;
			}


			bool sectionFound = false;
			bool keyFound = false;

			string[] arr_section = wholeString.Split(SectionSeperator, StringSplitOptions.None);

			for (int i = 1; i < arr_section.GetLength(0); i++)
			{
				string[] arr_type = arr_section[i].Split(LineFeedSeperator, StringSplitOptions.None);

				if (arr_type.Length > 0)
				{
					if (arr_type[0].Trim().Equals(thisSection, StringComparison.OrdinalIgnoreCase))
					{
						sectionFound = true;

						for (int j = 1; j < arr_type.GetLength(0); j++)
						{
							string[] arr_value = arr_type[j].Split(KeyValueSeperator, StringSplitOptions.RemoveEmptyEntries);

							if (arr_value.Length > 0)
							{
								//Debug.Log("arr_value[0]: [" + arr_value[0] + "] / thisKey: [" + thisKey + "]");

								if (arr_value[0].Equals(thisKey, StringComparison.OrdinalIgnoreCase))
								{
									keyFound = true;

									arr_value[1] = newValue;
									arr_type[j] = string.Join(KeyValueSeperator[0], arr_value);
									arr_section[i] = string.Join(LineFeedSeperator[0], arr_type);

									wholeString = string.Join(SectionSeperator[0], arr_section);

									break;
								}
							}
						}

						break;
					}
				}
			}


			if (!sectionFound)
			{
				//section 이 없다.
				if (useAutoAddingMode)
				{
					Debug.Log("[FileIO] SetValue : Add Section and Key-Value (" + thisSection + ", " + thisKey + ", " + newValue + ")");
					AddKeyValue(ref wholeString, thisSection, thisKey, newValue);
				}
				else
				{
					save_error++;
					Debug.LogError("[FileIO] SetValue : Failed to find the section (" + thisSection + ") : " + save_error);
				}
			}
			else if (!keyFound)
			{
				//section은 있는데 key가 없다.
				if (useAutoAddingMode)
				{
					Debug.Log("[FileIO] SetValue : Add Key-Value (" + thisSection + ", " + thisKey + ", " + newValue + ")");
					AddKeyValue(ref wholeString, thisSection, thisKey, newValue);
				}
				else
				{
					save_error++;
					Debug.LogError("[FileIO] SetValue : Failed to find the key (" + thisSection + ", " + thisKey + ") : " + save_error);
				}
			}
		}
		catch (Exception e)
		{
			save_error++;
			Debug.LogError("[FileIO] SetValue : Set Value Failed (" + thisSection + ", " + thisKey + ") : " + save_error + " :" + e);
		}
	}


	/// <summary>
	/// <para>──────────────────────────────</para>
	///	wholeString 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 불러옵니다.
	///	<para>──────────────────────────────</para>
	///	<para>　→ #thisSection 의 하단에 있는 thisKey 의 value.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <example>
	///		#options
	///		　play_count = 100	
	///		
	///		string playCount = GetValue(wholeString, "options", "play_count");
	///		
	///		playCount : "100"
	///	</example>
	/// <param name="wholeString">value 를 불러올 전체 string.</param>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">찾을 key.</param>
	/// <returns>→ #thisSection 의 하단에 있는 thisKey 의 value.</returns>
	public static string GetValue(string wholeString, string thisSection, string thisKey)
	{
		string value = NOT_FOUND_KEY;
		GetValue(wholeString, thisSection, thisKey, ref value);
		return value;
	}


	/// <summary> 
	/// <para>──────────────────────────────</para>
	/// wholeString 에서 #thisSection 의 하단에 있는 thisKey 의 value 를 변경합니다.
	/// <para>#thisSection 또는 thisKey 가 없으면 새롭게 생성하며 newValue 를 적용합니다.</para>
	/// <para>──────────────────────────────</para>	
	///	<para>　→ #thisSection 의 하단에 있는 thisKey 의 value 가 반영된 전체 string.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>	
	/// <example>
	///		#options
	///		　play_count = 100
	///		
	///		wholeString = SetValue(wholeString, "options", "play_count", "101");
	///		
	///		#options
	///		　play_count = 101
	/// </example>	
	/// <param name="wholeString">value 를 변경할 전체 string.</param>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">찾을 key.</param>
	/// <param name="newValue">새로운 value.</param>	
	/// <returns>→ #thisSection 의 하단에 있는 thisKey 의 value 가 반영된 전체 string.</returns>
	public static string SetValue(string wholeString, string thisSection, string thisKey, string newValue)
	{
		SetValue(ref wholeString, thisSection, thisKey, newValue);
		return wholeString;
	}


	#endregion ★ Methods (Handle Value)


	#region ★ Methods (Handle Form)


	/// <summary>
	/// <para>──────────────────────────────</para>
	/// wholeString 의 하단에 #thisSection 을 추가합니다.
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <example>
	///		#options
	///		　play_count = 100
	/// 
	///		AddSection(ref wholeString, "userdata");
	/// 
	///		#options
	///		　play_count = 100
	///		#userdata
	/// </example>
	/// <param name="wholeString">전체 string. (ref)</param>
	/// <param name="thisSection">추가할 section.</param>		
	public static void AddSection(ref string wholeString, string thisSection)
	{
		try
		{
			if (IsNullOrWhiteSpace(thisSection))
			{
				Debug.LogWarning("[FileIO] AddSection : thisSection is empty (" + thisSection + ")");
				return;
			}

			if (!SectionIsExist(wholeString, thisSection))
			{
				StringBuilder modifiedData = new StringBuilder();
				modifiedData.Append(wholeString);
				modifiedData.Append(LineFeedSeperator[0]);
				modifiedData.Append(SectionSeperator[0]);
				modifiedData.Append(thisSection);
				modifiedData.Append(LineFeedSeperator[0]);

				wholeString = modifiedData.ToString();

				Debug.Log("[FileIO] AddSection : SUCCESS (" + thisSection + ")");
			}
		}
		catch (Exception e)
		{
			Debug.LogError("[FileIO] AddSection : Fail (" + thisSection + ") : " + e);
		}
	}


	/// <summary>
	/// <para>──────────────────────────────</para>
	/// #thisSection 과 하단의 모든 key=value 를 삭제합니다.
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <example>
	///		#options
	///		　play_count = 100
	///		#userdata
	///		　user_num = 300
	///		
	///		DelSection(ref wholeString, "userdata");
	///		
	///		#options
	///		　play_count = 100
	/// </example>
	/// <param name="wholeString">전체 string (ref).</param>
	/// <param name="thisSection">삭제할 section.</param>	
	public static void DelSection(ref string wholeString, string thisSection)
	{
		try
		{
			if (SectionIsExist(wholeString, thisSection))
			{
				int found = 0;

				string[] arr_section = wholeString.Split(SectionSeperator, StringSplitOptions.None);
				string[] arr_newSection = new string[arr_section.GetLength(0) - 1];

				for (int i = 1; i < arr_section.GetLength(0); i++)
				{
					string[] arr_type = arr_section[i].Split(LineFeedSeperator, StringSplitOptions.None);

					if (arr_type.Length > 0)
					{
						if (arr_type[0].Trim() == thisSection)
						{
							//Debug.Log("[FileIO] DelSection : Found Section to Delete (" + thisSection + ")");

							found = 1;

							//마지막 section일 경우 이전 section의 줄바꿈 기호 하나를 삭제한다.
							if (i == arr_section.GetLength(0) - 1)
							{
								arr_newSection[i - found] = arr_newSection[i - found].Remove(arr_newSection[i - found].Length - LineFeedSeperator[0].Length);
							}
						}
						else
						{
							arr_newSection[i - found] = arr_section[i];
						}
					}
				}

				if (found == 1)
				{
					Debug.Log("[FileIO] DelSection : SUCCESS (" + thisSection + ")");

					wholeString = string.Join(SectionSeperator[0], arr_newSection);
				}
			}
		}
		catch (Exception e)
		{
			Debug.LogError("[FileIO] DelSection : Fail (" + thisSection + ") : " + e);
		}
	}


	/// <summary>
	/// <para>──────────────────────────────</para>
	/// #thisSection 의 하단에 thisKey=newValue 형태의 라인을 추가합니다.
	/// <para>#thisSection 이 없으면 wholeString 의 하단에 #thisSection 을 생성합니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <example>
	///		#options
	///		　play_count = 100
	///		
	///		AddKeyValue(ref wholeString, "options", "play_time", "3600");
	///		
	///		#options
	///		　play_count = 100
	///		　play_time = 3600
	/// </example>
	/// <param name="wholeString">전체 string (ref).</param>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">추가할 key.</param>
	/// <param name="newValue">추가할 value.</param>	
	public static void AddKeyValue(ref string wholeString, string thisSection, string thisKey, string newValue)
	{
		try
		{
			if (IsNullOrWhiteSpace(thisSection))
			{
				Debug.LogWarning("[FileIO] AddKeyValue : thisSection is empty (" + thisSection + ")");
				return;
			}
			if (IsNullOrWhiteSpace(thisKey))
			{
				Debug.LogWarning("[FileIO] AddKeyValue : thisKey is empty (" + thisKey + ")");
				return;
			}
			if (IsNullOrWhiteSpace(newValue))
			{
				Debug.LogWarning("[FileIO] AddKeyValue : newValue is empty (" + newValue + ")");
				return;
			}

			//section 이 없으면 추가한다.
			AddSection(ref wholeString, thisSection);

			//key 가 없으면 추가한다.
			if (!KeyIsExist(wholeString, thisSection, thisKey))
			{
				string[] arr_section = wholeString.Split(SectionSeperator, StringSplitOptions.None);

				for (int i = 1; i < arr_section.GetLength(0); i++)
				{
					string[] arr_type = arr_section[i].Split(LineFeedSeperator, StringSplitOptions.None);

					if (arr_type.Length > 0)
					{
						if (arr_type[0].Trim() == thisSection)
						{
							//Debug.Log("[FileIO] AddKeyValue : Add (" + thisSection + ", " + thisKey + ", " + newValue + ")");

							StringBuilder modifiedData = new StringBuilder();

							//줄바꿈 기호가 2개 이상일 경우 줄바꿈 기호 1개를 삭제한다.
							//Debug.Log("arr_section[i].LastIndexOf(LineFeedSeperator[0]) : " + arr_section[i].LastIndexOf(LineFeedSeperator[0]));
							//Debug.Log("arr_section[i].Length : " + arr_section[i].Length);

							string taleCut = arr_section[i];
							string doubleLineFeed = LineFeedSeperator[0] + LineFeedSeperator[0];
							for (int k = arr_section[i].LastIndexOf(doubleLineFeed); k >= 0; k -= doubleLineFeed.Length)
							{
								if (taleCut.Substring(k, doubleLineFeed.Length) == doubleLineFeed)
								{
									taleCut = taleCut.Remove(k, LineFeedSeperator[0].Length);
									//Debug.Log("("+k + ") [" + taleCut + "]");
								}
							}

							modifiedData.Append(taleCut);
							modifiedData.Append(thisKey);
							modifiedData.Append(KeyValueSeperator[0]);
							modifiedData.Append(newValue);
							modifiedData.Append(LineFeedSeperator[0]);

							// 마지막 section 이 아닐 경우 줄바꿈 문자를 추가한다.
							if (i < arr_section.GetLength(0) - 1)
								modifiedData.Append(LineFeedSeperator[0]);

							arr_section[i] = modifiedData.ToString();

							//Debug.Log(arr_section[i]);

							wholeString = string.Join(SectionSeperator[0], arr_section);

							Debug.Log("[FileIO] AddKeyValue : SUCCESS (" + thisSection + ", " + thisKey + ", " + newValue + ")");

							break;
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			Debug.LogError("[FileIO] AddKeyValue : Fail : (" + thisSection + ", " + thisKey + ") : " + e);
		}
	}


	/// <summary>
	/// <para>──────────────────────────────</para>
	/// #thisSection 의 하단에 thisKey=newValue 형태의 라인을 삭제합니다.
	/// <para>#thisSection 의 하단에 key가 모두 삭제되어도 #thisSection 이 삭제되지는 않습니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <example>
	///		#options
	///		　play_count = 100
	///		　play_time = 3600
	///		
	///		DelKeyValue(ref wholeString, "options", "play_count");
	///		
	///		#options
	///		　play_time = 3600
	/// </example>
	/// <param name="wholeString">전체 string (ref).</param>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">삭제할 key.</param>
	public static void DelKeyValue(ref string wholeString, string thisSection, string thisKey)
	{
		try
		{
			if (KeyIsExist(wholeString, thisSection, thisKey))
			{
				int found = 0;

				string[] arr_section = wholeString.Split(SectionSeperator, StringSplitOptions.None);

				for (int i = 1; i < arr_section.GetLength(0); i++)
				{
					string[] arr_type = arr_section[i].Split(LineFeedSeperator, StringSplitOptions.None);

					if (arr_type.Length > 0)
					{
						if (arr_type[0].Trim() == thisSection)
						{
							string[] arr_newType = new string[arr_type.GetLength(0) - 1];

							//Debug.Log("arr_type.GetLength(0) : " + arr_type.GetLength(0));
							for (int j = 0; j < arr_type.GetLength(0); j++)
							{
								string[] arr_value = arr_type[j].Split(KeyValueSeperator, StringSplitOptions.RemoveEmptyEntries);
								//Debug.Log("arr_value.Length : " + arr_value.Length);

								if (arr_value.Length > 0)
								{
									if (arr_value[0] == thisKey)
									{
										found = 1;
									}
									else
									{
										arr_newType[j - found] = arr_type[j];
									}
								}
							}

							if (found == 1)
							{
								arr_section[i] = string.Join(LineFeedSeperator[0], arr_newType);

								wholeString = string.Join(SectionSeperator[0], arr_section);

								Debug.Log("[FileIO] DelKeyValue : SUCCESS (" + thisSection + ", " + thisKey + ")");
							}

							break;
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			Debug.LogError("[FileIO] DelKeyValue : Fail : (" + thisSection + ", " + thisKey + ") : " + e);
		}
	}


	/// <summary>
	/// <para>──────────────────────────────</para>
	/// wholeString 의 하단에 #thisSection 을 추가합니다.
	/// <para>──────────────────────────────</para>
	///	<para>　→ wholeString 의 하단에 #thisSection 이 추가된 전체 string.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <example>
	///		#options
	///		　play_count = 100
	///		
	///		wholeString = AddSection(wholeString, "userdata");
	///		
	///		#options
	///		　play_count = 100
	///		#userdata	
	/// </example>
	/// <param name="wholeString">전체 string.</param>
	/// <param name="thisSection">추가할 section.</param>	
	/// <returns>→ wholeString 의 하단에 #thisSection 이 추가된 전체 string.</returns>
	public static string AddSection(string wholeString, string thisSection)
	{
		AddSection(ref wholeString, thisSection);
		return wholeString;
	}


	/// <summary>
	/// <para>──────────────────────────────</para>
	/// #thisSection 과 하단의 모든 key=value 를 삭제합니다.
	/// <para>──────────────────────────────</para>
	///	<para>　→ #thisSection 과 하단의 모든 key=value 가 삭제된 전체 string.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <example>
	///		#options
	///		　play_count = 100
	///		#userdata
	///		　user_num = 300
	///		
	///		wholeString = DelSection(wholeString, "userdata");
	///		
	///		#options
	///		　play_count = 100
	/// </example>
	/// <param name="wholeString">전체 string.</param>
	/// <param name="thisSection">삭제할 section.</param>	
	/// <returns>→ #thisSection 과 하단의 모든 key=value 가 삭제된 전체 string.</returns>
	public static string DelSection(string wholeString, string thisSection)
	{
		DelSection(ref wholeString, thisSection);
		return wholeString;
	}


	/// <summary>
	/// <para>──────────────────────────────</para>
	/// #thisSection 의 하단에 thisKey=newValue 형태의 라인을 추가합니다.
	/// <para>#thisSection 이 없으면 wholeString 의 하단에 #thisSection 을 생성합니다.</para>
	/// <para>──────────────────────────────</para>
	///	<para>　→ #thisSection 의 하단에 thisKey=newValue 가 추가된 전체 string.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <example>
	///		#options
	///		　play_count = 100
	///		
	///		wholeString = AddKeyValue(wholeString, "options", "play_time", "3600");
	///		
	///		#options
	///		　play_count = 100
	///		　play_time = 3600
	/// </example>
	/// <param name="wholeString">전체 string.</param>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">추가할 key.</param>
	/// <param name="newValue">추가할 value.</param>
	/// <returns>→ #thisSection 의 하단에 thisKey=newValue 가 추가된 전체 string.</returns>
	public static string AddKeyValue(string wholeString, string thisSection, string thisKey, string newValue)
	{
		AddKeyValue(ref wholeString, thisSection, thisKey, newValue);
		return wholeString;
	}


	/// <summary>
	/// <para>──────────────────────────────</para>
	/// #thisSection 의 하단에 thisKey=newValue 형태의 라인을 삭제합니다.
	/// <para>#thisSection 의 하단에 key가 모두 삭제되어도 #thisSection 이 삭제되지는 않습니다.</para>
	/// <para>──────────────────────────────</para>
	///	<para>　→ #thisSection 의 하단에 thisKey=newValue 가 삭제된 전체 string.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <example>
	///		#options
	///		　play_count = 100
	///		　play_time = 3600
	///		
	///		wholeString = DelKeyValue(wholeString, "options", "play_count");
	///		
	///		#options
	///		　play_time = 3600
	/// </example>
	/// <param name="wholeString">전체 string.</param>
	/// <param name="thisSection">찾을 section.</param>
	/// <param name="thisKey">삭제할 key.</param>
	/// <returns>→ #thisSection 의 하단에 thisKey=value 가 삭제된 전체 string.</returns>	
	public static string DelKeyValue(string wholeString, string thisSection, string thisKey)
	{
		DelKeyValue(ref wholeString, thisSection, thisKey);
		return wholeString;
	}


	/// <summary>
	/// <para>──────────────────────────────</para>
	/// wholeString 에 #thisSection 이 존재하는지 확인합니다.
	/// <para>──────────────────────────────</para>
	/// <para>　→ #thisSection 존재 여부.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="wholeString">전체 string.</param>
	/// <param name="thisSection">확인할 section.</param>
	/// <returns>→ #thisSection 존재 여부.</returns>
	public static bool SectionIsExist(string wholeString, string thisSection)
	{
		try
		{
			if (IsNullOrWhiteSpace(thisSection))
			{
				Debug.LogWarning("[FileIO] SectionIsExist : thisSection is empty (" + thisSection + ")");
				return false;
			}

			string[] arr_section = wholeString.Split(SectionSeperator, StringSplitOptions.None);

			for (int i = 1; i < arr_section.GetLength(0); i++)
			{
				string[] arr_type = arr_section[i].Split(LineFeedSeperator, StringSplitOptions.None);

				if (arr_type.Length > 0)
				{
					if (arr_type[0].Trim() == thisSection)
					{
						Debug.Log("[FileIO] SectionIsExist : thisSection Exists (" + thisSection + ")");

						return true;
					}
				}
			}

			return false;
		}
		catch (Exception e)
		{
			Debug.LogError("[FileIO] SectionIsExist : Fail (" + thisSection + ") : " + e);

			return false;
		}
	}


	/// <summary>
	/// <para>──────────────────────────────</para>
	/// #thisSection 의 하단에 thisKey 가 존재하는지 확인합니다.
	/// <para>──────────────────────────────</para>
	/// <para>　→ thisKey 존재 여부.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="wholeString">전체 string.</param>
	/// <param name="thisSection">확인할 section.</param>
	/// <param name="thisKey">확인할 key.</param>
	/// <returns>→ thisKey 존재 여부.</returns>
	public static bool KeyIsExist(string wholeString, string thisSection, string thisKey)
	{
		try
		{
			if (IsNullOrWhiteSpace(thisSection))
			{
				Debug.LogWarning("[FileIO] KeyIsExist : thisSection is empty (" + thisSection + ")");
				return false;
			}
			if (IsNullOrWhiteSpace(thisKey))
			{
				Debug.LogWarning("[FileIO] KeyIsExist : thisKey is empty (" + thisKey + ")");
				return false;
			}

			string[] arr_section = wholeString.Split(SectionSeperator, StringSplitOptions.None);

			for (int i = 1; i < arr_section.GetLength(0); i++)
			{
				string[] arr_type = arr_section[i].Split(LineFeedSeperator, StringSplitOptions.None);

				if (arr_type.Length > 0)
				{
					if (arr_type[0].Trim() == thisSection)
					{
						for (int j = 1; j < arr_type.GetLength(0); j++)
						{
							string[] arr_value = arr_type[j].Split(KeyValueSeperator, StringSplitOptions.RemoveEmptyEntries);

							if (arr_value.Length > 0)
							{
								if (arr_value[0] == thisKey)
								{
									//Debug.Log("[FileIO] KeyIsExist : thisKey Exists (" + thisSection + ", " + thisKey + ")");

									return true;
								}
							}
						}

						break;
					}
				}
			}

			return false;
		}
		catch (Exception e)
		{
			Debug.LogError("[FileIO] KeyIsExist : Fail (" + thisSection + ", " + thisKey + ") : " + e);

			return false;
		}
	}


	#endregion ★ Methods (Handle Form)


	#region ★ Methods (Utility)


	/// <summary>
	/// <para>──────────────────────────────</para>
	/// targetPath 에 폴더가 포함되어 있을 경우 [DataPath] 에 해당 폴더를 생성합니다.
	/// <para>하위 폴더를 지원합니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="targetPath">체크할 경로.</param>
	public static void AnalyzeFolder(string targetPath, int pathflag = 0)
	{
		if (!targetPath.StartsWith("/"))
			targetPath = "/" + targetPath;

		int slashPlace = targetPath.LastIndexOf("/");
		string folderPath = string.Empty;

		if (pathflag == 0)
			folderPath = Application.persistentDataPath;
		else if (pathflag == 1)
			folderPath = Application.dataPath;
		else if (pathflag == 2)
			folderPath = Application.dataPath + "/Resources";

		if (slashPlace > 0)
		{
			targetPath = targetPath.Remove(slashPlace);

			if (!Directory.Exists(folderPath + targetPath))
			{
				Directory.CreateDirectory(folderPath + targetPath);
				Debug.Log("[FileIO] AnalyzeFolder : Create Directory : " + folderPath + targetPath);
			}
		}
	}


	/// <summary>
	/// <para>──────────────────────────────</para>
	/// string 이 null 또는 공백만으로 되어있는지 검사합니다.
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="value">검사할 string.</param>
	/// <returns>null 또는 공백 여부.</returns>
	public static bool IsNullOrWhiteSpace(string value)
	{
		if (value != null)
		{
			for (int i = 0; i < value.Length; i++)
			{
				if (!char.IsWhiteSpace(value[i]))
				{
					return false;
				}
			}
		}
		return true;
	}


	/// <summary>
	/// <para>──────────────────────────────</para>
	/// [Resources]/referencePath 파일의 텍스트 내용을 string으로 불러옵니다.
	/// <para>지원하는 referencePath 의 확장자는 txt/html/htm/xml/bytes/json/csv/yaml/fnt 입니다.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <param name="referencePath">[Assets/Resources] 폴더 안에 있는 파일의 경로.</param>
	/// <returns>[Assets/Resources]/referencePath 파일의 전체 string.</returns>
	private static string ReadFromResources(string referencePath)
	{
		string wholeString = null;

		try
		{
			// referencePath 는 앞에 "/"가 없어야 함.
			if (referencePath.StartsWith("/"))
				referencePath = referencePath.Substring(1);

			//referencePath 는 확장자가 없어야 함.
			foreach (string ext in SupportExtensions)
			{
				if (referencePath.EndsWith(ext))
				{
					referencePath = referencePath.Remove(referencePath.Length - ext.Length);
					break;
				}
			}

			TextAsset data = (TextAsset)Resources.Load(referencePath, typeof(TextAsset));

			StringReader sr2 = null;
			sr2 = new StringReader(data.text);
			wholeString = sr2.ReadToEnd();
			sr2.Close();

			//Debug.Log("[FileIO] ReadFromResources : Success : " + referencePath);
		}
		catch (Exception e)
		{
			Debug.LogError("[FileIO] ReadFromResources : Fail : " + e);
		}

		return wholeString;
	}


	/// <summary>
	/// <para>──────────────────────────────</para>
	/// ClearText 의 내용을 암호화한 string 을 리턴합니다.
	/// <para>Windows Phone 미작동.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <returns>암호화가 진행된 string.</returns>
	/// <param name="ClearText">암호화를 진행할 string.</param>
	private static string EncryptString(string ClearText)
	{
		if (!isEncrypt)
			return ClearText;

#if UNITY_WP8
	return ClearText;		
#else
		try
		{
			byte[] clearTextBytes = Encoding.UTF8.GetBytes(ClearText);
			SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
			MemoryStream ms = new MemoryStream();
			byte[] rgbIV = Encoding.ASCII.GetBytes(KEY_1);
			byte[] key = Encoding.ASCII.GetBytes(KEY_2);
			CryptoStream cs = new CryptoStream(ms, rijn.CreateEncryptor(key, rgbIV), CryptoStreamMode.Write);

			cs.Write(clearTextBytes, 0, clearTextBytes.Length);
			cs.Close();

			return Convert.ToBase64String(ms.ToArray());
		}
		catch
		{
			return ClearText;
		}
#endif
	}


	/// <summary>
	/// <para>──────────────────────────────</para>
	/// EncryptedText 의 내용을 복호화한 string 을 리턴합니다.
	/// <para>Windows Phone 미작동.</para>
	/// <para>──────────────────────────────</para>
	/// </summary>
	/// <returns>암호화가 해제된 string.</returns>
	/// <param name="EncryptedText">암호화가 진행된 string.</param>
	private static string DecryptString(string EncryptedText)
	{
		if (!isEncrypt)
			return EncryptedText;

#if UNITY_WP8
	return EncryptedText;		
#else
		try
		{
			byte[] encryptedTextBytes = Convert.FromBase64String(EncryptedText);
			SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
			MemoryStream ms = new MemoryStream();
			byte[] rgbIV = Encoding.ASCII.GetBytes(KEY_1);
			byte[] key = Encoding.ASCII.GetBytes(KEY_2);
			CryptoStream cs = new CryptoStream(ms, rijn.CreateDecryptor(key, rgbIV), CryptoStreamMode.Write);

			cs.Write(encryptedTextBytes, 0, encryptedTextBytes.Length);
			cs.Close();

			return Encoding.UTF8.GetString(ms.ToArray());
		}
		catch
		{
			return EncryptedText;
		}
#endif
	}


	#endregion ★ Methods (Utility)
}

#endregion ★ FileIO Class
