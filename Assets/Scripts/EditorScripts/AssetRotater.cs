using UnityEngine;
namespace Ghost
{
	public class AssetRotater : MonoBehaviour
	{
		[Header("Rotation")]
		public bool m_RotateChildren;

		[Header("Scaling")]
		public bool m_LockXAndZFactors;
		public bool m_ScaleChildren;

		public Vector2 m_XScaleBounds = new Vector2(1, 1);
		public Vector2 m_YScaleBounds = new Vector2(1, 1);
		public Vector2 m_ZScaleBounds = new Vector2(1, 1);

		private void OnValidate()
		{
			if (m_LockXAndZFactors)
			{
				m_ZScaleBounds = m_XScaleBounds;
			}
		}

		[ContextMenu("Use Grass Preset")]
		void GrassPreset()
		{
			m_RotateChildren = true;
			m_ScaleChildren = true;
			m_LockXAndZFactors = true;
			m_XScaleBounds = new Vector2(0.75f, 1.25f);
			m_ZScaleBounds = new Vector2(0.75f, 1.25f);
		}


		[ContextMenu("Modify Transforms")]
		void ModifyTransforms()
		{
			foreach (Transform child in transform)
			{
				if (m_ScaleChildren)
				{
					child.localScale = new Vector3(
						Random.Range(m_XScaleBounds.x, m_XScaleBounds.y),
						Random.Range(m_YScaleBounds.x, m_YScaleBounds.y),
						Random.Range(m_ZScaleBounds.x, m_ZScaleBounds.y));
				}

				if (m_RotateChildren)
				{
					child.transform.rotation = Quaternion.Euler(0, Random.Range(-180f, 180f), 0);
				}
			}
		}
	}
}

