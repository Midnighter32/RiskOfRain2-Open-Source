using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200030A RID: 778
	public class DamageDealtMessage : MessageBase
	{
		// Token: 0x06001028 RID: 4136 RVA: 0x00051304 File Offset: 0x0004F504
		public override void Serialize(NetworkWriter writer)
		{
			base.Serialize(writer);
			writer.Write(this.victim);
			writer.Write(this.damage);
			writer.Write(this.attacker);
			writer.Write(this.position);
			writer.Write(this.crit);
			writer.Write(this.damageType);
			writer.Write(this.damageColorIndex);
		}

		// Token: 0x06001029 RID: 4137 RVA: 0x00051370 File Offset: 0x0004F570
		public override void Deserialize(NetworkReader reader)
		{
			base.Deserialize(reader);
			this.victim = reader.ReadGameObject();
			this.damage = reader.ReadSingle();
			this.attacker = reader.ReadGameObject();
			this.position = reader.ReadVector3();
			this.crit = reader.ReadBoolean();
			this.damageType = reader.ReadDamageType();
			this.damageColorIndex = reader.ReadDamageColorIndex();
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x0600102A RID: 4138 RVA: 0x000513D8 File Offset: 0x0004F5D8
		public bool isSilent
		{
			get
			{
				return (this.damageType & DamageType.Silent) > DamageType.Generic;
			}
		}

		// Token: 0x04001416 RID: 5142
		public GameObject victim;

		// Token: 0x04001417 RID: 5143
		public float damage;

		// Token: 0x04001418 RID: 5144
		public GameObject attacker;

		// Token: 0x04001419 RID: 5145
		public Vector3 position;

		// Token: 0x0400141A RID: 5146
		public bool crit;

		// Token: 0x0400141B RID: 5147
		public DamageType damageType;

		// Token: 0x0400141C RID: 5148
		public DamageColorIndex damageColorIndex;
	}
}
