using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HecticUFO
{
    public class UFOCamera : UnityObject
    {
        public Renderer RestartText;
        public Renderer CreditsText;
        public Renderer DefeatText;
        public Renderer WinText;
        public Renderer FeedText;
        public Renderer DestroyText;

        public readonly Camera UnityCamera; 
        public UFOCamera()
            : base (Assets.Prefabs.CameraPrefab)
        {
            FeedText = FindChildComponent<Renderer>("Feed");
            DestroyText = FindChildComponent<Renderer>("Destroy");
            RestartText = FindChildComponent<Renderer>("Restart");
            DefeatText = FindChildComponent<Renderer>("Defeat");
            WinText = FindChildComponent<Renderer>("Win");
            CreditsText = FindChildComponent<Renderer>("Credits");

            FeedText.enabled = true;
            DestroyText.enabled = false;
            RestartText.enabled = false;
            DefeatText.enabled = false;
            WinText.enabled = false;
            CreditsText.enabled = false;

            UnityCamera = FindChildComponent<Camera>("UnityCamera");
            UnityCamera.transform.LookAt(WorldPosition);
        }
    }
}
