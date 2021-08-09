## Appical - Bank Account Management API

### Bonus Non-Functional Requirements (this step is optional)
**Please describe how you would approach this problem if the system had to support 20,000 transactions / second. What would have
to be changed in the architecture and why?**

---

### Additional features
- Hosted online: [appical.kurtlourens.com](https://appical.kurtlourens.com)
- CI / CD with Azure DevOps 
	- Build: [![Build Status](https://dev.azure.com/khaoznet/KhaozNet/_apis/build/status/Khaoz-Topsy.Appical?branchName=master)](https://dev.azure.com/khaoznet/KhaozNet/_build/latest?definitionId=79&branchName=master)
	- Release: [![Release Status](https://vsrm.dev.azure.com/khaoznet/_apis/public/Release/badge/b5441643-fd7c-4330-92d7-bffc23a7e0a4/37/44)](https://vsrm.dev.azure.com/khaoznet/_apis/public/Release/badge/b5441643-fd7c-4330-92d7-bffc23a7e0a4/37/44)

---

#### Decisions made
- I moved the validation closer to the database
	- Although the API/MVC modelstate validation is nice and works really well. I wouldn't want to have persistence validation and ViewModel validation getting out of sync
	- 	Such as the limitation on how long a name of an account can be
	- Also allows us to test the validation through the Unit Tests
- I added appsettings.Production.json to the git repo out of convenience. 
	- Since this file will potentially contain secrets it is better to keep this file somewhere that the CI CD can access
