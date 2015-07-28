using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace BrainBit.UI{
public delegate void DragRecieve(Dragable drag);

public class DragableRecieve : MonoBehaviour, IDropHandler
{
	public string Group;
	public DragRecieve Dragable;
	private Dragable _value;


	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
		Dragable drag = eventData.pointerDrag.GetComponent<Dragable> ();
		if (drag != null) {

			if (drag.Group == this.Group && this._value == null) {
				this.Value = drag;
				drag.transform.SetParent(this.transform);
				drag.transform.position = this.transform.position;
				drag.Slot = this;
			}
		}
	}
	#endregion

	public Dragable Value {
		get {
			return this._value;
		}

		set {
			this._value = value;
			if(this.Dragable != null)
			{
				this.Dragable(this._value);
			}
		}
	}
}
}
