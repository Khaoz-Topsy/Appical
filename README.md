## Appical 

#### Decisions made
- I moved the validation closer to the database
	- Although the API/MVC modelstate validation is nice and works really well. I wouldn't want to have persistence validation and ViewModel validation getting out of sync
	- 	Such as the limitation on how long a name of an account can be
	- Also allows us to test the validation through the Unit Tests

#### Observations & Notes
- I added appsettings.Production.json to the git repo out of convenience. Since this file will potentially contain secrets it is better to keep this file somewhere that the CI CD can access
