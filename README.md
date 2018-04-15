
# DarlRestExample

This is a simple .net core console app that illustrates performing inferences with the DARL inference service.

This is a free service described at [darl.ai/swagger](http://darl.ai/swagger) that permits students and trial customers to experiment wih the DARL language.

DARL is a language that represents knowledge as Fuzzy logic rules. You can read more about the syntax at [Darl.ai/help](http://darl.ai/help/darl)

There is an online syntax-checking and suggestion-giving editor at [darl.ai/darldevelop](http://darl.ai/darldevelop).

You can create your own using the [CodeMirror editor](https://codemirror.net/) and the [DARL extension](https://github.com/drandysip/darl-codemirror) 

You can do much more with DARL than just inference. You can data mine to DARL using unsupervised, supervised or reinforcement learning algorithms, and you can use DARL as the source for an intelligent questionnaire engine, which used within our Bot engine. [See more](http://darl.ai)

This example uses a ruleset that performs the logic and arithmetic for calculation of Taxes in the UK. (It's a little out of date)

The example demonstrates:
* The data structures used to transfer data, DarlInfData and DarlVar
* Performing a remote inference
* Handling uncertainty - supplying fuzzy values

## DarlVar
This is a generic way to represent an input or output value. A cut down but compatible version of the class is supplied to make the features of DARL explicit. [Read more here](https://www.darl.ai/help/darlvar)

### name 
is the name of the input or output

### dataType
is restricted in this example to the enums categorical, numeric or textual and represents tyhe type of data contained.

### Value
represents the central or 'crisp' value being input or output.

### values
a list of doubles used to represent uncertain numeric inputs which are required to be ordered in increasing value.

## DarlInfData
This simple structure just contains the source DARL code and the set of data items to run through the inference engine.

### source
the DARL source. If this has syntax errors they will be returned as the result rather than the inferences

### values
A list of DarlVars containing the data to inference from.

# Code
```C#
       static async Task<List<DarlVar>> PerformInference(string source, List<DarlVar> values)
        {
            var data = new DarlInfData { source = source, values = values };
            var valueString = JsonConvert.SerializeObject(data);
            var client = new HttpClient();
            var response = await client.PostAsync("https://darl.ai/api/Linter/DarlInf", new StringContent(valueString, Encoding.UTF8, "application/json"));
            var resp = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<DarlVar>>(resp);
        }
```
This function sends code and source to the inference API and returns a set of results

```C#
       static async Task DarlInference()
        {
            var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("DarlRestExample.UKTaxNI.darl"));
            var source = reader.ReadToEnd();
            var values = new List<DarlVar>();
            values.Add(new DarlVar { name = "EARNED_INCOME", Value = "15600", dataType = DarlVar.DataType.numeric });
            values.Add(new DarlVar { name = "DIVIDEND_INCOME", Value = "15600", dataType = DarlVar.DataType.numeric });
            values.Add(new DarlVar { name = "AGE_YEARS", Value = "52", dataType = DarlVar.DataType.numeric });
            values.Add(new DarlVar { name = "MARRIED", Value = "False", dataType = DarlVar.DataType.categorical });
            values.Add(new DarlVar { name = "BLIND", Value = "False", dataType = DarlVar.DataType.categorical });
            var response = await PerformInference(source, values);
            Console.WriteLine("Simple crisp example");
            foreach (var r in response)
                Console.WriteLine(r.ToString());
            values.Clear();
            values.Add(new DarlVar { name = "EARNED_INCOME",  values = new List<double> { 12000.0, 18000.0 }, dataType = DarlVar.DataType.numeric });
            values.Add(new DarlVar { name = "DIVIDEND_INCOME", values = new List<double> { 18000.0, 19000.0 }, dataType = DarlVar.DataType.numeric });
            values.Add(new DarlVar { name = "AGE_YEARS", values = new List<double> { 37.0, 42.0 }, dataType = DarlVar.DataType.numeric });
            values.Add(new DarlVar { name = "MARRIED", Value = "False", dataType = DarlVar.DataType.categorical });
            values.Add(new DarlVar { name = "BLIND", categories = new Dictionary<string, double> { { "True", 0.7 }, { "False", 0.3 } }, dataType = DarlVar.DataType.categorical });
            response = await PerformInference(source, values);
            Console.WriteLine("Fuzzy Interval example");
            foreach (var r in response)
                Console.WriteLine(r.ToString());
            values.Clear();
            values.Add(new DarlVar { name = "EARNED_INCOME", dataType = DarlVar.DataType.numeric });
            values.Add(new DarlVar { name = "DIVIDEND_INCOME", dataType = DarlVar.DataType.numeric, Value = "33000", unknown = true });
            values.Add(new DarlVar { name = "AGE_YEARS", values = new List<double> { 37.0, 42.0 }, dataType = DarlVar.DataType.numeric });
            values.Add(new DarlVar { name = "MARRIED", Value = "False", dataType = DarlVar.DataType.categorical });
            values.Add(new DarlVar { name = "BLIND", Value = "False", dataType = DarlVar.DataType.categorical });
            response = await PerformInference(source, values);
            Console.WriteLine("Unknown handling example");
            foreach (var r in response)
                Console.WriteLine(r.ToString());
            values.Clear();

        }
```
This loads an embedded .darl file and then runs a series of examples through the inference engine

## Examples
The first pass shows a standard 'crisp' i.e. non-fuzzy example. In this case we know the 5 values needed to perform the calculations.

The second shows the hidden power of DARL. Underneath the hood, the inference engine runs Fuzzy Arithmetic. In this case, rather than giving a single value for the numeric inputs we are giving range values. We might also put in triangular or trapezoidal fuzzy numbers, in which case we'd use three or four values respectively. So rather than using the central "Value" field we use the "values" field instead. One of the categorical inputs is also fuzzified. Rather than selecting a single category, both are provided with different certainty (degree of truth) values. DARL makes sense all this using exactly the same source.

Finally in the last example we mark the some of the data values as unknown or supply no value, and the system correctly infers that the outputs cannot be calculated and marks them as unknown too.

# Summing up

DARL reinvents the terribly out of fashion expert system for the 21st century. There are a huge number of example applications, especially in law and compliance, where knowledge needs to be captured in an exact- or fuzzy - fashion in order to automate processes once the domain only of professionals. DARL can do this!




