﻿using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
namespace Base.UI {
    public class Gameover : MenuUISubframe {
        [SerializeField] private GameObject SuccessUI;
        [SerializeField] private GameObject FailUI;

        public override Task SetupFrame(UIManagerFunctions Mainframe) {
            return base.SetupFrame(Mainframe);
        }

        public override Tween EnableUI(float Time = 0, bool Snap = true) {
            return base.EnableUI(Time, Snap);
        }

        public override Tween DisableUI(float Time = 0, bool Snap = true) {
            return base.DisableUI(Time, Snap);
        }

        public void EnableOverUI(bool Success) {
            switch (Success) {
                case true:
                    SuccessUI.SetActive(true);
                    FailUI.SetActive(false);
                    break;
                case false:
                    SuccessUI.SetActive(false);
                    FailUI.SetActive(true);
                    break;
            }
        }
    }
}