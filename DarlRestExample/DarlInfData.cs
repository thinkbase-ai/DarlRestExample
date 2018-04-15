using System;
using System.Collections.Generic;
using System.Text;

namespace DarlRestExample
{
    class DarlInfData
    {
        /// <summary>
        /// The darl source
        /// </summary>
        public string source { get; set; }

        /// <summary>
        /// The associated data values
        /// </summary>
        public List<DarlVar> values { get; set; }
    }
}
