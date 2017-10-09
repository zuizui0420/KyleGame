using KyleGame;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettingInstaller", menuName = "Installers/GameSettingInstaller")]
public class GameSettingInstaller : ScriptableObjectInstaller<GameSettingInstaller>
{
	public GameSetting GameSetting;

    public override void InstallBindings()
    {
	    Container.BindInstance(GameSetting);
    }
}