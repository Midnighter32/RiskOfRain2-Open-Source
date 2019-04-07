using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200023D RID: 573
	public class EffectData
	{
		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000ACD RID: 2765 RVA: 0x000355E5 File Offset: 0x000337E5
		// (set) Token: 0x06000ACE RID: 2766 RVA: 0x000355ED File Offset: 0x000337ED
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

		// Token: 0x06000ACF RID: 2767 RVA: 0x00035618 File Offset: 0x00033818
		public static void Copy([NotNull] EffectData src, [NotNull] EffectData dest)
		{
			dest.origin = src.origin;
			dest.rotation = src.rotation;
			dest.rootObject = src.rootObject;
			dest.modelChildIndex = src.modelChildIndex;
			dest.scale = src.scale;
			dest.color = src.color;
			dest.start = src.start;
			dest.genericUInt = src.genericUInt;
			dest.genericFloat = src.genericFloat;
			dest.genericBool = src.genericBool;
		}

		// Token: 0x06000AD0 RID: 2768 RVA: 0x0003569D File Offset: 0x0003389D
		public void SetNetworkedObjectReference(GameObject networkedObject)
		{
			this.rootObject = networkedObject;
			this.modelChildIndex = -1;
		}

		// Token: 0x06000AD1 RID: 2769 RVA: 0x000356AD File Offset: 0x000338AD
		public GameObject ResolveNetworkedObjectReference()
		{
			return this.rootObject;
		}

		// Token: 0x06000AD2 RID: 2770 RVA: 0x000356B8 File Offset: 0x000338B8
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

		// Token: 0x06000AD3 RID: 2771 RVA: 0x00035708 File Offset: 0x00033908
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

		// Token: 0x06000AD4 RID: 2772 RVA: 0x00035740 File Offset: 0x00033940
		public GameObject ResolveHurtBoxReference()
		{
			if (this.modelChildIndex == -1)
			{
				return this.rootObject;
			}
			GameObject gameObject = this.rootObject;
			if (gameObject == null)
			{
				return null;
			}
			ModelLocator component = gameObject.GetComponent<ModelLocator>();
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

		// Token: 0x06000AD5 RID: 2773 RVA: 0x000357A5 File Offset: 0x000339A5
		public void SetChildLocatorTransformReference(GameObject rootObject, int childIndex)
		{
			this.rootObject = rootObject;
			this.modelChildIndex = (short)childIndex;
		}

		// Token: 0x06000AD6 RID: 2774 RVA: 0x000357B8 File Offset: 0x000339B8
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

		// Token: 0x06000AD7 RID: 2775 RVA: 0x0003582C File Offset: 0x00033A2C
		public EffectData Clone()
		{
			EffectData effectData = new EffectData();
			EffectData.Copy(this, effectData);
			return effectData;
		}

		// Token: 0x06000AD8 RID: 2776 RVA: 0x00035848 File Offset: 0x00033A48
		public void Serialize(NetworkWriter writer)
		{
			writer.Write(this.origin);
			writer.Write(this.rotation);
			writer.Write(this.rootObject);
			writer.Write((byte)(this.modelChildIndex + 1));
			writer.Write(this.scale);
			writer.Write(this.color);
			writer.Write(this.start);
			writer.WritePackedUInt32(this.genericUInt);
			writer.Write(this.genericFloat);
			writer.Write(this.genericBool);
		}

		// Token: 0x06000AD9 RID: 2777 RVA: 0x000358D4 File Offset: 0x00033AD4
		public void Deserialize(NetworkReader reader)
		{
			this.origin = reader.ReadVector3();
			this.rotation = reader.ReadQuaternion();
			this.rootObject = reader.ReadGameObject();
			this.modelChildIndex = (short)(reader.ReadByte() - 1);
			this.scale = reader.ReadSingle();
			this.color = reader.ReadColor32();
			this.start = reader.ReadVector3();
			this.genericUInt = reader.ReadPackedUInt32();
			this.genericFloat = reader.ReadSingle();
			this.genericBool = reader.ReadBoolean();
		}

		// Token: 0x04000E94 RID: 3732
		private Vector3 _origin;

		// Token: 0x04000E95 RID: 3733
		public Quaternion rotation = Quaternion.identity;

		// Token: 0x04000E96 RID: 3734
		private GameObject rootObject;

		// Token: 0x04000E97 RID: 3735
		private short modelChildIndex = -1;

		// Token: 0x04000E98 RID: 3736
		public float scale = 1f;

		// Token: 0x04000E99 RID: 3737
		public Color32 color = Color.white;

		// Token: 0x04000E9A RID: 3738
		public Vector3 start = Vector3.zero;

		// Token: 0x04000E9B RID: 3739
		public uint genericUInt;

		// Token: 0x04000E9C RID: 3740
		public float genericFloat;

		// Token: 0x04000E9D RID: 3741
		public bool genericBool;
	}
}
