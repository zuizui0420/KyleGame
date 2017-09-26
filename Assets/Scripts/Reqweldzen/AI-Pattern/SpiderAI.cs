using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace KyleGame
{
	public partial class Spider
	{
		private readonly Vector3ReactiveProperty _destination = new Vector3ReactiveProperty();
		private readonly FloatReactiveProperty _movementSpeed = new FloatReactiveProperty();

		private abstract class SpiderStateBase : State<Spider>
		{
			protected SpiderStateBase(Spider owner) : base(owner)
			{
			}

			/// <summary>
			///     Alias: Owner.transform
			/// </summary>
			protected Transform Self
			{
				get { return Owner.transform; }
			}

			/// <summary>
			///     Alias: Owner._playerTransform
			/// </summary>
			protected Transform Player
			{
				get { return Owner._playerTransform; }
			}

			/// <summary>
			///     Alias: Owner._movementSpeed
			/// </summary>
			protected float MovementSpeed
			{
				get { return Owner._movementSpeed.Value; }
				set { Owner._movementSpeed.Value = value; }
			}

			/// <summary>
			///     Alias: Owner._destination
			/// </summary>
			protected Vector3 Destination
			{
				get { return Owner._destination.Value; }
				set { Owner._destination.Value = value; }
			}

			protected void Move(Vector3 moveDir)
			{
				Owner._characterController.Move(moveDir);
			}

			protected void Stop()
			{
				Move(Vector3.zero);
				MovementSpeed = 0;
			}

			protected Vector3 GetDirection(Vector3 destination)
			{
				return (destination - Self.position).normalized;
			}

			protected Vector3 GetDirection(Transform target)
			{
				return (target.position - Self.position).normalized;
			}

			protected float GetDistance(Vector3 destination)
			{
				return Vector3.Distance(Self.position, destination);
			}

			protected float GetDistance(Transform target)
			{
				return Vector3.Distance(Self.position, target.position);
			}
		}

		/// <summary>
		///     ステート：巡回
		/// </summary>
		private class StateWander : SpiderStateBase
		{
			private const float ReactionAngle = 90f;
			private const float ReactionDistance = 5f;

			private readonly CompositeDisposable _disposableList = new CompositeDisposable();


			public StateWander(Spider owner) : base(owner)
			{
			}

			public override void Enter()
			{
				MovementSpeed = 1.5f;

				ObserveStart();
			}

			public override void Exit()
			{
				_disposableList.Clear();
			}

			private void ObserveStart()
			{
				// プレイヤーが視界に入ったら追跡
				Owner.UpdateAsObservable()
					.Where(_ => OnObjectReflectedInOwnerEyes())
					.TakeUntilDestroy(Owner)
					.Subscribe(_ => { Owner.ChangeState(SpiderState.Pursuit); })
					.AddTo(_disposableList);

				Observable.FromCoroutine(WandererCoroutine).TakeUntilDestroy(Owner).Subscribe().AddTo(_disposableList);
			}

			private bool OnObjectReflectedInOwnerEyes()
			{
				var angle = Vector3.Angle(GetDirection(Player), Self.forward);
				var distance = GetDistance(Player);

				var visible = angle <= ReactionAngle;
				var visibility = distance <= ReactionDistance;

				return visibility && visible;
			}

			private IEnumerator WandererCoroutine(CancellationToken token)
			{
				while (!token.IsCancellationRequested)
				{
					Destination = Owner.NextDestination();

					while (!token.IsCancellationRequested)
					{
						var moveDir = GetDirection(Destination) * MovementSpeed;

						Move(moveDir);

						if (GetDistance(Destination) < 1.0f)
							break;

						yield return null;
					}
				}
			}

			//private IEnumerator HeadRotateCoroutine(Vector3 destination, CancellationToken token)
			//{
			//	var orig = Self.rotation;
			//	var direction = destination - Self.position;
			//	direction.y = 0;
			//	var dest = Quaternion.LookRotation(direction);

			//	var startTime = Time.timeSinceLevelLoad;

			//	while (true)
			//	{
			//		if (token.IsCancellationRequested)
			//		{
			//			Self.rotation = dest;
			//			yield break;
			//		}

			//		var diff = Time.timeSinceLevelLoad - startTime;
			//		if (diff > 0.25f)
			//			break;

			//		var rate = diff / 0.25f;

			//		Self.rotation = Quaternion.Slerp(orig, dest, rate);
			//		yield return null;
			//	}
			//}
		}

		/// <summary>
		///     ステート：追跡
		/// </summary>
		private class StatePursuit : SpiderStateBase
		{
			public StatePursuit(Spider owner) : base(owner)
			{
			}

			public override void Enter()
			{
				MovementSpeed = 5f;
				if (Owner._spiderType == SpiderType.InstantSpark)
				{
					Owner._spiderAnimation.Spark();
				}
			}

			public override void Execute()
			{
				if (!(GetDistance(Player) < 2f))
				{
					Destination = Player.position;

					var moveDir = GetDirection(Destination) * MovementSpeed;

					Move(moveDir);
				}
				else
				{
					Owner.ChangeState(SpiderState.Explode);
				}
			}

			public override void Exit()
			{
				Stop();
			}
		}

		/// <summary>
		///     ステート：自爆
		/// </summary>
		private class StateExplosion : SpiderStateBase
		{
			public StateExplosion(Spider owner) : base(owner)
			{
			}

			public override void Enter()
			{
				switch(Owner._spiderType)
					{
						case SpiderType.InstantSpark:
						Owner._spiderAnimation.Suicide();
							break;
						case SpiderType.TimerBomb:
						Owner._spiderAnimation.Spark();
							Owner._spiderAnimation.Suicide();
						break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				
			}
		}
	}
}