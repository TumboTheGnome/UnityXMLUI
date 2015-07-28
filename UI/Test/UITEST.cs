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
		this._ui.Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		this._ui.Canvas.worldCamera = Camera.main;

	}
}

public class testData:IUIBindable
{
	public const int Length = 10;
	private string _name;
	private List<string> _names;
	private UIElementMeta _view;
	private bool _show;
	private Sprite _img;

	private float _slider = 1;
	private List<Item> items;

	#region IUIBInd implementation

	public UIElementMeta View {
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
		if (UIAssets.Self != null) {
			this._img = UIAssets.Self.Images[0];
		}

		this.items = new List<Item> (){new Item(), new Item(), new Item()};
		this.items [0].Name = "Sword";
		this.items [1].Name = "Spear";
		this.items [2].Name = "Laser";


		this._populate ();
	}

	public string Name {
		get {
			return this._name;
		}
	}

	public bool ShowDialog{
		get{
			return this._show;
		}
	}

	public Sprite Image{
		get{
			return this._img;
		}

		set{
			this._img = value;
		}
	}

	public List<Item> Items{
		get{
			return this.items;
		}
	}

	public float SliderValue{
		get{
			return this._slider;
		}
	}

	public void UpdateDialog()
	{
		this._show = !this._show;
		this._view.Update ();
	}

	public void Random ()
	{
		this._slider -= 0.1f;
		if (this._slider <= 0) {
			this._slider = 1;
		}

		this._img = UIAssets.Self.Images [1];
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

	public void DragBind(Dragable drag)
	{
		if (drag != null) {
			if(drag.Meta != null)
			{
				if(drag.Meta.Data != null)
				{
				Item i = (Item)drag.Meta.Data;
				Debug.LogWarning(i.Name);
				}
			}
		}
		//Debug.Log ("The drag is: "+drag);
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

public class Item{
	public string Name;
	public int Cost;
}
