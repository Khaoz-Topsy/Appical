## Appical - Bank Account Management API

### Additional features
- CI / CD with Azure DevOps [![Build Status](https://dev.azure.com/khaoznet/KhaozNet/_apis/build/status/Khaoz-Topsy.Appical?branchName=master)](https://dev.azure.com/khaoznet/KhaozNet/_build/latest?definitionId=79&branchName=master)


#### Decisions made
- I moved the validation closer to the database
	- Although the API/MVC modelstate validation is nice and works really well. I wouldn't want to have persistence validation and ViewModel validation getting out of sync
	- 	Such as the limitation on how long a name of an account can be
	- Also allows us to test the validation through the Unit Tests

#### Observations & Notes
- I added appsettings.Production.json to the git repo out of convenience. Since this file will potentially contain secrets it is better to keep this file somewhere that the CI CD can access
