using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CapitaliseInput : MonoBehaviour
{

	public void InputChanged()
	{
		string contents = GetComponent<InputField>().text;
		contents = contents.ToUpper();
		GetComponent<InputField>().text = contents;
	}
}
