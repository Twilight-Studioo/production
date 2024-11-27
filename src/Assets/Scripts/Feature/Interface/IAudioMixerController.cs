using Feature.Common.Constants;

namespace Feature.Interface
{
    public interface IAudioMixerController
    {
        public void SetMasterVolume(float volume);
        
        public void SetBgmVolume(float volume);
        
        public void SetSeVolume(float volume);
        
        public void PlayLoopFile(AudioAssetType audioType);

        public void PlayOneShotSE(AudioAssetType type);

        public void LoadSaveData();
    }
}