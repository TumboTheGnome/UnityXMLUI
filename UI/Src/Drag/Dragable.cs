using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
namespace BrainBit.UI{


public class Dragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {
	public UIElementMeta  Meta;
	public string Group;
	private static GameObject Current;
	private Transform _parent;
	private Vector3 _point;
	private DragableRecieve _slot;
	private CanvasGroup _group;


	void Awake()
	{
		this._parent = this.transform.parent;
		this._group = this.gameObject.AddComponent<CanvasGroup> ();

	}

	public DragableRecieve Slot{
		get{
			return this._slot;
		}

		set{
			this._slot = value;
		}
	}

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{

	}

	#endregion

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
		Dragable.Current = this.gameObject;

		//this.transform.SetAsLastSibling ();
		this._group.blocksRaycasts = false;

		if (this._slot != null) {
			this._slot.Value = null;
			this._slot = null;
		} else {
			this._point = this.transform.position;
		}
	}

	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		this.transform.position =Input.mousePosition;
	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		if (this._slot == null) {
			this.transform.SetParent(this._parent);
			this.transform.position = this._point;
		}
		this._group.blocksRaycasts = true;
		Dragable.Current = null;
	}

	#endregion
}
}