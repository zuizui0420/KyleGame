using UniRx;
using UnityEngine;

namespace KyleGame
{
	public partial class Spider : StatefulWalker<Spider, SpiderState>
	{
		private Transform _playerTransform;

		private ISpiderAnimation _spiderAnimation;

		[SerializeField]
		private SpiderType _spiderType = SpiderType.TimerBomb;

		protected override Transform PlayerTransform
		{
			get { return _playerTransform; }
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();

			_spiderAnimation = GetComponent<ISpiderAnimation>();
			_playerTransform = GameObject.Find("Player").transform;
			
			MovementSpeed.TakeUntilDestroy(this)
				.Subscribe(x => _spiderAnimation.Speed = x);


			StateList.Add(new StateWander(this));
			StateList.Add(new StatePursuit(this));
			StateList.Add(new StateExplosion(this));

			StateMachine = new StateMachine<Spider>();

			ChangeState(SpiderState.Wander);
		}
	}

	public enum SpiderType
	{
		InstantSpark,
		TimerBomb
	}

	public enum SpiderState
	{
		Wander,
		Pursuit,
		Explode
	}
}