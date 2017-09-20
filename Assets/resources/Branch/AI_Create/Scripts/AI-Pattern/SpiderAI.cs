using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace KyleGame
{
	public partial class Spider
	{
		/// <summary>
		///     ステート：巡回
		/// </summary>
		private class StateWander : State<Spider>
		{
			private const float ReactionAngle = 90f;
			private const float ReactionDistance = 5f;

			private readonly CompositeDisposable _subscriptionList = new CompositeDisposable();

			/// <summary>
			/// Alias: Owner.transform
			/// </summary>
			private Transform Self
			{
				get { return Owner.transform; }
			}

			/// <summary>
			/// Alias: Owner._playerTransform
			/// </summary>
			private Transform Player
			{
				get { return Owner._playerTransform; }
			}
			
			private Vector3 _destination;

			public StateWander(Spider owner) : base(owner)
			{
			}

			public override void Enter()
			{
				_destination = Owner._destinationAnchorList[Owner._anchorNum].position;

				Owner.UpdateAsObservable()
					.Where(_ => OnObjectReflectedInOwnerEyes())
					.TakeUntilDestroy(Owner)
					.Subscribe(_ => Owner.ChangeState(SpiderState.Pursuit))
					.AddTo(_subscriptionList);

				Owner.UpdateAsObservable()
					.TakeUntilDestroy(Owner)
					.Subscribe(_ =>
					{
						Owner.transform.position =
							Vector3.Lerp(Owner.transform.position, _destination, Time.deltaTime);
					})
					.AddTo(_subscriptionList);

				Owner.UpdateAsObservable()
					.Where(_ => Vector3.Distance(_destination, Owner.transform.position) < ReactionDistance)
					.TakeUntilDestroy(Owner)
					.Subscribe(_ =>
					{
						Owner._anchorNum = ++Owner._anchorNum >= Owner._destinationAnchorList.Length ? 0 : Owner._anchorNum;
						_destination = Owner._destinationAnchorList[Owner._anchorNum].position;
					})
					.AddTo(_subscriptionList);
			}

			private bool OnObjectReflectedInOwnerEyes()
			{
				var angle = Vector3.Angle((Player.position - Self.transform.position).normalized, Self.forward);
				var distance = Vector3.Distance(Player.position, Self.position);

				var visible = angle <= ReactionAngle;
				var visibility = distance <= ReactionDistance;

				return visibility && visible;
			}

			public override void Exit()
			{
				_subscriptionList.Clear();
			}
		}

		/// <summary>
		///     ステート：追跡
		/// </summary>
		private class StatePursuit : State<Spider>
		{
			private Transform Self { get { return Owner.transform; } }
			private Transform Player { get { return Owner._playerTransform; } }

			public StatePursuit(Spider owner) : base(owner)
			{
			}

			public override void Execute()
			{
				if (!(Vector3.Distance(Owner.transform.position, Owner._playerTransform.position) < 3f))
				{
					Self.position = Vector3.Lerp(Self.position, Player.position, Time.deltaTime);
				}
				else
				{
					Owner.ChangeState(SpiderState.Explode);
				}
			}
		}

		/// <summary>
		///     ステート：自爆
		/// </summary>
		private class StateExplosion : State<Spider>
		{
			public StateExplosion(Spider owner) : base(owner)
			{
			}

			public override void Enter()
			{
				Observable.Timer(TimeSpan.FromSeconds(1))
					.Subscribe(_ => { Destroy(Owner.gameObject); });
			}
		}
	}
}