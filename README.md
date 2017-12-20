# CSVParser  
Parse *.csv file  
#### Table of Contents:  
  1. [Project overview](#Project-overview)   
  2. [Usage](#Usage)  
  
### Project overview  
This is a Visual Studio 2017 solution with two projects:  
```
CSVParser:  
Project to build .dll library for parsing *.csv files
```  
```
TestUse:  
Windows forms project to show an example of how to use CSVParser.dll.  
It makes use of the dll located in \lib folder.
```  
  
### Usage  
To use **CSVParser** in your project first you have to add reference: 
- Add reference to the CSVParser.dll
- Add namespace ``using CSVParser;``  
- Create instance of a CSV class. There are three constructors available:  
```C#  
//[filename] is file name with or without extension, with absolute or relative path
CSV csv = new CSV(string [filename]);  
  
//[filename] is file name with or without extension  
//[filepath] is relative or absolute path to the *.csv file  
CSV csv = new CSV(string [filename],string [filepath]);  

//[filename] is file name with or without extension  
//[filepath] is relative or absolute path to the *.csv file  
//[listseparator] is character by which values are sepparated in the *.csv file  
CSV csv = new CSV(string [filename],string [filepath],string [listseparator]);  
```  
First two constructors pick [listseparator] based on location settings of the system.  
  
- Available fields and respective data types:  
```C#  
csv.Data; - List<List<String>>  
```  
- Available methods and data types of inputs and outputs:  
```C#  
csv.GetStringArray(void); - String[,]  
csv.GetDataTable(void); - DataTable  
```  
  
  
