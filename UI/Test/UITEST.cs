using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using BrainBit.UI;

[RequireComponent(typeof(UIAssets))]
public class UITEST : MonoBehaviour
{
	public TextAsset XML;
	private UI _ui;
	private testData _data = new testData ();

	void Start ()
	{
		this._ui = new UI (new RulesBuilderBase ());
		this._ui.Load (this.XML.text, this._data);
		this._ui.Canvas.renderMode = RenderMode.ScreenSpaceCamera;
		this._ui.Canvas.worldCamera = Camera.main;

	}

	void OnGUI ()
	{
		if (GUILayout.Button ("TEST")) {
			this._data.Random ();
		}
	}
}

public class testData:IUIBindable
{
	public const int Length = 10;
	private string _name;
	private List<string> _names;
	private UIEllementMeta _view;

	#region IUIBInd implementation

	public UIEllementMeta View {
		get {
			return this._view;
		}
		set {
			this._view = value;
		}
	}

	#endregion

	public testData ()
	{
		this._populate ();
	}

	public string Name {
		get {
			return this._name;
		}
	}

	public void Random ()
	{
		this._populate ();

		if (this._view != null) {
			this._view.Update ();
		}
	}

	public List<string> Test {
		get {
			return this._names;
		}
	}

	private string _randomString ()
	{
		System.Random random = new System.Random ((int)DateTime.Now.Ticks);
		StringBuilder builder = new StringBuilder ();
		char ch;
		for (int i = 0; i < Length; i++) {
			ch = Convert.ToChar (Convert.ToInt32 (Math.Floor (26 * random.NextDouble () + 65)));
			builder.Append (ch);
		}
		return builder.ToString ();
	}

	private void _populate ()
	{
		this._name = this._randomString ();
		this._names = new List<string> ();
		for (int i  = 0; i< Length-2; i++) {
			this._names.Add (this._randomString ());
		}
	}
}
