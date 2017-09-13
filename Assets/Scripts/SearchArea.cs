using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class SearchArea : MonoBehaviour
{
	private readonly Subject<Collider> _onPlayerInAreaSubject = new Subject<Collider>();

	public IObservable<Collider> OnPlayerInAreaEvent
	{
		get { return _onPlayerInAreaSubject; }
	}

	private void Awake()
	{
		this.OnTriggerStayAsObservable()
			.Subscribe(_onPlayerInAreaSubject.OnNext,
				_onPlayerInAreaSubject.OnCompleted);
	}
}