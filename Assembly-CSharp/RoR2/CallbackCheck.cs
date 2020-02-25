using System;
using JetBrains.Annotations;

namespace RoR2
{
	// Token: 0x020000D9 RID: 217
	public class CallbackCheck<TResult, TArg> where TResult : struct
	{
		// Token: 0x0600043A RID: 1082 RVA: 0x00011644 File Offset: 0x0000F844
		public void AddCallback([NotNull] CallbackCheck<TResult, TArg>.CallbackDelegate callback)
		{
			if (this.callbacks.Length <= this.callbackCount + 1)
			{
				Array.Resize<CallbackCheck<TResult, TArg>.CallbackDelegate>(ref this.callbacks, this.callbackCount + 1);
			}
			CallbackCheck<TResult, TArg>.CallbackDelegate[] array = this.callbacks;
			int num = this.callbackCount;
			this.callbackCount = num + 1;
			array[num] = callback;
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x00011690 File Offset: 0x0000F890
		public void RemoveCallback([NotNull] CallbackCheck<TResult, TArg>.CallbackDelegate callback)
		{
			for (int i = 0; i < this.callbackCount; i++)
			{
				if (this.callbacks[i] == callback)
				{
					int num = this.callbackCount - 1;
					while (i < num)
					{
						this.callbacks[i] = this.callbacks[i + 1];
						i++;
					}
					CallbackCheck<TResult, TArg>.CallbackDelegate[] array = this.callbacks;
					int num2 = this.callbackCount - 1;
					this.callbackCount = num2;
					array[num2] = null;
					return;
				}
			}
		}

		// Token: 0x0600043C RID: 1084 RVA: 0x000116F8 File Offset: 0x0000F8F8
		public void Clear()
		{
			Array.Clear(this.callbacks, 0, this.callbackCount);
			this.callbackCount = 0;
		}

		// Token: 0x0600043D RID: 1085 RVA: 0x00011714 File Offset: 0x0000F914
		public TResult? Evaluate(TArg arg)
		{
			TResult? result = null;
			for (int i = 0; i < this.callbackCount; i++)
			{
				this.callbacks[i](arg, ref result);
				if (result != null)
				{
					break;
				}
			}
			return result;
		}

		// Token: 0x04000412 RID: 1042
		private CallbackCheck<TResult, TArg>.CallbackDelegate[] callbacks = Array.Empty<CallbackCheck<TResult, TArg>.CallbackDelegate>();

		// Token: 0x04000413 RID: 1043
		private int callbackCount;

		// Token: 0x020000DA RID: 218
		// (Invoke) Token: 0x06000440 RID: 1088
		public delegate void CallbackDelegate(TArg arg, ref TResult? resultOverride);
	}
}
