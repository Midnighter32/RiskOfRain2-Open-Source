using System;
using UnityEngine;

// Token: 0x0200003F RID: 63
[ExecuteInEditMode]
public class GlobalShaderTextures : MonoBehaviour
{
	// Token: 0x06000118 RID: 280 RVA: 0x000072DD File Offset: 0x000054DD
	private void OnValidate()
	{
		Shader.SetGlobalTexture(this.warpRampShaderVariableName, this.warpRampTexture);
		Shader.SetGlobalTexture(this.eliteRampShaderVariableName, this.eliteRampTexture);
		Shader.SetGlobalTexture(this.snowMicrofacetNoiseVariableName, this.snowMicrofacetTexture);
	}

	// Token: 0x06000119 RID: 281 RVA: 0x000072DD File Offset: 0x000054DD
	private void Start()
	{
		Shader.SetGlobalTexture(this.warpRampShaderVariableName, this.warpRampTexture);
		Shader.SetGlobalTexture(this.eliteRampShaderVariableName, this.eliteRampTexture);
		Shader.SetGlobalTexture(this.snowMicrofacetNoiseVariableName, this.snowMicrofacetTexture);
	}

	// Token: 0x04000129 RID: 297
	public Texture warpRampTexture;

	// Token: 0x0400012A RID: 298
	public string warpRampShaderVariableName;

	// Token: 0x0400012B RID: 299
	public Texture eliteRampTexture;

	// Token: 0x0400012C RID: 300
	public string eliteRampShaderVariableName;

	// Token: 0x0400012D RID: 301
	public Texture snowMicrofacetTexture;

	// Token: 0x0400012E RID: 302
	public string snowMicrofacetNoiseVariableName;
}
