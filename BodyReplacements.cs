using ModelReplacement;
using UnityEngine;
using Dissonance;
using GameNetcodeStuff;

// CREDIT TO TheGooberator FOR THE MOUTH MOVEMENT CODE
// CHECK HIM OUT HERE https://www.youtube.com/channel/UCH3X6daPk227gJRjzYNvvRw

namespace ModelReplacement
{
	//Your model class
	public class MRGOJIRA : BodyReplacementBase
    {

        protected override GameObject LoadAssetsAndReturnModel()
        { 
            string model_name = "gojira";
            return Assets.MainAssetBundle.LoadAsset<GameObject>(model_name);
        }
		// Paste this segment INSIDE your model class
		protected override void AddModelScripts()
		{
			//Set the path on the next line to the path to your models jaw bone
			GameObject jaw = ((Component)base.replacementModel.transform.Find("mechagoji_human_size_ARM/root/bip_pelvis/bip_spine_0/bip_spine_1/bip_neck/neck1/bip_head/jaw")).gameObject;
			JawSync jawSync = jaw.AddComponent(typeof(JawSync)) as JawSync;
			jawSync.player = ((BodyReplacementBase)this).controller;
			jawSync.init();
		}
	}
	// Paste the below segment separately from your model class
    public class JawSync : MonoBehaviour
    {
        public PlayerControllerB player;
		// Experiment with initialJawOpening and maxJawOpening angles and see what works for your mod
		public float initialJawOpening = 60f;

		public float maxJawOpening = 30f;

		// If you don't want to implement the config settings in Plugin.cs, set Plugin.voiceSensitivity.Value to a number from 5 to 20
		public float sensitivity => Plugin.voiceSensitivity.Value;

        Vector3 startingLocalRotation;

        protected VoicePlayerState voice;

		private void Start()
		{
			startingLocalRotation = ((Component)this).gameObject.transform.localEulerAngles;
		}

		private void Update()
        {
			if ((Object)(object)StartOfRound.Instance.voiceChatModule == (Object)null) return;
			if (voice == null) init();
			else
			{
				float x = voice.IsSpeaking && !player.isPlayerDead ? Mathf.Clamp(Map(voice.Amplitude * sensitivity, 0, 1, initialJawOpening, maxJawOpening), maxJawOpening, initialJawOpening) : initialJawOpening;

				Vector3 localRotation = startingLocalRotation;
				localRotation.x = x;
				((Component)this).gameObject.transform.localRotation = Quaternion.Euler(localRotation);
			}
		}

		public void init()
		{
			StartOfRound.Instance.RefreshPlayerVoicePlaybackObjects();
			voice = player.voicePlayerState;
			if (voice == null && (Object)(object)player == (Object)(object)StartOfRound.Instance.localPlayerController)
			{
				voice = StartOfRound.Instance.voiceChatModule.FindPlayer(StartOfRound.Instance.voiceChatModule.LocalPlayerName);
			}
		}

		float Map(float value, float min1, float max1, float min2, float max2)
		{
			float perc = (value - min1) / (max1 - min1);
			return perc * (max2 - min2) + min2;
		}


	}
}