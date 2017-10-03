using System;
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

		[SerializeField]
		private bool _isIdleMode;

		protected override Transform PlayerTransform
		{
			get { return _playerTransform; }
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			

			_playerTransform = GameObject.Find("Player").transform;
			
			MovementSpeed.TakeUntilDestroy(this)
				.Subscribe(x => _spiderAnimation.Speed = x);


			StateList.Add(new StateWander(this));
			StateList.Add(new StateIdle(this));
			StateList.Add(new StatePursuit(this));
			StateList.Add(new StateExplosion(this));

			StateMachine = new StateMachine<Spider>();

			ChangeState(_isIdleMode ? SpiderState.Idle : SpiderState.Wander);
		}

		private void SelectAnimation()
		{
			switch (_spiderType)
			{
				case SpiderType.InstantSpark:
					_spiderAnimation = gameObject.AddComponent<FastSpiderAnimation>();
					break;
				case SpiderType.TimerBomb:
					_spiderAnimation = gameObject.AddComponent<SlowSpiderAnimation>();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
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
		Idle,
		Pursuit,
		Explode
	}
}