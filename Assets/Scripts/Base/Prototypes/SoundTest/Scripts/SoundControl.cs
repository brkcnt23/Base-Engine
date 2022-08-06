namespace Base.SoundManagement {
    public static class SoundControl  {

        private static SoundManager _soundManager;
        public static void Setup(SoundManager manager) {
            _soundManager = manager;
            manager.ManagerStrapping();
        }
        public static Sound PlaySound(this Enum_MainMusicCollection soundName) {
            return _soundManager.PlaySound(soundName.ToString());
        }

        public static Sound GetSound(this Enum_MainMusicCollection soundName) {
            return _soundManager.GetSound(soundName.ToString());
        }
    }
}