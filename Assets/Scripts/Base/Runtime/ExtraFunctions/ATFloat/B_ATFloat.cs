using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Object = System.Object;
namespace Base {
    [Serializable]
    public class B_ATFloat {
        public float BaseValue;
        protected bool isDirty = true;
        protected float lastBaseValue = float.MinValue;
        protected float _value;
        public virtual float Value {
            get {
                if (isDirty || lastBaseValue != BaseValue) {
                    lastBaseValue = BaseValue;
                    _value = CalculateFinalValue();
                    isDirty = false;
                }
                return _value;
            }
        }

        protected readonly ReadOnlyCollection<B_ATModifier> StatModifiers;
        protected readonly List<B_ATModifier> statModifiers;

        public B_ATFloat() {
            statModifiers = new List<B_ATModifier>();
            StatModifiers = statModifiers.AsReadOnly();
        }

        public B_ATFloat(float baseValue) : this() {
            BaseValue = baseValue;
        }

        public virtual void AddModifier(B_ATModifier mod) {
            isDirty = true;
            statModifiers.Add(mod);
            statModifiers.Sort(CompareModifierOrder);
        }

        public virtual bool RemoveModifier(B_ATModifier mod) {
            if (statModifiers.Remove(mod)) {
                isDirty = true;
                return true;
            }
            return false;
        }

        public virtual bool RemoveAllModifiersFromSource(object source) {
            bool didRemove = false;

            for (int i = statModifiers.Count - 1; i >= 0; i--) {
                if (statModifiers[i].Source == source) {
                    isDirty = true;
                    didRemove = true;
                    statModifiers.RemoveAt(i);
                }
            }

            return didRemove;
        }

        protected virtual int CompareModifierOrder(B_ATModifier a, B_ATModifier b) {
            if (a.Order < b.Order)
                return -1;
            else if (a.Order > b.Order)
                return 1;
            return 0;
        }

        protected virtual float CalculateFinalValue() {
            float finalValue = BaseValue;
            float sumPercentToAdd = 0;

            for (int i = 0; i < statModifiers.Count; i++) {

                B_ATModifier mod = statModifiers[i];

                if (mod.Type == AT_AttributeModifierType.Flat) {
                    finalValue += mod.Value;
                }
                else if (mod.Type == AT_AttributeModifierType.PercentMult) {
                    finalValue *= 1 + mod.Value;
                }
                else if (mod.Type == AT_AttributeModifierType.PercentAdd) {
                    sumPercentToAdd += mod.Value;
                    if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != AT_AttributeModifierType.PercentAdd) {
                        finalValue *= 1 + sumPercentToAdd;
                        sumPercentToAdd = 0;
                    }
                }
            }
            return (float)Math.Round(finalValue, 4);
        }


    }

    public enum AT_AttributeModifierType {
        Flat = 100,
        PercentMult = 200,
        PercentAdd = 300
    }
    [Serializable]
    public class B_ATModifier {
        public int Order;
        public float Value;
        public AT_AttributeModifierType Type;
        public Object Source;

        public B_ATModifier(float value, AT_AttributeModifierType type, int order, object source) {
            Value = value;
            Type = type;
            Order = order;
            Source = source;
        }

        public B_ATModifier(float value, AT_AttributeModifierType type) : this(value, type, (int)type, null) {
            
        }

        public B_ATModifier(float value, AT_AttributeModifierType type, int order) : this(value, type, order, null) {
            
        }

        public B_ATModifier(float value, AT_AttributeModifierType type, object source) : this(value, type, (int)type,
            source) {
            
        }
    }
}