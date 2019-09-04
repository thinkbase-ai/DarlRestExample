using System;
using System.Collections.Generic;
using System.Text;

namespace DarlRestExample
{
    public class DarlVarInput
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string name { get; set; }

        /// <summary>
        /// This result is unknown if true.
        /// </summary>
        /// <value><c>true</c> if unknown; otherwise, <c>false</c>.</value>
        public bool unknown { get; set; } = false;
        /// <summary>
        /// The confidence placed in this result
        /// </summary>
        /// <value>The weight.</value>
        public double weight { get; set; } = 1.0;

        /// <summary>
        /// The array containing the up to 4 values representing the fuzzy number.
        /// </summary>
        /// <value>The values.</value>
        /// <remarks>Since all fuzzy numbers used by DARL are convex, i,e. their envelope doesn't have any in-folding
        /// sections, the user can specify numbers with a simple sequence of doubles.
        /// So 1 double represents a crisp or singleton value.
        /// 2 doubles represent an interval,
        /// 3 a triangular fuzzy set,
        /// 4 a trapezoidal fuzzy set.
        /// The values must be ordered in ascending value, but it is permissible for two or more to hold the same value.</remarks>
        public List<double> values { get; set; }

        /// <summary>
        /// list of categories, each indexed against a truth value.
        /// </summary>
        /// <value>The categories.</value>
        public List<StringDoublePair> categories { get; set; }


        public List<DateTime> times { get; set; }

        /// <summary>
        /// Indicates approximation has taken place in calculating the values.
        /// </summary>
        /// <value><c>true</c> if approximate; otherwise, <c>false</c>.</value>
        /// <remarks>Under some circumstances the coordinates of the fuzzy number
        /// in "values" may not exactly represent the "cuts" values.</remarks>
//        public bool approximate { get; set; } = false;


        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>The type of the data.</value>
        public DarlVar.DataType dataType { get; set; }

        /// <summary>
        /// Gets or sets the sequence.
        /// </summary>
        /// <value>The sequence.</value>
        public List<List<string>> sequence { get; set; }

        /// <summary>
        /// Single central or most confident value, expressed as a string.
        /// </summary>
        /// <value>The value.</value>
        public string value { get; set; } = string.Empty;


        public static List<DarlVarInput> Convert(List<DarlVar> inputs)
        {
            var list = new List<DarlVarInput>();
            foreach (var d in inputs)
            {
                list.Add(
                        new DarlVarInput
                        {
                            categories = ConvertToList(d.categories),
                            dataType = d.dataType,
                            name = d.name,
                            sequence = d.sequence,
                            times = d.times,
                            unknown = d.unknown,
                            value = d.value,
                            values = d.values,
                            weight = d.weight
                        }
                    );
            }
            return list;
        }

        public static List<StringDoublePair> ConvertToList(Dictionary<string, double> dict)
        {
            if (dict == null)
                return null;
            var list = new List<StringDoublePair>();
            foreach (var k in dict.Keys)
            {
                list.Add(new StringDoublePair { name = k, value = dict[k] });
            }
            return list;
        }
    }
}
