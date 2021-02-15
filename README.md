# fluentdb-migrator

## Intro
This is a general service to illistrate the usage of fluent db migrator engine. Personally, I find it very useful as it allows you to deploy db changes and code changes in one swoop.

## Usage

Make sure you have the following installed:
1. .Net Core 3.1
2. Docker

You will first want to run postgress in the background with:
```docker run --rm --name pg-docker -e POSTGRES_PASSWORD=docker -d -p 5432:5432 postgres```

After start up simply run `FluentDatabase.DummyService` in debug mode to run the sample migration. You should see output in the console if this fails check to make sure your docker container actually came up.

For a detailed guide on the service checkout this blog (2/15 - Coming soon).

If you found this useful or found a way to do things better let me know on the blog. Thanks!
