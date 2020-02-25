using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003B9 RID: 953
	public struct LayerIndex
	{
		// Token: 0x170002B0 RID: 688
		// (get) Token: 0x06001716 RID: 5910 RVA: 0x000646F9 File Offset: 0x000628F9
		public LayerMask mask
		{
			get
			{
				return (this.intVal >= 0) ? (1 << this.intVal) : this.intVal;
			}
		}

		// Token: 0x06001717 RID: 5911 RVA: 0x0006471C File Offset: 0x0006291C
		static LayerIndex()
		{
			for (int i = 0; i < 32; i++)
			{
				string text = LayerMask.LayerToName(i);
				if (text != "" && (LayerIndex.assignedLayerMask & 1U << i) == 0U)
				{
					Debug.LogWarningFormat("Layer \"{0}\" is defined in this project's \"Tags and Layers\" settings but is not defined in LayerIndex!", new object[]
					{
						text
					});
				}
			}
		}

		// Token: 0x170002B1 RID: 689
		// (get) Token: 0x06001718 RID: 5912 RVA: 0x000648AF File Offset: 0x00062AAF
		public LayerMask collisionMask
		{
			get
			{
				return LayerIndex.collisionMasks[this.intVal];
			}
		}

		// Token: 0x06001719 RID: 5913 RVA: 0x000648C4 File Offset: 0x00062AC4
		private static LayerIndex GetLayerIndex(string layerName)
		{
			LayerIndex layerIndex = new LayerIndex
			{
				intVal = LayerMask.NameToLayer(layerName)
			};
			if (layerIndex.intVal == LayerIndex.invalidLayer.intVal)
			{
				Debug.LogErrorFormat("Layer \"{0}\" is not defined in this project's \"Tags and Layers\" settings.", new object[]
				{
					layerName
				});
			}
			else
			{
				LayerIndex.assignedLayerMask |= 1U << layerIndex.intVal;
			}
			return layerIndex;
		}

		// Token: 0x0600171A RID: 5914 RVA: 0x00064928 File Offset: 0x00062B28
		private static LayerMask[] CalcCollisionMasks()
		{
			LayerMask[] array = new LayerMask[32];
			for (int i = 0; i < 32; i++)
			{
				LayerMask layerMask = default(LayerMask);
				for (int j = 0; j < 32; j++)
				{
					if (!Physics.GetIgnoreLayerCollision(i, j))
					{
						layerMask |= 1 << j;
					}
				}
				array[i] = layerMask;
			}
			return array;
		}

		// Token: 0x040015F8 RID: 5624
		public int intVal;

		// Token: 0x040015F9 RID: 5625
		private static uint assignedLayerMask = 0U;

		// Token: 0x040015FA RID: 5626
		public static readonly LayerIndex invalidLayer = new LayerIndex
		{
			intVal = -1
		};

		// Token: 0x040015FB RID: 5627
		public static readonly LayerIndex defaultLayer = LayerIndex.GetLayerIndex("Default");

		// Token: 0x040015FC RID: 5628
		public static readonly LayerIndex transparentFX = LayerIndex.GetLayerIndex("TransparentFX");

		// Token: 0x040015FD RID: 5629
		public static readonly LayerIndex ignoreRaycast = LayerIndex.GetLayerIndex("Ignore Raycast");

		// Token: 0x040015FE RID: 5630
		public static readonly LayerIndex water = LayerIndex.GetLayerIndex("Water");

		// Token: 0x040015FF RID: 5631
		public static readonly LayerIndex ui = LayerIndex.GetLayerIndex("UI");

		// Token: 0x04001600 RID: 5632
		public static readonly LayerIndex fakeActor = LayerIndex.GetLayerIndex("FakeActor");

		// Token: 0x04001601 RID: 5633
		public static readonly LayerIndex noCollision = LayerIndex.GetLayerIndex("NoCollision");

		// Token: 0x04001602 RID: 5634
		public static readonly LayerIndex pickups = LayerIndex.GetLayerIndex("Pickups");

		// Token: 0x04001603 RID: 5635
		public static readonly LayerIndex world = LayerIndex.GetLayerIndex("World");

		// Token: 0x04001604 RID: 5636
		public static readonly LayerIndex entityPrecise = LayerIndex.GetLayerIndex("EntityPrecise");

		// Token: 0x04001605 RID: 5637
		public static readonly LayerIndex debris = LayerIndex.GetLayerIndex("Debris");

		// Token: 0x04001606 RID: 5638
		public static readonly LayerIndex projectile = LayerIndex.GetLayerIndex("Projectile");

		// Token: 0x04001607 RID: 5639
		public static readonly LayerIndex manualRender = LayerIndex.GetLayerIndex("ManualRender");

		// Token: 0x04001608 RID: 5640
		public static readonly LayerIndex background = LayerIndex.GetLayerIndex("Background");

		// Token: 0x04001609 RID: 5641
		public static readonly LayerIndex ragdoll = LayerIndex.GetLayerIndex("Ragdoll");

		// Token: 0x0400160A RID: 5642
		public static readonly LayerIndex noDraw = LayerIndex.GetLayerIndex("NoDraw");

		// Token: 0x0400160B RID: 5643
		public static readonly LayerIndex prefabBrush = LayerIndex.GetLayerIndex("PrefabBrush");

		// Token: 0x0400160C RID: 5644
		public static readonly LayerIndex postProcess = LayerIndex.GetLayerIndex("PostProcess");

		// Token: 0x0400160D RID: 5645
		public static readonly LayerIndex uiWorldSpace = LayerIndex.GetLayerIndex("UI, WorldSpace");

		// Token: 0x0400160E RID: 5646
		private static readonly LayerMask[] collisionMasks = LayerIndex.CalcCollisionMasks();

		// Token: 0x020003BA RID: 954
		public static class CommonMasks
		{
			// Token: 0x0400160F RID: 5647
			public static readonly LayerMask bullet = LayerIndex.world.mask | LayerIndex.entityPrecise.mask;
		}
	}
}
