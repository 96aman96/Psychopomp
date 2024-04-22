using UnityEngine;
using System;
using UnityEngine.SceneManagement;


namespace CompassNavigatorPro
{

    public partial class CompassPro : MonoBehaviour {

		[NonSerialized]
		public bool needFogOfWarUpdate, needFogOfWarTextureUpdate;

		const string FOG_OF_WAR_LAYER = "FogOfWarLayer";
		Texture2D fogOfWarTexture;
		Color32[] fogOfWarColorBuffer;
		Material fogOfWarMaterial;
		int fogOfWarAutoClearLastPosX = int.MaxValue, fogOfWarAutoClearLastPosZ;

		bool currentMiniMapUsesFogOfWar => _fogOfWarEnabled && (currentMiniMapContents == MiniMapContents.TopDownWorldView || currentMiniMapContents == MiniMapContents.WorldMappedTexture);

		#region Fog Of War

		void UpdateFogOfWarOnLoadScene(Scene scene, LoadSceneMode loadMode) {
			if (loadMode == LoadSceneMode.Single) {
				UpdateFogOfWar ();
			}
		}

		void UpdateFogOfWarTexture () {

			if (miniMapCamera == null)
				return;
			
			Transform fogOfWarLayer = transform.Find (FOG_OF_WAR_LAYER);
			if (fogOfWarLayer != null) {
				DestroyImmediate (fogOfWarLayer.gameObject);
			}
			
			if (!currentMiniMapUsesFogOfWar) return;

			if (fogOfWarTexture == null || fogOfWarTexture.width != _fogOfWarTextureSize || fogOfWarTexture.height != _fogOfWarTextureSize) {
				fogOfWarTexture = new Texture2D (_fogOfWarTextureSize, _fogOfWarTextureSize, TextureFormat.Alpha8, false);
				fogOfWarTexture.filterMode = FilterMode.Bilinear;
				fogOfWarTexture.wrapMode = TextureWrapMode.Clamp;
                ResetFogOfWar(_fogOfWarDefaultAlpha);
            } else {
				if (fogOfWarColorBuffer == null || fogOfWarColorBuffer.Length != fogOfWarTexture.width * fogOfWarTexture.height || !Application.isPlaying) {
                    ResetFogOfWar(_fogOfWarDefaultAlpha);
                }
			}

            // Update volumes
            CompassProFogVolume[] fv = Misc.FindObjectsOfType<CompassProFogVolume> ();
			Array.Sort (fv, VolumeComparer);
			for (int k = 0; k < fv.Length; k++) {
				Collider collider = fv [k].GetComponent<Collider> ();
				if (collider != null && collider.gameObject.activeInHierarchy) {
					SetFogOfWarAlpha (collider.bounds, fv [k].alpha, fv [k].border);
				}
			}
			needFogOfWarTextureUpdate = true;
		}

		void UpdateFogOfWarPosition () {
			
			if (!currentMiniMapUsesFogOfWar)
				return;
			
			if (needFogOfWarUpdate) {
				needFogOfWarUpdate = false;
				UpdateFogOfWarTexture ();
			}

			if (_fogOfWarAutoClear) {
				int x = (int)followPos.x;
				int z = (int)followPos.z;
				if (x != fogOfWarAutoClearLastPosX || z != fogOfWarAutoClearLastPosZ) {
					fogOfWarAutoClearLastPosX = x;
					fogOfWarAutoClearLastPosZ = z;
					SetFogOfWarAlpha (followPos, _fogOfWarAutoClearRadius, 0, 1f);
				}
			}

			if (needFogOfWarTextureUpdate) {
				needFogOfWarTextureUpdate = false;
				if (fogOfWarTexture != null) {
                    fogOfWarTexture.SetPixels32(fogOfWarColorBuffer);
					fogOfWarTexture.Apply ();
				}
			}
		}

		int VolumeComparer (CompassProFogVolume v1, CompassProFogVolume v2) {
			if (v1.order < v2.order) {
				return -1;
			} else if (v1.order > v2.order) {
				return 1;
			} else {
				return 0;
			}
		}

		#endregion
	}


}



