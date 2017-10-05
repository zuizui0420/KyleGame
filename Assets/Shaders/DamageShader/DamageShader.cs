using UnityEngine;

public class DamageShader : MonoBehaviour
{
	[SerializeField]
	private Material _damageMaterial;

	[Range(0, 1)]
	public float damageRatio;

	private float DamageRatio
	{
		get { return _damageMaterial.GetFloat("_DamageRatio"); }
		set { _damageMaterial.SetFloat("_DamageRatio", value); }
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		DamageRatio = damageRatio;
		Graphics.Blit(src, dest, _damageMaterial);
	}
}