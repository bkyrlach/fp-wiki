# CoffeeScript

Site = angular.module 'fp-wiki', ['ngResource', 'ngSanitize', 'ng-showdown']

Site.factory 'SearchApi', ['$resource', (($resource) -> $resource '/api/Search/', {}, {'get': { method: 'GET', isArray: true }})]
Site.factory 'HelpApi', ['$resource', (($resource) -> $resource '/api/HelpContent/')]