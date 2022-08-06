using System.Threading.Tasks;
using DG.Tweening;
namespace Base.UI {
    public class Main : MenuUISubframe {
        public override Task SetupFrame(UIManagerFunctions Mainframe) {
            return base.SetupFrame(Mainframe);
        }

        public override Tween EnableUI(float Time = 0, bool Snap = true) {
            return base.EnableUI(Time, Snap);
        }

        public override Tween DisableUI(float Time = 0, bool Snap = true) {
            return base.DisableUI(Time, Snap);
        }
    }
}