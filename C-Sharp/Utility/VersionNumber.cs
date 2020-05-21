using UnityEngine;
using System.Reflection;

[assembly: AssemblyVersion("1.0.*")]
public class VersionNumber : MonoBehaviour
{

	/// <summary>
	/// 버전 정보를 표시한다.
	/// </summary>
	public bool ShowVersionInformation = false;
	
	/// <summary>
	/// showTime 동안 버전 정보를 표시한다. (ShowVersionInformation 을 자동으로 켠다.)
	/// </summary>
	public bool ShowVersionAndDestroy = true;

	/// <summary>
	/// 버전 정보를 표시할 시간 (초).
	/// </summary>
	public float showTime = 5f;

	public Color textColor = Color.white;

	public enum TextPosition
	{
		Right_Bottom, Right_Top, Left_Bottom, Left_Top
	}

	public TextPosition textPosition = TextPosition.Right_Bottom;

	public string textFormat = "v{0}";

	public bool useCustomStyle = false;
	public GUIStyle customStyle;

	string version;
	Rect position = new Rect(0, 0, Screen.width, 20);

	/// <summary>
	/// Gets the version.
	/// </summary>
	/// <value>The version.</value>
	public string Version
	{
		get
		{
			if (version == null)
			{
				version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
			return version;
		}
	}


	/// Use this for initialization
	void Awake()
	{
		DontDestroyOnLoad(this);
		
		// Log current version in log file
		Debug.Log(CodeManager.GetMethodName(true) + string.Format("Currently running version is {0}", Version));

		if (ShowVersionAndDestroy)
		{
			ShowVersionInformation = true;
			Destroy(this, showTime);
		}

		switch(textPosition)
		{
			default:
			case TextPosition.Right_Bottom:			
				position.x = Screen.width - position.width - 10f;
				position.y = Screen.height - position.height - 10f;
				break;

			case TextPosition.Right_Top:
				position.x = Screen.width - position.width - 10f;
				position.y = 10f;
				break;

			case TextPosition.Left_Bottom:
				position.x = 10f;
				position.y = Screen.height - position.height - 10f;
				break;

			case TextPosition.Left_Top:
				position.x = 10f;
				position.y = 10f;
				break;
		}		
	}


	void OnGUI()
	{
		if (!ShowVersionInformation)
		{
			return;
		}
		
		if (useCustomStyle)
		{
			GUI.Label(position, string.Format(textFormat, Version), customStyle);
		}
		else
		{
			GUI.contentColor = textColor;
			GUI.Label(position, string.Format(textFormat, Version));			
		}
	}
}