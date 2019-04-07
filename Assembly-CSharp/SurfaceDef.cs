using System;
using UnityEngine;

// Token: 0x02000024 RID: 36
[CreateAssetMenu]
public class SurfaceDef : ScriptableObject
{
	// Token: 0x0400009F RID: 159
	public Color approximateColor;

	// Token: 0x040000A0 RID: 160
	public GameObject impactEffectPrefab;

	// Token: 0x040000A1 RID: 161
	public GameObject footstepEffectPrefab;

	// Token: 0x040000A2 RID: 162
	public string impactSoundString;

	// Token: 0x040000A3 RID: 163
	public string materialSwitchString;
}
