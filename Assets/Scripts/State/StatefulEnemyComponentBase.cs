using System;
using System.Collections.Generic;
using Assets.Scripts;

public abstract class StatefulEnemyComponentBase<T, TEnum> : BaseEnemyComponent
	where T : class where TEnum : IConvertible
{
	protected List<State<T>> StateList = new List<State<T>>();

	protected StateMachine<T> StateMachine;

	public virtual void ChangeState(TEnum state)
	{
		if (StateMachine == null) return;

		StateMachine.ChangeState(StateList[state.ToInt32(null)]);
	}

	public virtual bool IsCurrentState(TEnum state)
	{
		if (StateMachine == null) return false;

		return StateMachine.CurrentState == StateList[state.ToInt32(null)];
	}

	protected virtual void Update()
	{
		if (StateMachine != null)
			StateMachine.Update();
	}
}