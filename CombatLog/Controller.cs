using CombatLogOptions.Utility;
using Kingmaker.PubSubSystem;
using UnityEngine;

namespace CombatLogOptions.CombatLog
{
    internal class Controller : IModEventHandler, IAreaLoadingStagesHandler, IAreaHandler
    {
        public Builder Builder { get; private set; }

        public static Controller Instance { get; private set; }
        public int Priority => 100;
        public Color HighlightColor => new(
            Builder.CurrentOptions[Options.HIGHLIGHT_RED],
            Builder.CurrentOptions[Options.HIGHLIGHT_GREEN],
            Builder.CurrentOptions[Options.HIGHLIGHT_BLUE],
            Builder.CurrentOptions[Options.HIGHLIGHT_ALPHA]);

        public float HighlightDilate => Builder.CurrentOptions[Options.HIGHLIGHT_DILATE];
        public float HighlightSoftness => Builder.CurrentOptions[Options.HIGHLIGHT_SOFTNESS];

        public void Bind()
        {
            Builder ??= new();
            Builder.Build();
        }

        public void Unbind()
        {
            Builder.Save();
        }

        public void OnAreaBeginUnloading() => Unbind();
        public void OnAreaLoadingComplete() => Bind();
        public void OnAreaDidLoad() { }
        public void OnAreaScenesLoaded() { }

        public void HandleModEnable()
        {
            EventBus.Subscribe(this);
            Instance = this;
        }

        public void HandleModDisable()
        {
            EventBus.Unsubscribe(this);
            Instance = null;
        }
    }
}
