namespace KyleGame
{
	public interface ISparkDrivenObject
	{
		bool IsActive { get; set; }

		void OnHitSpark();
	}
}