using GraphQL.Client;
using GraphQL.Common.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
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
            values.Add(new DarlVar { name = "EARNED_INCOME", value = "15600", dataType = DarlVar.DataType.numeric });
            values.Add(new DarlVar { name = "DIVIDEND_INCOME", value = "15600", dataType = DarlVar.DataType.numeric });
            values.Add(new DarlVar { name = "AGE_YEARS", value = "52", dataType = DarlVar.DataType.numeric });
            values.Add(new DarlVar { name = "MARRIED", value = "False", dataType = DarlVar.DataType.categorical });
            values.Add(new DarlVar { name = "BLIND", value = "False", dataType = DarlVar.DataType.categorical });
            var response = await PerformInference(source, values);
            Console.WriteLine("Simple crisp example");
            foreach (var r in response)
                Console.WriteLine(r.ToString());
            values.Clear();
            values.Add(new DarlVar { name = "EARNED_INCOME",  values = new List<double> { 12000.0, 18000.0 }, dataType = DarlVar.DataType.numeric });
            values.Add(new DarlVar { name = "DIVIDEND_INCOME", values = new List<double> { 18000.0, 19000.0 }, dataType = DarlVar.DataType.numeric });
            values.Add(new DarlVar { name = "AGE_YEARS", values = new List<double> { 37.0, 42.0 }, dataType = DarlVar.DataType.numeric });
            values.Add(new DarlVar { name = "MARRIED", value = "False", dataType = DarlVar.DataType.categorical });
            values.Add(new DarlVar { name = "BLIND", categories = new Dictionary<string, double> { { "True", 0.7 }, { "False", 0.3 } }, dataType = DarlVar.DataType.categorical });
            response = await PerformInference(source, values);
            Console.WriteLine("Fuzzy Interval example");
            foreach (var r in response)
                Console.WriteLine(r.ToString());
            values.Clear();
            values.Add(new DarlVar { name = "EARNED_INCOME", dataType = DarlVar.DataType.numeric });
            values.Add(new DarlVar { name = "DIVIDEND_INCOME", dataType = DarlVar.DataType.numeric, value = "33000", unknown = true });
            values.Add(new DarlVar { name = "AGE_YEARS", values = new List<double> { 37.0, 42.0 }, dataType = DarlVar.DataType.numeric });
            values.Add(new DarlVar { name = "MARRIED", value = "False", dataType = DarlVar.DataType.categorical });
            values.Add(new DarlVar { name = "BLIND", value = "False", dataType = DarlVar.DataType.categorical });
            response = await PerformInference(source, values);
            Console.WriteLine("Unknown handling example");
            foreach (var r in response)
                Console.WriteLine(r.ToString());
            values.Clear();

        }

        static async Task<List<DarlVar>> PerformInference(string source, List<DarlVar> values)
        {

            GraphQLClient client = new GraphQLClient("https://darl.dev/graphql/");
            var authcode = "Your authorization code here";
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {authcode}");
            client.Options.JsonSerializerSettings.Converters.Add(new StringEnumConverter());
            var req = new GraphQLRequest() {
                Query = @"mutation ifd($code: String!, $inputs: [darlVarUpdate]!){inferFromDarl(code: $code, inputs: $inputs){name value dataType unknown weight}}",
                Variables = new {code = source, inputs = DarlVarInput.Convert(values)},
                OperationName = "ifd"
            };
            var resp = await client.PostAsync(req);
            if(resp.Errors != null)//error handling, for instance DARL compilation errors
            {
                var errors = new List<DarlVar>();
                int errorCount = 1;
                foreach(var error in resp.Errors)
                {
                    errors.Add(new DarlVar { name = $"error{errorCount++}", value = error.Message, dataType = DarlVar.DataType.textual });
                }
                return errors; //report errors
            }
            return resp.GetDataFieldAs<List<DarlVar>>("inferFromDarl");
        }
    }
}
