using UniRx;

public interface ISpiderAnimation
{
	/// <summary>
	///     �ړ��X�s�[�h
	/// </summary>
	float Speed { set; }

	void Spark();
	IObservable<Unit> Suicide();
	IObservable<Unit> Dead();
}