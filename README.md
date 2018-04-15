# DarlRestExample

This is a simple .net core console app that illustrates performing inferences with the DARL inference service.

DARL is a language that represents knowledge as Fuzzy logic rules. You can read more about the syntax at [Darl.ai/help](http://darl.ai/help/darl)

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




