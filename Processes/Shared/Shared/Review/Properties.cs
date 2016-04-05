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

            [Description("Commit will be reviewed later")]
            PendingReview,

            [Description("No review as commit only includes assets")]
            AssetCommit,

            [Description("Skip code review")]
            NoReview
        };
    }
}
