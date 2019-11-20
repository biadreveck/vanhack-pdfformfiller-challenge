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
## Introduction
With this project I wanted to do a little more than the challenge required, so instead of hardcoding everything to the specified form, I chose to do it more dynamically. Therefore, with this API you can upload any PDF file and it will be stored on the API server. In addition, you can map all fields in the uploaded file to fixed values, database values, or some calculation.

This project was implemented assuming the data is stored in a NoSQL database (such as MongoDB), so the sample database files are in JSON format.
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
### Upload
#### POST /pdfform/v1/upload/{pdfCode}
This endpoint is used to upload and store a PDF file on API server.
+ Params:
  + pdfCode: a PDF code used to link the uploaded file with dynamic data
  
Responses:
+ 200 - OK
+ 400 - Bad Request
+ 409 - Conflict
+ 500 - Internal Server Error
### Scan
#### GET /pdfform/v1/scan/{pdfCode}
This endpoint is used to get information about the form on a uploaded PDF file. You can use this as a helper to create a PDF form map.
+ Params:
  + pdfCode: the PDF code of the file you which to scan
  
Responses:
+ 200 - OK
+ 400 - Bad Request
+ 404 - Not Found
+ 500 - Internal Server Error

Example request: https://localhost:5001/pdfform/v1/scan/ESDC-EMP5624

Example response:
```
HTTP/1.0 200 OK
Content-Type: application/json
{
  "pdfCode": "ESDC-EMP5624",
  "pages": [
    {
      "number": 1,
      "image": null,
      "formFields": [
        "name": "page1.field1",
        "position":{
          "x":67.074,
          "y":406.999
        },
        "size":{
          "width":10.0,
          "height":10.0
        }
      ]
    }
  ]
}
```
Response object: [PdfScanResult](#pdfscanresult)
### Map
#### GET /pdfform/v1/map/{pdfCode}
This endpoint is used to retrieve a existing map of the PDF file.
+ Params:
  + pdfCode: the PDF code of the file you which to retrieve the map from
  
Responses:
+ 200 - OK
+ 400 - Bad Request
+ 404 - Not Found
+ 500 - Internal Server Error

Example request: https://localhost:5001/pdfform/v1/map/ESDC-EMP5624

Example response:
```
HTTP/1.0 200 OK
Content-Type: application/json
{
  "id": "ESDC-EMP5624",
  "pdfCode": "ESDC-EMP5624",
  "collections": [ "joboffers.json" ],
  "fieldMaps": [
    {
      "name": "page1.field1",
      "condition": {
        "type": 0,
        "left": {
          "type": 1,
          "value": {
            "collection": "joboffers.json",
            "documentField": "IsOnGlobalTalentList"
          }
        },
        "right": {
          "type": 0,
          "value": true
        }
      },
      "value": {
        "type": 0,
        "value": "1"
      }
    }
  ]
}
```
Response object: PdfFormMap
#### POST /pdfform/v1/map
This endpoint is used to create a map for a PDF file. This map is required to fill the PDF form dynamically.

Responses:
+ 200 - OK
+ 400 - Bad Request
+ 409 - Conflict
+ 500 - Internal Server Error

Example request: https://localhost:5001/pdfform/v1/map
```
HTTP/1.0
Content-Type: application/json
{
  "pdfCode": "ESDC-EMP5624",
  "collections": [ "joboffers.json" ],
  "fieldMaps": [
    {
      "name": "page1.field1",
      "condition": {
        "type": 0,
        "left": {
          "type": 1,
          "value": {
            "collection": "joboffers.json",
            "documentField": "IsOnGlobalTalentList"
          }
        },
        "right": {
          "type": 0,
          "value": true
        }
      },
      "value": {
        "type": 0,
        "value": "1"
      }
    }
  ]
}
```
Request object: PdfFormMap

Example response:
```
HTTP/1.0 200 OK
Content-Type: application/json
{
  "id": "ESDC-EMP5624",
  "pdfCode": "ESDC-EMP5624",
  "collections": [ "joboffers.json" ],
  "fieldMaps": [
    {
      "name": "page1.field1",
      "condition": {
        "type": 0,
        "left": {
          "type": 1,
          "value": {
            "collection": "joboffers.json",
            "documentField": "IsOnGlobalTalentList"
          }
        },
        "right": {
          "type": 0,
          "value": true
        }
      },
      "value": {
        "type": 0,
        "value": "1"
      }
    }
  ]
}
```
Response object: PdfFormMap
#### PUT /pdfform/v1/map/{pdfCode}
This endpoint is used to update the information of an existing map of the PDF file.
+ Params:
  + pdfCode: the PDF code of the file you which to update the map
  
Responses:
+ 200 - OK
+ 400 - Bad Request
+ 404 - Not Found
+ 500 - Internal Server Error

Example request: https://localhost:5001/pdfform/v1/map/ESDC-EMP5624
```
HTTP/1.0
Content-Type: application/json
{
  "id": "ESDC-EMP5624",
  "pdfCode": "ESDC-EMP5624",
  "collections": [ "joboffers.json" ],
  "fieldMaps": [
    {
      "name": "page1.field1",
      "condition": {
        "type": 0,
        "left": {
          "type": 1,
          "value": {
            "collection": "joboffers.json",
            "documentField": "IsOnGlobalTalentList"
          }
        },
        "right": {
          "type": 0,
          "value": true
        }
      },
      "value": {
        "type": 0,
        "value": "1"
      }
    }
  ]
}
```
Request object: PdfFormMap
#### DELETE /pdfform/v1/map/{pdfCode}
This is endpoint used to get information about the form on a uploaded PDF file. You can use this as a helper to create a PDF form map.
+ Params:
  + pdfCode: the PDF code of the file you which to fill
  
Responses:
+ 200 - OK
+ 400 - Bad Request
+ 404 - Not Found
+ 500 - Internal Server Error

Example request: https://localhost:5001/pdfform/v1/map/ESDC-EMP5624
### Fill
#### GET /pdfform/v1/fill/{pdfCode}?ids={arrayResourceIds}
This is the main endpoint used to fill an existing PDF file and return the filled PDF file.
+ Params:
  + pdfCode: the PDF code of the file you which to fill
  + arrayResourceIds: list of DB resources ids to fill the form with their data (these resources are retrieved from the collections registered to the PDF file on map endpoint)

Responses:
+ 204 - No Content
+ 400 - Bad Request
+ 404 - Not Found
+ 500 - Internal Server Error

Example request: https://localhost:5001/pdfform/v1/fill/ESDC-EMP5624?ids[0]=employerid&ids[1]=jobofferid

Example response: the filled PDF file
```
HTTP/1.0 200 OK
Content-Type: application/pdf
```
### More usage information
You can get more information about the API accessing Swagger, when running your locally through url: [baseUrl]/swagger
## Documentation
### Objects
#### PdfFormMap
+ Id *string*: id of the pdf form map.
+ PdfCode *string*: pdf code of the PDF file that the map is linked to.
+ Collections *string[]*: list of database collections names that this map is related to.
+ FieldMaps *PdfFormFieldMap[]*: list of fields maps linking values to the PDF form fields.
#### PdfFormFieldMap
+ Name *string*: name of the PDF file form field to linked to this field map.
+ Condition *PdfMapCondition*: condition to fill this field on PDF form.
+ Value *PdfMapDynamicValue*: fill the PDF form field with this value, if the condition is true.
#### PdfMapCondition
+ Type *PdfMapConditionType*: condition type.
+ Left *PdfMapDynamicValue*: left value of the condition.
+ Right *PdfMapDynamicValue*: right value of the condition.
#### PdfMapArithmetic
+ Type *PdfMapArithmeticType*: arithmetic type.
+ Left *PdfMapDynamicValue*: left value of the arithmetic.
+ Right *PdfMapDynamicValue*: right value of the arithmetic.
#### PdfMapDynamicValue
+ Type *PdfMapDynamicValueType*: dynamic value type.
+ Value *string|PdfMapDatabaseValue|PdfMapArithmetic*: a dynamic value. Can be fixed, retrieved from database or some calculation.
#### PdfMapDatabaseValue
+ Collection *string*: collection name from which the value will be retrieved.
+ DocumentField *string*: document field from the collection from which the value will be retrieved. *(can be nested like: field.nestedField)*
#### PdfScanResult
+ PdfCode *string*: pdf code of the scanned PDF file.
+ Pages *PdfScanPage[]*: list of pages informations of the scanned file.
#### PdfScanPage
+ Number *int*: page number.
+ Image *string*: page image in base64 **[not implemented]**.
+ FormFields *PdfScanFormField[]*: list of informations of the form fields on the page.
#### PdfScanFormField
+ Name *string*: form field name. [use this to map a form field to a dynamic value]
+ Position *Position*: position of the form field on the page.
+ Size *Size*: size of the form field.
#### Position
+ X *double*: X axis of the pposition.
+ Y *double*: Y axis of the pposition.
#### Size
+ Width *double*: width of the form field.
+ Height *double*: height of the form field.
### Enums
#### PdfMapConditionType
+ [0] Equal: the left value is equal to the right value.
+ [1] NotEqual: the left value is different from the right value.
+ [2] HasValue: the left value is an array that contains the right value.
+ [3] GreaterThan: the left value is greater than the right value.
+ [4] LessThan: the left value is less than the right value.
+ [5] GreaterThanOrEqual: the left value is greater than or equal to the right value.
+ [6] LessThanOrEqual: the left value is less than or equal to the right value.
#### PdfMapArithmeticType
+ [0] Subtract: the right value is subtracted from the left value.
+ [1] Add: the right value is added to the left value.
+ [2] Multiply: the left value is multiplied by the right value.
+ [3] Divide: the left value is divided by the right value.
+ [4] Modulus: the remainder of the left value divided by the right value.
#### PdfMapDynamicValueType
+ [0] Fixed: the dynamic value is a fixed string value.
+ [1] Database: the dynamic value is a PdfMapDatabaseValue, linked to a DB document field.
+ [2] Arithmetic: the dynamic value is a PdfMapArithmetic with some calculation.
