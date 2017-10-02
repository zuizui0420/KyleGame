using UniRx;
using UnityEngine;

namespace KyleGame
{
	public class BossBarrier : MonoBehaviour
	{
		[SerializeField]
		private ParticleSystem _sparkEffect;

		private readonly AsyncSubject<Unit> _isBrokeSubject = new AsyncSubject<Unit>();
		public IObservable<Unit> IsBrokeEvent { get { return _isBrokeSubject; } }

		public void HalfBroken()
		{
			_sparkEffect.Play();
		}

		public void Broke()
		{
			gameObject.SetActive(false);
		}
	}
}