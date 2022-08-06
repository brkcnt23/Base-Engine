using System.Collections.Generic;
using Base.UI;
namespace Base {
    public static class B_UIControl {
        
        private static UIManagerFunctions _uiManager;

        public static List<MenuUISubframe> SubFrames => _uiManager.Subframes;

        public static void Setup(UIManagerFunctions uiManager) {
            _uiManager = uiManager;
            _uiManager.ManagerStrapping();
        }
        
    }
}