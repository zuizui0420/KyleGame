using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace KyleGame
{
	public partial class Drone
	{
		private readonly Vector3ReactiveProperty _destination = new Vector3ReactiveProperty();
		private readonly FloatReactiveProperty _movementSpeed = new FloatReactiveProperty();

		private Vector3 _origin;

		private abstract class DroneStateBase : State<Drone>
		{
			protected DroneStateBase(Drone owner) : base(owner)
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
		private class StateWander : DroneStateBase
		{
			private const float ReactionAngle = 90;
			private const float ReactionDistance = 5f;

			private readonly CompositeDisposable _disposableList = new CompositeDisposable();


			public StateWander(Drone owner) : base(owner)
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
					.Subscribe(_ =>
					{
						Owner._origin = Self.position;
						Owner.ChangeState(DroneState.Pursuit);
					})
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
			//	var orig = Body.rotation;
			//	var direction = destination - Self.position;
			//	direction.y = 0;
			//	var dest = Quaternion.LookRotation(direction);

			//	var startTime = Time.timeSinceLevelLoad;

			//	while (true)
			//	{
			//		if (token.IsCancellationRequested)
			//		{
			//			Body.rotation = dest;
			//			yield break;
			//		}

			//		var diff = Time.timeSinceLevelLoad - startTime;
			//		if (diff > 0.5f)
			//			break;

			//		var rate = diff / 0.5f;

			//		Body.rotation = Quaternion.Slerp(orig, dest, rate);
			//		yield return null;
			//	}
			//}
		}

		/// <summary>
		///     ステート：追跡
		/// </summary>
		private class StatePursuit : DroneStateBase
		{
			private const float MaxDemarcationDistance = 15f;
			private const float RelativeDistance = 3f;

			private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

			public StatePursuit(Drone owner) : base(owner)
			{
			}

			public override void Enter()
			{
				MovementSpeed = 3f;

				Observable.Return(Unit.Default)
					.Delay(TimeSpan.FromSeconds(3))
					.Subscribe(_ =>
					{
						Owner.ChangeState(DroneState.Attack);
					})
					.AddTo(_compositeDisposable);
			}

			public override void Execute()
			{
				if (GetDistance(Owner._origin) >= MaxDemarcationDistance)
				{
					Owner.ChangeState(DroneState.Return);
				}
				else if(!(GetDistance(Player) < RelativeDistance))
				{
					Destination = Player.position;

					var moveDir = GetDirection(Destination) * MovementSpeed;

					Move(moveDir);
				}
			}

			public override void Exit()
			{
				_compositeDisposable.Clear();
			}
		}

		/// <summary>
		///     ステート：攻撃
		/// </summary>
		private class StateAttack : DroneStateBase
		{
			public StateAttack(Drone owner) : base(owner)
			{
			}

			public override void Enter()
			{
				Debug.Log("Attack");
				Observable.NextFrame()
					.Subscribe(_ => Owner.ChangeState(DroneState.Pursuit));
			}
		}

		/// <summary>
		/// ステート：帰投
		/// </summary>
		private class StateReturn : DroneStateBase
		{
			public StateReturn(Drone owner) : base(owner)
			{
			}

			public override void Enter()
			{
				Observable.FromCoroutine(WandererCoroutine)
					.TakeUntilDestroy(Owner)
					.Subscribe(_ =>
					{
						Owner.ChangeState(DroneState.Wander);
					});
			}

			private IEnumerator WandererCoroutine(CancellationToken token)
			{
				Destination = Owner._origin;

				while (!token.IsCancellationRequested)
				{
					var moveDir = GetDirection(Destination) * MovementSpeed;

					Move(moveDir);

					if (GetDistance(Destination) < 1.0f)
						break;

					yield return null;
				}
			}

			//private IEnumerator HeadRotateCoroutine(Vector3 destination, CancellationToken token)
			//{
			//	var orig = Body.rotation;
			//	var direction = destination - Self.position;
			//	direction.y = 0;
			//	var dest = Quaternion.LookRotation(direction);

			//	var startTime = Time.timeSinceLevelLoad;

			//	while (true)
			//	{
			//		if (token.IsCancellationRequested)
			//		{
			//			Body.rotation = dest;
			//			yield break;
			//		}

			//		var diff = Time.timeSinceLevelLoad - startTime;
			//		if (diff > 0.5f)
			//			break;

			//		var rate = diff / 0.5f;

			//		Body.rotation = Quaternion.Slerp(orig, dest, rate);
			//		yield return null;
			//	}
			//}
		}
	}

	public enum DroneState
	{
		Wander,
		Pursuit,
		Attack,
		Return,
	}
}