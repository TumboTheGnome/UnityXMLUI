using UnityEngine;
using System.Collections;

namespace BrainBit.UI{
	public interface IUIBindable{
		UIEllementMeta View{ get; set; }
	}
}
