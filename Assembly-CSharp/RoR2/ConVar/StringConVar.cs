using System;

namespace RoR2.ConVar
{
	// Token: 0x0200067B RID: 1659
	public class StringConVar : BaseConVar
	{
		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x060026EB RID: 9963 RVA: 0x000A99BF File Offset: 0x000A7BBF
		// (set) Token: 0x060026EC RID: 9964 RVA: 0x000A99C7 File Offset: 0x000A7BC7
		public string value { get; protected set; }

		// Token: 0x060026ED RID: 9965 RVA: 0x0000972B File Offset: 0x0000792B
		public StringConVar(string name, ConVarFlags flags, string defaultValue, string helpText) : base(name, flags, defaultValue, helpText)
		{
		}

		// Token: 0x060026EE RID: 9966 RVA: 0x000A99D0 File Offset: 0x000A7BD0
		public override void SetString(string newValue)
		{
			this.value = newValue;
		}

		// Token: 0x060026EF RID: 9967 RVA: 0x000A99D9 File Offset: 0x000A7BD9
		public override string GetString()
		{
			return this.value;
		}
	}
}
