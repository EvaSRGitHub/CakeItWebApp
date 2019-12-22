# CakeIt 

Simple website created with ASP.NET CORE MVC, MS SQL 

[![Build Status](https://ci.appveyor.com/api/projects/status/github/EvaSRGitHub/CakeItWebApp?branch=master&svg=true
)](https://ci.appveyor.com/api/projects/status/github/EvaSRGitHub/CakeItWebApp)

### Introduction
This is a C# Web course project. Its aim is to pracitce asp.net core technology and create an demonstration application.
CakeIt is a website, which main purpose is to sell cakes from a list of options and to give the oportunity to its customers to 
create and order customized cakes, made by their choise of sponge, cream, topping and decoration. Additonally the site has 
sections with cooking forum, downloadable cooking books and video tutorials for its subscibers.

### Technologies and Architecture
The poroject is written in C# and asp.net core 2.1
It uses the following NuGet pacages:
 *AutoMapper v8.0.0
 *HtmlSanitizer v4.0.199
 *TinyMCE" v4.9.2 
 *X.PagedList.Mvc.Core v7.6.0
For testing: 
 *Moq v4.10.1
 *Shouldly v3.0.2
 *Xunit v2.4.1

The app is asp.net core 2.1 where the scafolded identity is, the cantrollers and views. The data layer is separated in class library, 
so are the database entities, the view models and the service layer. 

### Launch
To setup the project first you need to download it locally. To run it write 'update-database' in Visual Studio Package Manager Console.
The poject is build on EF Core 2.1. I plan to seed some inital data, but for now if you like you can test it with your own.
The app send confirmation e-mails after each order. To test this feacher you should make account in SENDGRID Email provider and 
put configuration data in the appsettings.json.
There is an option for Facebook authentication, which is commented, as you have to include configuration daga in 
appsetting.json to work.

### Status
The project is still under development. 

### Tests 
Eighty percent of the Service logic is tested with unit tests. I have tried with integration tests but stumble on a problem 
and leave it for now. In future will practice with selenium.

### Pull requests welcome!
Spotted an error? Something doesn't make sense? Send me a pull request! Thanks!

### License
MIT © EvaSR