using UnityEngine;
using System.Collections;

public class FadeShader : MonoBehaviour {

	//[SerializeField]
	Material m_Material;

	[Range(0, 1)]
	public float fade;

	public bool isAddisive;

	void Start() {
		m_Material = new Material(Shader.Find("Custom/Fader"));

	}

	void MaterialUpdate() {

		int isAdd = 0;
		if(isAddisive) {
			isAdd = 1;
		}

		m_Material.SetFloat("_Fade", fade);
		m_Material.SetInt("_IsAddisive", isAdd);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dest) {

		MaterialUpdate();
		Graphics.Blit(src, dest, m_Material);
	}
}
