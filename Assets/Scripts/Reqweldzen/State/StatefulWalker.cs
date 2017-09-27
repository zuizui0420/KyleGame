using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace KyleGame
{
	[Serializable]
	public struct DestinationData
	{
		public Transform[] DestinationList;
	}
	
	[RequireComponent(typeof(NavMeshAgent))]
	public abstract class StatefulWalker<T, TEnum> : StatefulEnemyComponentBase<T, TEnum>
		where T : class where TEnum : IConvertible
	{
		protected readonly Vector3ReactiveProperty Destination = new Vector3ReactiveProperty();
		protected readonly FloatReactiveProperty MovementSpeed = new FloatReactiveProperty();

		private int _anchorNum;

		private NavMeshAgent _navMeshAgent;

		private Vector3 _origin;

		private bool _resume;

		public float ReactionAngle;
		public float ReactionDistance;

		public DestinationData DestinationData;

		protected abstract Transform PlayerTransform { get; }

		protected override void OnInitialize()
		{
			_navMeshAgent = GetComponent<NavMeshAgent>();

			Destination.TakeUntilDestroy(this).Subscribe(x => _navMeshAgent.destination = x);
			MovementSpeed.TakeUntilDestroy(this).Subscribe(x => _navMeshAgent.speed = x);
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

			protected NavMeshAgent Agent { get { return Owner._navMeshAgent; } }

			protected float ReactionAngle { get; set; }
			protected float ReactionDistance { get; set; }

			protected Vector3 Origin
			{
				get { return Owner._origin; }
				set { Owner._origin = value; }
			}

			protected bool Resume
			{
				get { return Owner._resume; }
				set { Owner._resume = value; }
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
				get { return Owner.MovementSpeed.Value; }
				set { Owner.MovementSpeed.Value = value; }
			}

			/// <summary>
			///     Alias: Owner._destination
			/// </summary>
			protected Vector3 Destination
			{
				get { return Owner.Destination.Value; }
				set { Owner.Destination.Value = value; }
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

				//Debug.Log(string.Format("Angle: {0}, {1}\nDistance: {2}, {3}", angle, visible, distance, visibility));

				return visibility && visible;
			}

			protected IEnumerator WandererCoroutine(Func<IEnumerator> onArrivalDestination, CancellationToken token)
			{
				while (!token.IsCancellationRequested)
				{
					if (Resume)
					{
						Destination = Origin;
						Resume = false;
						Origin = Vector3.zero;
					}
					else
					{
						Destination = Owner.NextDestination();
					}
					while (!token.IsCancellationRequested)
					{
						if (GetDistance(Destination) < 1.0)
						{
							yield return onArrivalDestination();
							break;
						}

						yield return null;
					}
				}
			}
		}
	}
}