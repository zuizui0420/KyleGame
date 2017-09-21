using UnityEngine;
using System.Collections;

public class DamageShader : MonoBehaviour {

	[SerializeField]
	Material m_Material;

	[Range(0, 1)]
	public float damageRatio;

	void Start() {
		//m_Material = new Material(Shader.Find("Custom/Damage"));

	}

	void MaterialUpdate() {

		m_Material.SetFloat("_DamageRatio", damageRatio);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest) {

		MaterialUpdate();
		Graphics.Blit(src, dest, m_Material);
	}
}
