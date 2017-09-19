using UniRx;
using UnityEngine;

namespace KyleGame
{
	public abstract class BasePlayerComponent : MonoBehaviour
	{
		protected PlayerCore Core;
		protected IInputEventProvider InputEventProvider { get; private set; }

		private void Start()
		{
			Core = GetComponent<PlayerCore>();
			InputEventProvider = GetComponent<IInputEventProvider>();

			OnStart();
		}

		protected virtual void OnStart()
		{
			Core.OnInitializeAsync.Subscribe(_ => OnInitialize());
		}

		protected abstract void OnInitialize();
	}
}