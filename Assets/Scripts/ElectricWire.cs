using System.Linq;
using UniRx;
using UnityEngine;

namespace KyleGame
{
	public class ElectricWire : MonoBehaviour
	{
		private void Start()
		{
			var electricWireStream = GetComponentsInChildren<ElectricWireElement>().Select(x => x.CollisionEnterObservable).Merge();
		}
	}
}