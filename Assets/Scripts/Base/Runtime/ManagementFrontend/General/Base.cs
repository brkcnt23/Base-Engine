namespace Base {
    public static class Base {
        
        public static bool IsInitialized => B_GameControl.IsInitialized;
        public static GameStates GameState => B_GameControl.CurrentGameState;
        
    }
}