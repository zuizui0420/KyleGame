using System;

namespace KyleGame
{
	public abstract class StatefulBoss<T, TEnum> : StatefulEnemyComponentBase<T, TEnum>
		where T : class where TEnum : IConvertible
	{
		
	}
}