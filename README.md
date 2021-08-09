## Appical - Bank Account Management API

### Bonus Non-Functional Requirements (this step is optional)
**Please describe how you would approach this problem if the system had to support 20,000 transactions / second. What would have
to be changed in the architecture and why?**

The first issue to tackle would be to ensure that the API is stateless, so that we can spawn multiple instances of the API and not run into any Authentication issues. We could use something like JWT tokens or cookies. JWT tokens do increase the payload of our requests as the tokens contain extra bytes because the data within them are cryptographically signed. But they are quite popular at the moment with a lot of support and are easy to manage with front-end applications.
With this API already being planned to make use of Docker, we can more easily scale our API to handle large amounts of incoming requests with Docker containers. We could make use of technologies such as Kubernetes or Docker Swarm to scale our API by increasing or decreasing the number of running containers with the amount of incoming requests.
As our API scales to handle the number of incoming requests, the database would start getting more of the requests and would eventually become the bottleneck. A small change that has a large impact is introducing some caching to our API. Caching data that does not change often is a great way to reduce load on the Database. Using InMemory cache on API controllers is an easy option, but will have diminishing results with many instances of the APIs running. Redis offers us a quick "database" where we can store data that our API instances can access quickly and reduce the number of calls to the database.
Caching will only work for requests to fetch data and requests that manipulate data will still be going to the database. To handle these requests we can take a look at the existing indexes for our most queried tables. Increasing their performance will reduce the amount of time the SQL Server will have to deal with each request and therefore increase our throughput. We can also increase the resources on the SQL Server (Vertical scaling), providing it with more CPUs and RAM (very easy to do when using cloud providers, with on premise solutions this can be quite tricky as the server will have to be shutdown). Upgrading the hardware can help but at a certain point the returns will decrease. We can also Scale out (Horizontal scaling) by splitting the database for each tenant (a tenant can be an organisation, client or group of clients). This way a large organisation responsible for a large amount of requests will not affect other clients that are on a different split of the database. This would be quite a big change for our project as the database being connected to would depend on the user making the request

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
