# Meal Planner
This web application was built using the [SAFE Stack](https://safe-stack.github.io/). I created this web application as a learning exercise to gain a better understanding of what it takes to build a fully functional web application using f#.

It's my first fully fledged application using f# so it may not be the perfect f# code and I am open to suggestions so feel free to create an issue if you'd like to make a suggestion for improvements.

It features dependency injection, loading configuration from appSettings.json, storing data in a database and authentication using OAuth (Google). So I'd like to think this is a fully fledged application.
It's built using Azure Devops and hosted on Azure here: http://meal-planner-cv.azurewebsites.net/

Thank you for reading.

## Building and Starting the application
Before you run the project **for the first time only** you must install dotnet "local tools" with this command:

```bash
dotnet tool restore
```

To concurrently run the server and the client components in watch mode use the following command:

```bash
dotnet fake build -t run
```

Then open `http://localhost:8080` in your browser.