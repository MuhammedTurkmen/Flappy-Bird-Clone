using System.Collections;
using System.Collections.Generic;
using MET.Applications.Events;
using MET.Core.Attributes;
using MET.Core.Manager;
using MET.Core.Patterns;
using UnityEngine;
using UnityEngine.Audio;
using static MET.Applications.Events.GameEvents;

namespace MET.Applications
{
    public class AdvancedSoundManager : Singleton<AdvancedSoundManager>
    {
        [Separator("Mixer")]
        [SerializeField] private AudioMixer masterMixer;

        [SerializeField] private AudioMixerGroup masterGroup, musicGroup, SFXGroup;

        [Separator("Music")]
        [SerializeField] private AudioSource musicA;
        [SerializeField] private AudioSource musicB;
        [SerializeField] private float musicCrossfadeTime = 1f;

        [Separator("SFX")]
        [SerializeField] private AudioSource sfxPrefab;
        [SerializeField] private int sfxPool = 20;

        [Separator("Randomization")]
        [SerializeField][MinMax(0, 2, 0.05f)] private Vector2 randomPitchRange = new(0.95f, 1.05f);
        [SerializeField][MinMax(0, 2, 0.05f)] private Vector2 randomVolumeRange = new(0.9f, 1f);

        private const string MASTER_VOL_PARAM = "MasterVolume";
        private const string MUSIC_VOL_PARAM = "MusicVolume";
        private const string SFX_VOL_PARAM = "SFXVolume";

        private Queue<AudioSource> sfxQueue;
        private bool aIsActive = true;

        protected override void Awake()
        {
            base.Awake();

            sfxQueue = new Queue<AudioSource>(sfxPool);
            for (int i = 0; i < sfxPool; i++)
            {
                var go = new GameObject("SFX_" + i);
                go.transform.parent = transform;
                var src = Instantiate(sfxPrefab, go.transform);
                src.playOnAwake = false;
                src.GetComponent<AudioSource>().outputAudioMixerGroup = SFXGroup;
                sfxQueue.Enqueue(src);
            }

            if (musicA == null) musicA = CreateMusicSource("MusicA");
            if (musicB == null) musicB = CreateMusicSource("MusicB");
        }

        private void Start()
        {
            EventBus.Me.Subscribe<OnSaveLoaded>(OnSaveLoaded);
        }

        private void OnSaveLoaded(OnSaveLoaded args)
        {
            SetupSounds();
        }

        public void SetupSounds()
        {
            float masterVolume = DataManager.Me.allData.AudioSettingsData.MasterVolume;
            float musicVolume = DataManager.Me.allData.AudioSettingsData.MusicVolume;
            float sfxVolume = DataManager.Me.allData.AudioSettingsData.SFXVolume;

            SetMasterVolume(masterVolume);
            SetMusicVolume(musicVolume);
            SetSFXVolume(sfxVolume);
        }

        private AudioSource CreateMusicSource(string name)
        {
            var go = new GameObject(name);
            go.transform.parent = transform;
            var src = go.AddComponent<AudioSource>();
            src.loop = true;
            src.playOnAwake = false;
            src.spatialBlend = 0f;
            src.GetComponent<AudioSource>().outputAudioMixerGroup = musicGroup;
            return src;
        }

        public void PlayMusicCrossfade(AudioClip clip, float crossfadeTime = -1f)
        {
            if (crossfadeTime < 0) crossfadeTime = musicCrossfadeTime;
            var target = aIsActive ? musicB : musicA;
            var from = aIsActive ? musicA : musicB;
            target.clip = clip;
            target.volume = 0f;
            target.Play();
            StartCoroutine(CrossfadeCoroutine(from, target, crossfadeTime));
            aIsActive = !aIsActive;
        }

        private IEnumerator CrossfadeCoroutine(AudioSource from, AudioSource to, float t)
        {
            float elapsed = 0f;
            while (elapsed < t)
            {
                float p = elapsed / t;
                from.volume = 1f - p;
                to.volume = p;
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            from.Stop();
            to.volume = 1f;
        }

        public void PlaySFX(AudioClip clip, Vector3? position = null, float volumeMultiplier = 1f)
        {
            if (clip == null) return;
            var src = sfxQueue.Count > 0 ? sfxQueue.Dequeue() : null;
            if (src == null) { AudioSource.PlayClipAtPoint(clip, position ?? Camera.main.transform.position, volumeMultiplier); return; }

            src.transform.position = position ?? transform.position;
            src.clip = clip;
            src.pitch = Random.Range(randomPitchRange.x, randomPitchRange.y);
            src.volume = Mathf.Clamp01(Random.Range(randomVolumeRange.x, randomVolumeRange.y) * volumeMultiplier);
            src.spatialBlend = position.HasValue ? 1f : 0f;
            src.Play();
            StartCoroutine(ReturnWhenDone(src));
        }

        private IEnumerator ReturnWhenDone(AudioSource src)
        {
            yield return new WaitForSecondsRealtime(src.clip.length / Mathf.Abs(src.pitch));
            sfxQueue.Enqueue(src);
        }

        public void SetMasterVolume(float linear0to1)
        {
            float dB = Mathf.Log10(Mathf.Max(0.0001f, linear0to1)) * 20f;
            masterMixer.SetFloat(MASTER_VOL_PARAM, dB);
            PlayerPrefs.SetFloat("MasterVolume", linear0to1);
        }

        public void SetMusicVolume(float linear0to1)
        {
            float dB = Mathf.Log10(Mathf.Max(0.0001f, linear0to1)) * 20f;
            masterMixer.SetFloat(MUSIC_VOL_PARAM, dB);
            PlayerPrefs.SetFloat("MusicVolume", linear0to1);
        }

        public void SetSFXVolume(float linear0to1)
        {
            float dB = Mathf.Log10(Mathf.Max(0.0001f, linear0to1)) * 20f;
            masterMixer.SetFloat(SFX_VOL_PARAM, dB);
            PlayerPrefs.SetFloat("SFXVolume", linear0to1);
        }
    }
}