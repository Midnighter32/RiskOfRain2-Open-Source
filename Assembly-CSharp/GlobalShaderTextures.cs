using System;
using UnityEngine;

// Token: 0x0200003B RID: 59
[ExecuteInEditMode]
public class GlobalShaderTextures : MonoBehaviour
{
	// Token: 0x060000FC RID: 252 RVA: 0x00007235 File Offset: 0x00005435
	private void OnValidate()
	{
		Shader.SetGlobalTexture(this.warpRampShaderVariableName, this.warpRampTexture);
		Shader.SetGlobalTexture(this.eliteRampShaderVariableName, this.eliteRampTexture);
		Shader.SetGlobalTexture(this.snowMicrofacetNoiseVariableName, this.snowMicrofacetTexture);
	}

	// Token: 0x060000FD RID: 253 RVA: 0x00007235 File Offset: 0x00005435
	private void Start()
	{
		Shader.SetGlobalTexture(this.warpRampShaderVariableName, this.warpRampTexture);
		Shader.SetGlobalTexture(this.eliteRampShaderVariableName, this.eliteRampTexture);
		Shader.SetGlobalTexture(this.snowMicrofacetNoiseVariableName, this.snowMicrofacetTexture);
	}

	// Token: 0x0400012E RID: 302
	public Texture warpRampTexture;

	// Token: 0x0400012F RID: 303
	public string warpRampShaderVariableName;

	// Token: 0x04000130 RID: 304
	public Texture eliteRampTexture;

	// Token: 0x04000131 RID: 305
	public string eliteRampShaderVariableName;

	// Token: 0x04000132 RID: 306
	public Texture snowMicrofacetTexture;

	// Token: 0x04000133 RID: 307
	public string snowMicrofacetNoiseVariableName;
}
