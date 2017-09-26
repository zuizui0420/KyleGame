public interface ISpiderAnimation
{
	/// <summary>
	///     移動スピード
	/// </summary>
	float Speed { set; }

	void Spark();
	void Suicide();
}