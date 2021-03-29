# Mechanic Checker

The Mechanic Checker Web Application is live on this URL: http://mechanicchecker.us-east-1.elasticbeanstalk.com/

For additional documentation please refer to our [Mechanic Checker Wiki](https://github.com/COMP231W21-G5/Mechanic-Checker/wiki)!

The Mechanic Checker Web Application allows Canadian users located in the Greater Toronto Area (GTA) to compare auto part-related items and automotive-related services, from local stores and major retailers, i.e. Ebay, Amazon, and Alibaba, with plans for adding support for more major retailers, e.g. Walmart. Local stores can create an account to post their auto part-related items and automotive-related services listings. Additionally for local stores users can filter search results based on local stores near their location via the Google Maps API. For the major retailers, the Mechanic Checker uses product APIs provided by said retailers to obtain their auto part-related items listings. 

The tech stack consists of [.NET Core 2.1](https://docs.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-2.1?view=aspnetcore-5.0), and [Nodejs](https://nodejs.org/en/), where Nodejs is used in the .NET Core 2.1 application as middleware.

This is Phase 1 of a 2-Phase [Centennial College](https://www.centennialcollege.ca/) capstone project for created for [Software Development Project 1 (COMP-231)](https://www.centennialcollege.ca/programs-courses/full-time/course/software-development-project-i/), for the professor Jake Nesovic. Phase 2 of the Mechanic Checker project will be done in [Software Development Project 2 (COMP-313)](https://www.centennialcollege.ca/programs-courses/full-time/course/software-development-project-2-COMP-313/).

## Mechanic Checker Team

Ibrahim Jomaa and Emmanuel Ajayi are the Tech Leads for this project. They have admin permission on this repository, and are listed in the `CODEOWNERS` file. Their tasks include reviewing and approving [pull requests](https://github.com/COMP231W21-G5/Mechanic-Checker/pulls).

The Mechanic Checker team is as follows:
- Michael Asemota:
    - Role: Scrum Master / Customer
    - Program: [Software Engineering Technology (Co-op)](https://www.centennialcollege.ca/programs-courses/full-time/software-engineering-technology/)
    - [GitHub](https://github.com/Asemota33)
    - [LinkedIn](https://www.linkedin.com/in/michaelasemota)
- Ibrahim Jomaa: 
    - Role: Software Developer | Tech Lead
    - Program: [Software Engineering Technology (Co-op)](https://www.centennialcollege.ca/programs-courses/full-time/software-engineering-technology/)
    - [GitHub](https://github.com/Function-0)
    - [LinkedIn](https://www.linkedin.com/in/ibrahim-jomaa/)
- Sanjib Saha: 
    - Role: Software Developer
    - Program: [Software Engineering Technology](https://www.centennialcollege.ca/programs-courses/full-time/software-engineering-technology/)
    - [GitHub](https://github.com/SanjibSaha27)
    - [LinkedIn](https://www.linkedin.com/in/sanjib-saha-79914b1bb/)
- Shaminda Abeysekara: 
    - Role: Software Developer
    - Program: [Software Engineering Technology](https://www.centennialcollege.ca/programs-courses/full-time/software-engineering-technology/)
    - [GitHub](https://github.com/Shaminda1017)
    - [LinkedIn](https://www.linkedin.com/in/shamindaabeysekara)
- Emmanuel Ajayi: 
    - Role: Software Developer | Tech Lead
    - Program: [Game Programming (Co-op)](https://www.centennialcollege.ca/programs-courses/full-time/game-programming/)
    - [GitHub](https://github.com/Dami908)
    - [LinkedIn](https://www.linkedin.com/in/emmalare)
- Nusrat Jahan: 
    - Role: Software Developer
    - Program: [Software Engineering Technician](https://www.centennialcollege.ca/programs-courses/full-time/software-engineering-technician/)
    - [GitHub](https://github.com/nusratjt)
    - [LinkedIn](https://www.linkedin.com/in/nusrat-jahan-6047aa171/)
- Shaniquo McKenzie: 
    - Role: Software Developer
    - Program: [Health Informatics Technology (Co-op)](https://www.centennialcollege.ca/programs-courses/full-time/health-informatics-technology/)
    - [GitHub](https://github.com/shaniquo)
    - [LinkedIn](https://www.linkedin.com/in/shaniquo-mckenzie)

## Running Locally

The Mechanic Checker source code requires the [Visual Studio IDE](https://visualstudio.microsoft.com/) to access the `MechanicChecker.sln` solution file. This is located under the path: 
```
MechanicChecker/MechanicChecker.sln
```

Additional dependencies may be required to install using [NuGet](https://docs.microsoft.com/en-us/nuget/what-is-nuget).

Unit testing folders can also be found where the `MechanicChecker.sln` file is located. The folders are as follows:
- MechanicCheckerCoreUnitTests
- mechanicCheckerTest

## Deployment Architecture

The Mechanic Checker is hosted on [Amazon Web Services (AWS)](https://aws.amazon.com/), at region us-east-1. Using [GitHub Actions](https://github.com/features/actions), a CI/CD pipeline is used to push code from GitHub to [AWS Elastic Beanstalk](https://aws.amazon.com/elasticbeanstalk/). An [AWS MySQL relational database (Amazon RDS)](https://aws.amazon.com/rds/), and a [AWS Simple Storage Service (Amazon S3)](https://aws.amazon.com/s3/) has also been provisioned on AWS. The 1 year [AWS Free Tier](https://aws.amazon.com/free/) membership is being used to fund AWS's Cloud Services.

### Deployment Diagram

![Deployment Diagram v4](https://user-images.githubusercontent.com/30096267/112782808-e37bf300-901b-11eb-8018-62993253d93a.png)

## License

The Mechanic Checker Web Application is licensed under the MIT License.

See the [LICENSE](https://github.com/COMP231W21-G5/Mechanic-Checker/blob/develop/LICENSE) file for more information.


