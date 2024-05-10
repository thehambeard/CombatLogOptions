using System.Collections.Generic;

namespace CombatLogOptions.CombatLog
{
    internal class Options
    {
        public const string OPTIONS_FILE = "options.json";
        public const string CANVAS_ALPHA = "CANVAS_ALPHA";

        public const string BACKGROUND_GROUP = "BACKGROUND_GROUP";
        public const string BACKGROUND_RED = "BACKGROUND_RED";
        public const string BACKGROUND_GREEN = "BACKGROUND_GREEN";
        public const string BACKGROUND_BLUE = "BACKGROUND_BLUE";
        public const string BACKGROUND_ALPHA = "BACKGROUND_ALPHA";

        public const string SHADOW_GROUP = "SHADOW_GROUP";
        public const string SHADOW_RED = "SHADOW_RED";
        public const string SHADOW_GREEN = "SHADOW_GREEN";
        public const string SHADOW_BLUE = "SHADOW_BLUE";
        public const string SHADOW_ALPHA = "SHADOW_ALPHA";

        public const string HIGHLIGHT_GROUP = "HIGHLIGHT_GROUP";
        public const string HIGHLIGHT_RED = "HIGHLIGHT_RED";
        public const string HIGHLIGHT_GREEN = "HIGHLIGHT_GREEN";
        public const string HIGHLIGHT_BLUE = "HIGHLIGHT_BLUE";
        public const string HIGHLIGHT_ALPHA = "HIGHLIGHT_ALPHA";
        public const string HIGHLIGHT_DILATE = "HIGHLIGHT_DILATE";
        public const string HIGHLIGHT_SOFTNESS = "HIGHLIGHT_SOFTNESS";

        public static readonly Dictionary<string, Dictionary<string, string>> OptionGroups = new()
        {
            { "BACKGROUND_GROUP", new()
                {
                    { "RED", BACKGROUND_RED },
                    { "GREEN", BACKGROUND_GREEN },
                    { "BLUE", BACKGROUND_BLUE },
                    { "ALPHA", BACKGROUND_ALPHA }
                }
            },

            { "SHADOW_GROUP", new()
                {
                    { "RED", SHADOW_RED },
                    { "GREEN", SHADOW_GREEN },
                    { "BLUE", SHADOW_BLUE },
                    { "ALPHA", SHADOW_ALPHA }
                }
            },
            { "HIGHLIGHT_GROUP", new()
                {
                    { "RED", HIGHLIGHT_RED },
                    { "GREEN", HIGHLIGHT_GREEN },
                    { "BLUE", HIGHLIGHT_BLUE },
                    { "ALPHA", HIGHLIGHT_ALPHA },
                    { "DILATE", HIGHLIGHT_DILATE },
                    { "SOFTNESS", HIGHLIGHT_SOFTNESS}
                }
            },
        };
    }
}
