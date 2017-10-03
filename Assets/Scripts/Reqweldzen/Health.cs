using UniRx;
using UnityEngine;

namespace KyleGame
{
	public class Health
	{
		private IntReactiveProperty _currentHealth;
		
		public int MaxHealth { get; private set; }

		public ReadOnlyReactiveProperty<bool> IsDead { get; private set; }

		public IReadOnlyReactiveProperty<int> CurrentHealth { get { return _currentHealth; } }

		private Health() { }

		public static Health Generate(int lifePoint)
		{
			var health = new Health
			{
				_currentHealth = new IntReactiveProperty(lifePoint),
				MaxHealth = lifePoint
			};
			health.IsDead = health._currentHealth.Select(x => x <= 0).ToReadOnlyReactiveProperty();

			return health;
		}

		private int ClampHp(float amount)
		{
			return Mathf.Clamp((int)amount, 0, MaxHealth);
		}

		public void TakeDamage(float amount)
		{
			_currentHealth.Value = ClampHp(_currentHealth.Value - amount);
		}

		public void RaiseDamage(float amount)
		{
			_currentHealth.Value = ClampHp(_currentHealth.Value + amount);
		}
	}
}