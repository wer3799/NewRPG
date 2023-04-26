﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotalCreations {
    namespace UI {
        [Serializable]
        public class MinMaxHelper : PropertyAttribute {
            public static MinMaxHelper Zero = new MinMaxHelper(0, 0);
            public static MinMaxHelper MinMax = new MinMaxHelper(float.MinValue, float.MaxValue);
            public static MinMaxHelper MaxMin = new MinMaxHelper(float.MaxValue, float.MinValue);

            public float min;
            public float max;

            public MinMaxHelper(float min, float max) {
                this.min = min;
                this.max = max;
            }

            public MinMaxHelper(float value) {
                this.min = value;
                this.max = value;
            }

            public bool IsUnder(float value) {
                return min > value;
            }

            public bool IsOver(float value) {
                return max < value;
            }

            public bool IsOutside(float value) {
                return IsUnder(value) || IsOver(value);
            }

            public bool IsInside(float value) {
                return value >= min && value <= max;
            }

            public bool IsInside(MinMaxHelper value) {
                return value.min >= min && value.min <= max && value.max >= min && value.max <= max;
            }

            public float Distance() {
                return max - min;
            }

            public float ValueFromPercent(float percent) {
                return min + (max - min) * percent;
            }

            public float PercentFromValue(float value, bool limitBottom = true, bool limitTop = true) {
                return PercentFromValue(this, value, limitBottom, limitTop);
            }

            public float PercentFromValue(MinMaxHelper range, float value, bool limitBottom = true, bool limitTop = true) {
                if (limitBottom && value <= range.min) {
                    return 0f;
                } else if (limitTop && value >= range.max) {
                    return 1f;
                }
                return (value - range.min) / (range.max - range.min);
            }

            public float Random(float step = 0f) {
                float randomValue = ValueFromPercent(UnityEngine.Random.Range(0f, 1f));

                if (step == 0f) {
                    return randomValue;
                } else {
                    float times = randomValue / step;
                    int timesInt = ((int)(times));
                    float result;
                    if (times - (float)timesInt >= 0.5f) {
                        result = this.min + ((float)(timesInt + 1)) * step;
                        if (result > max) {
                            return max;
                        }
                        return result;
                    } else {
                        return this.min + ((float)timesInt) * step;
                    }
                }
            }

            public override string ToString() {
                string text = "min: " + min + " | max: " + max;
                return text;
            }

            public MinMaxHelper Clone() {
                return new MinMaxHelper(min, max);
            }

            public static MinMaxHelper operator +(MinMaxHelper c1, float c2) {
                return new MinMaxHelper(c1.min + c2, c1.max + c2);
            }

            public static MinMaxHelper operator +(MinMaxHelper c1, MinMaxHelper c2) {
                return new MinMaxHelper(c1.min + c2.min, c1.max + c2.max);
            }

            public static MinMaxHelper operator -(MinMaxHelper c1, float c2) {
                return new MinMaxHelper(c1.min - c2, c1.max - c2);
            }

            public static MinMaxHelper operator -(MinMaxHelper c1, MinMaxHelper c2) {
                return new MinMaxHelper(c1.min - c2.min, c1.max - c2.max);
            }

            public static MinMaxHelper operator /(MinMaxHelper c1, float c2) {
                if (c2 < 0) {
                    Debug.LogError("Divide by null not possible!");
                }
                return new MinMaxHelper(c1.min / c2, c1.max / c2);
            }

            public static MinMaxHelper operator *(MinMaxHelper c1, float c2) {
                return new MinMaxHelper(c1.min * c2, c1.max * c2);
            }
        }

        public static class HelperClass {
            public static Vector3 FillVector3(this float self) {
                return new Vector3(self, self, self);
            }
        }
    }
}
