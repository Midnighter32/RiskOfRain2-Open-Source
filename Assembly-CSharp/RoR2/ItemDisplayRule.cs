using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200010A RID: 266
	[Serializable]
	public struct ItemDisplayRule : IEquatable<ItemDisplayRule>
	{
		// Token: 0x060004FC RID: 1276 RVA: 0x00014124 File Offset: 0x00012324
		public bool Equals(ItemDisplayRule other)
		{
			return this.ruleType == other.ruleType && object.Equals(this.followerPrefab, other.followerPrefab) && string.Equals(this.childName, other.childName) && this.localPos.Equals(other.localPos) && this.localAngles.Equals(other.localAngles) && this.localScale.Equals(other.localScale) && this.limbMask == other.limbMask;
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x000141B0 File Offset: 0x000123B0
		public override bool Equals(object obj)
		{
			if (obj is ItemDisplayRule)
			{
				ItemDisplayRule other = (ItemDisplayRule)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x060004FE RID: 1278 RVA: 0x000141D8 File Offset: 0x000123D8
		public override int GetHashCode()
		{
			return (int)((((((this.ruleType * (ItemDisplayRuleType)397 ^ (ItemDisplayRuleType)((this.followerPrefab != null) ? this.followerPrefab.GetHashCode() : 0)) * (ItemDisplayRuleType)397 ^ (ItemDisplayRuleType)((this.childName != null) ? this.childName.GetHashCode() : 0)) * (ItemDisplayRuleType)397 ^ (ItemDisplayRuleType)this.localPos.GetHashCode()) * (ItemDisplayRuleType)397 ^ (ItemDisplayRuleType)this.localAngles.GetHashCode()) * (ItemDisplayRuleType)397 ^ (ItemDisplayRuleType)this.localScale.GetHashCode()) * (ItemDisplayRuleType)397 ^ (ItemDisplayRuleType)this.limbMask);
		}

		// Token: 0x040004CE RID: 1230
		public ItemDisplayRuleType ruleType;

		// Token: 0x040004CF RID: 1231
		public GameObject followerPrefab;

		// Token: 0x040004D0 RID: 1232
		public string childName;

		// Token: 0x040004D1 RID: 1233
		public Vector3 localPos;

		// Token: 0x040004D2 RID: 1234
		public Vector3 localAngles;

		// Token: 0x040004D3 RID: 1235
		public Vector3 localScale;

		// Token: 0x040004D4 RID: 1236
		public LimbFlags limbMask;
	}
}
