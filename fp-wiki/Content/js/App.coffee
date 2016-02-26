# CoffeeScript

Site = angular.module 'fp-wiki', ['ngResource']

Site.factory 'SearchApi', ['$resource', (($resource) -> $resource '/api/Search/', {}, {'get': { method: 'GET', isArray: true }})]