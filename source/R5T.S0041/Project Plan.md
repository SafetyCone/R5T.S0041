# R5T.S0041
Script to survey all functionality in local project files.


## Instructions

Daily run:

=> Run Program.Run()

Which runs:

1. Run Program.RunMethod() -> GetAllDescriptors()
2. Run Program.RunMethod() -> CreateDatedFiles()
3. Run Program.RunMethod() -> CompareFilesForTwoDates() (Be sure to set the inputs dates correctly at the top of the method.)
4. Run Program.RunMethod() -> CreateSummaryFile() (Be sure to set the inputs dates correctly at the top of the method.)
5. Run Program.RunMethod() -> CreateSummaryPresentationFile() (Be sure to set the inputs dates correctly at the top of the method.)


To add attributes:

1. Add the values in: R5T.S0041\Code\Values\IInstanceVariety.cs
2. Add values in: IInstanceVarietyOperator.GetAllInstanceVarietyNames().
3. Modify: IInstanceVarietyOperator.GetAllInstanceVarietyNames_InPresentationOrder(), make sure the instance names are in the desired order for output.
4. Modify: Program.GetAllDescriptors(), taking care to decided whether we want a method, a property, or a type.
	A. Add namespaces in: R5T.Z0006\Code\Values\Interfaces\INamespacedTypeNames.cs

That's it!
