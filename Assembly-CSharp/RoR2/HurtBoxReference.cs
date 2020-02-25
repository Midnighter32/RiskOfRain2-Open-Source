using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000246 RID: 582
	[Serializable]
	public struct HurtBoxReference : IEquatable<HurtBoxReference>
	{
		// Token: 0x06000CD6 RID: 3286 RVA: 0x000399D8 File Offset: 0x00037BD8
		public static HurtBoxReference FromHurtBox(HurtBox hurtBox)
		{
			HurtBoxReference result;
			if (!hurtBox)
			{
				result = default(HurtBoxReference);
				return result;
			}
			result = new HurtBoxReference
			{
				rootObject = (hurtBox.healthComponent ? hurtBox.healthComponent.gameObject : null),
				hurtBoxIndexPlusOne = (byte)(hurtBox.indexInGroup + 1)
			};
			return result;
		}

		// Token: 0x06000CD7 RID: 3287 RVA: 0x00039A34 File Offset: 0x00037C34
		public static HurtBoxReference FromRootObject(GameObject rootObject)
		{
			return new HurtBoxReference
			{
				rootObject = rootObject,
				hurtBoxIndexPlusOne = 0
			};
		}

		// Token: 0x06000CD8 RID: 3288 RVA: 0x00039A5C File Offset: 0x00037C5C
		public GameObject ResolveGameObject()
		{
			if (this.hurtBoxIndexPlusOne == 0)
			{
				return this.rootObject;
			}
			GameObject gameObject = this.rootObject;
			HurtBox[] array;
			if (gameObject == null)
			{
				array = null;
			}
			else
			{
				ModelLocator component = gameObject.GetComponent<ModelLocator>();
				if (component == null)
				{
					array = null;
				}
				else
				{
					Transform modelTransform = component.modelTransform;
					if (modelTransform == null)
					{
						array = null;
					}
					else
					{
						HurtBoxGroup component2 = modelTransform.GetComponent<HurtBoxGroup>();
						array = ((component2 != null) ? component2.hurtBoxes : null);
					}
				}
			}
			HurtBox[] array2 = array;
			if (array2 != null)
			{
				int num = (int)(this.hurtBoxIndexPlusOne - 1);
				if (num < array2.Length)
				{
					return array2[num].gameObject;
				}
			}
			return null;
		}

		// Token: 0x06000CD9 RID: 3289 RVA: 0x00039ACC File Offset: 0x00037CCC
		public HurtBox ResolveHurtBox()
		{
			GameObject gameObject = this.ResolveGameObject();
			if (!gameObject)
			{
				return null;
			}
			return gameObject.GetComponent<HurtBox>();
		}

		// Token: 0x06000CDA RID: 3290 RVA: 0x00039AF0 File Offset: 0x00037CF0
		public void Write(NetworkWriter writer)
		{
			writer.Write(this.rootObject);
			writer.Write(this.hurtBoxIndexPlusOne);
		}

		// Token: 0x06000CDB RID: 3291 RVA: 0x00039B0A File Offset: 0x00037D0A
		public void Read(NetworkReader reader)
		{
			this.rootObject = reader.ReadGameObject();
			this.hurtBoxIndexPlusOne = reader.ReadByte();
		}

		// Token: 0x06000CDC RID: 3292 RVA: 0x00039B24 File Offset: 0x00037D24
		public bool Equals(HurtBoxReference other)
		{
			return object.Equals(this.rootObject, other.rootObject) && this.hurtBoxIndexPlusOne == other.hurtBoxIndexPlusOne;
		}

		// Token: 0x06000CDD RID: 3293 RVA: 0x00039B4C File Offset: 0x00037D4C
		public override bool Equals(object obj)
		{
			if (obj is HurtBoxReference)
			{
				HurtBoxReference other = (HurtBoxReference)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06000CDE RID: 3294 RVA: 0x00039B73 File Offset: 0x00037D73
		public override int GetHashCode()
		{
			return ((this.rootObject != null) ? this.rootObject.GetHashCode() : 0) * 397 ^ this.hurtBoxIndexPlusOne.GetHashCode();
		}

		// Token: 0x04000CED RID: 3309
		public GameObject rootObject;

		// Token: 0x04000CEE RID: 3310
		public byte hurtBoxIndexPlusOne;
	}
}
