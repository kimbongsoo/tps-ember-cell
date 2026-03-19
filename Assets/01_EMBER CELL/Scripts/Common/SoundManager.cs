using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEC
{
    public class SoundManager : SingletonBase<SoundManager>
    {
        // 우리의 SoundManager가 하는 일은 무엇인가?
        // 1. ClockStone의 AudioController에게 플레이 시킬 사운드 전달
        //사운드 설정에 대한 값/이벤트를 전달

        public float MasterVolume // Volume 0 ~ 1
        {
            get => AudioController.GetGlobalVolume();
            set => AudioController.SetGlobalVolume(value);
        }

        public float MusicVolume // Volume 0 ~ 1
        {
            get => AudioController.GetCategoryVolume("Music");
            set => AudioController.SetCategoryVolume("Music", value);
        }

        public float SfxVolume // Volume 0 ~ 1
        {
            get => AudioController.GetCategoryVolume("SFX");
            set => AudioController.SetCategoryVolume("SFX", value);
        }

        public void Initialize()
        {
            //TODO : 개인 설정되어있는 Game Option - Volume 값을 Audio Controller에 전달해서 세팅

            //Hint : PlayerPrefabs를 이용해서, 개인 PC에 옵션 값을 저장하고, 그 값을 불러와서 AudioController의 볼륨을 셋팅하는 기능 구현.
            
        }

        public void PlayMusic(string musicName)
        {
            AudioController.PlayMusic(musicName);
        }

        public void StopMusic()
        {
            AudioController.StopMusic();
        }

        public void PlaySound(string sfxID)
        {
            AudioController.Play(sfxID);
        }

        public void PlaySound(string sfxID, Vector3 position)
        {
            AudioController.Play(sfxID, position);
        }

        public void StopSound(string sfxID)
        {
            AudioController.Stop(sfxID);
        }

    }
}
