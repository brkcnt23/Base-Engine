using System.Threading.Tasks;
using UnityEngine;
namespace Base.UI {
    public class UI_CPanelSubframe : UI_TComponentsSubframe {
        [SerializeField] private bool SafeArea;
        public override Task SetupComponentSubframe(MenuUISubframe Manager) {
            if (SafeArea) {
                this.AddSafeArea();
            }
            return base.SetupComponentSubframe(Manager);
        }
    }
}