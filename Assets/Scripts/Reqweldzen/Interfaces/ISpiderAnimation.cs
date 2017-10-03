using UniRx;

public interface ISpiderAnimation
{
	/// <summary>
	///     移動スピード
	/// </summary>
	float Speed { set; }

	void Spark();
	IObservable<Unit> Suicide();
	IObservable<Unit> Dead();
}