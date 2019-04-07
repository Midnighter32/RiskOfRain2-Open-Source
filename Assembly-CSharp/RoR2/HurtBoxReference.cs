using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000325 RID: 805
	[Serializable]
	public struct HurtBoxReference
	{
		// Token: 0x0600108F RID: 4239 RVA: 0x00052B70 File Offset: 0x00050D70
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

		// Token: 0x06001090 RID: 4240 RVA: 0x00052BCC File Offset: 0x00050DCC
		public static HurtBoxReference FromRootObject(GameObject rootObject)
		{
			return new HurtBoxReference
			{
				rootObject = rootObject,
				hurtBoxIndexPlusOne = 0
			};
		}

		// Token: 0x06001091 RID: 4241 RVA: 0x00052BF4 File Offset: 0x00050DF4
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

		// Token: 0x06001092 RID: 4242 RVA: 0x00052C64 File Offset: 0x00050E64
		public HurtBox ResolveHurtBox()
		{
			GameObject gameObject = this.ResolveGameObject();
			if (!gameObject)
			{
				return null;
			}
			return gameObject.GetComponent<HurtBox>();
		}

		// Token: 0x06001093 RID: 4243 RVA: 0x00052C88 File Offset: 0x00050E88
		public void Write(NetworkWriter writer)
		{
			writer.Write(this.rootObject);
			writer.Write(this.hurtBoxIndexPlusOne);
		}

		// Token: 0x06001094 RID: 4244 RVA: 0x00052CA2 File Offset: 0x00050EA2
		public void Read(NetworkReader reader)
		{
			this.rootObject = reader.ReadGameObject();
			this.hurtBoxIndexPlusOne = reader.ReadByte();
		}

		// Token: 0x040014AF RID: 5295
		public GameObject rootObject;

		// Token: 0x040014B0 RID: 5296
		public byte hurtBoxIndexPlusOne;
	}
}
