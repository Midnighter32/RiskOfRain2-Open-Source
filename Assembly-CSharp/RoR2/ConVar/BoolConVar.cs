using System;

namespace RoR2.ConVar
{
	// Token: 0x02000678 RID: 1656
	public class BoolConVar : BaseConVar
	{
		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x060026DB RID: 9947 RVA: 0x000A98E4 File Offset: 0x000A7AE4
		// (set) Token: 0x060026DC RID: 9948 RVA: 0x000A98EC File Offset: 0x000A7AEC
		public bool value { get; protected set; }

		// Token: 0x060026DD RID: 9949 RVA: 0x0000972B File Offset: 0x0000792B
		public BoolConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
		{
		}

		// Token: 0x060026DE RID: 9950 RVA: 0x000A98F5 File Offset: 0x000A7AF5
		public void SetBool(bool newValue)
		{
			this.value = newValue;
		}

		// Token: 0x060026DF RID: 9951 RVA: 0x000A9900 File Offset: 0x000A7B00
		public override void SetString(string newValue)
		{
			int num;
			if (TextSerialization.TryParseInvariant(newValue, out num))
			{
				this.value = (num != 0);
			}
		}

		// Token: 0x060026E0 RID: 9952 RVA: 0x000A9921 File Offset: 0x000A7B21
		public override string GetString()
		{
			if (!this.value)
			{
				return "0";
			}
			return "1";
		}
	}
}
