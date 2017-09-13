using UniRx;
using UnityEngine;

namespace Assets.Scripts
{
	public abstract class BaseEnemyComponent : MonoBehaviour
	{
		protected EnemyCore Core;

		protected IInputEventProvider InputEventProvider { get; private set; }

		private void Start()
		{
			Core = GetComponent<EnemyCore>();
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