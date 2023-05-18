using HarmonyLib;
using Il2CppAssets.Scripts.Models.Rounds;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Bridge;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MelonLoader;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Il2CppAssets.Scripts.Utils;
using UnityEngine.PlayerLoop;

namespace EveryXSecondsSendRound {
    public class Main : MelonMod {

        private bool gameStarted = false;

        private int waitTime = 30;
        private int maxNextRounds = 3;

        [System.Obsolete]
        public override void OnApplicationStart() {
            base.OnApplicationStart();
            //Logger.Log("Voice Activation Has Loaded");
        }

        public override void OnUpdate() {
            base.OnUpdate();

            if (InGame.instance != null) {
                if (InGame.instance.bridge != null) {
                    if (Input.GetKeyDown(KeyCode.F6)) {
                        PopupScreen.instance.ShowSetValuePopup("Time Between Rounds", "Sets the time (seconds) between sending new rounds",
                            new Action<int>(i => {
                                if (i < 1) { i = 1; }
                                if (i > 60) { i = 60; }
                                waitTime = i;
                            }), 1);
                    } else if (Input.GetKeyDown(KeyCode.F7)) {
                        PopupScreen.instance.ShowSetValuePopup("Round Max", "Sets the max number of rounds that can be sent every X seconds",
                            new Action<int>(i => {
                                if (i < 1) { i = 1; }
                                if (i > 10) { i = 10; }
                                maxNextRounds = i;
                            }), 3);
                    }

                    if (!gameStarted) {
                        gameStarted = true;
                        Task task = new Task(Wait);
                        task.Start();
                    }
                }
            }
        }

        private void Wait() {
            Thread.Sleep(waitTime * 1000);
            Random rand = new Random();
            int rounds = rand.Next(maxNextRounds) + 1;
            for (int i = 0; i < rounds; i++) {
                InGame.instance.bridge.SendRaceRound();
            }
            Wait();
        }
    }
}
