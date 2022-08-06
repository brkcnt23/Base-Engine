using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Base.Plugins.Unlockable {
    public class P_Unlockable_Object : MonoBehaviour {
        [Header("Database")] [HideLabel] public P_Unlockable_Data Data;

        [Header("Main")] [Header("2D")] [SerializeField]
        protected Image IMG_ProgressImage_Back;

        [SerializeField] protected Image IMG_ProgressImage_Front;
        [SerializeField] protected TextMeshProUGUI CostText;

        [SerializeField] protected SpriteRenderer REND_Selection;
        [Header("3D")] [SerializeField] protected GameObject Visuals;
        [SerializeField] protected GameObject OriginalModel;
        [SerializeField] protected GameObject GhostModel;
        [SerializeField] protected Collider InteractionCollider;


        [Header("State Objects")] [SerializeField]
        protected GameObject GeneralObjects;

        [SerializeField] protected GameObject OnHiddenObjects;
        [SerializeField] protected GameObject OnUninteractableObjects;
        [SerializeField] protected GameObject OnInteractableObjects;
        [SerializeField] protected GameObject OnUnlockedObjects;


        private void OnMouseEnter() {
            OnSelect();
        }

        private void OnMouseExit() {
            OnDeselect();
        }

        private void OnMouseUpAsButton() {
            if (FindObjectOfType<BaseEngine>() == null)
                OnUnlock(10000, out bool t);

            else
                OnUnlock(Enum_MainSave.Player_Coin.ToInt(), out bool t);
        }

        #region Events

        [FoldoutGroup("Events")] public UnityEvent E_OnHidden;
        [FoldoutGroup("Events")] public UnityEvent E_OnUninteractable;
        [FoldoutGroup("Events")] public UnityEvent E_OnInteractable;
        [FoldoutGroup("Events")] public UnityEvent E_OnUnlockSuccess;
        [FoldoutGroup("Events")] public UnityEvent E_OnUnlockFail;
        [FoldoutGroup("Events")] public UnityEvent E_OnSelect;
        [FoldoutGroup("Events")] public UnityEvent E_OnDeselect;

        #endregion

        public virtual P_Unlockable_Object Setup(P_Unlockable_Data data) {
            this.Data = data;
            InteractionCollider = GetComponent<Collider>();

            switch (this.Data.Status) {
                case P_Unlockable_Status.Hidden:
                    E_OnHidden?.Invoke();
                    break;
                case P_Unlockable_Status.Uninteractable:
                    E_OnUninteractable?.Invoke();
                    break;
                case P_Unlockable_Status.Interactable:
                    E_OnInteractable?.Invoke();
                    break;
                case P_Unlockable_Status.Bought:
                    E_OnUnlockSuccess?.Invoke();
                    break;
            }

            VisualControlSetup();
            return this;
        }

        public virtual P_Unlockable_Object MakeInteractable() {
            if (this.Data.Status != P_Unlockable_Status.Uninteractable) return this;
            E_OnInteractable?.Invoke();
            Data.Status = P_Unlockable_Status.Interactable;
            VisualControlSetup();
            StartCoroutine(IE_OnEnableForBuy());
            return this;
        }

        public virtual P_Unlockable_Object OnUnlock(in int money, out bool success) {
            if (this.Data.Status != P_Unlockable_Status.Interactable) {
                success = true;
                StartCoroutine(IE_OnUnlock(success));
                return this;
            }

            if (money >= Data.Cost) {
                success = true;
                Data.Status = P_Unlockable_Status.Bought;
                E_OnUnlockSuccess?.Invoke();
                P_Unlockable_Manager.instance.OnUnlockableBought(this);
                VisualControlSetup();
                StartCoroutine(IE_OnUnlock(success));
            }
            else {
                success = false;
                E_OnUnlockFail?.Invoke();
                StartCoroutine(IE_OnUnlock(success));
            }

            return this;
        }

        public virtual P_Unlockable_Object OnSelect() {
            E_OnSelect?.Invoke();
            StartCoroutine(IE_OnSelect());
            return this;
        }

        public virtual P_Unlockable_Object OnDeselect() {
            E_OnDeselect?.Invoke();
            StartCoroutine(IE_OnDeselect());
            return this;
        }

        #region IEnumerators

        protected virtual IEnumerator IE_OnEnableForBuy() {
            yield return null;
        }

        protected virtual IEnumerator IE_OnUnlock(bool success) {
            yield return null;
        }

        protected virtual IEnumerator IE_OnSelect() {
            REND_Selection.transform.DOScale(REND_Selection.transform.localScale * 1.2f, .5f)
                .SetLoops(-1, LoopType.Yoyo).SetId("Select");
            yield return null;
        }

        protected virtual IEnumerator IE_OnDeselect() {
            DOTween.Kill("Select");
            REND_Selection.transform.localScale = Vector3.one * 3;
            yield return null;
        }

        private void VisualControlSetup() {
            if (GhostModel == null) GhostModel = OriginalModel.transform.parent.Find("Models_Ghost").gameObject;
            SetStateSpesificObjects();
            SetFunctionSpesificObjects();

            switch (this.Data.Status) {
                case P_Unlockable_Status.Hidden:

                    break;
                case P_Unlockable_Status.Uninteractable:

                    break;
                case P_Unlockable_Status.Interactable:

                    break;
                case P_Unlockable_Status.Bought:

                    break;
            }
        }

        #endregion

        #region Setup Helpers

        private void SetStateSpesificObjects() {
            GeneralObjects.SetActive(true);
            switch (this.Data.Status) {
                case P_Unlockable_Status.Hidden:
                    OnHiddenObjects.SetActive(true);
                    OnUninteractableObjects.SetActive(false);
                    OnInteractableObjects.SetActive(false);
                    OnUnlockedObjects.SetActive(false);
                    break;
                case P_Unlockable_Status.Uninteractable:
                    OnHiddenObjects.SetActive(false);
                    OnUninteractableObjects.SetActive(true);
                    OnInteractableObjects.SetActive(false);
                    OnUnlockedObjects.SetActive(false);
                    break;
                case P_Unlockable_Status.Interactable:
                    OnHiddenObjects.SetActive(false);
                    OnUninteractableObjects.SetActive(false);
                    OnInteractableObjects.SetActive(true);
                    OnUnlockedObjects.SetActive(false);
                    break;
                case P_Unlockable_Status.Bought:
                    OnHiddenObjects.SetActive(false);
                    OnUninteractableObjects.SetActive(false);
                    OnInteractableObjects.SetActive(true);
                    OnUnlockedObjects.SetActive(false);
                    break;
            }
        }

        private void SetFunctionSpesificObjects() {
            switch (this.Data.Status) {
                case P_Unlockable_Status.Hidden:

                    REND_Selection.gameObject.SetActive(false);
                    InteractionCollider.enabled = false;
                    CostText.text = "";

                    OriginalModel.SetActive(false);
                    GhostModel.SetActive(false);
                    IMG_ProgressImage_Back.gameObject.SetActive(false);
                    
                    IMG_ProgressImage_Front.gameObject.SetActive(false);

                    break;
                case P_Unlockable_Status.Uninteractable:

                    REND_Selection.gameObject.SetActive(false);
                    InteractionCollider.enabled = false;
                    CostText.text = "";

                    OriginalModel.SetActive(false);
                    GhostModel.SetActive(true);
                    IMG_ProgressImage_Back.gameObject.SetActive(true);
                    IMG_ProgressImage_Back.color = Color.red;
                    
                    IMG_ProgressImage_Front.gameObject.SetActive(false);

                    break;
                case P_Unlockable_Status.Interactable:

                    REND_Selection.gameObject.SetActive(true);
                    InteractionCollider.enabled = true;
                    CostText.text = "";

                    OriginalModel.SetActive(false);
                    GhostModel.SetActive(true);
                    IMG_ProgressImage_Back.gameObject.SetActive(true);
                    IMG_ProgressImage_Back.color = Color.green;
                    
                    IMG_ProgressImage_Front.gameObject.SetActive(true);

                    break;
                case P_Unlockable_Status.Bought:

                    REND_Selection.gameObject.SetActive(true);
                    InteractionCollider.enabled = true;
                    CostText.text = "";

                    OriginalModel.SetActive(true);
                    GhostModel.SetActive(false);
                    IMG_ProgressImage_Back.gameObject.SetActive(true);
                    IMG_ProgressImage_Back.color = Color.green;
                    
                    IMG_ProgressImage_Front.gameObject.SetActive(true);

                    break;
            }
        }

#if UNITY_EDITOR

        public void CreateGhostObject(Material ghostMaterial = null, int objectIndex = 0) {
            if (ghostMaterial == null) {
                ghostMaterial = new Material(Shader.Find("Standard"));
                ghostMaterial.color = Color.white;
            }

            //Modify the name of the original object
            OriginalModel.transform.name = "Models_Original";

            //Check if there is a ghost object already
            if (OriginalModel.transform.parent.Find("Models_Ghost") == null) {
                //Create a new ghost object
                GameObject ghostObject =
                    Instantiate(OriginalModel.transform.gameObject, OriginalModel.transform.parent);
                this.GhostModel = ghostObject;
                this.GhostModel.name = "Models_Ghost";
            }
            else {
                //Set the ghost object to the existing one
                this.GhostModel = OriginalModel.transform.parent.Find("Models_Ghost").gameObject;
            }

            //Modify the ghost object

            List<GameObject> meshes = new List<GameObject>();
            foreach (var x in GhostModel.GetComponentsInChildren<MeshRenderer>()) {
                meshes.Add(x.gameObject);
            }

            for (int i = 0; i < meshes.Count; i++) {
                meshes[i].TryGetComponent(out Renderer rends);
                Material[] materials = new Material[rends.sharedMaterials.Length];
                for (int x = 0; x < rends.sharedMaterials.Length; x++) {
                    materials[x] = ghostMaterial;
                }

                rends.sharedMaterials = materials;
            }
        }

        public void ClearGhostObjectData() {
            if (GhostModel != null) {
                DestroyImmediate(GhostModel);
            }
        }

        public void OnlyActivateGhostObject() {
            OriginalModel.SetActive(false);
            GhostModel.SetActive(true);
        }

        public void OnlyActivateOriginalObject() {
            OriginalModel.SetActive(true);
            GhostModel.SetActive(false);
        }

        public void ChangeVisualsMode() {
            bool isOriginal = OriginalModel.activeSelf;
            OriginalModel.SetActive(!isOriginal);
            GhostModel.SetActive(isOriginal);
        }

        private void CheckGhostObjectViability() {
            if (GhostModel == null) {
                CreateGhostObject();
            }
        }

        public void EditorVisualControlSetup(P_Unlockable_Status status) {
            this.Data.Status = status;
            SetStateSpesificObjects();
            SetFunctionSpesificObjects();

            switch (status) {
                case P_Unlockable_Status.Hidden:

                    break;
                case P_Unlockable_Status.Uninteractable:

                    break;
                case P_Unlockable_Status.Interactable:

                    break;
                case P_Unlockable_Status.Bought:

                    break;
            }
        }

#endif

        #endregion
    }
}