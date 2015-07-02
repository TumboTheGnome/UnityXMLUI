using UnityEngine;
using System.Collections;

namespace BrainBit.UI{
	public interface IUIBindable{
		UIElementMeta View{ get; set; }
	}
}
