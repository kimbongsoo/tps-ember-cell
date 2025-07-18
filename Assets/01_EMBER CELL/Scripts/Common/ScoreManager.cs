using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace KBS
{
    public class ScoreManager : SingletonBase<ScoreManager>
    {
        private int score;

        public int Score => score;

        public void AddScore(int value)
        {
            score += value;
            MainHUD.Instance.SetScore(score);
        }
    }
}
