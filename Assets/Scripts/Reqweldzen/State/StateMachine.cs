public class StateMachine<T>
{
	public StateMachine()
	{
		CurrentState = null;
	}

	public State<T> CurrentState { get; private set; }

	public void ChangeState(State<T> state)
	{
		if (CurrentState != null)
			CurrentState.Exit();

		CurrentState = state;

		CurrentState.Enter();
	}

	public void Update()
	{
		if (CurrentState != null)
			CurrentState.Execute();
	}
}