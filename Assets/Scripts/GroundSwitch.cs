using UniRx;
using UnityEngine;

public class GroundSwitch : MonoBehaviour
{
	[SerializeField]
	private bool _awakeOnce = false;

	private readonly BehaviorSubject<bool> _raiseEventSubject = new BehaviorSubject<bool>(false);

	public IObservable<bool> RaiseEvent { get { return _raiseEventSubject; } }

	private void Start()
	{
		if (_awakeOnce)
		{
			_raiseEventSubject.Where(x => x).Take(1).TakeUntilDestroy(this).Subscribe(_ => { }, () => _raiseEventSubject.OnCompleted());
		}
		else
		{
			_raiseEventSubject.Where(x => x).TakeUntilDestroy(this).Subscribe(_ => { }, () => _raiseEventSubject.OnCompleted());
		}
	}
}