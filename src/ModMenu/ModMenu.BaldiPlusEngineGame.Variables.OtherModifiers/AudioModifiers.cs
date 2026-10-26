using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using PlusModMenu.Core.Common.Extensions;
using PlusModMenu.Core.Unity;
using HarmonyLib;
using MidiPlayerTK;
using ModMenu.BaldiPlusEngineGame.Internal;
using ModMenu.BaldiPlusEngineGame.Settings;
using ModMenu.BaldiPlusEngineGame.Variables.Menus;
using ModMenu.Common;
using ModMenu.Settings.Editors;
using PlusModMenu.Api;
using PlusModMenu.Api.Attributes;
using PlusModMenu.Api.Collectors;
using PlusModMenu.Api.Events;
using PlusModMenu.Api.Interfaces;
using PlusModMenu.Api.Interfaces.Services;
using PlusModMenu.Api.Settings;
using UnityEngine;

namespace ModMenu.BaldiPlusEngineGame.Variables.OtherModifiers;

[DataContract]
internal class AudioModifiers : IModifiers
{
	private static readonly FieldInfo _midiCorrupted = AccessTools.Field(typeof(MusicManager), "midiCorrupted");

	private string _soundFont = string.Empty;

	public bool BypassLock { get; set; }

	[DataMember]
	public string SoundFont
	{
		get
		{
			return _soundFont;
		}
		set
		{
			_soundFont = value;
			if (string.IsNullOrEmpty(_soundFont))
			{
				MidiPlayerGlobal.MPTK_SelectSoundFont("GMGSx_Unmodified");
			}
			else
			{
				MidiPlayerGlobal.MPTK_LoadLiveSF("file://" + value);
			}
		}
	}

	[DataMember]
	public int DrumBank { get; set; }

	[DataMember]
	public int InstrumentBank { get; set; }

	[DataMember]
	[Label<Strings>("MIDI_PITCH")]
	[Help<HelpText>("MIDI_PITCH_TITLE", "MIDI_PITCH_TEXT")]
	[Group<SideMenuGroupOtherModifiersMidi>]
	[OverrideEditorControl<RangeEditor>]
	[ValueRange(-24, 24)]
	public virtual int Pitch
	{
		get
		{
			MusicManager instance = Singleton<MusicManager>.Instance;
			if (!(instance != null))
			{
				return 0;
			}
			return instance.MidiPlayer.MPTK_Transpose;
		}
		set
		{
			if (!(Singleton<MusicManager>.Instance == null))
			{
				BypassLock = true;
				Singleton<MusicManager>.Instance.MidiPlayer.MPTK_Transpose = value;
				BypassLock = false;
			}
		}
	}

	[DataMember]
	[OverrideEditorControl<LockEditor>]
	public virtual bool LockPitch { get; set; }

	[DataMember]
	[Label<Strings>("MIDI_SPEED")]
	[Help<HelpText>("MIDI_SPEED_TITLE", "MIDI_SPEED_TEXT")]
	[Group<SideMenuGroupOtherModifiersMidi>]
	[OverrideEditorControl<RangeEditor>]
	[ValueRange(0.1f, 10f)]
	public virtual float Speed
	{
		get
		{
			MusicManager instance = Singleton<MusicManager>.Instance;
			if (!(instance != null))
			{
				return 0f;
			}
			return instance.MidiPlayer.MPTK_Speed;
		}
		set
		{
			if (!(Singleton<MusicManager>.Instance == null))
			{
				BypassLock = true;
				Singleton<MusicManager>.Instance.SetSpeed(value);
				BypassLock = false;
			}
		}
	}

	[DataMember]
	[OverrideEditorControl<LockEditor>]
	public virtual bool LockSpeed { get; set; }

	[DataMember]
	[Label<Strings>("MIDI_LOOP")]
	[Help<HelpText>("MIDI_LOOP_TITLE", "MIDI_LOOP_TEXT")]
	[Group<SideMenuGroupOtherModifiersMidi>]
	public virtual bool Loop
	{
		get
		{
			MusicManager instance = Singleton<MusicManager>.Instance;
			if (instance != null)
			{
				return instance.MidiPlayer.MPTK_Loop;
			}
			return false;
		}
		set
		{
			if (!(Singleton<MusicManager>.Instance == null))
			{
				BypassLock = true;
				Singleton<MusicManager>.Instance.SetLoop(value);
				BypassLock = false;
			}
		}
	}

	[DataMember]
	[OverrideEditorControl<LockEditor>]
	public virtual bool LockLoop { get; set; }

	[DataMember]
	[Label<Strings>("MUSIC_CORRUPTION")]
	[Help<HelpText>("MUSIC_CORRUPTION_TITLE", "MUSIC_CORRUPTION_TEXT")]
	[Group<SideMenuGroupOtherModifiersMusicManager>]
	public virtual bool MusicCorruption
	{
		get
		{
			MusicManager instance = Singleton<MusicManager>.Instance;
			if (instance != null)
			{
				return _midiCorrupted.GetValue<bool>(instance);
			}
			return false;
		}
		set
		{
			if (!(Singleton<MusicManager>.Instance == null))
			{
				BypassLock = true;
				Singleton<MusicManager>.Instance.SetCorruption(value);
				BypassLock = false;
			}
		}
	}

	[DataMember]
	[OverrideEditorControl<LockEditor>]
	public virtual bool LockMusicCorruption { get; set; }

	[DataMember]
	[OverrideEditorControl<LockEditor>]
	public virtual bool LockSongSelection { get; set; }

	public CallbackList<object> MusicItems { get; } = new CallbackList<object>();

	public AudioModifiers()
	{
		Service.Get<IEventService>().SubscribeOnce<IModMenuInitializedEvent>((object? _) =>
		{
			MidiSoundFontInitialization();
			MusicManagerSongInitialization();
		});
	}

	private void MidiSoundFontInitialization()
	{
		ISettingsGroup group = SettingsRegistryService.Registry.GetGroup(typeof(SideMenuGroupOtherModifiersSoundFont));
		UIText loadingReason = UIText.Resource<Strings>("NOT_AVAILABLE_NOW");
		List<(string name, string path)> soundFonts = new List<(string, string)>();
		List<int> banks = new List<int>();
		ISettingHandle soundFontChoice = null;
		ISettingHandle drumBankChoice = null;
		ISettingHandle instrumentBankChoice = null;
		soundFontChoice = group.AddChoice(UIText.Resource<Strings>("MIDI_SOUNDFONT"), () => soundFonts.ConvertAll(x => x.path.Length == 0 ? UIText.Resource<Strings>("DEFAULT") : UIText.Literal(x.name)), () => Math.Max(soundFonts.FindIndex(x => string.Equals(x.path, SoundFont, StringComparison.Ordinal)), 0), x =>
		{
			string path = soundFonts[x].path;
			if (!string.Equals(path, SoundFont, StringComparison.Ordinal))
			{
				SetLoading(loading: true);
				SoundFont = path;
			}
		}, UIText.Resource<HelpText>("MIDI_SOUNDFONT_TEXT", DataPaths.GetPath("SoundFonts")));
		group.AddButton(UIText.Resource<Strings>("REFRESH_SOUNDFONT_LIST"), GrabSoundFonts);
		drumBankChoice = group.AddChoice(UIText.Resource<Strings>("DRUM_BANK"), BankNames, () => banks.IndexOf(MidiPlayerGlobal.ImSFCurrent.IndexDrumBank), x =>
		{
			MidiPlayerGlobal.MPTK_SelectBankDrum(banks[x]);
		}, UIText.Resource<HelpText>("DRUM_BANK_TEXT"));
		instrumentBankChoice = group.AddChoice(UIText.Resource<Strings>("INSTRUMENT_BANK"), BankNames, () => banks.IndexOf(MidiPlayerGlobal.ImSFCurrent.IndexInstrumentBank), x =>
		{
			MidiPlayerGlobal.MPTK_SelectBankInstrument(banks[x]);
		}, UIText.Resource<HelpText>("INSTRUMENT_BANK_TEXT"));
		MidiPlayerGlobal.OnEventPresetLoaded.AddListener(() =>
		{
			SetBanks();
			SetLoading(loading: false);
		});
		GrabSoundFonts();
		SetBanks();
		void GrabSoundFonts()
		{
			soundFonts.Clear();
			soundFonts.Add((string.Empty, string.Empty));
			FileInfo[] files = new DirectoryInfo(DataPaths.GetPath("SoundFonts")).GetFiles("*.sf2");
			foreach (FileInfo fileInfo in files)
			{
				soundFonts.Add((fileInfo.Name, fileInfo.FullName));
			}
			soundFontChoice.Refresh();
		}
		void SetBanks()
		{
			banks.Clear();
			banks.AddRange(from x in MidiPlayerGlobal.ImSFCurrent.Banks.Select((ImBank x, int i) => new { x, i })
				where x.x != null
				select x.i);
			drumBankChoice.Refresh();
			instrumentBankChoice.Refresh();
		}
		IReadOnlyList<UIText> BankNames()
		{
			return banks.ConvertAll(x => UIText.Literal(x.ToString()));
		}
		void SetLoading(bool loading)
		{
			soundFontChoice.SetAvailable(!loading, loadingReason);
			drumBankChoice.SetAvailable(!loading, loadingReason);
			instrumentBankChoice.SetAvailable(!loading, loadingReason);
		}
	}

	private void MusicManagerSongInitialization()
	{
		ISettingsGroup group = SettingsRegistryService.Registry.GetGroup(typeof(SideMenuGroupOtherModifiersMusicManager));
		int selectedSong = 0;
		ISettingHandle songChoice = group.AddChoice(UIText.Resource<Strings>("SELECT_SONG"), () => MusicItems.Select(SongName).ToList(), () => Math.Min(selectedSong, Math.Max(MusicItems.Count - 1, 0)), x =>
		{
			selectedSong = x;
		}, UIText.Resource<HelpText>("SELECT_SONG_TEXT"));
		MusicItems.ItemAdded += (object x) =>
		{
			if (!(x is LoopingSoundObject) && !(x is string))
			{
				throw new InvalidOperationException("The music item is invalid.");
			}
			songChoice.Refresh();
		};
		MusicItems.ItemRemoved += (object x) =>
		{
			songChoice.Refresh();
		};
		foreach (string midiFile in MidiPlayerGlobal.CurrentMidiSet.MidiFiles)
		{
			MusicItems.Add(midiFile);
		}
		group.AddButton(UIText.Resource<Strings>("STOP_MUSIC"), StopMusic);
		group.AddButton(UIText.Resource<Strings>("PLAY_SELECTED_SONG"), () =>
		{
			if (MusicItems.Count == 0)
			{
				return;
			}
			StopMusic();
			object song = MusicItems[Math.Min(selectedSong, MusicItems.Count - 1)];
			BypassLock = true;
			if (song is LoopingSoundObject loopingSoundObject)
			{
				Singleton<MusicManager>.Instance.QueueFile(loopingSoundObject, loop: false);
			}
			else
			{
				Singleton<MusicManager>.Instance.PlayMidi((string)song, loop: false);
			}
			BypassLock = false;
		});
		void StopMusic()
		{
			BypassLock = true;
			Singleton<MusicManager>.Instance.StopMidi();
			Singleton<MusicManager>.Instance.StopFile();
			BypassLock = false;
		}
		static UIText SongName(object x)
		{
			return x is LoopingSoundObject loopingSoundObject ? UIText.Resource<Strings>("SONG__TYPE_CLIP", loopingSoundObject.name) : UIText.Resource<Strings>("SONG__TYPE_MIDI", (string)x);
		}
		SingletonCollector<LoopingSoundObject>.ItemAdded += MusicItems.Add;
		SingletonCollector<LoopingSoundObject>.ItemRemoved += (LoopingSoundObject x) =>
		{
			MusicItems.Remove(x);
		};
	}

	public void UpdateControl(string propertyName, object? value)
	{
		Service.Get<ISettingsRegistry>().Refresh(this, propertyName);
	}
}
