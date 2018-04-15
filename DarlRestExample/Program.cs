using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DarlRestExample
{
    class Program
    {
        static void Main(string[] args)
        {
            DarlInference().Wait();
        }

        static async Task DarlInference()
        {
            var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("DarlRestExample.UKTaxNI.darl"));
            var source = reader.ReadToEnd();
            var values = new List<DarlVar>();
            values.Add(new DarlVar { name = "EARNED_INCOME", /*Value = "15600",*/ values = new List<double> { 12000.0, 18000.0 }, dataType = DarlVar.DataType.numeric });
            values.Add(new DarlVar { name = "DIVIDEND_INCOME", Value = "15600", dataType = DarlVar.DataType.numeric });
            values.Add(new DarlVar { name = "AGE_YEARS", Value = "52", dataType = DarlVar.DataType.numeric });
            values.Add(new DarlVar { name = "MARRIED", Value = "False", dataType = DarlVar.DataType.categorical });
            values.Add(new DarlVar { name = "BLIND", Value = "False", dataType = DarlVar.DataType.categorical });
            var data = new DarlInfData { source = source, values = values };
            var valueString = JsonConvert.SerializeObject(data);
            var client = new HttpClient();
            var response = await client.PostAsync("https://darl.ai/api/Linter/DarlInf", new StringContent(valueString, Encoding.UTF8, "application/json"));
            var resp = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<List<DarlVar>>(resp);

        }
    }
}
