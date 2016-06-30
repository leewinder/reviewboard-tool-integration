using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RB_Tools.Shared.Review
{
    // Properties for reviews
    public class Properties
    {
        // Level of review possible
        public enum Level
        {
            [Description("Raise code review")]
            FullReview,

            [Description("To be reviewed")]
            PendingReview,

            [Description("Assets only commit")]
            AssetCommit,

            [Description("Version change only")]
            VersionChange,

            [Description("Previously reviewed")]
            PreviouslyReviewed,

            [Description("Skip code review")]
            NoReview
        };
    }
}
