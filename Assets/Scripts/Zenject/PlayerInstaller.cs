using KyleGame.ViewModel;
using UnityEngine;
using Zenject;

namespace KyleGame
{
	public class PlayerInstaller : MonoInstaller<PlayerInstaller>
	{
		public GameObject PlayerPrefab;
		public GameObject BulletPrefab;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<PlayerSystem>().FromComponentInNewPrefab(PlayerPrefab).AsSingle();
		}
	}
}