using UniRx;
using UnityEngine;

namespace KyleGame
{
	public class EnemyCore : MonoBehaviour
	{
		private readonly AsyncSubject<Unit> _onInitializeAsyncSubject = new AsyncSubject<Unit>();

		public IObservable<Unit> OnInitializeAsync
		{
			get { return _onInitializeAsyncSubject; }
		}

		private void Awake()
		{
			_onInitializeAsyncSubject.Subscribe(_ => { });
		}

		private void Start()
		{
			Initialize();
		}

		public void Initialize()
		{
			_onInitializeAsyncSubject.OnNext(Unit.Default);
			_onInitializeAsyncSubject.OnCompleted();
		}
	}
}