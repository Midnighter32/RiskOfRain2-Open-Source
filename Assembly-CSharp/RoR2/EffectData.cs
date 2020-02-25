using System;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000127 RID: 295
	public class EffectData
	{
		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x06000546 RID: 1350 RVA: 0x000153CB File Offset: 0x000135CB
		// (set) Token: 0x06000547 RID: 1351 RVA: 0x000153D3 File Offset: 0x000135D3
		public Vector3 origin
		{
			get
			{
				return this._origin;
			}
			set
			{
				if (!Util.PositionIsValid(value))
				{
					Debug.LogFormat("EffectData.origin assignment position is invalid! Position={0}", new object[]
					{
						value
					});
					return;
				}
				this._origin = value;
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x06000548 RID: 1352 RVA: 0x000153FE File Offset: 0x000135FE
		// (set) Token: 0x06000549 RID: 1353 RVA: 0x00015406 File Offset: 0x00013606
		public GameObject rootObject { get; private set; } = EffectData.defaultRootObject;

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x0600054A RID: 1354 RVA: 0x0001540F File Offset: 0x0001360F
		// (set) Token: 0x0600054B RID: 1355 RVA: 0x00015417 File Offset: 0x00013617
		public short modelChildIndex { get; private set; } = EffectData.defaultModelChildIndex;

		// Token: 0x0600054C RID: 1356 RVA: 0x00015420 File Offset: 0x00013620
		public static void Copy([NotNull] EffectData src, [NotNull] EffectData dest)
		{
			dest.origin = src.origin;
			dest.rotation = src.rotation;
			dest.rootObject = src.rootObject;
			dest.modelChildIndex = src.modelChildIndex;
			dest.scale = src.scale;
			dest.color = src.color;
			dest.start = src.start;
			dest.surfaceDefIndex = src.surfaceDefIndex;
			dest.genericUInt = src.genericUInt;
			dest.genericFloat = src.genericFloat;
			dest.genericBool = src.genericBool;
			dest.networkSoundEventIndex = src.networkSoundEventIndex;
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x000154BD File Offset: 0x000136BD
		public void SetNetworkedObjectReference(GameObject networkedObject)
		{
			this.rootObject = networkedObject;
			this.modelChildIndex = -1;
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x000154CD File Offset: 0x000136CD
		public GameObject ResolveNetworkedObjectReference()
		{
			return this.rootObject;
		}

		// Token: 0x0600054F RID: 1359 RVA: 0x000154D8 File Offset: 0x000136D8
		public void SetHurtBoxReference(HurtBox hurtBox)
		{
			if (!hurtBox || !hurtBox.healthComponent)
			{
				this.rootObject = null;
				this.modelChildIndex = -1;
				return;
			}
			this.rootObject = hurtBox.healthComponent.gameObject;
			this.modelChildIndex = hurtBox.indexInGroup;
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x00015528 File Offset: 0x00013728
		public void SetHurtBoxReference(GameObject gameObject)
		{
			HurtBox hurtBox = (gameObject != null) ? gameObject.GetComponent<HurtBox>() : null;
			if (hurtBox)
			{
				this.SetHurtBoxReference(hurtBox);
				return;
			}
			this.rootObject = gameObject;
			this.modelChildIndex = -1;
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x00015560 File Offset: 0x00013760
		public GameObject ResolveHurtBoxReference()
		{
			if (this.modelChildIndex == -1)
			{
				return this.rootObject;
			}
			GameObject rootObject = this.rootObject;
			if (rootObject == null)
			{
				return null;
			}
			ModelLocator component = rootObject.GetComponent<ModelLocator>();
			if (component == null)
			{
				return null;
			}
			Transform modelTransform = component.modelTransform;
			if (modelTransform == null)
			{
				return null;
			}
			HurtBoxGroup component2 = modelTransform.GetComponent<HurtBoxGroup>();
			if (component2 == null)
			{
				return null;
			}
			HurtBox hurtBox = component2.hurtBoxes.ElementAtOrDefault((int)this.modelChildIndex);
			if (hurtBox == null)
			{
				return null;
			}
			return hurtBox.gameObject;
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x000155C5 File Offset: 0x000137C5
		public void SetChildLocatorTransformReference(GameObject rootObject, int childIndex)
		{
			this.rootObject = rootObject;
			this.modelChildIndex = (short)childIndex;
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x000155D8 File Offset: 0x000137D8
		public Transform ResolveChildLocatorTransformReference()
		{
			if (this.rootObject)
			{
				if (this.modelChildIndex == -1)
				{
					return this.rootObject.transform;
				}
				ModelLocator component = this.rootObject.GetComponent<ModelLocator>();
				if (component && component.modelTransform)
				{
					ChildLocator component2 = component.modelTransform.GetComponent<ChildLocator>();
					if (component2)
					{
						return component2.FindChild((int)this.modelChildIndex);
					}
				}
			}
			return null;
		}

		// Token: 0x06000554 RID: 1364 RVA: 0x0001564C File Offset: 0x0001384C
		public EffectData Clone()
		{
			EffectData effectData = new EffectData();
			EffectData.Copy(this, effectData);
			return effectData;
		}

		// Token: 0x06000555 RID: 1365 RVA: 0x00015667 File Offset: 0x00013867
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool ColorEquals(in Color32 x, in Color32 y)
		{
			return x.r == y.r && x.g == y.g && x.b == y.b && x.a == y.a;
		}

		// Token: 0x06000556 RID: 1366 RVA: 0x000156A4 File Offset: 0x000138A4
		public void Serialize(NetworkWriter writer)
		{
			uint num = 0U;
			bool flag = !this.rotation.Equals(EffectData.defaultRotation);
			bool flag2 = this.rootObject != EffectData.defaultRootObject;
			bool flag3 = this.modelChildIndex != EffectData.defaultModelChildIndex;
			bool flag4 = this.scale != EffectData.defaultScale;
			bool flag5 = !EffectData.ColorEquals(this.color, EffectData.defaultColor);
			bool flag6 = !this.start.Equals(EffectData.defaultStart);
			bool flag7 = this.surfaceDefIndex != EffectData.defaultSurfaceDefIndex;
			bool flag8 = this.genericUInt != EffectData.defaultGenericUInt;
			bool flag9 = this.genericFloat != EffectData.defaultGenericFloat;
			bool flag10 = this.genericBool != EffectData.defaultGenericBool;
			bool flag11 = this.networkSoundEventIndex != EffectData.defaultNetworkSoundEventIndex;
			if (flag)
			{
				num |= EffectData.useNonDefaultRotationFlag;
			}
			if (flag2)
			{
				num |= EffectData.useNonDefaultRootObjectFlag;
			}
			if (flag3)
			{
				num |= EffectData.useNonDefaultModelChildIndexFlag;
			}
			if (flag4)
			{
				num |= EffectData.useNonDefaultScaleFlag;
			}
			if (flag5)
			{
				num |= EffectData.useNonDefaultColorFlag;
			}
			if (flag6)
			{
				num |= EffectData.useNonDefaultStartFlag;
			}
			if (flag7)
			{
				num |= EffectData.useNonDefaultSurfaceDefIndexFlag;
			}
			if (flag8)
			{
				num |= EffectData.useNonDefaultGenericUIntFlag;
			}
			if (flag9)
			{
				num |= EffectData.useNonDefaultGenericFloatFlag;
			}
			if (flag10)
			{
				num |= EffectData.useNonDefaultGenericBoolFlag;
			}
			if (flag11)
			{
				num |= EffectData.useNonDefaultNetworkSoundEventIndexFlag;
			}
			writer.WritePackedUInt32(num);
			writer.Write(this.origin);
			if (flag)
			{
				writer.Write(this.rotation);
			}
			if (flag2)
			{
				writer.Write(this.rootObject);
			}
			if (flag3)
			{
				writer.Write((byte)(this.modelChildIndex + 1));
			}
			if (flag4)
			{
				writer.Write(this.scale);
			}
			if (flag5)
			{
				writer.Write(this.color);
			}
			if (flag6)
			{
				writer.Write(this.start);
			}
			if (flag7)
			{
				writer.WritePackedIndex32((int)this.surfaceDefIndex);
			}
			if (flag8)
			{
				writer.WritePackedUInt32(this.genericUInt);
			}
			if (flag9)
			{
				writer.Write(this.genericFloat);
			}
			if (flag10)
			{
				writer.Write(this.genericBool);
			}
			if (flag11)
			{
				writer.WriteNetworkSoundEventIndex(this.networkSoundEventIndex);
			}
		}

		// Token: 0x06000557 RID: 1367 RVA: 0x000158C0 File Offset: 0x00013AC0
		public void Deserialize(NetworkReader reader)
		{
			EffectData.<>c__DisplayClass54_0 CS$<>8__locals1;
			CS$<>8__locals1.flags = reader.ReadPackedUInt32();
			bool flag = EffectData.<Deserialize>g__HasFlag|54_0(EffectData.useNonDefaultRotationFlag, ref CS$<>8__locals1);
			bool flag2 = EffectData.<Deserialize>g__HasFlag|54_0(EffectData.useNonDefaultRootObjectFlag, ref CS$<>8__locals1);
			bool flag3 = EffectData.<Deserialize>g__HasFlag|54_0(EffectData.useNonDefaultModelChildIndexFlag, ref CS$<>8__locals1);
			bool flag4 = EffectData.<Deserialize>g__HasFlag|54_0(EffectData.useNonDefaultScaleFlag, ref CS$<>8__locals1);
			bool flag5 = EffectData.<Deserialize>g__HasFlag|54_0(EffectData.useNonDefaultColorFlag, ref CS$<>8__locals1);
			bool flag6 = EffectData.<Deserialize>g__HasFlag|54_0(EffectData.useNonDefaultStartFlag, ref CS$<>8__locals1);
			bool flag7 = EffectData.<Deserialize>g__HasFlag|54_0(EffectData.useNonDefaultSurfaceDefIndexFlag, ref CS$<>8__locals1);
			bool flag8 = EffectData.<Deserialize>g__HasFlag|54_0(EffectData.useNonDefaultGenericUIntFlag, ref CS$<>8__locals1);
			bool flag9 = EffectData.<Deserialize>g__HasFlag|54_0(EffectData.useNonDefaultGenericFloatFlag, ref CS$<>8__locals1);
			bool flag10 = EffectData.<Deserialize>g__HasFlag|54_0(EffectData.useNonDefaultGenericBoolFlag, ref CS$<>8__locals1);
			bool flag11 = EffectData.<Deserialize>g__HasFlag|54_0(EffectData.useNonDefaultNetworkSoundEventIndexFlag, ref CS$<>8__locals1);
			this.origin = reader.ReadVector3();
			this.rotation = (flag ? reader.ReadQuaternion() : EffectData.defaultRotation);
			this.rootObject = (flag2 ? reader.ReadGameObject() : EffectData.defaultRootObject);
			this.modelChildIndex = (flag3 ? ((short)(reader.ReadByte() - 1)) : EffectData.defaultModelChildIndex);
			this.scale = (flag4 ? reader.ReadSingle() : EffectData.defaultScale);
			this.color = (flag5 ? reader.ReadColor32() : EffectData.defaultColor);
			this.start = (flag6 ? reader.ReadVector3() : EffectData.defaultStart);
			this.surfaceDefIndex = (SurfaceDefIndex)(flag7 ? reader.ReadPackedIndex32() : ((int)EffectData.defaultSurfaceDefIndex));
			this.genericUInt = (flag8 ? reader.ReadPackedUInt32() : EffectData.defaultGenericUInt);
			this.genericFloat = (flag9 ? reader.ReadSingle() : EffectData.defaultGenericFloat);
			this.genericBool = (flag10 ? reader.ReadBoolean() : EffectData.defaultGenericBool);
			this.networkSoundEventIndex = (flag11 ? reader.ReadNetworkSoundEventIndex() : EffectData.defaultNetworkSoundEventIndex);
		}

		// Token: 0x0600055A RID: 1370 RVA: 0x00015BC5 File Offset: 0x00013DC5
		[CompilerGenerated]
		internal static bool <Deserialize>g__HasFlag|54_0(in uint mask, ref EffectData.<>c__DisplayClass54_0 A_1)
		{
			return (A_1.flags & mask) > 0U;
		}

		// Token: 0x0400057B RID: 1403
		private Vector3 _origin;

		// Token: 0x0400057C RID: 1404
		public Quaternion rotation = EffectData.defaultRotation;

		// Token: 0x0400057F RID: 1407
		public float scale = EffectData.defaultScale;

		// Token: 0x04000580 RID: 1408
		public Color32 color = EffectData.defaultColor;

		// Token: 0x04000581 RID: 1409
		public Vector3 start = EffectData.defaultStart;

		// Token: 0x04000582 RID: 1410
		public SurfaceDefIndex surfaceDefIndex = EffectData.defaultSurfaceDefIndex;

		// Token: 0x04000583 RID: 1411
		public uint genericUInt = EffectData.defaultGenericUInt;

		// Token: 0x04000584 RID: 1412
		public float genericFloat = EffectData.defaultGenericFloat;

		// Token: 0x04000585 RID: 1413
		public bool genericBool = EffectData.defaultGenericBool;

		// Token: 0x04000586 RID: 1414
		public NetworkSoundEventIndex networkSoundEventIndex = EffectData.defaultNetworkSoundEventIndex;

		// Token: 0x04000587 RID: 1415
		private static readonly uint useNonDefaultRotationFlag = 1U;

		// Token: 0x04000588 RID: 1416
		private static readonly uint useNonDefaultRootObjectFlag = 2U;

		// Token: 0x04000589 RID: 1417
		private static readonly uint useNonDefaultModelChildIndexFlag = 4U;

		// Token: 0x0400058A RID: 1418
		private static readonly uint useNonDefaultScaleFlag = 8U;

		// Token: 0x0400058B RID: 1419
		private static readonly uint useNonDefaultColorFlag = 16U;

		// Token: 0x0400058C RID: 1420
		private static readonly uint useNonDefaultStartFlag = 32U;

		// Token: 0x0400058D RID: 1421
		private static readonly uint useNonDefaultSurfaceDefIndexFlag = 64U;

		// Token: 0x0400058E RID: 1422
		private static readonly uint useNonDefaultGenericUIntFlag = 128U;

		// Token: 0x0400058F RID: 1423
		private static readonly uint useNonDefaultGenericFloatFlag = 256U;

		// Token: 0x04000590 RID: 1424
		private static readonly uint useNonDefaultGenericBoolFlag = 512U;

		// Token: 0x04000591 RID: 1425
		private static readonly uint useNonDefaultNetworkSoundEventIndexFlag = 1024U;

		// Token: 0x04000592 RID: 1426
		private static readonly Quaternion defaultRotation = Quaternion.identity;

		// Token: 0x04000593 RID: 1427
		private static readonly GameObject defaultRootObject = null;

		// Token: 0x04000594 RID: 1428
		private static readonly short defaultModelChildIndex = -1;

		// Token: 0x04000595 RID: 1429
		private static readonly float defaultScale = 1f;

		// Token: 0x04000596 RID: 1430
		private static readonly Color32 defaultColor = Color.white;

		// Token: 0x04000597 RID: 1431
		private static readonly Vector3 defaultStart = Vector3.zero;

		// Token: 0x04000598 RID: 1432
		private static readonly SurfaceDefIndex defaultSurfaceDefIndex = SurfaceDefIndex.Invalid;

		// Token: 0x04000599 RID: 1433
		private static readonly uint defaultGenericUInt = 0U;

		// Token: 0x0400059A RID: 1434
		private static readonly float defaultGenericFloat = 0f;

		// Token: 0x0400059B RID: 1435
		private static readonly bool defaultGenericBool = false;

		// Token: 0x0400059C RID: 1436
		private static readonly NetworkSoundEventIndex defaultNetworkSoundEventIndex = NetworkSoundEventIndex.Invalid;
	}
}
