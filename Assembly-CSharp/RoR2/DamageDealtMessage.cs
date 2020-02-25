using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200022B RID: 555
	public class DamageDealtMessage : MessageBase
	{
		// Token: 0x06000C6E RID: 3182 RVA: 0x00038144 File Offset: 0x00036344
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

		// Token: 0x06000C6F RID: 3183 RVA: 0x000381B0 File Offset: 0x000363B0
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

		// Token: 0x1700019B RID: 411
		// (get) Token: 0x06000C70 RID: 3184 RVA: 0x00038218 File Offset: 0x00036418
		public bool isSilent
		{
			get
			{
				return (this.damageType & DamageType.Silent) > DamageType.Generic;
			}
		}

		// Token: 0x04000C58 RID: 3160
		public GameObject victim;

		// Token: 0x04000C59 RID: 3161
		public float damage;

		// Token: 0x04000C5A RID: 3162
		public GameObject attacker;

		// Token: 0x04000C5B RID: 3163
		public Vector3 position;

		// Token: 0x04000C5C RID: 3164
		public bool crit;

		// Token: 0x04000C5D RID: 3165
		public DamageType damageType;

		// Token: 0x04000C5E RID: 3166
		public DamageColorIndex damageColorIndex;
	}
}
