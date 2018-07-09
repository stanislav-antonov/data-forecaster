using System.Collections.Generic;
using System.Linq;

namespace DataForecaster
{
    public static class StudentTDistribution
    {
        public enum Alpha
        {
            _005 = 0,
            _001 = 1,
            _0005 = 2
        }

        private static readonly SortedDictionary<int, double>[] distributions = {
            // For alpha = .005
            new SortedDictionary<int, double>()
            {
                [1] = 63.657,
                [2] = 9.925,
                [3] = 5.841,
                [4] = 4.604,
                [5] = 4.032,

                [6] = 3.707,
                [7] = 3.499,
                [8] = 3.355,
                [9] = 3.250,
                [10] = 3.169,

                [11] = 3.106,
                [12] = 3.055,
                [13] = 3.012,
                [14] = 2.977,
                [15] = 2.947,

                [16] = 2.921,
                [17] = 2.898,
                [18] = 2.878,
                [19] = 2.861,
                [20] = 2.845,

                [21] = 2.831,
                [22] = 2.819,
                [23] = 2.807,
                [24] = 2.797,
                [25] = 2.787,

                [26] = 2.779,
                [27] = 2.771,
                [28] = 2.763,
                [29] = 2.756,
                [30] = 2.750,

                [35] = 2.724,
                [40] = 2.704,
                [50] = 2.678,
                [60] = 2.660,
                [120] = 2.617,

                [int.MaxValue] = 2.576
            }
        };

        public static double Value(int df, Alpha alpha)
        {
            var distribution = distributions[(int)alpha];
            double result = 0;

            if (distribution.ContainsKey(df))
            {
                result = distribution[df];
            }
            else
            {
                var dfs = distribution.Keys.ToArray();
                for (int i = 0; i < dfs.Length - 1; i++)
                {
                    var df1 = dfs[i];
                    var df2 = dfs[i + 1];

                    if (df > df1 && df < df2)
                    {
                        // calculate linear equation
                        var d1 = distribution[df1];
                        var d2 = distribution[df2];

                        var k = (d2 - d1) / (df2 - df1);
                        var b = d1 - k * df1;

                        result = k * df + b;

                        break;
                    }
                }
            }

            return result;
        }
    }
}
