using UniRx;
using UnityEngine;

namespace KyleGame
{
	public partial class Spider : StatefulEnemyComponentBase<Spider, SpiderState>
	{
		private int _anchorNum;

		private EnemyCharacterController _characterController;

		[SerializeField]
		private Transform[] _destinationAnchorList;

		private Transform _playerTransform;

		private ISpiderAnimation _spiderAnimation;

		[SerializeField]
		private SpiderType _spiderType = SpiderType.TimerBomb;

		protected override void OnInitialize()
		{
			_characterController = GetComponent<EnemyCharacterController>();
			_spiderAnimation = GetComponent<ISpiderAnimation>();
			_playerTransform = GameObject.Find("Player").transform;

			_destination.TakeUntilDestroy(this)
				.Subscribe(x =>
				{
					var orig = transform.rotation;
					var direction = x - transform.position;
					direction.y = 0;
					var dest = Quaternion.LookRotation(direction);

					transform.rotation = dest/*Quaternion.Slerp(orig, dest, Time.deltaTime)*/;
				});
			_movementSpeed.TakeUntilDestroy(this)
				.Subscribe(x => _spiderAnimation.Speed = x);


			StateList.Add(new StateWander(this));
			StateList.Add(new StatePursuit(this));
			StateList.Add(new StateExplosion(this));

			StateMachine = new StateMachine<Spider>();

			ChangeState(SpiderState.Wander);
		}

		private Vector3 NextDestination()
		{
			var ret = _destinationAnchorList[_anchorNum].position;
			_anchorNum = ++_anchorNum >= _destinationAnchorList.Length ? 0 : _anchorNum;

			return ret;
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