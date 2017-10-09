namespace KyleGame
{
	public interface ILaserDrivenObject
	{
		bool IsActive { get; set; }

		void OnHitLaser();
	}
}