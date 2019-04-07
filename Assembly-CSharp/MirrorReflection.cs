using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000007 RID: 7
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class MirrorReflection : MonoBehaviour
{
	// Token: 0x06000011 RID: 17 RVA: 0x000026EE File Offset: 0x000008EE
	private void OnEnable()
	{
		base.gameObject.layer = LayerMask.NameToLayer("Water");
		this.setMaterial();
	}

	// Token: 0x06000012 RID: 18 RVA: 0x0000270B File Offset: 0x0000090B
	private void OnDisable()
	{
		if (this.m_ReflectionCamera != null)
		{
			UnityEngine.Object.DestroyImmediate(this.m_ReflectionCamera);
		}
	}

	// Token: 0x06000013 RID: 19 RVA: 0x000026EE File Offset: 0x000008EE
	private void Start()
	{
		base.gameObject.layer = LayerMask.NameToLayer("Water");
		this.setMaterial();
	}

	// Token: 0x06000014 RID: 20 RVA: 0x00002726 File Offset: 0x00000926
	public void setMaterial()
	{
		this.m_SharedMaterial = base.GetComponent<Renderer>().sharedMaterial;
	}

	// Token: 0x06000015 RID: 21 RVA: 0x0000273C File Offset: 0x0000093C
	private Camera CreateReflectionCameraFor(Camera cam)
	{
		string name = base.gameObject.name + "Reflection" + cam.name;
		GameObject gameObject = GameObject.Find(name);
		if (!gameObject)
		{
			gameObject = new GameObject(name, new Type[]
			{
				typeof(Camera)
			});
			gameObject.hideFlags = HideFlags.HideAndDontSave;
		}
		if (!gameObject.GetComponent(typeof(Camera)))
		{
			gameObject.AddComponent(typeof(Camera));
		}
		Camera component = gameObject.GetComponent<Camera>();
		component.backgroundColor = this.clearColor;
		component.clearFlags = (this.reflectSkybox ? CameraClearFlags.Skybox : CameraClearFlags.Color);
		this.SetStandardCameraParameter(component, this.reflectionMask);
		if (!component.targetTexture)
		{
			component.targetTexture = this.CreateTextureFor(cam);
		}
		return component;
	}

	// Token: 0x06000016 RID: 22 RVA: 0x0000280B File Offset: 0x00000A0B
	private void SetStandardCameraParameter(Camera cam, LayerMask mask)
	{
		cam.cullingMask = (mask & ~(1 << LayerMask.NameToLayer("Water")));
		cam.backgroundColor = Color.black;
		cam.enabled = false;
	}

	// Token: 0x06000017 RID: 23 RVA: 0x0000283C File Offset: 0x00000A3C
	private RenderTexture CreateTextureFor(Camera cam)
	{
		int width = Mathf.FloorToInt((float)(cam.pixelWidth / (int)this.Quality));
		int height = Mathf.FloorToInt((float)(cam.pixelHeight / (int)this.Quality));
		return new RenderTexture(width, height, 24)
		{
			hideFlags = HideFlags.DontSave
		};
	}

	// Token: 0x06000018 RID: 24 RVA: 0x00002880 File Offset: 0x00000A80
	public void RenderHelpCameras(Camera currentCam)
	{
		if (this.m_HelperCameras == null)
		{
			this.m_HelperCameras = new Dictionary<Camera, bool>();
		}
		if (!this.m_HelperCameras.ContainsKey(currentCam))
		{
			this.m_HelperCameras.Add(currentCam, false);
		}
		if (this.m_HelperCameras[currentCam] && !this.UpdateSceneView)
		{
			return;
		}
		if (!this.m_ReflectionCamera)
		{
			this.m_ReflectionCamera = this.CreateReflectionCameraFor(currentCam);
		}
		this.RenderReflectionFor(currentCam, this.m_ReflectionCamera);
		this.m_HelperCameras[currentCam] = true;
	}

	// Token: 0x06000019 RID: 25 RVA: 0x00002906 File Offset: 0x00000B06
	public void LateUpdate()
	{
		if (this.m_HelperCameras != null)
		{
			this.m_HelperCameras.Clear();
		}
	}

	// Token: 0x0600001A RID: 26 RVA: 0x0000291B File Offset: 0x00000B1B
	public void WaterTileBeingRendered(Transform tr, Camera currentCam)
	{
		this.RenderHelpCameras(currentCam);
		if (this.m_ReflectionCamera && this.m_SharedMaterial)
		{
			this.m_SharedMaterial.SetTexture(this.reflectionSampler, this.m_ReflectionCamera.targetTexture);
		}
	}

	// Token: 0x0600001B RID: 27 RVA: 0x0000295A File Offset: 0x00000B5A
	public void OnWillRenderObject()
	{
		this.WaterTileBeingRendered(base.transform, Camera.current);
	}

	// Token: 0x0600001C RID: 28 RVA: 0x00002970 File Offset: 0x00000B70
	private void RenderReflectionFor(Camera cam, Camera reflectCamera)
	{
		if (!reflectCamera)
		{
			return;
		}
		if (this.m_SharedMaterial && !this.m_SharedMaterial.HasProperty(this.reflectionSampler))
		{
			return;
		}
		int pixelLightCount = QualitySettings.pixelLightCount;
		if (this.m_DisablePixelLights)
		{
			QualitySettings.pixelLightCount = 0;
		}
		reflectCamera.cullingMask = (this.reflectionMask & ~(1 << LayerMask.NameToLayer("Water")));
		this.SaneCameraSettings(reflectCamera);
		reflectCamera.backgroundColor = this.clearColor;
		reflectCamera.clearFlags = (this.reflectSkybox ? CameraClearFlags.Skybox : CameraClearFlags.Color);
		if (this.reflectSkybox && cam.gameObject.GetComponent(typeof(Skybox)))
		{
			Skybox skybox = (Skybox)reflectCamera.gameObject.GetComponent(typeof(Skybox));
			if (!skybox)
			{
				skybox = (Skybox)reflectCamera.gameObject.AddComponent(typeof(Skybox));
			}
			skybox.material = ((Skybox)cam.GetComponent(typeof(Skybox))).material;
		}
		GL.invertCulling = true;
		Transform transform = base.transform;
		Vector3 eulerAngles = cam.transform.eulerAngles;
		reflectCamera.transform.eulerAngles = new Vector3(-eulerAngles.x, eulerAngles.y, eulerAngles.z);
		reflectCamera.transform.position = cam.transform.position;
		Vector3 position = transform.transform.position;
		position.y = transform.position.y;
		Vector3 up = transform.transform.up;
		float w = -Vector3.Dot(up, position) - this.clipPlaneOffset;
		Vector4 plane = new Vector4(up.x, up.y, up.z, w);
		Matrix4x4 matrix4x = Matrix4x4.zero;
		matrix4x = MirrorReflection.CalculateReflectionMatrix(matrix4x, plane);
		this.m_Oldpos = cam.transform.position;
		Vector3 position2 = matrix4x.MultiplyPoint(this.m_Oldpos);
		reflectCamera.worldToCameraMatrix = cam.worldToCameraMatrix * matrix4x;
		Vector4 clipPlane = this.CameraSpacePlane(reflectCamera, position, up, 1f);
		Matrix4x4 matrix4x2 = cam.projectionMatrix;
		matrix4x2 = MirrorReflection.CalculateObliqueMatrix(matrix4x2, clipPlane);
		reflectCamera.projectionMatrix = matrix4x2;
		reflectCamera.transform.position = position2;
		Vector3 eulerAngles2 = cam.transform.eulerAngles;
		reflectCamera.transform.eulerAngles = new Vector3(-eulerAngles2.x, eulerAngles2.y, eulerAngles2.z);
		reflectCamera.Render();
		GL.invertCulling = false;
		if (this.m_DisablePixelLights)
		{
			QualitySettings.pixelLightCount = pixelLightCount;
		}
	}

	// Token: 0x0600001D RID: 29 RVA: 0x00002C00 File Offset: 0x00000E00
	private void SaneCameraSettings(Camera helperCam)
	{
		helperCam.depthTextureMode = DepthTextureMode.None;
		helperCam.backgroundColor = Color.black;
		helperCam.clearFlags = CameraClearFlags.Color;
		helperCam.renderingPath = RenderingPath.Forward;
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00002C24 File Offset: 0x00000E24
	private static Matrix4x4 CalculateObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane)
	{
		Vector4 b = projection.inverse * new Vector4(MirrorReflection.Sgn(clipPlane.x), MirrorReflection.Sgn(clipPlane.y), 1f, 1f);
		Vector4 vector = clipPlane * (2f / Vector4.Dot(clipPlane, b));
		projection[2] = vector.x - projection[3];
		projection[6] = vector.y - projection[7];
		projection[10] = vector.z - projection[11];
		projection[14] = vector.w - projection[15];
		return projection;
	}

	// Token: 0x0600001F RID: 31 RVA: 0x00002CD8 File Offset: 0x00000ED8
	private static Matrix4x4 CalculateReflectionMatrix(Matrix4x4 reflectionMat, Vector4 plane)
	{
		reflectionMat.m00 = 1f - 2f * plane[0] * plane[0];
		reflectionMat.m01 = -2f * plane[0] * plane[1];
		reflectionMat.m02 = -2f * plane[0] * plane[2];
		reflectionMat.m03 = -2f * plane[3] * plane[0];
		reflectionMat.m10 = -2f * plane[1] * plane[0];
		reflectionMat.m11 = 1f - 2f * plane[1] * plane[1];
		reflectionMat.m12 = -2f * plane[1] * plane[2];
		reflectionMat.m13 = -2f * plane[3] * plane[1];
		reflectionMat.m20 = -2f * plane[2] * plane[0];
		reflectionMat.m21 = -2f * plane[2] * plane[1];
		reflectionMat.m22 = 1f - 2f * plane[2] * plane[2];
		reflectionMat.m23 = -2f * plane[3] * plane[2];
		reflectionMat.m30 = 0f;
		reflectionMat.m31 = 0f;
		reflectionMat.m32 = 0f;
		reflectionMat.m33 = 1f;
		return reflectionMat;
	}

	// Token: 0x06000020 RID: 32 RVA: 0x00002E90 File Offset: 0x00001090
	private static float Sgn(float a)
	{
		if (a > 0f)
		{
			return 1f;
		}
		if (a < 0f)
		{
			return -1f;
		}
		return 0f;
	}

	// Token: 0x06000021 RID: 33 RVA: 0x00002EB4 File Offset: 0x000010B4
	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 point = pos + normal * this.clipPlaneOffset;
		Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
		Vector3 lhs = worldToCameraMatrix.MultiplyPoint(point);
		Vector3 vector = worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(vector.x, vector.y, vector.z, -Vector3.Dot(lhs, vector));
	}

	// Token: 0x0400000B RID: 11
	public LayerMask reflectionMask = -1;

	// Token: 0x0400000C RID: 12
	[SerializeField]
	private MirrorReflection.QualityLevels Quality = MirrorReflection.QualityLevels.Medium;

	// Token: 0x0400000D RID: 13
	[Tooltip("Color used instead of skybox if you choose to not render it.")]
	public Color clearColor = Color.grey;

	// Token: 0x0400000E RID: 14
	public bool reflectSkybox = true;

	// Token: 0x0400000F RID: 15
	public bool m_DisablePixelLights;

	// Token: 0x04000010 RID: 16
	[Tooltip("You won't be able to select objects in the scene when thi is active.")]
	public bool UpdateSceneView = true;

	// Token: 0x04000011 RID: 17
	public float clipPlaneOffset = 0.07f;

	// Token: 0x04000012 RID: 18
	private string reflectionSampler = "_ReflectionTex";

	// Token: 0x04000013 RID: 19
	private Vector3 m_Oldpos;

	// Token: 0x04000014 RID: 20
	private Camera m_ReflectionCamera;

	// Token: 0x04000015 RID: 21
	private Material m_SharedMaterial;

	// Token: 0x04000016 RID: 22
	private Dictionary<Camera, bool> m_HelperCameras;

	// Token: 0x02000008 RID: 8
	private enum QualityLevels
	{
		// Token: 0x04000018 RID: 24
		High = 1,
		// Token: 0x04000019 RID: 25
		Medium,
		// Token: 0x0400001A RID: 26
		Low = 4,
		// Token: 0x0400001B RID: 27
		VeryLow = 8
	}
}
