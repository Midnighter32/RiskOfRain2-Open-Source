using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace RoR2
{
	// Token: 0x02000409 RID: 1033
	[MeansImplicitUse]
	public abstract class SearchableAttribute : Attribute
	{
		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x0600191C RID: 6428 RVA: 0x0006C333 File Offset: 0x0006A533
		// (set) Token: 0x0600191D RID: 6429 RVA: 0x0006C33B File Offset: 0x0006A53B
		public object target { get; private set; }

		// Token: 0x0600191E RID: 6430 RVA: 0x0006C344 File Offset: 0x0006A544
		public static List<SearchableAttribute> GetInstances<T>() where T : SearchableAttribute
		{
			List<SearchableAttribute> result;
			if (SearchableAttribute.instancesListsByType.TryGetValue(typeof(T), out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x0600191F RID: 6431 RVA: 0x0006C36C File Offset: 0x0006A56C
		private static IEnumerable<Assembly> GetScannableAssemblies()
		{
			return from a in AppDomain.CurrentDomain.GetAssemblies()
			where !SearchableAttribute.assemblyBlacklist.Contains(a.GetName().Name)
			select a;
		}

		// Token: 0x06001920 RID: 6432 RVA: 0x0006C39C File Offset: 0x0006A59C
		private static void Scan()
		{
			IEnumerable<Type> enumerable = SearchableAttribute.GetScannableAssemblies().SelectMany((Assembly a) => a.GetTypes());
			SearchableAttribute.<>c__DisplayClass7_0 CS$<>8__locals1;
			CS$<>8__locals1.allInstancesList = new List<SearchableAttribute>();
			CS$<>8__locals1.memoizedInput = null;
			CS$<>8__locals1.memoizedOutput = null;
			foreach (Type type in enumerable)
			{
				foreach (SearchableAttribute attribute in type.GetCustomAttributes(false))
				{
					SearchableAttribute.<Scan>g__Register|7_2(attribute, type, ref CS$<>8__locals1);
				}
				foreach (MemberInfo memberInfo in type.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					foreach (SearchableAttribute attribute2 in memberInfo.GetCustomAttributes(false))
					{
						SearchableAttribute.<Scan>g__Register|7_2(attribute2, memberInfo, ref CS$<>8__locals1);
					}
				}
			}
		}

		// Token: 0x06001921 RID: 6433 RVA: 0x0006C4C8 File Offset: 0x0006A6C8
		static SearchableAttribute()
		{
			try
			{
				SearchableAttribute.Scan();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		// Token: 0x06001923 RID: 6435 RVA: 0x0006CB48 File Offset: 0x0006AD48
		[CompilerGenerated]
		internal static List<SearchableAttribute> <Scan>g__GetInstancesListForType|7_1(Type attributeType, ref SearchableAttribute.<>c__DisplayClass7_0 A_1)
		{
			if (attributeType.Equals(A_1.memoizedInput))
			{
				return A_1.memoizedOutput;
			}
			List<SearchableAttribute> list;
			if (!SearchableAttribute.instancesListsByType.TryGetValue(attributeType, out list))
			{
				list = new List<SearchableAttribute>();
				SearchableAttribute.instancesListsByType.Add(attributeType, list);
			}
			A_1.memoizedInput = attributeType;
			A_1.memoizedOutput = list;
			return list;
		}

		// Token: 0x06001924 RID: 6436 RVA: 0x0006CB9A File Offset: 0x0006AD9A
		[CompilerGenerated]
		internal static void <Scan>g__Register|7_2(SearchableAttribute attribute, object target, ref SearchableAttribute.<>c__DisplayClass7_0 A_2)
		{
			attribute.target = target;
			A_2.allInstancesList.Add(attribute);
			SearchableAttribute.<Scan>g__GetInstancesListForType|7_1(attribute.GetType(), ref A_2).Add(attribute);
		}

		// Token: 0x04001773 RID: 6003
		private static readonly Dictionary<Type, List<SearchableAttribute>> instancesListsByType = new Dictionary<Type, List<SearchableAttribute>>();

		// Token: 0x04001774 RID: 6004
		private static HashSet<string> assemblyBlacklist = new HashSet<string>
		{
			"mscorlib",
			"UnityEngine",
			"UnityEngine.AIModule",
			"UnityEngine.ARModule",
			"UnityEngine.AccessibilityModule",
			"UnityEngine.AnimationModule",
			"UnityEngine.AssetBundleModule",
			"UnityEngine.AudioModule",
			"UnityEngine.BaselibModule",
			"UnityEngine.ClothModule",
			"UnityEngine.ClusterInputModule",
			"UnityEngine.ClusterRendererModule",
			"UnityEngine.CoreModule",
			"UnityEngine.CrashReportingModule",
			"UnityEngine.DirectorModule",
			"UnityEngine.FileSystemHttpModule",
			"UnityEngine.GameCenterModule",
			"UnityEngine.GridModule",
			"UnityEngine.HotReloadModule",
			"UnityEngine.IMGUIModule",
			"UnityEngine.ImageConversionModule",
			"UnityEngine.InputModule",
			"UnityEngine.JSONSerializeModule",
			"UnityEngine.LocalizationModule",
			"UnityEngine.ParticleSystemModule",
			"UnityEngine.PerformanceReportingModule",
			"UnityEngine.PhysicsModule",
			"UnityEngine.Physics2DModule",
			"UnityEngine.ProfilerModule",
			"UnityEngine.ScreenCaptureModule",
			"UnityEngine.SharedInternalsModule",
			"UnityEngine.SpatialTrackingModule",
			"UnityEngine.SpriteMaskModule",
			"UnityEngine.SpriteShapeModule",
			"UnityEngine.StreamingModule",
			"UnityEngine.StyleSheetsModule",
			"UnityEngine.SubstanceModule",
			"UnityEngine.TLSModule",
			"UnityEngine.TerrainModule",
			"UnityEngine.TerrainPhysicsModule",
			"UnityEngine.TextCoreModule",
			"UnityEngine.TextRenderingModule",
			"UnityEngine.TilemapModule",
			"UnityEngine.TimelineModule",
			"UnityEngine.UIModule",
			"UnityEngine.UIElementsModule",
			"UnityEngine.UNETModule",
			"UnityEngine.UmbraModule",
			"UnityEngine.UnityAnalyticsModule",
			"UnityEngine.UnityConnectModule",
			"UnityEngine.UnityTestProtocolModule",
			"UnityEngine.UnityWebRequestModule",
			"UnityEngine.UnityWebRequestAssetBundleModule",
			"UnityEngine.UnityWebRequestAudioModule",
			"UnityEngine.UnityWebRequestTextureModule",
			"UnityEngine.UnityWebRequestWWWModule",
			"UnityEngine.VFXModule",
			"UnityEngine.VRModule",
			"UnityEngine.VehiclesModule",
			"UnityEngine.VideoModule",
			"UnityEngine.WindModule",
			"UnityEngine.XRModule",
			"UnityEditor",
			"Unity.Locator",
			"System.Core",
			"System",
			"Mono.Security",
			"System.Configuration",
			"System.Xml",
			"Unity.DataContract",
			"Unity.PackageManager",
			"UnityEngine.UI",
			"UnityEditor.UI",
			"UnityEditor.TestRunner",
			"UnityEngine.TestRunner",
			"nunit.framework",
			"UnityEngine.Timeline",
			"UnityEditor.Timeline",
			"UnityEngine.Networking",
			"UnityEditor.Networking",
			"UnityEditor.GoogleAudioSpatializer",
			"UnityEngine.GoogleAudioSpatializer",
			"UnityEditor.SpatialTracking",
			"UnityEngine.SpatialTracking",
			"UnityEditor.VR",
			"UnityEditor.Graphs",
			"UnityEditor.WindowsStandalone.Extensions",
			"SyntaxTree.VisualStudio.Unity.Bridge",
			"Rewired_ControlMapper_CSharp_Editor",
			"Rewired_CSharp_Editor",
			"Unity.ProBuilder.AddOns.Editor",
			"Wwise-Editor",
			"Unity.RenderPipelines.Core.Editor",
			"Unity.RenderPipelines.Core.Runtime",
			"Unity.TextMeshPro.Editor",
			"Unity.PackageManagerUI.Editor",
			"Rewired_NintendoSwitch_CSharp",
			"Unity.Postprocessing.Editor",
			"Rewired_CSharp",
			"Unity.Postprocessing.Runtime",
			"Rewired_NintendoSwitch_CSharp_Editor",
			"Wwise",
			"Unity.RenderPipelines.Core.ShaderLibrary",
			"Unity.TextMeshPro",
			"Rewired_UnityUI_CSharp_Editor",
			"Facepunch.Steamworks",
			"Rewired_Editor",
			"Rewired_Core",
			"Rewired_Windows_Lib",
			"Rewired_NintendoSwitch_Editor",
			"Rewired_NintendoSwitch_EditorRuntime",
			"Zio",
			"AssetIdRemapUtility",
			"ProBuilderCore",
			"ProBuilderMeshOps",
			"KdTreeLib",
			"pb_Stl",
			"Poly2Tri",
			"ProBuilderEditor",
			"netstandard",
			"System.Xml.Linq",
			"Unity.Cecil",
			"Unity.SerializationLogic",
			"Unity.Legacy.NRefactory",
			"ExCSS.Unity",
			"Unity.IvyParser",
			"UnityEditor.iOS.Extensions.Xcode",
			"SyntaxTree.VisualStudio.Unity.Messaging",
			"Microsoft.GeneratedCode",
			"Anonymously",
			"Hosted",
			"DynamicMethods",
			"Assembly"
		};
	}
}
