using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200006F RID: 111
public static class RendererSetMaterialsExtension
{
	// Token: 0x060001C8 RID: 456 RVA: 0x000097C0 File Offset: 0x000079C0
	private static void InitSharedMaterialsArrays(int maxMaterials)
	{
		RendererSetMaterialsExtension.sharedMaterialArrays = new Material[maxMaterials + 1][];
		if (maxMaterials > 0)
		{
			RendererSetMaterialsExtension.sharedMaterialArrays[0] = Array.Empty<Material>();
			for (int i = 1; i < RendererSetMaterialsExtension.sharedMaterialArrays.Length; i++)
			{
				RendererSetMaterialsExtension.sharedMaterialArrays[i] = new Material[i];
			}
		}
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x00009809 File Offset: 0x00007A09
	static RendererSetMaterialsExtension()
	{
		RendererSetMaterialsExtension.InitSharedMaterialsArrays(16);
	}

	// Token: 0x060001CA RID: 458 RVA: 0x00009814 File Offset: 0x00007A14
	public static void SetSharedMaterials(this Renderer renderer, Material[] sharedMaterials, int count)
	{
		if (RendererSetMaterialsExtension.sharedMaterialArrays.Length < count)
		{
			RendererSetMaterialsExtension.InitSharedMaterialsArrays(count);
		}
		Material[] array = RendererSetMaterialsExtension.sharedMaterialArrays[count];
		Array.Copy(sharedMaterials, array, count);
		renderer.sharedMaterials = array;
		Array.Clear(array, 0, count);
	}

	// Token: 0x060001CB RID: 459 RVA: 0x00009850 File Offset: 0x00007A50
	public static void SetSharedMaterials(this Renderer renderer, List<Material> sharedMaterials)
	{
		int count = sharedMaterials.Count;
		if (RendererSetMaterialsExtension.sharedMaterialArrays.Length < count)
		{
			RendererSetMaterialsExtension.InitSharedMaterialsArrays(count);
		}
		Material[] array = RendererSetMaterialsExtension.sharedMaterialArrays[count];
		sharedMaterials.CopyTo(array, 0);
		renderer.sharedMaterials = array;
		Array.Clear(array, 0, count);
	}

	// Token: 0x060001CC RID: 460 RVA: 0x00009894 File Offset: 0x00007A94
	public static void SetMaterials(this Renderer renderer, Material[] materials, int count)
	{
		if (RendererSetMaterialsExtension.sharedMaterialArrays.Length < count)
		{
			RendererSetMaterialsExtension.InitSharedMaterialsArrays(count);
		}
		Material[] array = RendererSetMaterialsExtension.sharedMaterialArrays[count];
		Array.Copy(materials, array, count);
		renderer.materials = array;
		Array.Clear(array, 0, count);
	}

	// Token: 0x060001CD RID: 461 RVA: 0x000098D0 File Offset: 0x00007AD0
	public static void SetMaterials(this Renderer renderer, List<Material> materials)
	{
		int count = materials.Count;
		if (RendererSetMaterialsExtension.sharedMaterialArrays.Length < count)
		{
			RendererSetMaterialsExtension.InitSharedMaterialsArrays(count);
		}
		Material[] array = RendererSetMaterialsExtension.sharedMaterialArrays[count];
		materials.CopyTo(array, 0);
		renderer.materials = array;
		Array.Clear(array, 0, count);
	}

	// Token: 0x040001D7 RID: 471
	private static Material[][] sharedMaterialArrays;
}
