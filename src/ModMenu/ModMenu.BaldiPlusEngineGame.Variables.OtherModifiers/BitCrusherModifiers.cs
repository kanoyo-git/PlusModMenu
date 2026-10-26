using System.Runtime.Serialization;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Interfaces;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;

[DataContract]
internal class BitCrusherModifiers : IModifiers
{
	private class BitCrushComponent : MonoBehaviour
	{
		private float _baseSampleRate;

		internal int _simulatedSampleRate;

		internal int _bitDepth = 32;

		internal bool _enabled;

		private void Awake()
		{
			_baseSampleRate = AudioSettings.outputSampleRate;
			_simulatedSampleRate = AudioSettings.outputSampleRate;
		}

		private void OnAudioFilterRead(float[] data, int _)
		{
			if (!_enabled)
			{
				return;
			}
			float num = (float)_simulatedSampleRate / _baseSampleRate;
			float num2 = Mathf.Pow(2f, _bitDepth);
			float num3 = 0f;
			float num4 = 0f;
			int num5 = 0;
			while (num5 < data.Length)
			{
				num3 += num;
				if (num3 >= 1f)
				{
					num3--;
					num4 = Mathf.Floor(data[num5] * num2 + 0.5f) / num2;
				}
				data[num5++] = num4;
				data[num5++] = num4;
			}
		}
	}

	private static readonly BitCrushComponent _bitCrusherComponent = Singleton<GlobalCam>.Instance.Cam.gameObject.AddComponent<BitCrushComponent>();

	[DataMember]
	[Label<Strings>("BIT_CRUSHER")]
	[Help<HelpText>("BIT_CRUSHER_TITLE", "BIT_CRUSHER_TEXT")]
	[Group<SideMenuGroupOtherModifiersBitCrusher>]
	public virtual bool Enabled
	{
		get
		{
			return _bitCrusherComponent._enabled;
		}
		set
		{
			_bitCrusherComponent._enabled = value;
		}
	}

	[DataMember]
	[Label<Strings>("SAMPLE_RATE")]
	[Help<HelpText>("SAMPLE_RATE_TITLE", "SAMPLE_RATE_TEXT")]
	[Group<SideMenuGroupOtherModifiersBitCrusher>]
	[ValueRange(20, 48000)]
	public virtual int SimulatedSampleRate
	{
		get
		{
			return _bitCrusherComponent._simulatedSampleRate;
		}
		set
		{
			_bitCrusherComponent._simulatedSampleRate = value;
		}
	}

	[DataMember]
	[Label<Strings>("BIT_DEPTH")]
	[Help<HelpText>("BIT_DEPTH_TITLE", "BIT_DEPTH_TEXT")]
	[Group<SideMenuGroupOtherModifiersBitCrusher>]
	[ValueRange(1, 32)]
	public virtual int BitDepth
	{
		get
		{
			return _bitCrusherComponent._bitDepth;
		}
		set
		{
			_bitCrusherComponent._bitDepth = value;
		}
	}
}
