# VanHackaton - PDF Form Filler
This is an REST API that dinamically fill PDF forms with database data.
## The Challenge
### Why
We are trying to automate some of our daily tasks. One of them is filling out immigration PDF forms. We need to be able to fill out a specific PDF form using data from our database.
### What
We would like to be able to fill out a PDF form automatically and generate a new version with all its fields filled out.
### Recommendations
+ You can use any programming language to accomplish this task;
+ You’re free to use any 3rd party API/library, but you must include all detailed info about it;
+ You must include detailed information about how to run your project.
### Output
+ The same PDF with some data filled in
### PDF File
https://catalogue.servicecanada.gc.ca/apps/EForms/pdf/en/ESDC-EMP5624.pdf
## Features
+ Language: C# with .NET Core 2.1
+ 3rd party tools:
  + [iText 7 Community](https://itextpdf.com/en)
### iText 7
This is a powerful PDF Toolkit for PDF generation, PDF programming, handling & manipulation. It was used in this project to scan and fill the forms of the PDF file. This API has an open source [AGPL license](https://www.gnu.org/licenses/agpl-3.0.html). For more information access their [website](https://itextpdf.com/en).
## Run locally
To run the project you must have installed [.NET Core SDK 2.1](https://dotnet.microsoft.com/download/dotnet-core/2.1)
### From command line
``` bash
$ cd [SOLUTION_PATH]/PdfFormFiller.Api
$ dotnet restore
$ dotnet clean
$ dotnet build
$ dotnet run
```
### From Visual Studio
To run from Visual Studio, you must have installed [Visual Studio 2019](https://visualstudio.microsoft.com/vs/) or later.
+ Open the solution on Visual Studio and press F5.
## Usage
Assuming you are running the project locally, the base url depends on how you run the project and what is configured on *launchSettings.json* file.
### Fill PDF form
#### GET /api/v1/pdfform/:pdfFormId/fill?ids=[arrayResourceIds]
Fill the specified PDF form file and returns to the filled PDF.
+ Params:
  + pdfFormId: PDF form id you which to fill
  + arrayResourceIds: list of DB resources ids to fill the form with their data (these are the resources mapped to the form)

Responses:
+ 200 - OK
+ 400 - Bad Request
+ 404 - Not Found
+ 500 - Internal Server Error

Example request: https://localhost:5001/api/pdfform/ESDC-EMP5624/fill?ids[0]=employerid&ids[1]=jobofferid

Exemple response: the filled PDF file
```
HTTP/1.0 200 OK
Content-Type: application/pdf
```
### More usage information
You can get more information about the API accessing Swagger, when running your locally through url: [base_url]/swagger
