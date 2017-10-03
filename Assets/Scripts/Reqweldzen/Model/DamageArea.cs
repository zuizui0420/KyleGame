using UnityEngine;

namespace KyleGame
{
	public class DamageArea : MonoBehaviour
	{
		private PlayerSystem _playerSystem;

		private void Awake()
		{
			_playerSystem = GetComponent<PlayerSystem>();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.name.Equals("DamageArea"))
			{
				_playerSystem.Damage();
			}
		}
	}
}