using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020001B4 RID: 436
	public class CombineMesh : MonoBehaviour
	{
		// Token: 0x0600095C RID: 2396 RVA: 0x00028D70 File Offset: 0x00026F70
		private void Start()
		{
			Renderer component = base.GetComponent<Renderer>();
			MeshFilter[] componentsInChildren = base.GetComponentsInChildren<MeshFilter>();
			CombineInstance[] array = new CombineInstance[componentsInChildren.Length];
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				array[i].mesh = componentsInChildren[i].sharedMesh;
				array[i].transform = componentsInChildren[i].transform.localToWorldMatrix;
				componentsInChildren[i].gameObject.SetActive(false);
			}
			MeshFilter meshFilter = base.gameObject.AddComponent<MeshFilter>();
			meshFilter.mesh = new Mesh();
			meshFilter.mesh.CombineMeshes(array, true, true, true);
			component.material = componentsInChildren[0].GetComponent<Renderer>().sharedMaterial;
			base.gameObject.SetActive(true);
		}
	}
}
