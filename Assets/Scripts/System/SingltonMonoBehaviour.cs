using UnityEngine;

/// <summary>
///     シングルトン勝手にやるマン
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviourExtension where T : SingletonMonoBehaviour<T>
{
	protected static readonly string[] findTags =
	{
		"GameController"
	};

	protected static T instance;

	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				var type = typeof(T);

				foreach (var tag in findTags)
				{
					var objs = GameObject.FindGameObjectsWithTag(tag);

					for (var j = 0; j < objs.Length; j++)
					{
						instance = (T) objs[j].GetComponent(type);
						if (instance != null)
							return instance;
					}
				}

				Debug.LogWarning(string.Format("{0} is not found", type.Name));
			}

			return instance;
		}
	}

	protected virtual void Awake()
	{
		CheckInstance();
	}

	protected bool CheckInstance()
	{
		if (instance == null)
		{
			instance = (T) this;
			return true;
		}
		if (Instance == this)
		{
			return true;
		}

		Destroy(gameObject);
		return false;
	}
}