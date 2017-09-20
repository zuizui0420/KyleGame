using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace KyleGame
{
	public partial class Drone
	{
		private Vector3 _origin;

		/// <summary>
		///     ステート：巡回
		/// </summary>
		private class StateWander : State<Drone>
		{
			private const float ReactionAngle = 90;
			private const float ReactionDistance = 10f;

			private readonly CompositeDisposable _disposableList = new CompositeDisposable();


			public StateWander(Drone owner) : base(owner)
			{
			}


			/// <summary>
			///     Alias: Owner.transform
			/// </summary>
			private Transform Self
			{
				get { return Owner.transform; }
			}

			/// <summary>
			///     Alias: Owner._body
			/// </summary>
			private Transform Body
			{
				get { return Owner._body; }
			}

			/// <summary>
			///     Alias: Owner._playerTrnsform
			/// </summary>
			private Transform Player
			{
				get { return Owner._playerTransform; }
			}

			public override void Enter()
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

			public override void Exit()
			{
				_disposableList.Clear();
			}

			public override void Execute()
			{
			}

			private Vector3 NextDestination()
			{
				var ret = Owner._destinationAnchorList[Owner._anchorNum].position;
				Owner._anchorNum = ++Owner._anchorNum >= Owner._destinationAnchorList.Length ? 0 : Owner._anchorNum;

				return ret;
			}

			private bool OnObjectReflectedInOwnerEyes()
			{
				var angle = Vector3.Angle((Player.position - Self.transform.position).normalized, Self.forward);
				var distance = Vector3.Distance(Player.position, Self.position);

				var visible = angle <= ReactionAngle;
				var visibility = distance <= ReactionDistance;

				return visibility && visible;
			}

			private IEnumerator WandererCoroutine(CancellationToken token)
			{
				var startTime = Time.timeSinceLevelLoad;

				while (true)
				{
					if (token.IsCancellationRequested) break;

					var origin = Self.position;
					var destination = NextDestination();
					Observable.FromCoroutine(_ => HeadRotateCoroutine(destination, token)).Subscribe();

					while (true)
					{
						if (token.IsCancellationRequested) break;

						var diff = Time.timeSinceLevelLoad - startTime;
						if (diff > 5.0f)
						{
							startTime = Time.timeSinceLevelLoad;
							break;
						}

						var rate = diff / 5.0f;

						Self.position = Vector3.Lerp(origin, destination, rate);
						yield return null;
					}
				}
			}

			

			private IEnumerator HeadRotateCoroutine(Vector3 destination, CancellationToken token)
			{
				var orig = Body.rotation;
				var direction = destination - Self.position;
				direction.y = 0;
				var dest = Quaternion.LookRotation(direction);

				var startTime = Time.timeSinceLevelLoad;

				while (true)
				{
					if (token.IsCancellationRequested)
					{
						Body.rotation = dest;
						yield break;
					}

					var diff = Time.timeSinceLevelLoad - startTime;
					if (diff > 0.5f)
						break;

					var rate = diff / 0.5f;

					Body.rotation = Quaternion.Slerp(orig, dest, rate);
					yield return null;
				}
			}
		}

		/// <summary>
		///     ステート：追跡
		/// </summary>
		private class StatePursuit : State<Drone>
		{
			private const float MaxDemarcationDistance = 15f;
			private const float RelativeDistance = 3f;

			private float _startTime;

			private Transform _playerCenter;

			/// <summary>
			/// Alias: Owner.transform
			/// </summary>
			private Transform Self
			{
				get { return Owner.transform; }
			}

			/// <summary>
			/// Alias: Owner._body;
			/// </summary>
			private Transform Body
			{
				get { return Owner._body; }
			}

			/// <summary>
			/// Alias: Owner._playerTransform
			/// </summary>
			private Transform Player
			{
				get { return Owner._playerTransform; }
			}

			public StatePursuit(Drone owner) : base(owner)
			{
			}

			public override void Enter()
			{
				_startTime = Time.timeSinceLevelLoad;

				_playerCenter = Player.Find("Center");
			}

			public override void Execute()
			{
				if (Vector3.Distance(Self.position, Owner._origin) >= MaxDemarcationDistance)
				{
					Owner.ChangeState(DroneState.Return);
					return;
				}

				var time = Time.timeSinceLevelLoad - _startTime;
				if (time >= 4.0f)
				{
					Owner.ChangeState(DroneState.Attack);
					return;
				}

				if (Vector3.Distance(Self.position, Player.position) >= RelativeDistance)
				{
					var direction = (Player.position - Self.position).normalized;

					Self.position += direction * 5.0f * Time.deltaTime;
				}
				Body.LookAt(_playerCenter);
			}
		}

		/// <summary>
		///     ステート：攻撃
		/// </summary>
		private class StateAttack : State<Drone>
		{
			public StateAttack(Drone owner) : base(owner)
			{
			}

			public override void Enter()
			{
				Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
				{
					Owner.ChangeState(DroneState.Pursuit);
				});
			}
		}

		/// <summary>
		/// ステート：帰投
		/// </summary>
		private class StateReturn : State<Drone>
		{
			public StateReturn(Drone owner) : base(owner)
			{
			}

			/// <summary>
			/// Alias: Owner.transform
			/// </summary>
			private Transform Self
			{
				get { return Owner.transform; }
			}

			/// <summary>
			/// Alias: Owner._body;
			/// </summary>
			private Transform Body
			{
				get { return Owner._body; }
			}

			/// <summary>
			/// Alias: Owner._playerTransform
			/// </summary>
			private Transform Player
			{
				get { return Owner._playerTransform; }
			}

			public override void Enter()
			{
				Observable.FromCoroutine(WandererCoroutine).TakeUntilDestroy(Owner).Subscribe(_ =>
				{
					Owner.ChangeState(DroneState.Wander);
				});
			}

			private IEnumerator WandererCoroutine(CancellationToken token)
			{
				var startTime = Time.timeSinceLevelLoad;

				var origin = Self.position;
				var destination = Owner._origin;
				Observable.FromCoroutine(_ => HeadRotateCoroutine(destination, token)).Subscribe();

				while (true)
				{
					if (token.IsCancellationRequested) break;

					var diff = Time.timeSinceLevelLoad - startTime;
					if (diff > 5.0f)
					{
						yield break;
					}

					var rate = diff / 5.0f;

					Self.position = Vector3.Lerp(origin, destination, rate);
					yield return null;
				}
			}

			private IEnumerator HeadRotateCoroutine(Vector3 destination, CancellationToken token)
			{
				var orig = Body.rotation;
				var direction = destination - Self.position;
				direction.y = 0;
				var dest = Quaternion.LookRotation(direction);

				var startTime = Time.timeSinceLevelLoad;

				while (true)
				{
					if (token.IsCancellationRequested)
					{
						Body.rotation = dest;
						yield break;
					}

					var diff = Time.timeSinceLevelLoad - startTime;
					if (diff > 0.5f)
						break;

					var rate = diff / 0.5f;

					Body.rotation = Quaternion.Slerp(orig, dest, rate);
					yield return null;
				}
			}
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