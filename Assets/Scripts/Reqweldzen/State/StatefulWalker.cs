using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace KyleGame
{
	[Serializable]
	public struct DestinationData
	{
		public Transform[] DestinationList;
	}

	[RequireComponent(typeof(EnemyCharacterController))]
	public abstract class StatefulWalker<T, TEnum> : StatefulEnemyComponentBase<T, TEnum>
		where T : class where TEnum : IConvertible
	{
		private readonly Vector3ReactiveProperty _destination = new Vector3ReactiveProperty();
		private readonly FloatReactiveProperty _movementSpeed = new FloatReactiveProperty();

		private int _anchorNum;

		private EnemyCharacterController _characterController;

		private Vector3 _origin;

		public DestinationData DestinationData;

		protected abstract Transform PlayerTransform { get; }

		protected override void OnInitialize()
		{
			_characterController = GetComponent<EnemyCharacterController>();
		}

		protected Vector3 NextDestination()
		{
			var ret = DestinationData.DestinationList[_anchorNum].position;
			_anchorNum = ++_anchorNum >= DestinationData.DestinationList.Length ? 0 : _anchorNum;

			return ret;
		}

		protected class WalkerStateBase<TOwner> : State<TOwner> where TOwner : StatefulWalker<TOwner, TEnum>
		{
			public WalkerStateBase(TOwner owner) : base(owner)
			{
			}

			protected float ReactionAngle { get; set; }
			protected float ReactionDistance { get; set; }

			protected Vector3 Origin
			{
				get { return Owner._origin; }
				set { Owner._origin = value; }
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
				get { return Owner.PlayerTransform; }
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

			protected bool OnObjectReflectedInOwnerEyes()
			{
				var angle = Vector3.Angle(GetDirection(Player), Self.forward);
				var distance = GetDistance(Player);

				var visible = angle <= ReactionAngle;
				var visibility = distance <= ReactionDistance;

				return visibility && visible;
			}

			protected IEnumerator WandererCoroutine(CancellationToken token)
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
		}
	}
}