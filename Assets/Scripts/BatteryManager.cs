using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace KyleGame
{
	public class BatteryManager : MonoBehaviour
	{
		[SerializeField]
		private bool _isDischarging;

		private readonly FloatReactiveProperty _currentBatteryAmount = new FloatReactiveProperty();
		public IReadOnlyReactiveProperty<float> CurrentBatteryAmount { get { return _currentBatteryAmount; } }

		private Image _batteryIcon;
		private const float MaxUseTime = 20f;

		private BoolReactiveProperty _useBattery = new BoolReactiveProperty(false);

		private void Start()
		{
			_batteryIcon = GetComponent<Image>();

			_currentBatteryAmount.Value = MaxUseTime;

			this.UpdateAsObservable().Where(_ => _useBattery.Value).Subscribe(_ =>
			{

			});
		}
	}
}