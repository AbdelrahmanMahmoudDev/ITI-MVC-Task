## Status

Currently in development, it is mostly done but there are still some features I want to integrate and some major cleanups in the codebase that need to be done

## Lessons learned
* Stick to the typing convention of the tool you're using, in this case it's Microsoft's PascalCase
* Razor Pages are a neat HTML template mechanism, but in practice you'll still need a whole lot of Javascript code to make sure some functionalities can occur smoothly
* The best front-end templates are the ones that don't require 50k JS scripts to be loaded up
* When using Entity Framework Core, it's important to use the right combination of good conventions and Fluent API to ensure smooth migrations and overall development

## Missing features
* Search
* Pagination
* External login

## Ongoing bugs
* There's a problem with the pathways authorized users get into the system, where logging in as an instructor might place them in an illogical part of the app